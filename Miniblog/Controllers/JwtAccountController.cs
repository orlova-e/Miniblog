using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.Models.Token;
using Miniblog.Token;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtAccountController : Controller
    {
        public IUserService _userService { get; private set; }
        public IRepository _repository { get; private set; }
        public IJwtSigningEncodingKey signingEncodingKey { get; set; }
        public JwtAccountController(IUserService userService, IRepository repository,
            IJwtSigningEncodingKey signingEncodingKey)
        {
            _userService = userService;
            _repository = repository;
            this.signingEncodingKey = signingEncodingKey;
        }
        [Route("SignIn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInAsync(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = _userService.GetFromDb(loginModel);
            if (user == null)
            {
                ModelState.AddModelError("", "User is not found");
                return BadRequest(loginModel);
            }
            string token = GetAccessToken(user, signingEncodingKey);
            //JsonTokens jsonTokens = new JsonTokens()
            //{
            //    access_token = token,
            //    access_token_expiration = DateTimeOffset.UtcNow,
            //    refresh_token = new GenerateRefreshToken().Generate()
            //};

            var data = new
            {
                access_token = token,
                access_token_expiration = DateTimeOffset.UtcNow.AddMinutes(20),
                refresh_token = new GenerateRefreshToken().Generate()
            };

            //IEnumerable<RefreshToken> refreshTokens = _repository.RefreshTokens.Find(rt => DateTimeOffset.UtcNow >= rt.AccessTokenExpiration);
            await _userService.PutRefreshTokenIntoDb(user, data.refresh_token, data.access_token_expiration);
            return Json(data);
        }
        [Route("SignUp")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsync(RegisterModel registerModel)
        {          
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IEnumerable<string> errors = _userService.ParametersAlreadyExist(registerModel); // !!!! CHECK !!!!
            if (errors.Any())
            {
                if (errors.Contains("Username"))
                {                                                   /// будет ли работать MODELSTATE???
                    ModelState.AddModelError(nameof(registerModel.Username), "A user with this name already exists");
                }
                if (errors.Contains("Email"))
                {
                    ModelState.AddModelError(nameof(registerModel.Email), "A user with this email already exists");
                }
                return BadRequest(registerModel);
            }
            User user = await _userService.CreateIntoDbAsync(registerModel);
            string token = GetAccessToken(user, signingEncodingKey);
            var data = new
            {
                access_token = token,
                access_token_expiration = DateTimeOffset.UtcNow.AddMinutes(20.0),
                refresh_token = new GenerateRefreshToken().Generate(),
                id = user.Id.ToString()
            };
            await _userService.PutRefreshTokenIntoDb(user, data.refresh_token, data.access_token_expiration);

            return Json(data);
        }
        private string GetAccessToken(User user, IJwtSigningEncodingKey signingEncodingKey)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("Email", user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Type.ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token",
                JwtRegisteredClaimNames.UniqueName, ClaimsIdentity.DefaultRoleClaimType);

            var now = DateTimeOffset.Now;
            var jwt = new JwtSecurityToken(
                issuer: ValidateTokenOptions.ISSUER,
                audience: ValidateTokenOptions.AUDIENCE,
                notBefore: now.DateTime,
                claims: claimsIdentity.Claims,
                signingCredentials: new SigningCredentials(
                    signingEncodingKey.GetSecurityKey(),
                    signingEncodingKey.SigningAlgorithm)
                );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return jwtToken;
        }
    }
}
