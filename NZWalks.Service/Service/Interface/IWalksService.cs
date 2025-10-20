using NZWalks.Model.Models.DTO;

namespace NZWalks.Service.Service.Interface;

public interface IWalksService
{
    Task<WalkDTO> CreateAsync(AddWalkRequestDTO addWalkRequestDTO);
    Task<IEnumerable<WalkDTO>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending, int pageNumber, int pageSize);
    Task<WalkDTO?> GetByIdAsync(Guid id);
    Task<WalkDTO?> UpdateAsync(Guid id, UpdateWalkRequestDTO updateWalkRequestDTO);
    Task<WalkDTO?> DeleteAsync(Guid id);
}