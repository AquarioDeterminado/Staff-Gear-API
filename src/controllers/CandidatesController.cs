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
using System;

namespace API.src.controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public CandidateController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCandidate([FromBody] JobCandidateDTO request)
        {
            string? validationError;
            if ((validationError = _IsValidApplication(request)) != null)
                return BadRequest(validationError);

            var candidate = _mapper.Map<JobCandidate>(request);
            _db.JobCandidate.Add(candidate);
            await _db.SaveChangesAsync();

            // Implementation for candidate application
            return Ok(new { WasSaved = true });
        }

        private string? _IsValidApplication(JobCandidateDTO request)
        {
            if (request == null)
            {
                return "Invalid candidate application data.";
            } 
            else if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Resume))
            {
                return "Name, Email, and Resume are required fields.";
            } 
            else if (!request.Email.Contains("@"))
            {
                return "Invalid email format." ;
            } 
            else if (_db.JobCandidate.Any(c => c.Email == request.Email))
            {
                return "A candidate with this email already exists.";
            } 
            else
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCandidates()
        {
            //TODO: Check Authorization

            var candidates = await _db.JobCandidate.ToListAsync();
            var dtoList = _mapper.Map<List<JobCandidateDTO>>(candidates);
            return Ok(dtoList);
        }

        [HttpGet("accept/{id}")]
        public async Task<IActionResult> AcceptCandidate(int id)
        {
            var candidate = await _db.JobCandidate.FindAsync(id);
            if (candidate == null)
                return NotFound("Candidate not found.");

            try
            {
                var businessEntity = new BusinessEntity();

                var person = new Person
                {
                    BusinessEntity = businessEntity,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    PersonType = "EM",
                };

                var email = new EmailAddress
                {
                    BusinessEntity = person,
                    EmailAddress1 = candidate.Email,
                };

                var employee = new Employee
                {
                    BusinessEntity = person,
                    HireDate = DateOnly.FromDateTime(DateTime.Now),
                    Gender = "M",
                    JobTitle = "New Hire",
                    LoginID = candidate.Email,
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
                _db.JobCandidate.Remove(candidate);
                 
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict($"Error accepting candidate: {ex.Message}");
            }

            return Ok("Candidate accepted and converted to employee.");
        }

    }
}