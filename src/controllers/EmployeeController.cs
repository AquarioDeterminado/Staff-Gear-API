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
using API.src.models.viewModels;
using API.src.auth;
using System.Security.Claims;


namespace API.src.controllers
{

    public class AlterPasswordViewModel
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword {get;set; } = null;
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

        [Authorize(Policy = "RequireHR")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeViewModel request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.JobTitle) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Department))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                var businessEntity = new BusinessEntity();

                var person = new Person
                {
                    BusinessEntity = businessEntity,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PersonType = "EM",
                };

                var email = new EmailAddress
                {
                    BusinessEntity = person,
                    EmailAddress1 = request.Email,
                };

                var employee = new Employee
                {
                    BusinessEntity = person,
                    HireDate = DateOnly.FromDateTime(DateTime.Now),
                    Gender = "M",
                    JobTitle = request.JobTitle,
                    LoginID = request.Email,
                    MaritalStatus = "S",
                    BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
                    NationalIDNumber = Guid.NewGuid().ToString().Substring(0, 10)
                };

                var deptHistory = new EmployeeDepartmentHistory
                {
                    BusinessEntity = employee,
                    DepartmentID = _db.Department.FirstOrDefault(d => d.Name == "Sales")?.DepartmentID ?? 1,
                    ShiftID = 1,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = null
                };

                _db.BusinessEntity.Add(businessEntity);
                _db.Person.Add(person);
                _db.EmailAddress.Add(email);
                _db.Employee.Add(employee);
                _db.EmployeeDepartmentHistory.Add(deptHistory);
                
                await _db.SaveChangesAsync();

                var user = new User
                {
                    Username = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PassWord),
                    Role = request.Role ?? "Employee",
                    IsActive = true,
                    EmployeeId = employee.BusinessEntityID
                };
                _db.Users.Add(user);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict($"Error creating employee: {ex.Message}");
            }

            return Ok("Employee created.");
        }

        [Authorize(Policy = "RequireHR")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _db.Employee
                .Include(e => e.BusinessEntity)
                .Include(e => e.EmployeeDepartmentHistory)
                .ThenInclude(h => h.Department)
                .Include(e => e.BusinessEntity.EmailAddress)
                .Where(e => e.CurrentFlag == false)
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


        [Authorize(Policy = "AnyUser")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            if (id <= 0) return BadRequest("Insert a valid ID");
            
            if (!EmployeeSessionManager.UserIsHimself(User, id) && !User.IsInRole("HR"))
                return Forbid("Employees can only update their own information.");
            
            var employee = await _db.Employee
                .Where(e => e.BusinessEntityID == id)
                .Include(e => e.BusinessEntity)
                .Include(e => e.EmployeeDepartmentHistory).ThenInclude(h => h.Department)
                .Include(e => e.BusinessEntity.EmailAddress)
                .FirstOrDefaultAsync();

            if (employee is null)
                return NotFound(new { error = $"Employee {id} not found." });

            if (employee.CurrentFlag == true)
                return BadRequest("Employee is inactive.");

            var viewModel = new EmployeeViewModel
            {
                BusinessEntityID = employee.BusinessEntityID,
                FirstName = employee.BusinessEntity.FirstName,
                MiddleName = employee.BusinessEntity?.MiddleName,
                LastName = employee.BusinessEntity?.LastName ?? "",
                JobTitle = employee.JobTitle,
                Department = employee.EmployeeDepartmentHistory
                    .Where(h => h.EndDate == null)
                    .Select(h => h.Department.Name)
                    .FirstOrDefault() ?? "(without department)",
                Email = employee.BusinessEntity.EmailAddress
                    .OrderBy(em => em.EmailAddressID)
                    .Select(em => em.EmailAddress1)
                    .FirstOrDefault() ?? "",
                HireDate = employee.HireDate
            };

            return Ok(viewModel);
        }
        
        [Authorize(Policy = "AnyUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeViewModel request)
        {
            if (!EmployeeSessionManager.UserIsHimself(User, id) && !User.IsInRole("HR"))
                return Forbid("Employees can only update their own information.");
            
            var employee = await _db.Employee.FindAsync(id);
            if (employee == null)
                return NotFound("Employee not found.");

            if (employee.CurrentFlag == true)
                return BadRequest("Employee is inactive.");

            var person = await _db.Person.FindAsync(id);
            if (person == null)
                return NotFound("Person not found.");

            employee.BusinessEntity = person;
            person.FirstName = request.FirstName ?? person.FirstName;
            person.MiddleName = request.MiddleName ?? person.MiddleName;
            person.LastName = request.LastName ?? person.LastName;
            employee.JobTitle = request.JobTitle ?? employee.JobTitle;

            var lastDep = await _db.EmployeeDepartmentHistory
                .Where(edh => edh.BusinessEntityID == id && edh.EndDate == null).FirstOrDefaultAsync();

            if (request.Department != null && lastDep != null && request.Department != lastDep.Department.Name)
            {
                var newDep = await _db.Department
                    .Where(d => d.Name == request.Department)
                    .FirstOrDefaultAsync();
                if (newDep == null)
                    return NotFound("Department not found.");
                if (lastDep != null)
                    lastDep.EndDate = DateOnly.FromDateTime(DateTime.Now);

                var newDepHistory = new EmployeeDepartmentHistory
                {
                    BusinessEntityID = id,
                    DepartmentID = newDep.DepartmentID,
                    ShiftID = lastDep?.ShiftID ?? 1,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = null
                };
                await _db.EmployeeDepartmentHistory.AddAsync(newDepHistory);
            }

            var emailRecord = await _db.EmailAddress
                .Where(ea => ea.BusinessEntityID == id)
                .OrderBy(ea => ea.EmailAddressID)
                .FirstOrDefaultAsync();
            if (emailRecord != null)
            {
                emailRecord.EmailAddress1 = request.Email ?? emailRecord.EmailAddress1;
            }
            else
            {
                var newEmail = new EmailAddress
                {
                    BusinessEntityID = id,
                    EmailAddress1 = request.Email,
                    rowguid = Guid.NewGuid(),
                    ModifiedDate = DateTime.Now
                };
                await _db.EmailAddress.AddAsync(newEmail);
            }

            await _db.SaveChangesAsync();
            return Ok("Employee updated.");
        }

        

        [Authorize(Policy = "AnyUser")]
        [HttpPost("alterpassword/{id}")]
        public async Task<IActionResult> AlterPassword(int id, [FromBody] AlterPasswordViewModel request)
        {

            if (!await EmployeeSessionManager.EmployeeIsActive(id)) return BadRequest("Employee is inactive.");

            if (!EmployeeSessionManager.UserIsHimself(User, id))
                return Forbid("Employees can only alter their own password.");

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _db.Users.Where(u => u.EmployeeId == id).ToList().ForEach(u => u.PasswordHash = newPasswordHash);
            await _db.SaveChangesAsync();
            // Implementation for altering an employee's password
            return Ok("Employee password altered.");
        }


        [HttpGet("Payments/{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetEmployeePayments(int id)
        {
            if (id <= 0) return BadRequest("Indique um ID válido.");
            if (!await EmployeeSessionManager.EmployeeIsActive(id)) return BadRequest("Employee is inactive.");

            if (!EmployeeSessionManager.UserIsHimself(User, id) && !User.IsInRole("HR"))
                return Forbid("Employees can only update their own information.");

            var paymentsView = await _db.EmployeePayHistory
                .Where(p => p.BusinessEntityID == id)
                .OrderByDescending(p => p.RateChangeDate)
                .Select(p => new PaymentViewModel
                {
                    BusinessEntityID = p.BusinessEntityID,
                    FullName = _db.Person.Where(pe => pe.BusinessEntityID == p.BusinessEntityID)
                                         .Select(pe => pe.FirstName + " " + pe.LastName)
                                         .FirstOrDefault() ?? "Unknown",
                    Rate = p.Rate,
                    PayedDate = p.ModifiedDate
                })
                .ToListAsync();

            if (paymentsView.Count == 0) return NotFound($"Não existem pagamentos para o colaborador com ID {id}.");
            return Ok(paymentsView);

        }


        [HttpGet("Movements/{id}")]
        [Authorize(Roles = "HR, Employee")]
        public async Task<IActionResult> GetEmployeeMovements(int id)
        {
            if (id <= 0) return BadRequest("Indique um ID válido.");
            if (!await EmployeeSessionManager.EmployeeIsActive(id)) return BadRequest("Employee is inactive.");

            if (!EmployeeSessionManager.UserIsHimself(User, id) && !User.IsInRole("HR"))
                return Forbid("Employees can only update their own information.");

            var movementsView = await _db.EmployeeDepartmentHistory
                .Where(em => em.BusinessEntityID == id)
                .Include(em => em.Department)
                .Select(m => new MovementViewModel
                {
                    BusinessEntityID = m.BusinessEntityID,
                    FullName = _db.Person.Where(pe => pe.BusinessEntityID == m.BusinessEntityID)
                                         .Select(pe => pe.FirstName + " " + pe.LastName)
                                         .FirstOrDefault() ?? "Unknown",
                    DepartmentName = m.Department.Name,
                    JobTitle = _db.Employee.Where(e => e.BusinessEntityID == m.BusinessEntityID)
                                           .Select(e => e.JobTitle)
                                           .FirstOrDefault() ?? "Unknown",
                    StartDate = m.StartDate,
                    EndDate = m.EndDate
                })
                .ToListAsync();

            if (movementsView.Count == 0) return NotFound($"Não existem movimentações para o colaborador com ID {id}.");
            return Ok(movementsView);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            //TODO: Check Authorization

            var employee = await _db.Employee.FindAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            if (employee.CurrentFlag == true)
                return BadRequest("Employee is already inactive.");

            try
            {
                employee.CurrentFlag = true;
                _db.Employee.Update(employee);
                await _db.SaveChangesAsync();
                return Ok("Employee deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}