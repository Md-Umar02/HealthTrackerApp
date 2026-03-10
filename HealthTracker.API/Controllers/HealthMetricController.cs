using HealthTracker.Application.ApplicationConstants;
using HealthTracker.Application.Common;
using HealthTracker.Application.DTO.HealthMetric;
using HealthTracker.Application.InputModels;
using HealthTracker.Application.Services;
using HealthTracker.Application.Services.Interface;
using HealthTracker.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace HealthTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthMetricController : ControllerBase
    {
        private readonly IHealthMetricService _healthMetricService;
        protected readonly APIResponse _response;
        public HealthMetricController(IHealthMetricService healthMetricService)
        {
            _healthMetricService = healthMetricService;
            _response = new APIResponse();
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> Create([FromBody] CreateHealthMetricDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessage.CreateOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var metrics = await _healthMetricService.CreateAsync(identityUserId, dto);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.DisplayMessage = CommonMessage.CreateOperationSuccess;
                _response.Result = metrics;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.CreateOperationFailed;
                _response.AddError(CommonMessage.SystemError);
            }
            
            return Ok(_response);
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAll()
        {
            try
            {
                var metrics = await _healthMetricService.GetAllAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = metrics;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }
        [HttpPost]
        [Route("GetPagination")]
        public async Task<ActionResult<APIResponse>> GetPagination(PaginationInputModel pagination)
        {
            try
            {
                var metrics = await _healthMetricService.GetPagination(pagination);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = metrics;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }
        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<APIResponse>> GetAllByFilter(int? metricTypeId, int? userId)
        {
            try
            {
                var metrics = await _healthMetricService.GetAllByFilterAsync(metricTypeId, userId);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = metrics;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(_response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetById(int id)
        {
            try
            {
                var metric = await _healthMetricService.GetByIdAsync(id);
                if (metric == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.RecordNotFound;
                    return Ok(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = metric;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessage.SystemError);
            }
            
            return Ok(_response);
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateHealthMetricDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                    return Ok(_response);
                }
                var metric = await _healthMetricService.GetByIdAsync(dto.Id);
                if (metric == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                    return Ok(_response);
                }
                await _healthMetricService.UpdateAsync(dto);
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                _response.AddError(CommonMessage.SystemError);
            }
            return Ok(dto);
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if(id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessage.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                var metric = await _healthMetricService.GetByIdAsync(id);
                if(metric == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessage.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                    return Ok(_response);
                }
                await _healthMetricService.DeleteAsync(id);
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessage.UpdateOperationFailed;
                _response.AddError(CommonMessage.SystemError);
            }
            
            return Ok(_response);
        }

        [Authorize(Roles = "USER")]
        [HttpGet("me")]
        public async Task<ActionResult<APIResponse>> GetMyMetrics()
        {
            try
            {
                // 🔐 Get identity user id from JWT
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (identityUserId == null)
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.DisplayMessage = "User not authenticated";
                    return Ok(_response);
                }

                // 👇 Ask service to return ONLY this user's metrics
                var metrics = await _healthMetricService.GetByIdentityUserAsync(identityUserId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = metrics;
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
