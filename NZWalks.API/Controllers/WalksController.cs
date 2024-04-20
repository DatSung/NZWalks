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
        // GET: /nzwalks/walks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var walksDTO = _mapper.Map<List<WalkDTO>>(
                    await _unitOfWork.WalkRepository.GetAllAsync(
                        includeProperties: "Region,Difficulty"));

                return Ok(walksDTO);
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