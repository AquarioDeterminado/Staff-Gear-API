using API.src.models;
using API.src.models.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using API.src.utils;

namespace API.src.controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class NotificationController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public NotificationController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("{BusinessEntityID}")]
        [Authorize(Policy = "AnyUser")]
        public async Task<IActionResult> GetNotification(int BusinessEntityID)
        {
            if (!EmployeeSessionManager.UserIsHimself(User, BusinessEntityID))
            {
                return Forbid("Access denied: You can only access your own notifications.");
            }

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

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {

            var notification = await _db.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            var businessEntityID = notification.BusinessEntityID;

            if (!EmployeeSessionManager.UserIsHimself(User, businessEntityID))
            {
                return Forbid("Access denied: You can only access your own notifications.");
            }

            _db.Notification.Remove(notification);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [AllowAnonymous]
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

