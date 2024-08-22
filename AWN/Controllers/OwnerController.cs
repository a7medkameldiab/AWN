using AWN.Dtos;
using AWN.Dtos.AdminDto;
using AWN.Models;
using AWN.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AWN.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMediaSerivce _mediaService;


        public OwnerController(UserManager<Account> userManager, ApplicationDbContext context, IMediaSerivce mediaService)
        {
            _userManager = userManager;
            _context = context;
            _mediaService = mediaService;
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userI = await _userManager.FindByIdAsync(userId);

            if (userI is null)
                return NotFound("This Id Is Not Found !");

            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            if (admins == null || !admins.Any())
            {
                return NotFound(new { Message = "No admins found." });
            }

            var adminDtos = admins.Select(admin => new
            {
                admin.Id,
                admin.UserName,
                admin.Email,
                admin.PhoneNumber
            });

            return Ok(adminDtos);
        }

        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdmin([FromForm] RegisterAdminDto model)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userI = await _userManager.FindByIdAsync(userId);

            if (userI is null)
                return NotFound("This Id Is Not Found !");

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return BadRequest("This Email Is Already Registered , Use An Another Email !");

            var user = new Account
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = model.Password,
                PhoneNumber = model.PhoneNumber,
            };

            if (model.Photo is not null)
            {
                user.PhotoUrl = await _mediaService.AddAsync(model.Photo);
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Ok(new { Result = "Admin Added Successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete-admin/{email}")]
        public async Task<IActionResult> DeleteAdmin(string email)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userI = await _userManager.FindByIdAsync(userId);

            if (userI is null)
                return NotFound("This Id Is Not Found !");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound("This Email Is Not Found !");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return Ok(new { Result = "Admin Deleted Successfully" });

                return BadRequest(result.Errors);
            }

            return BadRequest("User is not an admin");
        }
    }
}
