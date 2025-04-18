using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IUserRepository User { get; private set; }
        public IBusinessCardRepository BusinessCard { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new UserRepository(_db);
            BusinessCard = new BusinessCardRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
