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
    public class ImagesController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImagesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }


        // POST: /nzwalks/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDTO imageUploadRequestDTO)
        {
            ValidateFileUpload(imageUploadRequestDTO);

            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(imageUploadRequestDTO.File.FileName);

                var localFilePath = Path.Combine(
                  _webHostEnvironment.ContentRootPath,
                  "Images",
                  imageUploadRequestDTO.FileName + fileExtension
                  );

                using var stream = new FileStream(localFilePath, FileMode.Create);

                await imageUploadRequestDTO.File.CopyToAsync(stream);

                var httpCR = _httpContextAccessor.HttpContext.Request;

                var urlFilePath = $"{httpCR.Scheme}://{httpCR.Host}{httpCR.PathBase}/Images/{imageUploadRequestDTO.FileName}{fileExtension}";

                // Map DTO to Domain
                var image = new Image
                {
                    File = imageUploadRequestDTO.File,
                    FileName = imageUploadRequestDTO.FileName,
                    FileExtension = fileExtension,
                    FileSizeInBytes = imageUploadRequestDTO.File.Length,
                    FileDescription = imageUploadRequestDTO.Description,
                    FilePath = urlFilePath,
                };

                // Add image to database
                await _unitOfWork.ImageRepository.AddAsync(image);
                await _unitOfWork.SaveAsync();

                return Ok(image);
            }

            return BadRequest(ModelState);

        }

        private void ValidateFileUpload(ImageUploadRequestDTO imageUploadRequestDTO)
        {
            var allowedExtensions = new string[] { ".jpg", ".jped", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(imageUploadRequestDTO.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension!");
            }

            if (imageUploadRequestDTO.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file!");
            }

        }

    }
}
