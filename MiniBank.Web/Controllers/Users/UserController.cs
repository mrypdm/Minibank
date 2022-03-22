using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Users;
using MiniBank.Core.Users.Services;
using MiniBank.Web.Controllers.Users.Dto;

namespace MiniBank.Web.Controllers.Users
{
    [ApiController]
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
        public UserDto GetById(string id)
        {
            var foundedUser = _service.GetById(id);
            
            return new UserDto
            {
                Id = foundedUser.Id,
                Login = foundedUser.Login,
                Email = foundedUser.Email
            };
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public IEnumerable<UserDto> GetAll()
        {
            return _service.GetAll().Select(u => new UserDto
            {
                Id = u.Id,
                Login = u.Login,
                Email = u.Email
            });
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        public void Create(UserCreateDto newUserInfo)
        {
            _service.Create(new User
            {
                Login = newUserInfo.Login,
                Email = newUserInfo.Email
            });
        }

        /// <summary>
        /// Change user info
        /// </summary>
        [HttpPut("{id}")]
        public void UpdateById(string id, UserUpdateDto updatedUserInfo)
        {
            _service.Update(new User
            {
                Id = id,
                Login = updatedUserInfo.Login,
                Email = updatedUserInfo.Email
            });
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        [HttpDelete("{id}")]
        public void DeleteById(string id)
        {
            _service.DeleteById(id);
        }
    }
}