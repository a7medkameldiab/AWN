using AWN.Dtos.UserDto;
using AWN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWN.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Account> _userManager;

        public UserController(ApplicationDbContext context, UserManager<Account> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("send-request-join")]
        public async Task<IActionResult> SendRequestJoinAsync([FromBody] RequestJoinDto dto)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var requestJoin = new RequestJoin
            {
                Name = dto.Name,
                Governorate = dto.Governorate,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                ReasonOfJoin = dto.ReasonOfJoin
            };

            await _context.SaveChangesAsync();
            user.requestJoins = requestJoin;

            await _context.requestJoins.AddAsync(requestJoin);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        [HttpPost("suggest-case")]
        public async Task<IActionResult> MakeSuggestionAsync([FromBody] SuggestionDto dto)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var suggestion = new Suggestion
            {
                Details = dto.Details,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
            };

            await _context.SaveChangesAsync();
            user.suggestions = new List<Suggestion>(){suggestion};

            await _context.suggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

    }
}
