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
    class CandidatesController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public CandidatesController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost("")]
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

        [HttpGet("")]
        public async Task<IActionResult> GetCandidates()
        {
            //TODO: Check Authorization

            var candidates = await _db.JobCandidate.ToListAsync();
            var dtoList = _mapper.Map<List<JobCandidateDTO>>(candidates);
            return Ok(dtoList);
        }

        [HttpPost("accept/{id}")]
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
                    BusinessEntityID = businessEntity.BusinessEntityID,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                };

                var email = new EmailAddress
                {
                    BusinessEntityID = businessEntity.BusinessEntityID,
                    EmailAddress1 = candidate.Email,
                };

                var employee = new Employee
                {
                    BusinessEntityID = businessEntity.BusinessEntityID,
                    HireDate = DateOnly.FromDateTime(DateTime.Now),
                };

                _db.BusinessEntity.Add(businessEntity);
                _db.Person.Add(person);
                _db.EmailAddress.Add(email);
                _db.Employee.Add(employee);
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