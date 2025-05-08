using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.IService;

namespace Mango.Services.AuthApi.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
        }


        // Add your action methods here

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDto registerDto)
        {
            var res = await _authService.Register(registerDto);
            if (res != null && res.StatusCode == 201)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            Console.WriteLine("here");
            var response = await _authService.Login(loginDto);
            Console.WriteLine(response.StatusCode);
            if(response != null && response.StatusCode == 200)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignDto roleAssignDto)
        {
            var response = await _authService.AssignRole(roleAssignDto.Email, roleAssignDto.RoleName.ToUpper());
            if (response == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
    
}