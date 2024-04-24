using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NZWalks.DataAccess.Data;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Model.Models.Domain;
using NZWalks.Model.Models.DTO;
using NZWalks.Service.CustomActionFilters;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("nzwalks/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RegionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RegionsController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // GET ALL REGIONS
        // GET: https://localhost:portnumber/nzwalks/regions
        [MapToApiVersion("1.0")]
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAllRegions action method was invoked");

            // GetAsync data from database - Model.Models.Domain
            var regions = await _unitOfWork.RegionRepository.GetAllAsync();
            _logger.LogInformation($"Finish GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

            // Map Domain Model to DTOs
            var regionsDTO = _mapper.Map<List<RegionDTO>>(regions);
            _logger.LogInformation("Mapped Model to DTO");

            // Process
            if (regionsDTO.IsNullOrEmpty())
            {
                // Return NotFound
                _logger.LogWarning("GetAllRegions did not get any objects");
                return NotFound();
            }

            // Just throw an exception, the ExceptionHandlerMiddleware will handle the message!
            throw new Exception();

            // Return Ok DTOs
            _logger.LogInformation($"Return Regions for client: {regionsDTO}");
            return Ok(regionsDTO);

        }

        // GET A REGION BY ID
        // GET: https://localhost:portnumber/nzwalks/regions/{id}
        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Validation

            // GetAsync data from database - Model.Models.Domain
            var region = await _unitOfWork.RegionRepository.GetAsync(x => x.Id == id);

            // Map Domain Model to DTOs
            var regionDTO = _mapper.Map<RegionDTO>(region);

            // Process
            if (regionDTO == null)
            {
                // Return NotFound
                return NotFound();
            }

            // Return Ok DTOs
            return Ok(regionDTO);
        }

        // POST CREATE NEW REGION
        // POST: https://localhost:portnumber/nzwalks/regions
        [MapToApiVersion("1.0")]
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDTO addRegionRequestDTO)
        {
            try
            {

                // Map DTOs to Domain Model
                var region = _mapper.Map<Region>(addRegionRequestDTO);

                //Use Domain Model to create new Region
                await _unitOfWork.RegionRepository.AddAsync(region);
                await _unitOfWork.SaveAsync();

                //Map Domain Model to DTOs
                var regionDTO = _mapper.Map<RegionDTO>(region);

                // Return CreatedAtAction
                return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
            }
            catch (Exception ex)
            {
                // Return Internal Server Error
                return StatusCode(500);
            }
        }

        // PUT UPDATE A REGION
        // PUT: https://localhost:portnumber/nzwalks/regions/{id}
        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDTO updateRegionRequestDTO)
        {
            try
            {
                // Process
                var region = await _unitOfWork.RegionRepository.GetAsync(x => x.Id == id);

                if (region == null)
                {
                    return NotFound();
                }

                // Update Region
                region.Name = updateRegionRequestDTO.Name;
                region.Code = updateRegionRequestDTO.Code;
                region.RegionImageUrl = updateRegionRequestDTO.RegionImageUrl;

                await _unitOfWork.SaveAsync();

                // Map Model to DTO
                var regionDTO = _mapper.Map<RegionDTO>(region);

                // Return Ok with DTOs
                return Ok(regionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE DELETE A REGION
        // DELETE: https://localhost:portnumber/nzwalks/regions/{id}
        [MapToApiVersion("1.0")]
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Process
            var region = await _unitOfWork.RegionRepository.GetAsync(x => x.Id == id);
            if (region == null)
            {
                // Return NotFound
                return NotFound();
            }

            // Delete Region
            _unitOfWork.RegionRepository.Remove(region);
            await _unitOfWork.SaveAsync();

            // Map Model to DTO
            var regionDTO = _mapper.Map<RegionDTO>(region);

            // Return back deleted Region
            return Ok(regionDTO);
        }
    }
}