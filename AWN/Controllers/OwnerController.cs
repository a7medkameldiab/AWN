using AWN.Dtos;
using AWN.Dtos.AdminDto;
using AWN.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AWN.Controllers
{
    [Authorize(Roles = "Owner")]
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;

        public OwnerController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdmin([FromForm] RegisterAdminDto model)
        {
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
                using var dataStream = new MemoryStream();

                await model.Photo.CopyToAsync(dataStream);

                user.Photo = dataStream.ToArray();
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
