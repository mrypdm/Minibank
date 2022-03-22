namespace MiniBank.Web.Controllers.Users.Dto
{
    public class UserCreateDto
    {
        public string Login { get; set; }
        public string Email { get; set; }
    }

    public class UserUpdateDto
    {
        public string Login { get; set; }
        public string Email { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }
}