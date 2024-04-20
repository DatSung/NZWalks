using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Model.Models.Domain;
using NZWalks.Model.Models.DTO;
using NZWalks.Service.CustomActionFilters;

namespace NZWalks.API.Controllers
{
    [Route("nzwalks/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalksController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // CREATE A WALK
        // POST: /nzwalks/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDTO addWalkRequestDTO)
        {
            try
            {

                // Map DTO to Model
                var walk = _mapper.Map<Walk>(addWalkRequestDTO);

                // Create new walk
                await _unitOfWork.WalkRepository.AddAsync(walk);
                await _unitOfWork.SaveAsync();

                // Map Model to DTO
                var walkDTO = _mapper.Map<WalkDTO>(walk);

                // Return Ok
                return Ok(walkDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET ALL WALKS
        // GET: /nzwalks/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 3

            )
        {
            try
            {
                var walksDTO = new List<WalkDTO>();

                // Filter
                if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
                {

                    switch (filterOn.Trim().ToLower())
                    {
                        case "name":
                            {
                                walksDTO = _mapper.Map<List<WalkDTO>>(await _unitOfWork.WalkRepository.GetAllAsync(
                                    filter: x => x.Name.Contains(filterQuery), includeProperties: "Region,Difficulty"));

                                break;
                            }

                        case "description":
                            {
                                walksDTO = _mapper.Map<List<WalkDTO>>(await _unitOfWork.WalkRepository.GetAllAsync(
                                    filter: x => x.Description.Contains(filterQuery), includeProperties: "Region,Difficulty"));

                                break;
                            }

                        case "lengthtnkm":
                            {
                                walksDTO = _mapper.Map<List<WalkDTO>>(await _unitOfWork.WalkRepository.GetAllAsync(
                                    filter: x => x.LengthInKm == double.Parse(filterQuery), includeProperties: "Region,Difficulty"));

                                break;
                            }

                        default:
                            {
                                walksDTO = _mapper.Map<List<WalkDTO>>(await _unitOfWork.WalkRepository.GetAllAsync(
                                    includeProperties: "Region,Difficulty"));

                                break;
                            }
                    }
                }
                else
                {
                    walksDTO = _mapper.Map<List<WalkDTO>>(await _unitOfWork.WalkRepository.GetAllAsync(
                        includeProperties: "Region,Difficulty"));
                }

                // Sort
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.Trim().ToLower())
                    {
                        case "name":
                            {
                                walksDTO = isAscending == true ? [.. walksDTO.OrderBy(x => x.Name)] : [.. walksDTO.OrderByDescending(x => x.Name)];
                                break;
                            }

                        case "description":
                            {
                                walksDTO = isAscending == true ? [.. walksDTO.OrderBy(x => x.Description)] : [.. walksDTO.OrderByDescending(x => x.Description)];
                                break;
                            }

                        case "lengthinkm":
                            {
                                walksDTO = isAscending == true ? [.. walksDTO.OrderBy(x => x.LengthInKm)] : [.. walksDTO.OrderByDescending(x => x.LengthInKm)];
                                break;
                            }
                    }
                }

                // Pagination
                var skilResult = (pageNumber - 1) * pageSize;

                return Ok(walksDTO.Skip(skilResult).Take(pageSize));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET A WALK
        // GET: /nzwalks/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var walkDTO = _mapper.Map<WalkDTO>(
                    await _unitOfWork.WalkRepository.GetAsync(
                        filter: x => x.Id == id, includeProperties: "Region,Difficulty"));

                return Ok(walkDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // UPDATE A WALK 
        // PUT: /nzwalks/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDTO updateWalkRequestDTO)
        {
            try
            {

                var walk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id);

                if (walk == null)
                {
                    return NotFound();
                }

                //Update Walk
                walk.Name = updateWalkRequestDTO.Name;
                walk.Description = updateWalkRequestDTO.Description;
                walk.LengthInKm = updateWalkRequestDTO.LengthInKm;
                walk.WalkImageUrl = updateWalkRequestDTO.WalkImageUrl;
                walk.DifficultyId = updateWalkRequestDTO.DifficultyId;
                walk.RegionId = updateWalkRequestDTO.RegionId;

                await _unitOfWork.SaveAsync();

                var walkDTO = _mapper.Map<WalkDTO>(
                    await _unitOfWork.WalkRepository.GetAsync(
                        filter: x => x.Id == id, includeProperties: "Region,Difficulty"));

                return Ok(walkDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE A WALK
        // DELETE: /nzwalks/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var walk = await _unitOfWork.WalkRepository.GetAsync(x => x.Id == id);

                if (walk == null)
                {
                    return NotFound();
                }

                _unitOfWork.WalkRepository.Remove(walk);
                await _unitOfWork.SaveAsync();

                var walkDTO = _mapper.Map<WalkDTO>(walk);

                return Ok(walkDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}