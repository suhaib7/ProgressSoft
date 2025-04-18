using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.BusinessCard;

namespace Service.Interface
{
    public interface IFileExportService
    {
        byte[] ToCsv(List<BusinessCardModel> cards);
        byte[] ToXml(List<BusinessCardModel> cards);
    }
}
