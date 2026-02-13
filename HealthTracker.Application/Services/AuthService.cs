using AutoMapper;
using HealthTracker.Application.DTO.Auth;
using HealthTracker.Application.Exceptions;
using HealthTracker.Application.Services.Interface;
using HealthTracker.Domain.Contracts;
using HealthTracker.Domain.Identity;
using HealthTracker.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, 
            SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto?> RegisterAsync(AuthRegisterDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("Email already registered");
            }

            var user = new ApplicationUser
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.Email,
                Age = request.Age
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "USER");

                var domainUser = new User
                {
                    IdentityUserId = user.Id,
                    Name = request.Name,
                    Email = request.Email,
                    Age = request.Age,
                    Created = DateTime.UtcNow
                };
                await _userRepository.CreateAsync(domainUser);

                var (token, expiration) = await GenerateTokenAsync(user);

                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = expiration
                };
            }

            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<AuthResponseDto?> LoginAsync(AuthRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("User not found with this email");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,   
                request.Password,
                isPersistent: true,
                lockoutOnFailure: true
                );
            var invalidCredential = await _userManager.CheckPasswordAsync(user, request.Password);
            if (result.Succeeded)
            {
                var (token, expiration) = await GenerateTokenAsync(user);
                AuthResponseDto authResponseDto = new AuthResponseDto
                {
                    UserId = user.Id,
                    Token = token,
                    Expiration = expiration
                };
                return authResponseDto;
            }
            else
            {
                if (result.IsLockedOut)
                {
                    throw new UnauthorizedException("Account is locked due to multiple failed login attempts");
                }

                if (result.IsNotAllowed)
                {
                    throw new UnauthorizedException("Login not allowed. Please confirm your email");
                }

                if (invalidCredential == false)
                {
                    throw new Exception( "Invalid Password");
                }

                //    // Default failure
                throw new UnauthorizedException("Invalid email or password");
            }
        }
        private async Task<(string Token, DateTime Expiration)> GenerateTokenAsync(ApplicationUser user)
        {
            //var jwtKey = _config["JwtSettings:Key"];
            //if (jwtKey == null)
            //{
            //    throw new Exception("JWT Key is not configured.");
            //}
            var jwtKey = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var jwtDuration = _config["JwtSettings:DurationInMinutes"] ?? throw new InvalidOperationException("JWT Duration is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            }.Union(roleClaims).ToList();

            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:DurationInMinutes"]));

            var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    signingCredentials: signingCredentials,
                    expires: expiration
                );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}
