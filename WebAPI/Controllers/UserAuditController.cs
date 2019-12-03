using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuditController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private AuthenticationContext _authenticationContext;
        public UserAuditController(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext)
        {
            _userManager = userManager;
            _authenticationContext = authenticationContext;
        }

        [HttpGet]
        [Authorize]
        //GET : /api/UserAudit
        public async Task<Object> GetUserAudit()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            bool fullAccess = false;

            var rolename = await _userManager.GetRolesAsync(user);

            foreach (var item in rolename)
            {
                if (item == "Admin") fullAccess = true;
            }

            if (fullAccess)
            {
                var userAudits = _authenticationContext.UserAudits;
                return userAudits;
            }
            else
            {
                var userAudits = _authenticationContext.UserAudits.Where(ua => ua.userId == userId);
                return userAudits;
            }
        }
    }
}