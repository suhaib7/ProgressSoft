using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Models.BusinessCard;

namespace Service.Interface
{
    public interface IFileParserService
    {
        List<BusinessCardModel> Parse(IFormFile file, bool saveCards = false);
    }
}
