using AWN.Dtos.AdminDto;
using AWN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AWN.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Account> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<Account> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("add-case")]
        public async Task<IActionResult> AddCaseAsync([FromForm] CreateCaseDto dto)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var donateCase = new DonateCase
            {
                Title = dto.Title,
                SubTitle = dto.SubTitle,
                CurrentAmount = dto.CurrentAmount,
                TargetAmount = dto.TargetAmount,
                State = dto.State,
                Location = dto.Location,
                TimesTamp = DateTime.Now
            };

            await _context.SaveChangesAsync();
            user.donateCases = new List<DonateCase>(){donateCase};

            var photos = new List<Photos>();

            foreach (var formFile in dto.Photos)
            {
                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        var data = memoryStream.ToArray();
                        photos.Add(new Photos
                        {
                            Photo = data,
                            DonateCase = donateCase
                        });
                    }
                }
            }

            donateCase.Photos = photos;

            _context.donateCases.Add(donateCase);
            await _context.SaveChangesAsync();

            // Create and send notification to all accounts
            var notification = new Notification
            {
                Content = $"A new donate case '{donateCase.Title}' has been created.",
                IsRead = false,
                TimesTamp = DateTime.Now
            };

            await _context.SaveChangesAsync();

            var accounts = _userManager.Users.ToList();
            foreach (var account in accounts)
            {
                user.notifications = new List<Notification>() { notification };
            }

            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok($"Case {donateCase.Id} : {donateCase.Title} => Added Successfully");
        }

        [HttpPut("update-case/{id}")]
        public async Task<IActionResult> UpdateCaseAsync(int id, [FromForm] UpdateCaseDto dto)
        {
            var userId = User.FindFirst("uid").Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound("This Id Is Not Found !");
            }

            var donateCase = await _context.donateCases
                .Include(c => c.Photos)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (donateCase == null)
            {
                return NotFound("Case not found");
            }
            if(dto.Title is not null)
            {
                donateCase.Title = dto.Title;
            }
            if(dto.SubTitle is not null)
            {
                donateCase.SubTitle = dto.SubTitle;
            }
            if(dto.CurrentAmount is not null)
            {
                donateCase.CurrentAmount = (double)dto.CurrentAmount;
            }
            if(dto.TargetAmount is not null)
            {
                donateCase.TargetAmount = (double)dto.TargetAmount;
            }
            if(dto.State is not null)
            {
                donateCase.State = (DonateCaseState)dto.State;
            }
            if(dto.Location is not null)
            {
                donateCase.Location = dto.Location;
            }

            if (dto.Photos != null && dto.Photos.Count > 0)
            {
                // Remove existing photos
                _context.photos.RemoveRange(donateCase.Photos);

                // Add new photos
                var photos = new List<Photos>();
                foreach (var formFile in dto.Photos)
                {
                    if (formFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(memoryStream);
                            var data = memoryStream.ToArray();
                            photos.Add(new Photos
                            {
                                Photo = data,
                                DonateCase = donateCase
                            });
                        }
                    }
                }
                donateCase.Photos = photos;
            }

            if (donateCase.State is DonateCaseState.Done)
            {

                // Create and send notification to all accounts
                var notification = new Notification
                {
                    Content = $"The donate case '{donateCase.Title}' has reached its target and is now done.",
                    IsRead = false,
                    TimesTamp = DateTime.Now
                };

                await _context.SaveChangesAsync();

                var accounts = _userManager.Users.ToList();
                foreach (var account in accounts)
                {
                    user.notifications = new List<Notification>() { notification };
                }

                _context.notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            _context.donateCases.Update(donateCase);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }
    }
}