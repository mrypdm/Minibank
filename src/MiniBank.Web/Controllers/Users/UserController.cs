using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Users;
using MiniBank.Core.Users.Services;
using MiniBank.Web.Controllers.Users.Dto;

namespace MiniBank.Web.Controllers.Users
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<UserDto> GetById(string id, CancellationToken token)
        {
            var foundedUser = await _service.GetById(id, token);

            return new UserDto
            {
                Id = foundedUser.Id,
                Login = foundedUser.Login,
                Email = foundedUser.Email
            };
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        public Task Create(UserCreateDto newUserInfo, CancellationToken token)
        {
            return _service.Create(new User
            {
                Login = newUserInfo.Login,
                Email = newUserInfo.Email
            }, token);
        }

        /// <summary>
        /// Change user info
        /// </summary>
        [HttpPut("{id}")]
        public Task UpdateById(string id, UserUpdateDto updatedUserInfo, CancellationToken token)
        {
            return _service.Update(new User
            {
                Id = id,
                Login = updatedUserInfo.Login,
                Email = updatedUserInfo.Email
            }, token);
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        [HttpDelete("{id}")]
        public Task DeleteById(string id, CancellationToken token)
        {
            return _service.DeleteById(id, token);
        }
    }
}