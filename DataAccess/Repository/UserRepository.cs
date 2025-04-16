using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models.User;

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
    }
}
