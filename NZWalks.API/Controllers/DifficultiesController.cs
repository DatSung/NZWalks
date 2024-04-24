using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Model.Models.DTO;
using System.Data;

namespace NZWalks.API.Controllers
{
    [Route("nzwalks/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class DifficultiesController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DifficultiesController> _logger;

        public DifficultiesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DifficultiesController> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllV1()
        {
            _logger.LogInformation("Difficulties GetAll action was invoke.");

            var difficulties = await _unitOfWork.DifficultyRepository.GetAllAsync();

            var difficultiesDTO = _mapper.Map<List<DifficultyDTO>>(difficulties);

            if (difficultiesDTO == null)
            {
                return NotFound("There are no difficulties!");
            }

            _logger.LogInformation("Return result to user.");
            return Ok(difficultiesDTO);
        }

        [MapToApiVersion("2.0")]
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAllV2()
        {
            _logger.LogInformation("Difficulties GetAll action was invoke.");

            var difficulties = await _unitOfWork.DifficultyRepository.GetAllAsync();

            var difficultiesDTO = _mapper.Map<List<DifficultyDTO>>(difficulties);

            if (difficultiesDTO == null)
            {
                return NotFound("There are no difficulties!");
            }

            _logger.LogInformation("Return result to user.");
            return Ok(difficultiesDTO);
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetByIdV1([FromRoute] Guid id)
        {
            _logger.LogInformation("Difficulties GetById action was invoke.");

            var difficulty = await _unitOfWork.DifficultyRepository.GetAsync(x => x.Id == id);

            if (difficulty == null)
            {
                return NotFound("The difficult was not exist!");
            }

            var difficultyDTO = _mapper.Map<DifficultyDTO>(difficulty);

            _logger.LogInformation("Return result to user.");

            return Ok(difficultyDTO);
        }

        [MapToApiVersion("2.0")]
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetByIdV2([FromRoute] Guid id)
        {
            _logger.LogInformation("Difficulties GetById action was invoke.");

            var difficulty = await _unitOfWork.DifficultyRepository.GetAsync(x => x.Id == id);

            if (difficulty == null)
            {
                return NotFound("The difficult was not exist!");
            }

            var difficultyDTO = _mapper.Map<DifficultyDTO>(difficulty);

            _logger.LogInformation("Return result to user.");

            return Ok(difficultyDTO);
        }

    }
}
