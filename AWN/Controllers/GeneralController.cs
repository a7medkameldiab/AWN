using AWN.Dtos;
using AWN.Dtos.UserDto;
using AWN.Models;
using AWN.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AWN.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly IMediaSerivce _mediaService;

        public GeneralController(ApplicationDbContext context, UserManager<Account> userManager, IMediaSerivce mediaService)
        {
            _context = context;
            _userManager = userManager;
            _mediaService = mediaService;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userId = User.FindFirst("uid").Value;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("This Id Is Not Found !");
                }

                var paymentCount = await _context.payments.CountAsync();
                var doneCaseCount = await _context.donateCases.CountAsync(dc => dc.State == DonateCaseState.Done);
                var collectedCaseCount = await _context.donateCases.CountAsync(dc => dc.State == DonateCaseState.Collected);
                var inProgressCaseCount = await _context.donateCases.CountAsync(dc => dc.State == DonateCaseState.InProgress);

                var statistics = new
                {
                    PaymentCount = paymentCount,
                    DoneCaseCount = doneCaseCount,
                    CollectedCaseCount = collectedCaseCount,
                    InProgressCaseCount = inProgressCaseCount
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("payments/{accountId}")]
        public async Task<IActionResult> GetAllPaymentAsync(string accountId)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var account = await _context.Users
                .Include(a => a.payments)
                .ThenInclude(a => a.DonateCase)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            var payments = account.payments.Select(p => new
            {
                p.Amount,
                p.DonateCaseId,
                p.DonateCase.Title,
                p.DonateCase.SubTitle,
                p.DonateCase.State,
                p.TimesTamp
            });

            return Ok(payments);
        }

        [HttpGet("done-case")]
        public async Task<IActionResult> GetDoneDonateCases()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var doneCases = await _context.donateCases
                .Where(dc => dc.State == DonateCaseState.Done)
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

            return Ok(doneCases);
        }
        [HttpGet("inprogress-case")]
        public async Task<IActionResult> GetInProgressDonateCases()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var inProgressCases = await _context.donateCases
              .Where(dc => dc.State == DonateCaseState.InProgress)
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

            return Ok(inProgressCases);
        }

        [HttpGet("notification")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var notifications = await _context.notifications
                .Select(n => new Notification
                {
                    Id = n.Id,
                    IsRead = n.IsRead,
                    Content = n.Content,
                    TimesTamp = n.TimesTamp
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPut("updateAccount")]
          public async Task<IActionResult> UpdateAccountAsync([FromForm] UpdateAccountDto model)
          {
            var userId = User.FindFirst("uid").Value;

            // Find the user by ID
            var account = await _userManager.FindByIdAsync(userId);
            if (account == null)
            {
                return NotFound("User not found");
            }

            if (model.UserName is not null)
            {
                account.UserName = model.UserName;
            }
            if (model.PhoneNumber is not null)
            {
                account.PhoneNumber = model.PhoneNumber;
            }
            if (model.Email is not null)
            {
                account.Email = model.Email;
            }
            if (model.DonationNumber is not null)
            {
                account.DonationNumber = model.DonationNumber;
            }
            if (model.Photo is not null)
            {
                account.PhotoUrl = await _mediaService.AddAsync(model.Photo);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHasher = new PasswordHasher<Account>();
                account.PasswordHash = passwordHasher.HashPassword(account, model.Password);
            }

            var updateResult = await _userManager.UpdateAsync(account);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(",", updateResult.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok("Account updated successfully.");
        }

        [HttpPost("donate-other")]
        public async Task<IActionResult> DonateOthers([FromBody] SuggestionDto dto)
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
                Sort = SuggestionSort.DonateOtherThanMoney
            };

            await _context.SaveChangesAsync();
            user.suggestions = new List<Suggestion>() { suggestion };

            await _context.suggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        [HttpPost("make-payment")]
        public async Task<IActionResult> MakePaymentAsync([FromBody] PaymentDto dto)
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
                return NotFound("This Id Is Not Found !");

            var donateCase = await _context.donateCases.FindAsync(dto.DonateCaseId);
            if (donateCase == null)
            {
                return NotFound("Donate case not found.");
            }

            var payment = new Payment
            {
                Amount = dto.Amount,
                DonateCaseId = dto.DonateCaseId,
                TimesTamp = DateTime.Now
            };

            await _context.SaveChangesAsync();
            user.payments = new List<Payment>() { payment };

            donateCase.CurrentAmount += dto.Amount;

            if (donateCase.CurrentAmount >= donateCase.TargetAmount)
            {
                donateCase.State = DonateCaseState.Collected;
            }

            _context.payments.Add(payment);
            _context.donateCases.Update(donateCase);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Payment processed successfully.", CurrentAmount = donateCase.CurrentAmount, State = donateCase.State });
        }

        [HttpDelete("delete-notification/{id}")]
        public async Task<IActionResult> DeleteNotificationAsync(int  id)
        {
            var userId = User.FindFirst("uid").Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound("This Id Is Not Found !");

            var notification = await _context.notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound("Notification not found");
            }

            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok("Notification is deleted successfully");
        }

        [HttpDelete("delete-user/{email}")]
        public async Task<IActionResult> DeleteUser(string email)
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

            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return Ok(new { Result = "User Deleted Successfully" });

                return BadRequest(result.Errors);
            }

            return BadRequest("User is not assign.");
        }

    }
}
