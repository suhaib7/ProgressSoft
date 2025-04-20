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
        public List<BusinessCardModel> Parse(IFormFile file, bool saveCards = false)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            using var stream = new StreamReader(file.OpenReadStream());

            return extension switch
            {
                ".csv" => ParseCsv(stream,saveCards),
                ".xml" => ParseXml(file.OpenReadStream(),saveCards),
                _ => throw new NotSupportedException("Unsupported file format")
            };
        }

        private List<BusinessCardModel> ParseXml(Stream stream, bool saveCards)
        {
            var serializer = new XmlSerializer(typeof(List<BusinessCardModel>));
            var cards = (List<BusinessCardModel>)serializer.Deserialize(stream)!;

            if (saveCards)
            {
                foreach (var card in cards)
                {
                    if (!string.IsNullOrEmpty(card.Photo))
                    {
                        card.Photo = SaveImageLocally(card.Photo);
                    }
                }
            }

            return cards;
        }

        private List<BusinessCardModel> ParseCsv(StreamReader reader, bool saveCards)
        {
            var cards = new List<BusinessCardModel>();
            string? line;
            bool isFirst = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirst) { isFirst = false; continue; }
                var parts = line.Split(';');

                string photoUrl = parts[6];
                string photoPath = photoUrl;

                if (saveCards)
                {
                    photoPath = SaveImageLocally(photoUrl);
                }

                cards.Add(new BusinessCardModel
                {
                    Name = parts[0],
                    Gender = parts[1],
                    DateOfBirth = DateTime.Parse(parts[2]),
                    Email = parts[3],
                    Phone = parts[4],
                    Address = parts[5],
                    Photo = photoPath
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

                // Check if the image URL is valid and starts with "http://"
                if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uriResult) ||
                    !(uriResult.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                      uriResult.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine($"Invalid or unsupported image URL: {imageUrl}");
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

                // Read the saved image as a byte array
                byte[] fileBytes = File.ReadAllBytes(fullPath);

                // Convert the byte array to a base64 string
                string base64String = Convert.ToBase64String(fileBytes);

                // Return the base64 encoded string (for storing in DB or API response)
                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save image: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
