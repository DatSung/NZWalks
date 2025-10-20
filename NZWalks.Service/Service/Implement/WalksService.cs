using AutoMapper;
using Microsoft.Extensions.Logging;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Model.Models.Domain;
using NZWalks.Model.Models.DTO;
using NZWalks.Service.Service.Interface;

namespace NZWalks.Service.Service.Implement
{
    public class WalksService : IWalksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WalksService> _logger;

        public WalksService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WalksService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WalkDTO> CreateAsync(AddWalkRequestDTO addWalkRequestDTO)
        {
            var walk = _mapper.Map<Walk>(addWalkRequestDTO);

            await _unitOfWork.WalkRepository.AddAsync(walk);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<WalkDTO>(walk);
        }

        public async Task<IEnumerable<WalkDTO>> GetAllAsync(
            string? filterOn, string? filterQuery, string? sortBy, bool? isAscending,
            int pageNumber, int pageSize)
        {
            var walks = await _unitOfWork.WalkRepository.GetAllAsync(includeProperties: "Region,Difficulty");

            // Filtering
            if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            {
                switch (filterOn.Trim().ToLower())
                {
                    case "name":
                        walks = walks.Where(x => x.Name.Contains(filterQuery, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "description":
                        walks = walks.Where(x => x.Description.Contains(filterQuery, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "lengthinkm":
                        if (double.TryParse(filterQuery, out var length))
                            walks = walks.Where(x => x.LengthInKm == length);
                        break;
                }
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                walks = sortBy.Trim().ToLower() switch
                {
                    "name" => isAscending == true ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name),
                    "description" => isAscending == true ? walks.OrderBy(x => x.Description) : walks.OrderByDescending(x => x.Description),
                    "lengthinkm" => isAscending == true ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm),
                    _ => walks
                };
            }

            // Pagination
            var skip = (pageNumber - 1) * pageSize;
            walks = walks.Skip(skip).Take(pageSize);

            return _mapper.Map<List<WalkDTO>>(walks);
        }

        public async Task<WalkDTO?> GetByIdAsync(Guid id)
        {
            var walk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id, includeProperties: "Region,Difficulty");
            return walk == null ? null : _mapper.Map<WalkDTO>(walk);
        }

        public async Task<WalkDTO?> UpdateAsync(Guid id, UpdateWalkRequestDTO updateWalkRequestDTO)
        {
            var walk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id);
            if (walk == null)
                return null;

            walk.Name = updateWalkRequestDTO.Name;
            walk.Description = updateWalkRequestDTO.Description;
            walk.LengthInKm = updateWalkRequestDTO.LengthInKm;
            walk.WalkImageUrl = updateWalkRequestDTO.WalkImageUrl;
            walk.DifficultyId = updateWalkRequestDTO.DifficultyId;
            walk.RegionId = updateWalkRequestDTO.RegionId;

            await _unitOfWork.SaveAsync();

            var updatedWalk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id, includeProperties: "Region,Difficulty");
            return _mapper.Map<WalkDTO>(updatedWalk);
        }

        public async Task<WalkDTO?> DeleteAsync(Guid id)
        {
            var walk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id);
            if (walk == null)
                return null;

            _unitOfWork.WalkRepository.Remove(walk);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<WalkDTO>(walk);
        }
    }
}
