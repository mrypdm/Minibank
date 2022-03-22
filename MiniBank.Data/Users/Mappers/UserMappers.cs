using MiniBank.Core.Users;

namespace MiniBank.Data.Users.Mappers
{
    public static class UserMappers
    {
        public static User ToModel(this UserDbModel model)
        {
            return new User
            {
                Id = model.Id,
                Login = model.Login,
                Email = model.Email
            };
        }

        public static UserDbModel ToDbModel(this User model)
        {
            return new UserDbModel
            {
                Id = model.Id,
                Login = model.Login,
                Email = model.Email
            };
        }

    }
}