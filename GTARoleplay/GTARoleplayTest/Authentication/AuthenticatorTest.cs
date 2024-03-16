using GTARoleplay.Account;
using GTARoleplay.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GTARoleplayTest.Authentication
{
    public class AuthenticatorTest
    {
        private User GetTestUser()
        {
            return new User()
            {
                Password = Authenticator.GetHashedPassword("123")
            };
        }

        [Fact]
        public void UserWithPassword_VerifyPasswordWithCorrect_PasswordIsVerified()
        {
            var user = GetTestUser();

            Assert.True(Authenticator.VerfiyPasswordAgainstUser(user, "123"));
        }

        [Fact]
        public void UserWithPassword_VerifyPasswordWithWrong_PasswordIsNotVerified()
        {
            var user = GetTestUser();

            Assert.False(Authenticator.VerfiyPasswordAgainstUser(user, "12345"));
        }
    }
}
