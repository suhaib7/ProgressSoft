using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.BusinessCard;
using Service.Interface;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Hosting;
using Utility;

namespace Service.Service
{
    public class FileParserService : IFileParserService
    {

        public List<BusinessCardModel> Parse(IFormFile file, IWebHostEnvironment _env)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            using var stream = new StreamReader(file.OpenReadStream());

            return extension switch
            {
                ".csv" => ParseCsv(stream),
                ".xml" => ParseXml(file.OpenReadStream()),
                _ => throw new NotSupportedException("Unsupported file format")
            };
        }

        private List<BusinessCardModel> ParseCsv(StreamReader reader)
        {
            var cards = new List<BusinessCardModel>();
            string? line;
            bool isFirst = true;
            using var httpClient = new HttpClient();

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirst) { isFirst = false; continue; }
                var parts = line.Split(';');

                string photoUrl = parts[6];
                string relativePhotoPath = SaveImageLocally(photoUrl);

                cards.Add(new BusinessCardModel
                {
                    Name = parts[0],
                    Gender = parts[1],
                    DateOfBirth = DateTime.Parse(parts[2]),
                    Email = parts[3],
                    Phone = parts[4],
                    Address = parts[5],
                    Photo = relativePhotoPath
                });
            }
            return cards;
        }
        private string SaveImageLocally(string imageUrl)
        {
            try
            {
                // Use the current working directory (root of your API project)
                string projectDirectory = Directory.GetCurrentDirectory();

                // Specify the folder where images will be saved relative to the project directory
                string imageFolder = Path.Combine(projectDirectory, "BusinessCardsImages");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                // Check if the image URL is valid
                if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uriResult))
                {
                    Console.WriteLine($"Invalid image URL: {imageUrl}");
                    return string.Empty;
                }

                // Download the image data
                var imageBytes = new HttpClient().GetByteArrayAsync(uriResult).Result;

                // Generate a unique file name for the image
                string extension = Path.GetExtension(uriResult.AbsolutePath);
                string fileName = Guid.NewGuid().ToString() + extension;

                // Define the full file path
                string fullPath = Path.Combine(imageFolder, fileName);

                // Save the image to the specified folder
                File.WriteAllBytes(fullPath, imageBytes);

                // Return the relative path (so it's easy to use in the API response or DB)
                return Base64Utils.Base64Encode(Path.Combine("images", fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save image: {ex.Message}");
                return string.Empty;
            }
        }
        private List<BusinessCardModel> ParseXml(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(List<BusinessCardModel>));
            return (List<BusinessCardModel>)serializer.Deserialize(stream)!;
        }
    }
}
