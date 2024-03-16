using GTARoleplay.Account;

namespace GTARoleplay.Authentication
{
    public class Authenticator
    {
        public static bool VerfiyPasswordAgainstUser(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public static string GetHashedPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }
    }
}
