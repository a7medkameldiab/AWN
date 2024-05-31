using AWN.Dtos.UserDto;
using AWN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> SendRequestJoinAsync([FromBody] RequestJoinDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(dto.AccountId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var requestJoin = new RequestJoin
            {
                AccountId = dto.AccountId,
                Name = dto.Name,
                Governorate = dto.Governorate,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                ReasonOfJoin = dto.ReasonOfJoin
            };

            await _context.requestJoins.AddAsync(requestJoin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = requestJoin.AccountId,
                Name = requestJoin.Name,
                Governorate = requestJoin.Governorate,
                City = requestJoin.City,
                PhoneNumber = requestJoin.PhoneNumber,
                ReasonOfJoin = requestJoin.ReasonOfJoin
            });
        }

        

        
    }
}
