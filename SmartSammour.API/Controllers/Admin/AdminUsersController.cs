using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Application.DTOs.Admin;
using SmartSammour.Infrastructure.Identity;

namespace SmartSammour.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET /api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .ToListAsync();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // GET /api/admin/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                Roles = roles
            });
        }

        // POST /api/admin/users
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateAdminUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required."
                });
            }

            var existing = await _userManager
                .FindByEmailAsync(dto.Email.Trim());

            if (existing != null)
            {
                return Conflict(new
                {
                    message = "A user with this email already exists."
                });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email.Trim(),
                Email = dto.Email.Trim(),
                FullName = dto.FullName.Trim(),
                EmailConfirmed = true
            };

            var createResult =
                await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                return BadRequest(new
                {
                    message = "User could not be created.",
                    errors = createResult.Errors
                        .Select(e => e.Description)
                });
            }

            var role = dto.IsSuperAdmin
                ? "SuperAdmin"
                : "Admin";

            var roleResult =
                await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return BadRequest(new
                {
                    message = "User was created but role assignment failed.",
                    errors = roleResult.Errors
                        .Select(e => e.Description)
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                new
                {
                    message = "Admin user created successfully.",
                    user.Id,
                    user.Email,
                    Role = role
                });
        }

        // POST /api/admin/users/{id}/reset-password
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            string id,
            ResetAdminPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new
                {
                    message = "New password is required."
                });
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    dto.NewPassword
                );

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Password reset failed.",
                    errors = result.Errors
                        .Select(e => e.Description)
                });
            }

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }

        // DELETE /api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId =
                _userManager.GetUserId(User);

            if (currentUserId == id)
            {
                return BadRequest(new
                {
                    message = "You cannot delete your own account."
                });
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var isSuperAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "SuperAdmin"
                );

            if (isSuperAdmin)
            {
                var superAdmins =
                    await _userManager.GetUsersInRoleAsync(
                        "SuperAdmin"
                    );

                if (superAdmins.Count <= 1)
                {
                    return BadRequest(new
                    {
                        message = "The last SuperAdmin cannot be deleted."
                    });
                }
            }

            var result =
                await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "User could not be deleted.",
                    errors = result.Errors
                        .Select(e => e.Description)
                });
            }

            return Ok(new
            {
                message = "User deleted successfully."
            });
        }
    }
}