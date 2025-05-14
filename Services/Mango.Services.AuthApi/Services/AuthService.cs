using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.IService;
using Microsoft.AspNetCore.Identity;
using Mango.Services.AuthApi.Data;

namespace Mango.Services.AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJWTTokenGenerator _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager,AppDbContext _dbContext,
         RoleManager<IdentityRole> roleManager, IJWTTokenGenerator tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _dbContext = _dbContext;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string rolename)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if(!await _roleManager.RoleExistsAsync(rolename))
                {
                    var role = new IdentityRole()
                    {
                        Name = rolename
                    };
                    await _roleManager.CreateAsync(role);
                }
                var result = await _userManager.AddToRoleAsync(user, rolename);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<ResponseDto> Login(LoginRequestDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, userDto.Password);
                if (result)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = _tokenService.GenerateToken(user,roles);
                    var userResponse = new UserDto()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Token = token
                    };
                    var response = new ResponseDto()
                    {
                        Message = "Login successful",
                        Result = userResponse,
                        StatusCode = 200,
                        Success = true
                    };
                    return response;
                }
                return new ResponseDto()
                {
                    Message = "Login Failed",
                    Result = { },
                    StatusCode = 400,
                    Success = false
                };
            }
            return new ResponseDto()
            {
                Message = "Login Failed",
                Result = { },
                StatusCode = 500,
                Success = false
            };
        }   

        public async Task<ResponseDto> Register(RegisterationRequestDto userDto)
        {
            var user = new ApplicationUser()
            {
                UserName = userDto.Email,
                Email = userDto.Email,
                Name = userDto.Name,
                PhoneNumber = userDto.PhoneNumber,
                NormalizedEmail = userDto.Email.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                var userResponse = new UserDto()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                var response = new ResponseDto();
                response.StatusCode = 201;
                response.Result = new {Token = "", userResponse };
                response.Message = "User registered successfully";
                response.Success = true;
                return response;
            }
            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description); // Add error descriptions to the list
            }

            return new ResponseDto
            {
                StatusCode = 500,
                Message = "User registration failed",
                Result = errors,
                Success = false
            };
        }
    }
}