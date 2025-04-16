using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models.BusinessCard;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccess.Repository
{
    public class BusinessCardRepository : Repository<BusinessCardModel>, IBusinessCardRepository
    {
        private ApplicationDbContext _db;
        public BusinessCardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(BusinessCardModel obj)
        {
            _db.BusinessCards.Update(obj);
        }
    }
}
