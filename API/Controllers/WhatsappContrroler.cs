using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Infrastructure.Data;
using RealEstateAPI.Services;
using System;

namespace RealEstateAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWhatsAppService _whatsAppService;

            public WhatsappController(ApplicationDbContext context, IWhatsAppService whatsAppService)
            {
                _context = context;
                _whatsAppService = whatsAppService;
            }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetPropertyWhatsAppLink(Guid propertyId)
        {
            // جيب العقار مع صاحبه
            var property = await _context.Properties
                .Include(p => p.Owner)  // ✅ Owner مش User
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null)
                return NotFound(new { message = "العقار مش موجود" });

            var owner = property.Owner;  // ✅ Owner مش User

            if (!owner.EnableWhatsApp)
                return BadRequest(new { message = "الوسيط مش بيقبل تواصل عبر واتساب" });

            var number = !string.IsNullOrEmpty(owner.WhatsAppNumber)
                ? owner.WhatsAppNumber
                : owner.PhoneNumber;

            if (string.IsNullOrEmpty(number))
                return BadRequest(new { message = "مفيش رقم متاح للتواصل" });

            var message = $"مرحباً {owner.FirstName}،\n" +
                         $"أنا مهتم بالعقار: {property.Title}\n" +
                         $"هل هو لا يزال متاحاً؟";

            var link = _whatsAppService.GenerateLink(number, message);

            return Ok(new
            {
                link,
                ownerName = $"{owner.FirstName} {owner.LastName}",
                whatsapp = number
            });
        }

            [HttpGet("user/{userId}")]  
        public async Task<IActionResult> GetUserWhatsAppLink(Guid userId, [FromQuery] string? message = null)
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                    return NotFound(new { message = "المستخدم مش موجود" });

                if (!user.EnableWhatsApp)
                    return BadRequest(new { message = "المستخدم مش بيقبل تواصل عبر واتساب" });

                var number = !string.IsNullOrEmpty(user.WhatsAppNumber)
                    ? user.WhatsAppNumber
                    : user.PhoneNumber;

                var defaultMessage = message ?? $"مرحباً {user.FirstName}، أريد الاستفسار عن عقاراتك";
                var link = _whatsAppService.GenerateLink(number, defaultMessage);

                return Ok(new { link });
            }
        }
    }

