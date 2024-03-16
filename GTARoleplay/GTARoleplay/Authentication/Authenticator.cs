using GTARoleplay.Account;
using static BCrypt.Net.BCrypt;

namespace GTARoleplay.Authentication
{
    public class Authenticator
    {
        public static bool VerfiyPasswordAgainstUser(User user, string password) => Verify(password, user.Password);

        public static string GetHashedPassword(string plainPassword) => HashPassword(plainPassword);
    }
}
