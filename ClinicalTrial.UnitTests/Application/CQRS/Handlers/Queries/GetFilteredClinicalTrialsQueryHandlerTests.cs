using ClinicalTrial.Application.CQRS.Handlers.Queries;
using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClinicalTrial.UnitTests.Application.CQRS.Handlers.Queries
{
    [TestClass]
    public class GetFilteredClinicalTrialsQueryHandlerTests
    {
        private Mock<IRepository<Domain.Entities.ClinicalTrial>> _repositoryMock;
        private Mock<ILogger<GetFilteredClinicalTrialsQueryHandler>> _loggerMock;
        private GetFilteredClinicalTrialsQueryHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Domain.Entities.ClinicalTrial>>();
            _loggerMock = new Mock<ILogger<GetFilteredClinicalTrialsQueryHandler>>();
            _handler = new GetFilteredClinicalTrialsQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFilteredClinicalTrials()
        {
            // Arrange
            var query = new GetFilteredClinicalTrialsQuery
            (
                "Active",
                10,
                DateTime.UtcNow.AddMonths(-1),
                DateTime.UtcNow
            );

            var expectedTrials = new List<Domain.Entities.ClinicalTrial>
            {
                new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Status = "Active", Participants = 20 }
            };

            _repositoryMock.Setup(r => r.GetAllFilteredAsync(query.status, query.minParticipants, query.startDate, query.endDate))
                           .ReturnsAsync(expectedTrials);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTrials.Count, result.Count());
            Assert.AreEqual(expectedTrials.First().Id, result.First().Id);

            _repositoryMock.Verify(r => r.GetAllFilteredAsync(query.status, query.minParticipants, query.startDate, query.endDate), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnEmptyList_WhenNoClinicalTrialsMatch()
        {
            // Arrange
            var query = new GetFilteredClinicalTrialsQuery
            (
                "Inactive",
                50,
                DateTime.UtcNow.AddMonths(-1),
                DateTime.UtcNow
            );

            _repositoryMock.Setup(r => r.GetAllFilteredAsync(query.status, query.minParticipants, query.startDate, query.endDate))
                           .ReturnsAsync(new List<ClinicalTrial.Domain.Entities.ClinicalTrial>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            _repositoryMock.Verify(r => r.GetAllFilteredAsync(query.status, query.minParticipants, query.startDate, query.endDate), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnAllClinicalTrials_WhenFiltersAreNull()
        {
            // Arrange
            var query = new GetFilteredClinicalTrialsQuery(null, null, null, null);

            var expectedTrials = new List<Domain.Entities.ClinicalTrial>
            {
                new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Status = "Active", Participants = 20 },
                new Domain.Entities.ClinicalTrial { Id = Guid.NewGuid(), Status = "Inactive", Participants = 50 }
            };

            _repositoryMock.Setup(r => r.GetAllFilteredAsync(null, null, null, null))
                           .ReturnsAsync(expectedTrials);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTrials.Count, result.Count());

            _repositoryMock.Verify(r => r.GetAllFilteredAsync(null, null, null, null), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldPropagateException_WhenRepositoryThrowsException()
        {
            // Arrange
            var query = new GetFilteredClinicalTrialsQuery(null, null, null, null);

            var exception = new Exception("Repository failure");

            _repositoryMock.Setup(r => r.GetAllFilteredAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                           .ThrowsAsync(exception);

            // Act & Assert
            var actualException = await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.AreEqual("Repository failure", actualException.Message);

            _repositoryMock.Verify(r => r.GetAllFilteredAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }
    }
}
