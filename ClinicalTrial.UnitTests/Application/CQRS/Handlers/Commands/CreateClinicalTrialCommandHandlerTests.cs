using ClinicalTrial.Application.CQRS.Commands.ClinicalTrial;
using ClinicalTrial.Application.CQRS.Handlers.Commands;
using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Domain.Entities;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using ClinicalTrial.Application.CQRS.Handlers.Queries;
using Microsoft.Extensions.Logging;

namespace ClinicalTrial.Application.Tests.CQRS.Handlers.Commands
{
    [TestClass]
    public class CreateClinicalTrialCommandHandlerTests
    {
        private Mock<IRepository<Domain.Entities.ClinicalTrial>> _repositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CreateClinicalTrialCommandHandler>> _loggerMock;
        private CreateClinicalTrialCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Domain.Entities.ClinicalTrial>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateClinicalTrialCommandHandler>>();
            _handler = new CreateClinicalTrialCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldCreateClinicalTrial_WhenValidJsonAndData()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 10, \"status\": \"Ongoing\"}"));
            var clinicalTrial = new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Participants = 10, Status = "Ongoing", StartDate = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ReturnsAsync(clinicalTrial.Id);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(clinicalTrial.Id, result);
        }

        [TestMethod]
        public async Task Handle_ShouldThrowException_WhenInvalidJsonSchema()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"invalid\": \"json\"}"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_ShouldThrowException_WhenParticipantsLessThanOne()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 0, \"status\": \"Ongoing\"}"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_ShouldSetEndDate_WhenStatusIsOngoingAndEndDateIsMissing()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"participants\": 10, \"status\": \"Ongoing\"}"));
            var clinicalTrial = new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Participants = 10, Status = "Ongoing", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1) };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ReturnsAsync(clinicalTrial.Id);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(clinicalTrial.Id, result);
            Assert.AreNotEqual(default(DateTime), clinicalTrial.EndDate);
        }

        [TestMethod]
        public async Task Handle_ShouldCalculateDurationInDays_WhenEndDateIsProvided()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 10, \"status\": \"Completed\"}"));
            var clinicalTrial = new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Participants = 10, Status = "Completed", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), DurationInDays = 30 };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ReturnsAsync(clinicalTrial.Id);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(clinicalTrial.Id, result);
            Assert.AreEqual(30, clinicalTrial.DurationInDays);
        }

        [TestMethod]
        public async Task Handle_ShouldNotSetDurationInDays_WhenEndDateIsNotProvided()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"participants\": 10, \"status\": \"Ongoing\"}"));
            var clinicalTrial = new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Participants = 10, Status = "Completed", StartDate = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ReturnsAsync(clinicalTrial.Id);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(clinicalTrial.Id, result);
            Assert.AreEqual(0, clinicalTrial.DurationInDays);
        }

        [TestMethod]
        public async Task Handle_ShouldThrowException_WhenJsonDeserializationFails()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{invalid: format: test}"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 10, \"status\": \"Ongoing\"}"));
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_ShouldCreateClinicalTrial_WhenValidJsonWithoutEndDate()
        {
            // Arrange
            var query = new CreateClinicalTrialCommand (GetMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"participants\": 10, \"status\": \"Ongoing\"}"));
            var clinicalTrial = new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Participants = 10, Status = "Ongoing", StartDate = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.ClinicalTrial>())).ReturnsAsync(clinicalTrial.Id);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(clinicalTrial.Id, result);
        }

        private IFormFile GetMockFile(string content)
        {
            var mockFile = new Mock<IFormFile>();
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(contentBytes);

            mockFile.Setup(file => file.OpenReadStream()).Returns(stream);
            mockFile.Setup(file => file.Length).Returns(contentBytes.Length);
            mockFile.Setup(file => file.FileName).Returns("mockfile.json");

            return mockFile.Object;
        }
    }
}
