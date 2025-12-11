using API.src.models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using API.src.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using API.src.models.dtos;

namespace API.src.controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "HR")]
    class HRController : ControllerBase
    {

        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public HRController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("Payments")]
        public async Task<IActionResult> GetAllEmployeePayments()
        {
            var entities = await _db.EmployeePayHistory
            .OrderByDescending(p => p.RateChangeDate)
            .ToListAsync();
            var payments = _mapper.Map<List<EmployeePayHistoryDTO>>(entities);
            
            return Ok(payments);
        }

        [HttpGet("Movements")]
        public async Task<IActionResult> GetAllEmployeeMovements()
        {
            var entities = await _db.EmployeeDepartmentHistory
            .OrderByDescending(d => d.StartDate).ToListAsync();
            var movements = _mapper.Map<List<EmployeeDepartmentHistoryDTO>>(entities);

            return Ok(movements);
        }

        


    }
}