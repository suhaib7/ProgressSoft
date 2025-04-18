using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.User;

namespace ProgressSoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO model)
        {
            var signUpUser = await _unitOfWork.User.SignUp(model);

            if (signUpUser == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(new { token = signUpUser });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var loginUser = await _unitOfWork.User.Login(model);
            if (loginUser == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { token = loginUser });
        }
    }
}
