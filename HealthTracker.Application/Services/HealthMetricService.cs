using AutoMapper;
using HealthTracker.Application.DTO.HealthMetric;
using HealthTracker.Application.Services.Interface;
using HealthTracker.Domain.Contracts;
using HealthTracker.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthTracker.Application.ViewModels;
using HealthTracker.Application.InputModels;

namespace HealthTracker.Application.Services
{
    public class HealthMetricService : IHealthMetricService
    {
        private readonly IHealthMetricRepository _healthMetricRepository;
        private readonly IPaginationService<HealthMetricDto, HealthMetric> _paginationService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public HealthMetricService(IHealthMetricRepository healthMetricRepository, 
                IPaginationService<HealthMetricDto, HealthMetric> paginationService, 
                IUserRepository userRepository, IMapper mapper) 
        {
            _paginationService = paginationService;
            _userRepository = userRepository;
            _healthMetricRepository = healthMetricRepository;
            _mapper = mapper;
        }
        public async Task<HealthMetricDto> CreateAsync(string identityUser, CreateHealthMetricDto dto)
        {
            var user = await _userRepository.GetByIdentityIdAsync(identityUser);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var entity = new HealthMetric
            {
                UserId = user.Id,
                MetricTypeId = dto.MetricTypeId,
                Value = dto.Value,
                Created = DateTime.UtcNow
            };//_mapper.Map<HealthMetric>(createHealthMetricDto);
            var createdMetric = await _healthMetricRepository.CreateAsync(entity);
            var saved = await _healthMetricRepository.GetByIdAsync(entity.Id);
            var userDto = _mapper.Map<HealthMetricDto>(saved);
            return userDto;
        }   

        public async Task DeleteAsync(int id)
        {
            var userTask = await _healthMetricRepository.GetByIdAsync(id);
            if (userTask != null)
            {
                await _healthMetricRepository.DeleteAsync(userTask);
            }
            return;
        }

        public async Task<IEnumerable<HealthMetricDto>> GetAllAsync()
        {
            //Manually mapping entities to DTOs
            //var users = await _healthMetricRepository.GetAllAsync();
            //return users.Select(u => new UserDto
            //{
            //    Id = u.Id,
            //    Name = u.Name,
            //    Email = u.Email,
            //    Age = u.Age
            //});
            var users = await _healthMetricRepository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<HealthMetricDto>>(users);
            return userDtos;
        }

        public async Task<IEnumerable<HealthMetricDto>> GetAllByFilterAsync(int? metricTypeId, int? userId)
        { 
            var query = await _healthMetricRepository.GetAllAsync();
            //return Task.FromResult(query.Result
            //    .Where(hm => metricTypeId == 0 || hm.MetricTypeId == metricTypeId) // (userId == 0 || hm.UserId == userId) &&(
            //    .Select(hm => _mapper.Map<HealthMetricDto>(hm)));
            if (metricTypeId > 0)
            {
                query = query.Where(hm => hm.MetricTypeId == metricTypeId);
            }
            if(userId > 0)
            {
                query = query.Where(hm => hm.UserId == metricTypeId);
            }
            var result =  _mapper.Map<List<HealthMetricDto>>(query);
            return result;
        }

        public async Task<HealthMetricDto?> GetByIdAsync(int id)
        {
            var user = await _healthMetricRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<HealthMetricDto>(user);
        }

        public async Task<IEnumerable<HealthMetricDto>> GetByIdentityUserAsync(string identityUserId)
        {
            // 🔎 Find domain user using identity id
            var user = await _userRepository.GetByIdentityIdAsync(identityUserId);

            if (user == null)
                throw new Exception("User not found");

            // 🔎 Get metrics using domain user id
            var metrics = await _healthMetricRepository.GetByUserIdAsync(user.Id);

            return _mapper.Map<IEnumerable<HealthMetricDto>>(metrics);
        }

        public async Task<PaginationVM<HealthMetricDto>> GetPagination(PaginationInputModel pagination)
        {
            var source = await _healthMetricRepository.GetAllAsync();
            var result = _paginationService.GetPagination((List<HealthMetric>)source, pagination);
            return result;
        }

        public async Task UpdateAsync(UpdateHealthMetricDto updateHealthMetricDto)
        {
            //var existingUser = await _healthMetricRepository.GetByIdAsync(id);
            //if (existingUser == null)
            //{
            //    return null;
            //}
            //return _mapper.Map<User>(updateUserDto);
            var entity = _mapper.Map<HealthMetric>(updateHealthMetricDto);
            await _healthMetricRepository.UpdateAsync(entity);
        }
    }
}
