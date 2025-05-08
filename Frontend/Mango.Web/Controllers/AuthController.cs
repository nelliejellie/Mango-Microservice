using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService; 
using Mango.Web.Models.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            if(result != null && result.Success)
            {
                
                if(string.IsNullOrEmpty(requestDto.RoleName))
                {
                    requestDto.RoleName = "CUSTOMER";
                }
                var assign = await _authService.AssignRole(assigner);
                if(assign != null && assign.Success)
                {
                    TempData["success"] = "Registeration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text="ADMIN",Value="ADMIN"},
                new SelectListItem{Text="CUSTOMER",Value="CUSTOMER"},
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            Console.WriteLine(loginRequest.Email);
            var res = await _authService.LoginAsync(loginRequest);
            Console.WriteLine(res.Success);
            Console.WriteLine(res.Result);
            if(res != null && res.Success)
            {
                var response = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(res.Result));
                Console.WriteLine(response.Token);
                return RedirectToAction("Index","Home");
            }else{
                ModelState.AddModelError("CustomError",res.Message);
                return View(loginRequest);
            }
            return View(loginRequest);
        }
    }
}