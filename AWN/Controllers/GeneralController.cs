﻿using AWN.Dtos;
using AWN.Dtos.UserDto;
using AWN.Models;
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

        public GeneralController(ApplicationDbContext context, UserManager<Account> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("done-case")]
        public async Task<IActionResult> GetDoneDonateCases()
        {
            var doneCases = await _context.donateCases
                .Where(dc => dc.State == DonateCaseState.Done)
                .Select(dc => new DonateCase
                {
                    Id = dc.Id,
                    Title = dc.Title,
                    SubTitle = dc.SubTitle,
                    TargetAmount = dc.TargetAmount,
                    CurrentAmount = dc.CurrentAmount,
                    State = dc.State,
                    Location = dc.Location,
                    TimesTamp = dc.TimesTamp,
                    ExcessAmount = dc.ExcessAmount
                })
                .ToListAsync();

            return Ok(doneCases);
        }
        [HttpGet("inprogress-case")]
        public async Task<IActionResult> GetInProgressDonateCases()
        {
            var inProgressCases = await _context.donateCases
                .Where(dc => dc.State == DonateCaseState.InProgress)
                .Select(dc => new DonateCase
                {
                    Id = dc.Id,
                    Title = dc.Title,
                    SubTitle = dc.SubTitle,
                    TargetAmount = dc.TargetAmount,
                    CurrentAmount = dc.CurrentAmount,
                    State = dc.State,
                    Location = dc.Location,
                    TimesTamp = dc.TimesTamp,
                    ExcessAmount = dc.ExcessAmount
                })
                .ToListAsync();

            return Ok(inProgressCases);
        }

        [HttpGet("notification")]
        public async Task<IActionResult> GetAllNotifications()
        {
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
          public async Task<IActionResult> UpdateAccountAsync([FromBody] UpdateAccountDto model)
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
            if (model.Photo is not null)
            {
                using var dataStream = new MemoryStream();

                await model.Photo.CopyToAsync(dataStream);

                account.Photo = dataStream.ToArray();
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
            var notification = await _context.notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound("Notification not found");
            }

            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok("Notification is deleted successfully");
        }
    }
}
