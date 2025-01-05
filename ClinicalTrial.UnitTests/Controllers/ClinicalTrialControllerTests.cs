using ClinicalTrial.Application.CQRS.Commands.ClinicalTrial;
using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;

namespace ClinicalTrial.UnitTests.Controllers
{
    [TestClass]
    public class ClinicalTrialControllerTests
    {
        private Mock<ISender> _senderMock;
        private ClinicalTrialController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _senderMock = new Mock<ISender>();
            _controller = new ClinicalTrialController(_senderMock.Object);
        }
        [TestMethod]
        public async Task UploadAsync_ShouldReturnOk_WithValidJsonFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.json");
            var resultId = Guid.NewGuid();
            _senderMock.Setup(s => s.Send(It.IsAny<CreateClinicalTrialCommand>(), default))
                       .ReturnsAsync(resultId);

            // Act
            var result = await _controller.UploadAsync(fileMock.Object) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual($"File processed successfully. Id of the processed file is: {resultId}", result.Value);
        }

        [TestMethod]
        public async Task UploadAsync_ShouldReturnBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadAsync(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid file type.", result.Value);
        }

        [TestMethod]
        public async Task UploadAsync_ShouldReturnBadRequest_OnNonJsonFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.txt");

            // Act
            var result = await _controller.UploadAsync(fileMock.Object) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid file type.", result.Value);
        }

        [TestMethod]
        public async Task UploadAsync_ShouldReturnBadRequest_WhenFileTypeIsInvalid()
        {
            // Arrange
            var invalidFile = new Mock<IFormFile>();
            invalidFile.Setup(f => f.FileName).Returns("invalid.txt");
            invalidFile.Setup(f => f.Length).Returns(100);

            // Act
            var result = await _controller.UploadAsync(invalidFile.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid file type.", badRequest.Value);
        }

        [TestMethod]
        public async Task UploadAsync_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            // Arrange
            var emptyFile = new Mock<IFormFile>();
            emptyFile.Setup(f => f.FileName).Returns("valid.json");
            emptyFile.Setup(f => f.Length).Returns(0);

            // Act
            var result = await _controller.UploadAsync(emptyFile.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid file type.", badRequest.Value);
        }

        [TestMethod]
        public async Task UploadAsync_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var validFile = new Mock<IFormFile>();
            validFile.Setup(f => f.FileName).Returns("valid.json");
            validFile.Setup(f => f.Length).Returns(100);
            _senderMock.Setup(s => s.Send(It.IsAny<CreateClinicalTrialCommand>(), default))
                       .ThrowsAsync(new Exception("Upload failed"));

            // Act
            var result = await _controller.UploadAsync(validFile.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;

            // Check the message directly
            var responseContent = badRequest.Value?.ToString();
            Assert.IsTrue(responseContent.Contains("Upload failed"));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnOk_WhenRecordExists()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var mockTrial = new Domain.Entities.ClinicalTrial { Id = validId, Title = "Trial 1" };
            _senderMock.Setup(s => s.Send(It.IsAny<GetClinicalTrialByIdQuery>(), default))
                       .ReturnsAsync(mockTrial);

            // Act
            var result = await _controller.GetByIdAsync(validId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(mockTrial, result.Value);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenRecordDoesNotExist()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _senderMock.Setup(s => s.Send(It.IsAny<GetClinicalTrialByIdQuery>(), CancellationToken.None))
                       .ReturnsAsync((Domain.Entities.ClinicalTrial)null);

            // Act
            var result = await _controller.GetByIdAsync(invalidId) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Clinical record not found.", result.Value);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnBadRequest_OnException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _senderMock.Setup(s => s.Send(It.IsAny<GetClinicalTrialByIdQuery>(), default))
                       .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetByIdAsync(id) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Database error", result.Value);
        }

        [TestMethod]
        public async Task GetFilterClinicalTrials_ShouldReturnOk_WithFilteredResults()
        {
            // Arrange
            var mockTrials = new List<Domain.Entities.ClinicalTrial>
        {
            new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Title = "Trial 1" },
            new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Title = "Trial 2" }
        };
            _senderMock.Setup(s => s.Send(It.IsAny<GetFilteredClinicalTrialsQuery>(), default))
                       .ReturnsAsync(mockTrials);

            // Act
            var result = await _controller.GetFilterClinicalTrials("Active", 10, null, null) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(mockTrials, result.Value);
        }

        [TestMethod]
        public async Task GetFilterClinicalTrials_ShouldReturnNotFound_WhenNoMatchingTrials()
        {
            // Arrange
            _senderMock.Setup(s => s.Send(It.IsAny<GetFilteredClinicalTrialsQuery>(), default))
                       .ReturnsAsync(new List<Domain.Entities.ClinicalTrial>());

            // Act
            var result = await _controller.GetFilterClinicalTrials("Inactive", 5, null, null) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("No trials found matching the given criteria.", result.Value);
        }

        [TestMethod]
        public async Task GetFilterClinicalTrials_ShouldReturnBadRequest_OnException()
        {
            // Arrange
            _senderMock.Setup(s => s.Send(It.IsAny<GetFilteredClinicalTrialsQuery>(), default))
                       .ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _controller.GetFilterClinicalTrials(null, null, null, null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Internal error", result.Value);
        }

        [TestMethod]
        public async Task GetFilterClinicalTrials_ShouldReturnOk_WhenTrialsMatchCriteria()
        {
            // Arrange
            var trials = new List<Domain.Entities.ClinicalTrial>
            {
                new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Title = "Trial 1" },
                new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Title = "Trial 2" }
            };

            _senderMock.Setup(s => s.Send(It.IsAny<GetFilteredClinicalTrialsQuery>(), default))
                       .ReturnsAsync(trials);

            // Act
            var result = await _controller.GetFilterClinicalTrials("Open", 10, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            CollectionAssert.AreEqual(trials, (ICollection)okResult.Value);
        }

    }
}
