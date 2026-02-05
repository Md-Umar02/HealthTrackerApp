using HealthTracker.Application.ApplicationConstants;
using HealthTracker.Application.Common;
using HealthTracker.Application.DTO.User;
using HealthTracker.Application.Exceptions;
using HealthTracker.Application.InputModels;
using HealthTracker.Application.Services;
using HealthTracker.Application.Services.Interface;
using HealthTracker.Domain.Models;
using HealthTracker.Infrastructure.DbContexts;
using HealthTracker.Infrastructure.Repositoies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace HealthTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        protected readonly APIResponse _response;
        public UserController(IUserService userService)
        {
            _userService = userService;
            _response = new APIResponse();
        }
        //[HttpPost]
        //public async Task<ActionResult<APIResponse>> Create([FromBody] CreateUserDto dto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.DisplayMessage = CommonMessage.CreateOperationFailed;
        //            _response.AddError(ModelState.ToString());
        //            return Ok(_response);
        //        }
        //        var user = await _userService.CreateAsync(dto);
        //        _response.StatusCode = HttpStatusCode.Created;
        //        _response.IsSuccess = true;
        //        _response.DisplayMessage = CommonMessage.CreateOperationSuccess;
        //        _response.Result = user;
        //    }
        //    catch(BadRequestException ex)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.DisplayMessage = CommonMessage.CreateOperationFailed;
        //        _response.AddError(CommonMessage.SystemError);
        //        _response.Result = ex.ValidationErrors;
        //    }
        //    catch (Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.DisplayMessage = CommonMessage.CreateOperationFailed;
        //        _response.AddError(CommonMessage.SystemError);
        //    }
        //    return Ok(_response);
        //}

        [Authorize(Roles = "ADMIN")]
        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = users;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [Route("GetPagination")]
        public async Task<ActionResult<APIResponse>> GetPagination(PaginationInputModel pagination)
        {
            try
            {
                var users = await _userService.GetPagination(pagination);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = users;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }

        [Authorize(Roles = "ADMIN")]
        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.RecordNotFound;
                    return Ok(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = user;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<APIResponse>> Update([FromBody] UpdateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                var user = await _userService.GetByIdAsync(dto.Id);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                    return Ok(_response);
                }
                await _userService.UpdateAsync(dto);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.DisplayMessage = CommonMessage.UpdateOperationSuccess;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<APIResponse>> DeleteAsync(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessage.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                await _userService.DeleteAsync(id);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _response.DisplayMessage = CommonMessage.DeleteOperationSuccess;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.DeleteOperationFailed;
                _response.AddError(CommonMessage.SystemError);
            }

            return Ok(_response);
        }

        [Authorize(Roles = "USER")]
        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet("me")]
        public async Task<ActionResult<APIResponse>> GetMyProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.RecordNotFound;
                    return Ok(_response);
                }
                var user = await _userService.GetByIdentityIdAsync(userId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = user;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }

            return Ok(_response);
        }
    }
}