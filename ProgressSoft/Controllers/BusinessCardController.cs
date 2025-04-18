using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Models.BusinessCard;
using Service.Interface;
using Service.Service;
using Utility;

namespace ProgressSoft.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileParserService _fileParserService;
        private readonly IFileExportService _fileExportService;
        private readonly IWebHostEnvironment _env;

        public BusinessCardController(IUnitOfWork unitOfWork, IFileParserService fileParserService, IFileExportService fileExportService, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _fileParserService = fileParserService;
            _fileExportService = fileExportService;
            _env = env;
        }

        [HttpGet("GetAll")]
        public IActionResult GetBusinessCards(string? name, string? dob, string? phone, string? gender, string? email)
        {
            var cards = _unitOfWork.BusinessCard.GetAll(x =>
                (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
                (string.IsNullOrEmpty(dob) || x.DateOfBirth == DateTime.Parse(dob)) &&
                (string.IsNullOrEmpty(phone) || x.Phone.Contains(phone)) &&
                (string.IsNullOrEmpty(gender) || x.Gender == gender) &&
                (string.IsNullOrEmpty(email) || x.Email.Contains(email))
            ).Select(x => new BusinessCardDto
            {
                Name = x.Name,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                Phone = x.Phone,
                Address = x.Address,
                Photo = string.IsNullOrEmpty(x.Photo)
                    ? null
                    : $"https://localhost:7137/{Base64Utils.Base64Decode(x.Photo).Replace("\\", "/")}"
            });

            return Ok(cards);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBusinessCard(int id)
        {
            var card = _unitOfWork.BusinessCard.Get(x => x.Id == id);
            if (card == null)
                return NotFound();

            _unitOfWork.BusinessCard.Remove(card);
            _unitOfWork.Save();

            return Ok();
        }

        [HttpPost("import")]
        public IActionResult ImportBusinessCards(IFormFile file)
        {
            var cards = _fileParserService.Parse(file, _env);
            foreach (var card in cards)
            {
                _unitOfWork.BusinessCard.Add(card);
            }
            _unitOfWork.Save();
            return Ok(new { Count = cards.Count });
        }

        [HttpGet("export")]
        public IActionResult Export(string format = "csv")
        {
            var cards = _unitOfWork.BusinessCard.GetAll().ToList();

            byte[] fileBytes;
            string contentType;
            string fileName;

            if (format.ToLower() == "xml")
            {
                fileBytes = _fileExportService.ToXml(cards); // Convert to XML bytes
                contentType = "application/xml";
                fileName = "BusinessCards.xml";
            }
            else
            {
                fileBytes = _fileExportService.ToCsv(cards); // Convert to CSV bytes
                contentType = "text/csv";
                fileName = "BusinessCards.csv";
            }

            return File(fileBytes, contentType, fileName);
        }
        [AllowAnonymous]
        [HttpGet("GenerateDummyData")]
        public async Task<IActionResult> GenerateDummyData()
        {
            TestPhotoGenerator.GenerateAndSaveTestPhotos();
            return Ok();
        }

    }
}
