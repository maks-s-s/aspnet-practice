using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using practice.Models;
using System;

namespace practice.Tests
{
    public static class IdentityMocks
    {
        public static Mock<UserManager<User>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(
                store.Object,
                null, null, null, null, null, null, null, null);

            return userManager;
        }

        public static Mock<SignInManager<User>> GetSignInManagerMock(Mock<UserManager<User>> userManagerMock)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signInManager = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                contextAccessor.Object,
                userClaimsPrincipalFactory.Object,
                null, null, null, null);

            return signInManager;
        }
    }
}
