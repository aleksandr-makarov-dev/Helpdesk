using Helpdesk.API.Modules.Users.Models;

namespace Helpdesk.API.Modules.Users
{
    public static class UserMapper
    {
        public static User ToUser(this RegisterRequest r)
        {
            return new User
            {
                UserName = r.Email,
                Email = r.Email
            };
        }
    }
}
