using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Configrations;
using ProductsApi.DTO;
using ProductsApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JWTConfig _jwtConfig;
        private readonly RoleManager<IdentityRole<int>> _role;



        public AccountController(UserManager<AppUser> userManager, IOptionsMonitor<JWTConfig> optionsMonitor, RoleManager<IdentityRole<int>> role)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _role = role;


        }

        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO requestRegister)
        {
            if (ModelState.IsValid)
            {
                //Check If Email Exists 
                var emailExists = await _userManager.FindByEmailAsync(requestRegister.Email);
                if (emailExists != null)
                {
                    return BadRequest("Email Already Exists");

                }

                var newUser = new AppUser()
                {
                    Email = requestRegister.Email,
                    UserName = requestRegister.Email

                };

                var isCreated = await _userManager.CreateAsync(newUser, requestRegister.Password);

                if (isCreated.Succeeded)
                {
                    // Check if the "User" role exists, if not, create it
                    //if (!await _role.RoleExistsAsync("User"))
                    //{
                    //    var userRole = new IdentityRole<int>("User");
                    //    await _role.CreateAsync(userRole);
                    //}

                    // Assign the "User" role to the new user
                    await _userManager.AddToRoleAsync(newUser, "User");



                    // Generate Token
                    var token = await GenerateToken(newUser);

                    return Ok(new UserRegistrationResponseDTO()
                    {
                        Result = true,
                        Token = token
                    });
                }

                return BadRequest(isCreated.Errors.Select(e => e.Description).ToList());


            }

            return BadRequest("Invalid Request Payload");
        }


        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(UserLoginRequestDTO requestLogin)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(requestLogin.Email);

                if (existingUser == null) return BadRequest("Inavlid Authentication");

                var validPassword = await _userManager.CheckPasswordAsync(existingUser, requestLogin.Password);
                if (validPassword)
                {
                    var token = await GenerateToken(existingUser);
                    return Ok(new UserLoginResponseDTO() { Result = true, Token = token });
                }
                return BadRequest("Inavlid Authentication");
            }
            return BadRequest("Inavlid Authentication");
        }
        private async Task<string> GenerateToken(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var roles = await _userManager.GetRolesAsync(user);
            var firstRole = roles.FirstOrDefault();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {

                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("role",firstRole),
                    new Claim("Id",user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
