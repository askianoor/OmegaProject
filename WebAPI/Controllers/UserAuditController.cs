using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuditController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private AuthenticationContext _authenticationContext;
        private readonly ApplicationSettings _appSettings;
        public UserAuditController(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _authenticationContext = authenticationContext;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [Authorize]
        //GET : /api/UserAudit
        public async Task<Object> GetUserAudit()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var rolename = await _userManager.GetRolesAsync(user);

            bool fullAccess = false;

            foreach (var item in rolename)
            {
                if (item == _appSettings.AdminRoleName) fullAccess = true;
            }
          
            if (fullAccess)
            {
                var userAudits = _authenticationContext.UserAudits.OrderByDescending(a => a.ActionDate);
                foreach (var item in userAudits)
                {
                    string username = _authenticationContext.Users.FirstOrDefault(u => u.Id == item.userId).UserName;
                    item.userId = username;
                }
                return new { userAudits, fullAccess };
            }
            else
            {
                var userAudits = _authenticationContext.UserAudits.Where(ua => ua.userId == userId).OrderByDescending(a => a.ActionDate);
                foreach (var item in userAudits)
                {
                    string username = _authenticationContext.Users.FirstOrDefault(u => u.Id == item.userId).UserName;
                    item.userId = username;
                }
                return new { userAudits, fullAccess };
            }
        }
    }
}