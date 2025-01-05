using ClinicalTrial.Application.CQRS.Handlers.Queries;
using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClinicalTrial.Application.Tests.CQRS.Handlers.Queries
{
    [TestClass]
    public class GetClinicalTrialByIdQueryHandlerTests
    {
        private Mock<IRepository<Domain.Entities.ClinicalTrial>> _repositoryMock;
        private Mock<ILogger<GetClinicalTrialByIdQueryHandler>> _loggerMock;
        private GetClinicalTrialByIdQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Domain.Entities.ClinicalTrial>>();
            _loggerMock = new Mock<ILogger<GetClinicalTrialByIdQueryHandler>>();
            _handler = new GetClinicalTrialByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnClinicalTrial_WhenIdIsValid()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var query = new GetClinicalTrialByIdQuery(testGuid);
            var expectedTrial = new Domain.Entities.ClinicalTrial { Id = testGuid };
            _repositoryMock.Setup(r => r.GetByIdAsync(testGuid)).ReturnsAsync(expectedTrial);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(expectedTrial, result);
            _repositoryMock.Verify(r => r.GetByIdAsync(testGuid), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnNull_WhenClinicalTrialNotFound()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var query = new GetClinicalTrialByIdQuery(testGuid);
            _repositoryMock.Setup(r => r.GetByIdAsync(testGuid)).ReturnsAsync((Domain.Entities.ClinicalTrial)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
            _repositoryMock.Verify(r => r.GetByIdAsync(testGuid), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var query = new GetClinicalTrialByIdQuery(testGuid);
            _repositoryMock.Setup(r => r.GetByIdAsync(testGuid)).ThrowsAsync(new Exception("Repository failure"));

            // Act
            await _handler.Handle(query, CancellationToken.None);
        }
    }
}
