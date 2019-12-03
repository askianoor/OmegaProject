using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _singInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly AuthenticationContext _authenticationContext;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IOptions<ApplicationSettings> appSettings, AuthenticationContext authenticationContext)
        {
            _userManager = userManager;
            _singInManager = signInManager;
            _appSettings = appSettings.Value;
            _authenticationContext = authenticationContext;
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            var applicationUser = new ApplicationUser() {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(applicationUser, "Users");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                try
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[] {new Claim("UserID",user.Id.ToString())}),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var access_token = tokenHandler.WriteToken(securityToken);

                    UserAudit userAudit = new UserAudit
                    {
                        userId = user.Id,
                        ActionDate = DateTimeOffset.UtcNow,
                        ActionName = "Login",
                        Status = "Success"
                    };
                    _authenticationContext.Add(userAudit);
                    _authenticationContext.SaveChanges();

                    return Ok(new { access_token });
                }
                catch (Exception)
                {
                    UserAudit userAudit = new UserAudit
                    {
                        userId = user.Id,
                        ActionDate = DateTimeOffset.UtcNow,
                        ActionName = "Login",
                        Status = "Failed - System Error"
                    };
                    _authenticationContext.Add(userAudit);
                    _authenticationContext.SaveChanges();
                    return BadRequest(new { message = "Funtion System Error." }); ;
                }

            }
            else {
                UserAudit userAudit = new UserAudit
                {
                    userId = user.Id,
                    ActionDate = DateTimeOffset.UtcNow,
                    ActionName = "Login",
                    Status = "Failed - Username or password is incorrect."
                };
                _authenticationContext.Add(userAudit);
                _authenticationContext.SaveChanges();
                return BadRequest(new { message = "Username or password is incorrect." });
            }
        }
    }
}