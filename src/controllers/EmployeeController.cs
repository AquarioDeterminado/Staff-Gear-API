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
            var employees = await _db.Employee
                .Include(e => e.BusinessEntity)
                .Include(e => e.EmployeeDepartmentHistory)
                    .ThenInclude(h => h.Department)
                .Include(e => e.BusinessEntity.EmailAddress)
                .Select(e => new EmployeeViewModel
                {
                    BusinessEntityID = e.BusinessEntityID,
                    FirstName = e.BusinessEntity.FirstName,
                    MiddleName = e.BusinessEntity.MiddleName,
                    LastName = e.BusinessEntity.LastName,
                    JobTitle = e.JobTitle,
                    Department = e.EmployeeDepartmentHistory
                        .Where(h => h.EndDate == null)
                        .Select(h => h.Department.Name)
                        .FirstOrDefault() ?? "(Sem departamento)",
                    Email = e.BusinessEntity.EmailAddress
                        .OrderBy(em => em.EmailAddressID)
                        .Select(em => em.EmailAddress1)
                        .FirstOrDefault() ?? "",
                    HireDate = e.HireDate
                })
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ToListAsync();

            return Ok(employees);
        }



        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            if (id <= 0) return BadRequest("Indique um ID válido.");


            var employee = await _db.Employee
                .Where(e => e.BusinessEntityID == id)
                .Include(e => e.BusinessEntity)
                .Include(e => e.EmployeeDepartmentHistory)
                    .ThenInclude(h => h.Department)
                .Include(e => e.BusinessEntity.EmailAddress)
                .FirstOrDefaultAsync();

            if (employee is null)
                return NotFound(new { error = $"Funcionário {id} não encontrado." });

            var viewModel = new EmployeeViewModel
            {
                BusinessEntityID = employee.BusinessEntityID,
                FirstName = employee.BusinessEntity.FirstName,
                MiddleName = employee.BusinessEntity?.MiddleName,
                LastName = employee.BusinessEntity.LastName,
                JobTitle = employee.JobTitle,
                Department = employee.EmployeeDepartmentHistory
                    .Where(h => h.EndDate == null)
                    .Select(h => h.Department.Name)
                    .FirstOrDefault() ?? "(Sem departamento)",
                Email = employee.BusinessEntity.EmailAddress
                    .OrderBy(em => em.EmailAddressID)
                    .Select(em => em.EmailAddress1)
                    .FirstOrDefault() ?? "",
                HireDate = employee.HireDate
            };

            return Ok(viewModel);
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
            if (id <= 0) return BadRequest("Indique um ID válido.");

            var entities = await _db.EmployeePayHistory
            .Where(p => p.BusinessEntityID == id)
            .OrderByDescending(p => p.RateChangeDate)
            .ToListAsync();

            if (entities.Count == 0) return NotFound($"Não existem pagamentos para o colaborador com ID {id}.");
            var payments = _mapper.Map<List<EmployeePayHistoryDTO>>(entities);

            return Ok(payments);
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

    public class EmployeeViewModel
    {
        public int BusinessEntityID { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string JobTitle { get; set; }
        public required string Department { get; set; }
        public required string Email { get; set; }
        public DateOnly HireDate { get; set; }
    }
}