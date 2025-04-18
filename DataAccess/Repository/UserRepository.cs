using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Models.User;
using Service.Service;
using Utility;

namespace DataAccess.Repository
{
    public class UserRepository : Repository<UserModel>, IUserRepository
    {
        private ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserModel user)
        {
            _db.Users.Update(user);
        }

        public async Task<string> SignUp(SignUpDTO model)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return null;
            }

            var hashedPassword = PasswordUtils.HashPassword(model.Password);

            var newUser = new UserModel
            {
                Email = model.Email,
                Name = model.Username,
                PasswordHash = hashedPassword
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            var token = GenerateToken(newUser.Id.ToString(), model.Email);
            return token;
        }

        public async Task<string> Login(LoginDTO model)
        {
            if (model.Email == "sa")
                model.Email = "superadmin@gmail.com";

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return null;
            }

            var isPasswordValid = PasswordUtils.VerifyPassword(user.PasswordHash,model.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            var token = GenerateToken(user.Id.ToString(), model.Email);

            return token;
        }

        private string GenerateToken(string userId, string userEmail)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7852cabb306efe753b6060709c4596f830cf04475536cefa762188cd206e717ef1a4475f01d44601c23073ac040bd0d7b7547b2d3cf33c3288d935e799b3bd6fb1f1819b1adda2fcf00ae28a7fd3e7be46b125752e5e870d2f6ff5e2c692c4d2638eb29cb21d81d2e95e6d1ca071de81a3ce6106859d0a322c8ba8ce1b1194691e0e303ba03647d9ca24564fa838778f69e8d434e7ec0121a3053ff973f4d756884c7e86f3731a3cadae281c4fa8330df5ad3172bbe491485ef1c9cc6292ed302e4f4e384ce855a0437c86453af049b63689113f8663bfe95b9f51629f0ac61a1b88a223d240b5738de9c7ea702827cdc091475d237be6870566f5d7f165e399"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
