using Azure;
using HealthTracker.Application.ApplicationConstants;
using HealthTracker.Application.Common;
using HealthTracker.Application.DTO.Auth;
using HealthTracker.Application.DTO.User;
using HealthTracker.Application.Exceptions;
using HealthTracker.Application.Services;
using HealthTracker.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly APIResponse _response;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _response = new APIResponse();
            _logger = logger;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<APIResponse>> Register(AuthRegisterDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.AddError(ModelState.ToString());
                    _response.AddWarning(CommonMessage.RegistrationFailed);
                    return _response;
                }
                var user = await _authService.RegisterAsync(request);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;
                _response.DisplayMessage = CommonMessage.RegistrationSuccess;
                _response.Result = user;
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new 
                { 
                    message = CommonMessage.RegistrationFailed,
                    errors = ex.ValidationErrors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = CommonMessage.RegistrationFailed,
                    errors = ex.Message
                });
                //_response.StatusCode = HttpStatusCode.InternalServerError;
                //_response.DisplayMessage = CommonMessage.RegistrationFailed;
                //_response.AddError(ex.Message);
            }
            return _response;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<APIResponse>> Login(AuthRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.AddWarning(CommonMessage.LoginFailed);
                    return _response;
                }
                var user = await _authService.LoginAsync(request);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DisplayMessage = CommonMessage.LoginSuccess;
                _response.Result = user;

                _logger.LogInformation("User {Email} logged in successfully.", request.Email);
            }
            catch (Exception)
            {
                _logger.LogError("Login failed for user {Email}.", request.Email);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.LoginFailed;
                _response.AddError(CommonMessage.SystemError);
            }
            return (_response);
        }
    }
}
