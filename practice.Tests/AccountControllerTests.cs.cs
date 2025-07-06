using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using practice.Controllers;
using practice.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace practice.Tests
{
    public class AccountControllerTests
    {
        private AccountController CreateController(
            Mock<UserManager<User>> userManagerMock = null,
            Mock<SignInManager<User>> signInManagerMock = null)
        {
            userManagerMock ??= IdentityMocks.GetUserManagerMock();
            signInManagerMock ??= IdentityMocks.GetSignInManagerMock(userManagerMock);

            var logger = new NullLogger<AccountController>();

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object, logger);

            return controller;
        }

        [Fact]
        public void Login_Get_ReturnsView_WhenNotAuthenticated()
        {
            var controller = CreateController();

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())
                }
            };

            var result = controller.Login();

            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Get_Redirects_WhenAuthenticated()
        {
            var controller = CreateController();

            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim("name", "testuser") }, "TestAuth"));

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };

            var result = controller.Login();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_Post_Success_RedirectsToHome()
        {
            var userManagerMock = IdentityMocks.GetUserManagerMock();
            var signInManagerMock = IdentityMocks.GetSignInManagerMock(userManagerMock);

            var testUser = new User { Email = "test@example.com", UserName = "testuser" };

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(testUser);

            signInManagerMock.Setup(s => s.PasswordSignInAsync(testUser, "correctpassword", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = CreateController(userManagerMock, signInManagerMock);

            var result = await controller.Login("test@example.com", "correctpassword");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_Post_Failure_ReturnsViewWithError()
        {
            var userManagerMock = IdentityMocks.GetUserManagerMock();
            var signInManagerMock = IdentityMocks.GetSignInManagerMock(userManagerMock);

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null); // пользователь не найден

            var controller = CreateController(userManagerMock, signInManagerMock);

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())
                }
            };

            var result = await controller.Login("wrong@example.com", "password");

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ErrorCount > 0);
        }


        [Fact]
        public async Task Register_Post_Success_RedirectsToLogin()
        {
            var userManagerMock = IdentityMocks.GetUserManagerMock();

            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = IdentityMocks.GetSignInManagerMock(userManagerMock);

            var controller = CreateController(userManagerMock, signInManagerMock);

            var result = await controller.Register("newuser@example.com", "Password123!");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Account", redirect.ControllerName);
        }

        [Fact]
        public async Task Logout_Post_RedirectsToLogin()
        {
            var userManagerMock = IdentityMocks.GetUserManagerMock();
            var signInManagerMock = IdentityMocks.GetSignInManagerMock(userManagerMock);

            signInManagerMock.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            var controller = CreateController(userManagerMock, signInManagerMock);

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
                {
                    User = new System.Security.Claims.ClaimsPrincipal(
                        new System.Security.Claims.ClaimsIdentity(new[] {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser")
                        }, "TestAuth"))
                }
            };

            var result = await controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Account", redirect.ControllerName);
        }
    }
}
