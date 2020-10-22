using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,ILoggerService logger, IConfiguration configuration,IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }
        /// <summary>
        /// User register
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var UserName = userDTO.EmailAddress;
                var Password = userDTO.Password;
                _logger.LogInfo($"{location} : Registration attempt for {UserName}.");
                var User = new IdentityUser { UserName = UserName, Email = UserName };
                var result = await _userManager.CreateAsync(User, Password);
             
                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"{location} : {error.Code} - {error.Description}");
                    }
                    return InternalError($"{location} : {UserName} User registration attempt failed.");
                }
                await _userManager.AddToRoleAsync(User, "Customer");
                return Created("login", new { result.Succeeded });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var UserName = userDTO.EmailAddress;
                var Password = userDTO.Password;
                _logger.LogInfo($"{location} : Login attemped fromuser {UserName}");
                var result = await _signInManager.PasswordSignInAsync(UserName, Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInfo($"{location} : {UserName} successfully authenticated.");
                    var user = await _userManager.FindByNameAsync(UserName);
                    var tokenString = await GenerateJSONWebToken(user);
                    return Ok( new { token=tokenString});
                }
                _logger.LogInfo($"{location} : {UserName} not authenticated.");
                return Unauthorized(userDTO);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r=>new Claim(ClaimsIdentity.DefaultNameClaimType,r)));

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Issuer"],claims,null,expires:DateTime.Now.AddHours(5),signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong.");
        }
    }
}
