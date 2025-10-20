using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Model.Models.DTO;
using NZWalks.Service.CustomActionFilters;
using NZWalks.Service.Service.Interface;

namespace NZWalks.API.Controllers
{
    [Route("nzwalks/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class WalksController : ControllerBase
    {
        private readonly IWalksService _walksService;
        private readonly ILogger<WalksController> _logger;

        public WalksController(IWalksService walksService, ILogger<WalksController> logger)
        {
            _walksService = walksService;
            _logger = logger;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDTO dto)
        {
            var result = await _walksService.CreateAsync(dto);
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            var result = await _walksService.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _walksService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalkRequestDTO dto)
        {
            var result = await _walksService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _walksService.DeleteAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
