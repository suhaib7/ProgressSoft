using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.BusinessCard;
using Service.Interface;
using System.Xml.Serialization;

namespace Service.Service
{
    public class FileExportService : IFileExportService
    {
        public byte[] ToCsv(List<BusinessCardModel> cards)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Name,DOB,Phone,Gender,Email,Address,PhotoBase64");
            foreach (var card in cards)
            {
                builder.AppendLine($"{card.Name},{card.DateOfBirth:yyyy-MM-dd},{card.Phone},{card.Gender},{card.Email},{card.Address},{card.Photo}");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ToXml(List<BusinessCardModel> cards)
        {
            var serializer = new XmlSerializer(typeof(List<BusinessCardModel>));
            using var ms = new MemoryStream();
            serializer.Serialize(ms, cards);
            return ms.ToArray();
        }
    }
}
