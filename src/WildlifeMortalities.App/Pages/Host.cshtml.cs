﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WildlifeMortalities.Data;
using WildlifeMortalities.Data.Entities.Users;

namespace WildlifeMortalities.App.Pages
{
    public class HostModel : PageModel
    {
        private readonly AppDbContext _context;

        public AppParameters AppParameters { get; set; } = new AppParameters();

        public HostModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return Page();
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(email))
            {
                return Page();
            }

            var sid = User.FindFirst("sid")?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sid))
            {
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Sid == sid);
            if (user == null)
            {
                user = new User
                {
                    Sid = sid,
                    EmailAddress = email,
                    Settings = UserSettings.Default
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            AppParameters = new AppParameters
            {
                UserId = user.Id,
                UserEmail = email,
                UserSettings = user.Settings,
            };

            Log.Information("User {Email}, {Sid} logged in", email, sid);

            return Page();
        }
    }
}
