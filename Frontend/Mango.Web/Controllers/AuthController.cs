using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService; 
using Mango.Web.Models.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProviderService _tokenProviderService;

        public AuthController(IAuthService authService, ITokenProviderService tokenProviderService)
        {
            _authService = authService;
            _tokenProviderService = tokenProviderService;

        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text="ADMIN",Value="ADMIN"},
                new SelectListItem{Text="CUSTOMER",Value="CUSTOMER"},
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDto requestDto)
        {
            var result = await _authService.RegisterAsync(requestDto);
            Console.WriteLine(result.Success);
            var assigner = new RoleAssignDto
            {
                Email = requestDto.Email,
                RoleName = requestDto.RoleName
            };
            if (result != null && result.Success)
            {

                if (string.IsNullOrEmpty(requestDto.RoleName))
                {
                    requestDto.RoleName = "CUSTOMER";
                }
                var assign = await _authService.AssignRole(assigner);
                if (assign != null && assign.Success)
                {
                    TempData["success"] = "Registeration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            { 
                TempData["error"] = "Registeration Failed";
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text="ADMIN",Value="ADMIN"},
                new SelectListItem{Text="CUSTOMER",Value="CUSTOMER"},
            };

            ViewBag.RoleList = roleList;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProviderService.RemoveToken();
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            try
            {
                var res = await _authService.LoginAsync(loginRequest);

                if (res != null && res.Success)
                {
                    var response = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(res.Result));
                    await SignInUser(response);
                    _tokenProviderService.SetToken(response.Token);
                    TempData["success"] = "Login Successful";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("CustomError", res.Message);
                    TempData["error"] = "Login Failed";
                    return View(loginRequest);
                }
                return View(loginRequest);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private async Task SignInUser(LoginResponseDto loginResponse)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponse.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }
    }
}