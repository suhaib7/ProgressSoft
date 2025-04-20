using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Moq;
using Newtonsoft.Json;
using ProgressSoft.Controllers;
using Xunit;

namespace UnitTest
{
    public class AccountControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new AccountController(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task SignUp_ReturnsOk_WhenUserIsCreated()
        {
            // Arrange
            var signUpDto = new SignUpDTO { /* fill with dummy data */ };
            _mockUnitOfWork.Setup(u => u.User.SignUp(signUpDto)).ReturnsAsync("mock-token");

            // Act
            var result = await _controller.SignUp(signUpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!["token"];
            Assert.Equal("mock-token", token);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenUserExists()
        {
            var signUpDto = new SignUpDTO { };
            _mockUnitOfWork.Setup(u => u.User.SignUp(signUpDto)).ReturnsAsync((string)null!);

            var result = await _controller.SignUp(signUpDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            // Arrange
            var loginDto = new LoginDTO { };
            _mockUnitOfWork.Setup(u => u.User.Login(loginDto)).ReturnsAsync("jwt-token");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!["token"];
            Assert.Equal("jwt-token", token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            var loginDto = new LoginDTO { };
            _mockUnitOfWork.Setup(u => u.User.Login(loginDto)).ReturnsAsync((string)null!);

            var result = await _controller.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }

}
