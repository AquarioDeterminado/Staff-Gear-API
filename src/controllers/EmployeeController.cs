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

    public class AlterPasswordViewModel
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public EmployeeController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO request)
        {
            // Implementation for creating an employee

            return Ok("Employee created.");
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllEmployees()
        {
            
            var employees = await (
                    from e in _db.Employee
                    join p in _db.Person on e.BusinessEntityID equals p.BusinessEntityID
                    join em in _db.EmailAddress on e.BusinessEntityID equals em.BusinessEntityID into emx
                    from em in emx.OrderBy(x => x.EmailAddressID).Take(1).DefaultIfEmpty()
                    join dh in _db.EmployeeDepartmentHistory on e.BusinessEntityID equals dh.BusinessEntityID into dhx
                    from dh in dhx.DefaultIfEmpty()
                    join d in _db.Department on dh.DepartmentID equals d.DepartmentID into dx
                    from d in dx.DefaultIfEmpty()
                    select new EmployeeDTO
                    {
                        BusinessEntityID = e.BusinessEntityID,
                        FirstName = p.FirstName,
                        MiddleName = p.MiddleName,
                        LastName = p.LastName,
                        JobTitle = e.JobTitle,
                        Department = dh != null && dh.EndDate == null ? d.Name : "(Sem departamento)",
                        Email = em != null ? em.EmailAddress1 : "(Sem email)",
                        HireDate = e.HireDate
                    }

            ).ToListAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            // Implementation for retrieving employees
            return Ok("List of employees.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO request)
        {
            // Implementation for updating an employee
            return Ok("Employee updated.");
        }

        [HttpPost("alterpassword/{id}")]
        public async Task<IActionResult> AlterPassword(int id, [FromBody] AlterPasswordViewModel request)
        {
            // Implementation for altering an employee's password
            return Ok("Employee password altered.");
        }

        [HttpGet("Payments/{id}")]
        public async Task<IActionResult> GetEmployeePayments(int id)
        {
            return Ok("List of employee payments.");
        }

        [HttpGet("Movements/{id}")]
        public async Task<IActionResult> GetEmployeeMovements(int id)
        {
            var movements = await _db.EmployeeDepartmentHistory
                .Where(em => em.BusinessEntityID == id)
                .ToListAsync();
            return Ok("List of employee movements.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            //TODO: Check Authorization

            var employee = await _db.Employee.FindAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            _db.EmployeePayHistory.RemoveRange(_db.EmployeePayHistory.Where(eph => eph.BusinessEntityID == id));
            _db.EmailAddress.RemoveRange(_db.EmailAddress.Where(ea => ea.BusinessEntityID == id));
            _db.Password.RemoveRange(_db.Password.Where(p => p.BusinessEntityID == id));
            _db.EmployeeDepartmentHistory.RemoveRange(_db.EmployeeDepartmentHistory.Where(edh => edh.BusinessEntityID == id));
            _db.EmployeePayHistory.RemoveRange(_db.EmployeePayHistory.Where(ep => ep.BusinessEntityID == id));
            _db.Employee.Remove(employee);
            await _db.SaveChangesAsync();

            return Ok("Employee deleted.");
        }

    }
}