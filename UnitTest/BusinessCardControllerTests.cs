using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models.BusinessCard;
using Moq;
using ProgressSoft.Controllers;
using Service.Interface;
using Xunit;

namespace UnitTest
{
    public class BusinessCardControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFileParserService> _mockFileParserService;
        private readonly Mock<IFileExportService> _mockFileExportService;
        private readonly BusinessCardController _controller;

        public BusinessCardControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFileParserService = new Mock<IFileParserService>();
            _mockFileExportService = new Mock<IFileExportService>();

            _controller = new BusinessCardController(
                _mockUnitOfWork.Object,
                _mockFileParserService.Object,
                _mockFileExportService.Object
            );
        }

        [Fact]
        public void GetBusinessCards_ReturnsFilteredCards()
        {
            // Arrange
            var mockData = new List<BusinessCardModel>
            {
                new BusinessCardModel
                {
                    Id = 1, Name = "Ali", DateOfBirth = DateTime.Parse("2000-01-01"),
                    Email = "ali@email.com", Gender = "Male", Phone = "123", Address = "Amman", Photo = ""
                }
            };

            // Explicitly provide all arguments to avoid optional arguments in expression trees
            _mockUnitOfWork.Setup(u => u.BusinessCard.GetAll(
                It.Is<Expression<Func<BusinessCardModel, bool>>>(e => true),
                It.IsAny<string>()
            )).Returns(mockData);

            // Act
            var result = _controller.GetBusinessCards("Ali", null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var cards = Assert.IsAssignableFrom<List<BusinessCardDto>>(okResult.Value);
            Assert.Single(cards);
            Assert.Equal("Ali", cards[0].Name);
        }
    }
    
}
