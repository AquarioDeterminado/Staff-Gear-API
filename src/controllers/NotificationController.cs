using API.src.models;
using API.src.models.dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using API.src.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace API.src.controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    class NotificationController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public NotificationController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("{BusinessEntityID}")]
        public async Task<IActionResult> GetNotification(int BusinessEntityID)
        {
            var notifications = await _db.Notification
                .Where(n => n.BusinessEntityID == BusinessEntityID)
                .ToListAsync();

            if (notifications == null || notifications.Count == 0)
            {
                return NotFound("No notifications found for the given BusinessEntityID.");
            }

            var dtoList = _mapper.Map<List<NotificationDTO>>(notifications);
            return Ok(dtoList);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _db.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            _db.Notification.Remove(notification);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("user/{BusinessEntityID}")]

        public async Task<IActionResult> DeleteNotificationsByUser(int BusinessEntityID)
        {
            var notifications = await _db.Notification
                .Where(n => n.BusinessEntityID == BusinessEntityID)
                .ToListAsync();

            if (!notifications.Any())
            {
                return NotFound("No notifications found for the given BusinessEntityID.");
            }

            _db.Notification.RemoveRange(notifications);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

