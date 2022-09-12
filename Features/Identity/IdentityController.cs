using HypeStock.Data.Models;
using HypeStock.Features.Identity.Models;
using HypeStock.Models.Identity;
using HypeStock.Models.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HypeStock.Features.Identity
{
    public class IdentityController: ApiController
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppSettings appSettings;
        private readonly IIdentityService identityService;

        public IdentityController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            IIdentityService identityService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.appSettings = appSettings.Value;
            this.identityService = identityService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult<LoginResponseModel>> Register(RegisterUserModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await this.userManager.CreateAsync(user, model.Password);

            //if (result.Succeeded) //TODO: Move to seperate endpoint called AssignUserRole
            //{
            //    var currentUser = await this.userManager.FindByNameAsync(model.UserName);
            //    await this.userManager.AddToRoleAsync(currentUser, "Editor");
            //}

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var loginResult = await Login(new LoginUserModel() 
            { 
                UserName = model.UserName,
                Password = model.Password,
            });

            return loginResult;
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginUserModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.UserName);
            var userRole = await this.userManager.GetRolesAsync(user);
            if (user == null)
            {
                return Unauthorized();
            }

            var passwordValid = await this.userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return Unauthorized();
            }

            var token = identityService.GenerateJwtToken(
                user.Id,
                user.UserName,
                this.appSettings.Secret,
                userRole.FirstOrDefault() ?? "Regular");

            return new LoginResponseModel
            { 
                Token = token,
            };  
        }

        [HttpPost]
        [Route(nameof(AddUserRole))]
        public async Task<ActionResult> AddUserRole(string roleName) //Investigate why roleName is null
        {
            var result = await roleManager.CreateAsync(new IdentityRole { Name = "Editor" });
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
