using AWN.Dtos.AdminDto;
using AWN.Models;
using AWN.Services;
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
        private readonly IMediaSerivce _mediaService;

        public AdminController(ApplicationDbContext context, UserManager<Account> userManager, IMediaSerivce mediaService)
        {
            _context = context;
            _userManager = userManager;
            _mediaService = mediaService;
        }

        [HttpGet("supports")]
        public async Task<IActionResult> GetAllSupportsAsync()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var supports = await _context.supports.ToListAsync();

            if(supports is null)
            {
                return NotFound("No supports attended !");
            }

            return Ok(supports);
        }

        [HttpGet("request-join")]
        public async Task<IActionResult> GetAllRequestsAsync()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var reqeustJoin = await _context.supports.ToListAsync();

            if (reqeustJoin is null)
            {
                return NotFound("No Reqeusts attended !");
            }

            return Ok(reqeustJoin);
        }

        [HttpGet("suggest-cases")]
        public async Task<IActionResult> GetAllSuggesCasesAsync()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var suggestions = await _context.suggestions
                .Where(d => d.Sort == SuggestionSort.SuggestCase)
                .Select(d => new
                {
                    d.PhoneNumber,
                    d.Address,
                    d.Details,
                    d.AccountId
                })
                .ToListAsync();

            if (suggestions == null || suggestions.Count == 0)
            {
                return NotFound("No suggest cases attended !");
            }

            return Ok(suggestions);
        }
        
        [HttpGet("donates-others")]
        public async Task<IActionResult> GetAllDonateOthersAsync()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var donateOthers = await _context.suggestions
                .Where(d => d.Sort == SuggestionSort.DonateOtherThanMoney)
                .Select(d => new
                {
                    d.PhoneNumber,
                    d.Address,
                    d.Details,
                    d.AccountId
                })
                .ToListAsync();

            if (donateOthers == null || donateOthers.Count == 0)
            {
                return NotFound("No donate others cases attended !");
            }

            return Ok(donateOthers);
        }

        [HttpGet("collected-case")]
        public async Task<IActionResult> GetCollectedDonateCases()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var collectedCases = await _context.donateCases
                .Where(dc => dc.State == DonateCaseState.Collected)
                .Select(dc => new 
                {
                    dc.Id,
                    dc.Title,
                    dc.SubTitle,
                    dc.TargetAmount,
                    dc.CurrentAmount,
                    dc.State,
                    dc.Location,
                    dc.TimesTamp,
                    dc.ExcessAmount,
                    dc.Category,
                    Photos = dc.Photos.Select(p => new
                    {
                        p.PhotoUrl
                    }).ToList(),
                    AccountCount = dc.Payments.Select(p => p.AccountId).Distinct().Count(),
                    PaymentCount = dc.Payments.Count(),
                    PaymentSum = dc.Payments.Sum(p => p.Amount)
                })
                .ToListAsync();

            return Ok(collectedCases);
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
                Category = dto.Category,
                TimesTamp = DateTime.Now
            };

            await _context.SaveChangesAsync();
            user.donateCases = new List<DonateCase>(){donateCase};
            if (dto.Photos is not null)
            {
                try
                {
                    var photos = new List<Photos>();

                    foreach (var formFile in dto.Photos)
                    {
                        if (formFile.Length > 0)
                        {
                            var photoUrl = await _mediaService.AddAsync(formFile);
                            if (photoUrl != null)
                            {
                                photos.Add(new Photos
                                {
                                    PhotoUrl = photoUrl,
                                    DonateCase = donateCase
                                });
                            }
                        }
                    }

                    donateCase.Photos = photos;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Server Error");
                }
            }
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
            if(dto.Category is not null)
            {
                donateCase.Category = dto.Category;
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
                       var photoUrl = await _mediaService.AddAsync(formFile);
                        if (photoUrl != null)
                        {
                            photos.Add(new Photos
                            {
                                PhotoUrl = photoUrl,
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