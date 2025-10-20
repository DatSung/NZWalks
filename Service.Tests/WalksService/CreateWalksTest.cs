using Microsoft.Extensions.DependencyInjection;
using NZWalks.DataAccess.Data;
using NZWalks.Model.Models.Domain;
using NZWalks.Model.Models.DTO;
using NZWalks.Service.Service.Interface;
using Service.Tests.TestBase;

namespace Service.Tests.WalksService
{
    [Collection("Test Collection")]
    public class CreateWalkTest : IAsyncLifetime
    {
        private readonly NZWalksDbContext _dbContext;
        private readonly Func<Task> _resetDatabase;
        private readonly IWalksService _walksService;

        public CreateWalkTest(StartUpTestBase fixture)
        {
            _dbContext = fixture.NZWalksDbContext;
            _resetDatabase = fixture.ResetDatabaseAsync;
            _walksService = fixture.ServiceProvider.GetRequiredService<IWalksService>();
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await _resetDatabase();

        [Fact]
        public async Task Handle_CreateWalkWithCorrectInput_ReturnsCreatedWalk()
        {
            // Arrange - seed Region và Difficulty trước
            var region = new Region
            {
                Id = Guid.NewGuid(),
                Name = "Test Region",
                Code = "TRG",
                RegionImageUrl = "https://example.com/r1.png"
            };
            var difficulty = new Difficulty
            {
                Id = Guid.NewGuid(),
                Name = "Easy"
            };

            await _dbContext.Regions.AddAsync(region);
            await _dbContext.Difficulties.AddAsync(difficulty);
            await _dbContext.SaveChangesAsync();

            // Tạo Walk dùng ID thật từ DB
            var addWalkDto = new AddWalkRequestDTO
            {
                Name = "Test Walk",
                Description = "This is a test walk",
                LengthInKm = 10.0,
                WalkImageUrl = "https://example.com/walk.png",
                RegionId = region.Id,         // ⚡ Dùng RegionId đã seed
                DifficultyId = difficulty.Id  // ⚡ Dùng DifficultyId đã seed
            };

            // Act
            var result = await _walksService.CreateAsync(addWalkDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addWalkDto.Name, result.Name);

            var walkInDb = await _dbContext.Walks.FindAsync(result.Id);
            Assert.NotNull(walkInDb);
        }
    }
}