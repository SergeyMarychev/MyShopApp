using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopApp.Application.Contracts.Users;
using MyShopApp.Application.Users;
using System.Security.Claims;

namespace MyShopApp.WebApi.Controllers
{
    [Authorize]
    public class ContactController : BaseApiController
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException();
            }
            return long.Parse(userIdClaim);
        }

        /// <summary>
        /// Связаться с поддержкой через чат
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Chat([FromBody] string message, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var request = new ContactRequestDto
            {
                Type = ContactType.Chat,
                Message = message
            };

            var result = await _contactService.ContactAsync(userId, request, ct);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Позвонить в поддержку
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Phone(CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var request = new ContactRequestDto
            {
                Type = ContactType.Phone
            };

            var result = await _contactService.ContactAsync(userId, request, ct);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
