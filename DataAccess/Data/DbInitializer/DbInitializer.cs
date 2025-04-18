using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Utility;

namespace DataAccess.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        public DbInitializer(ApplicationDbContext db)
        {
            _db = db;
        }
        public void Initialize()
            {
                try
                {
                    if (_db.Database.GetPendingMigrations().Count() > 0)
                    {
                        _db.Database.Migrate();
                    }

                    if (!_db.Users.Any(u => u.Name == "sa"))
                    {
                        var superAdmin = new UserModel
                        {
                            Id = -1,
                            Name = "sa",
                            Email = "SuperAdmin@gmail.com",
                            PasswordHash = PasswordUtils.HashPassword("1")
                        };

                        _db.Users.Add(superAdmin);
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex) { }
            }
    }
}
