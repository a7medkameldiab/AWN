using AWN.Dtos.UserDto;
using AWN.Helpers;
using AWN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace AWN.Services
{
    public class AuthService : IAuthService
    {
        // check if email is attend or not
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<Account> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<AuthModel> RegisterAsync(RegisterUserDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email Is Already Registered , Use An Another Email ! " };

            var account = new Account
            {
                Email = model.Email,
                PasswordHash = model.Password,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(account, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new AuthModel { Message = errors };
            }

            // add user to role
            await _userManager.AddToRoleAsync(account, "User");

            var jwtSecurityToken = await CreateJwtToken(account);

            return new AuthModel
            {
                Email = account.Email,
                UserName = account.UserName,
                PhoneNumber = account.PhoneNumber,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }

        public async Task<AuthModel> LogInAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email Or Passward Is Incorrect ! ";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roleList = await _userManager.GetRolesAsync(user);

            authModel.ExpireOn = jwtSecurityToken.ValidTo;
            authModel.UserName = user.UserName;
            authModel.Email = user.Email;
            authModel.PhoneNumber = user.PhoneNumber;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.IsAuthenticated = true;
            authModel.Roles = roleList.ToList();

            return authModel;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.RoleName))
                return "Invalid User Id Or Role !";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "User Already Assign To This Role";

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? string.Empty : "Something Went Wrong !";
        }

        //create jwt token 
        private async Task<JwtSecurityToken> CreateJwtToken(Account account)
        {
            var userClaims = await _userManager.GetClaimsAsync(account);
            var roles = await _userManager.GetRolesAsync(account);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("uid",account.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }
    }
}
