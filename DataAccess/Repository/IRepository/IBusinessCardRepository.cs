using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.BusinessCard;

namespace DataAccess.Repository.IRepository
{
    public interface IBusinessCardRepository : IRepository<BusinessCardModel>
    {
        void Update(BusinessCardModel businessCard);
    }
}
