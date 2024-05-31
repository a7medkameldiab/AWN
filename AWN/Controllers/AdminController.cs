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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-case")]
        public async Task<IActionResult> AddCaseAsync([FromForm] CreateCaseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return Ok($"Case {donateCase.Id} : {donateCase.Title} => Added Successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCaseAsync(int id, [FromForm] CreateCaseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donateCase = await _context.donateCases.Include(c => c.Photos).FirstOrDefaultAsync(c => c.Id == id);
            if (donateCase == null)
            {
                return NotFound("Case not found");
            }

            donateCase.Title = dto.Title;
            donateCase.SubTitle = dto.SubTitle;
            donateCase.CurrentAmount = dto.CurrentAmount;
            donateCase.TargetAmount = dto.TargetAmount;
            donateCase.State = dto.State;
            donateCase.Location = dto.Location;
            donateCase.TimesTamp = DateTime.Now;

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

            _context.donateCases.Update(donateCase);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                donateCase.Title,
                donateCase.Location,
                donateCase.TargetAmount,
                CountOfPhotos = donateCase.Photos.Count,
                donateCase.State,
            });
        }



    }
}