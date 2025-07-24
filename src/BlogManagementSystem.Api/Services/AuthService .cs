// src/BlogManagementSystem.Api/Services/AuthService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogManagementSystem.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterDto dto)
        {
            if (await _userManager.FindByNameAsync(dto.Username) != null)
                return new BaseResponse<string> { Success = false, Message = "Username already taken." };

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return new BaseResponse<string> { Success = false, Message = "Email already registered." };

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = string.Join("; ", result.Errors.Select(e => e.Description))
                };

            if (!await _roleManager.RoleExistsAsync("Author"))
                await _roleManager.CreateAsync(new IdentityRole("Author"));

            await _userManager.AddToRoleAsync(user, "Author");

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Registration successful. Please login."
            };
        }

        public async Task<BaseResponse<string>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return new BaseResponse<string> { Success = false, Message = "Invalid credentials." };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,           $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email,          user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtSec = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSec["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSec["Issuer"],
                audience: jwtSec["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSec["ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Login successful.",
                Data = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
