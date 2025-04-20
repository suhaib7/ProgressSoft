using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.BusinessCard;
using Service.Interface;
using Utility;

namespace ProgressSoft.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileParserService _fileParserService;
        private readonly IFileExportService _fileExportService;

        public BusinessCardController(IUnitOfWork unitOfWork, IFileParserService fileParserService, IFileExportService fileExportService)
        {
            _unitOfWork = unitOfWork;
            _fileParserService = fileParserService;
            _fileExportService = fileExportService;
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
                Id = x.Id.ToString(),
                Name = x.Name,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth.ToString("dd/MM/yyyy"),
                Email = x.Email,
                Phone = x.Phone,
                Address = x.Address,
                Photo = string.IsNullOrEmpty(x.Photo)
                    ? ""
                    : $"https://localhost:7137/{Base64Utils.Base64Decode(x.Photo).Replace("\\", "/")}"
            }).ToList();

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
            var cards = _fileParserService.Parse(file,true);
            foreach (var card in cards)
            {
                _unitOfWork.BusinessCard.Add(card);
            }
            _unitOfWork.Save();
            return Ok(new { Count = cards.Count });
        }

        [HttpGet("export")]
        public IActionResult Export(string format = "csv", string cardIds = null)
        {
            IEnumerable<BusinessCardModel> cards;

            // If specific card IDs are provided, filter by those IDs
            if (!string.IsNullOrEmpty(cardIds))
            {
                var ids = cardIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();
                cards = _unitOfWork.BusinessCard.GetAll().Where(c => ids.Contains(c.Id)).ToList();
            }
            else
            {
                // Otherwise, get all cards
                cards = _unitOfWork.BusinessCard.GetAll().ToList();
            }

            byte[] fileBytes;
            string contentType;
            string fileName;

            if (format.ToLower() == "xml")
            {
                fileBytes = _fileExportService.ToXml(cards.ToList());
                contentType = "application/xml";
                fileName = $"BusinessCards_{DateTime.Now:yyyyMMdd}.xml"; // Changed date format
            }
            else
            {
                fileBytes = _fileExportService.ToCsv(cards.ToList());
                contentType = "text/csv";
                fileName = $"BusinessCards_{DateTime.Now:yyyyMMdd}.csv"; // Changed date format
            }

            // Add Content-Disposition header explicitly
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(fileBytes, contentType, fileName);
        }

        [HttpPost("ProcessFile")]
        public IActionResult ProcessFile(IFormFile file)
        {
            var cards = _fileParserService.Parse(file,false);
            return Ok(cards);
        }

    }
}
