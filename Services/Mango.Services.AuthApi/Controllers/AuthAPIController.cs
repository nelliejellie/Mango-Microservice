using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.IService;
using Mango.MessagePublisher.Services;

namespace Mango.Services.AuthApi.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRabbitPublisher _rabbitPublisher;
        private readonly IConfiguration _configuration;

        public AuthAPIController(IAuthService authService,IRabbitPublisher rabbitPublisher, IConfiguration configuration)
        {
            _authService = authService;
            _rabbitPublisher = rabbitPublisher;
            _configuration = configuration;

        }


        // Add your action methods here

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDto registerDto)
        {
            var res = await _authService.Register(registerDto);
            if (res != null && res.StatusCode == 201)
            {
                res.Message = "User registered successfully";
                res.Success = true;
                _rabbitPublisher.PublishMessageAsync(new EmailDto
                {
                    ToEmail = registerDto.Email,
                    Subject = "Welcome to Mango",
                    Body = $"<h1>Welcome {registerDto.Name}</h1><p>Thank you for registering with us!</p>"
                }, _configuration["Queues:RegisterUserQueue"]);
                return Ok(res);

            }
            else
            {
                res.Message = "User registered failed";
                res.Success = false;
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
                response.Message = "User registered successfully";
                response.Success = true;
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