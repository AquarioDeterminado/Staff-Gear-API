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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.src.controllers
{

    class MovementViewModel
    {
        public int BusinessEntityID { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string JobTitle { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    class PaymentViewModel
    {
        public int BusinessEntityID { get; set; }
        public string FullName { get; set; }
        public decimal Rate { get; set; }
        public DateTime PayedDate { get; set; }
    }

    [ApiController]
    [Route("api/v1/[controller]")]
    public class HrController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public HrController(AdventureWorksContext db, IMapper mapper)
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

            // Grabbing full names for each payment entry
            var paymentsView = payments.Select(p => new PaymentViewModel
            {
                BusinessEntityID = p.BusinessEntityID,
                FullName = _db.Person
                            .Where(pe => pe.BusinessEntityID == p.BusinessEntityID)
                            .Select(pe => pe.FirstName + " " + pe.LastName)
                            .FirstOrDefault() ?? "Unknown",
                Rate = p.Rate,
                PayedDate = p.ModifiedDate
            }).ToList();
            
            return Ok(paymentsView);
        }

        [HttpGet("Movements")]
        public async Task<IActionResult> GetAllEmployeeMovements()
        {
            var entities = await _db.EmployeeDepartmentHistory
            .OrderByDescending(d => d.StartDate).ToListAsync();

            var movements = _mapper.Map<List<EmployeeDepartmentHistoryDTO>>(entities);

            var movementsView = movements.Select(m => new MovementViewModel
            {
                BusinessEntityID = m.BusinessEntityID ?? -1,
                FullName = _db.Person
                            .Where(pe => pe.BusinessEntityID == m.BusinessEntityID)
                            .Select(pe => pe.FirstName + " " + pe.LastName)
                            .FirstOrDefault() ?? "Unknown",
                DepartmentName = _db.Department
                                    .Where(d => d.DepartmentID == m.DepartmentID)
                                    .Select(d => d.Name)
                                    .FirstOrDefault() ?? "Unknown",
                JobTitle = _db.Employee
                                    .Where(e => e.BusinessEntityID == m.BusinessEntityID)
                                    .Select(e => e.JobTitle)
                                    .FirstOrDefault() ?? "Unknown",
                StartDate = m.StartDate,
                EndDate = m.EndDate
            }).ToList();

            return Ok(movementsView);
        }

        


    }
}