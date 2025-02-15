﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoNEL.API.Domain.Models;
using TwoNEL.API.Domain.Services;
using TwoNEL.API.Domain.Services.Communications;
using TwoNEL.API.Extensions;
using TwoNEL.API.Resources;

namespace TwoNEL.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        [SwaggerOperation(
            Summary = "List all users",
            Description = "List of Users",
            OperationId = "ListAllUsers")]
        [SwaggerResponse(200, "List of Users", typeof(IEnumerable<UserResource>))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResource>), 200)]
        public async Task<IEnumerable<UserResource>> GetAllAsync()
        {
            var users = await userService.ListAsync();
            var resources = mapper
                .Map<IEnumerable<User>, IEnumerable<UserResource>>(users);
            return resources;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResource), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 404)]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await userService.GetByIdAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            var userResource = mapper.Map<User, UserResource>(result.Resource);
            return Ok(userResource);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResource), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 404)]
        public async Task<IActionResult> PostAsync([FromBody] SaveUserResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var user = mapper.Map<SaveUserResource, User>(resource);
            var result = await userService.SaveAsync(user);

            if (!result.Success)
                return BadRequest(result.Message);

            var userResource = mapper.Map<User, UserResource>(result.Resource);
            return Ok(userResource);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            var response = await userService.Authenticate(request);

            if (response == null)
                return BadRequest(new { message = "Invalid Username or Password" });

            return Ok(response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResource), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 404)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] SaveUserResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var user = mapper.Map<SaveUserResource, User>(resource);
            var result = await userService.UpdateAsync(id, user);

            if (!result.Success)
                return BadRequest(result.Message);

            var userResource = mapper.Map<User, UserResource>(result.Resource);
            return Ok(userResource);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UserResource), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 404)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await userService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            var userResource = mapper.Map<User, UserResource>(result.Resource);
            return Ok(userResource);
        }
    }
}
