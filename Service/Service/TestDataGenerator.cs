using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.BusinessCard;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Font = System.Drawing.Font;

namespace Service.Service
{
    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;

    public class PhotoGenerator
    {
        public static string GetSamplePhoto(string name, string gender)
        {
            // Generate a unique color based on the name
            string backgroundColor = GetColorForName(name);
            string genderColor = gender.ToLower() == "male" ? "#3498db" : "#e74c3c";
            string initials = GetInitials(name);

            // Create an SVG with a colored background, avatar shape, and initials
            string svg = $@"<svg xmlns='http://www.w3.org/2000/svg' width='200' height='200' viewBox='0 0 200 200'>
            <defs>
                <linearGradient id='grad1' x1='0%' y1='0%' x2='100%' y2='100%'>
                    <stop offset='0%' style='stop-color:{backgroundColor};stop-opacity:1' />
                    <stop offset='100%' style='stop-color:{genderColor};stop-opacity:1' />
                </linearGradient>
            </defs>
            <rect width='200' height='200' fill='url(#grad1)'/>
            <circle cx='100' cy='85' r='40' fill='white' opacity='0.9'/>
            <path d='M100 130 C 60 130 60 200 100 200 C 140 200 140 130 100 130' fill='white' opacity='0.9'/>
            <text x='100' y='100' 
                font-family='Arial, Helvetica, sans-serif' 
                font-size='40' 
                fill='#333333'
                text-anchor='middle' 
                dominant-baseline='middle'
                font-weight='bold'>{initials}</text>
        </svg>";

            // Convert SVG to Base64
            byte[] svgBytes = Encoding.UTF8.GetBytes(svg);
            return Convert.ToBase64String(svgBytes);
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "??";

            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
            }
            return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : parts[0].ToUpper();
        }

        private static string GetColorForName(string name)
        {
            // Generate a consistent color based on the name
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
                // Use first 3 bytes for RGB values
                return $"#{hash[0]:X2}{hash[1]:X2}{hash[2]:X2}";
            }
        }
    }

    // Test class to verify the generated photos
    public class TestPhotoGenerator
    {
        public static void GenerateAndSaveTestPhotos()
        {
            var testData = new[]
            {
            new { Name = "John Doe", Gender = "Male" },
            new { Name = "Jane Smith", Gender = "Female" },
            new { Name = "Bob Johnson", Gender = "Male" },
            new { Name = "Sarah Williams", Gender = "Female" }
        };

            foreach (var person in testData)
            {
                string photo = PhotoGenerator.GetSamplePhoto(person.Name, person.Gender);

                // Save as SVG file
                string fileName = $"avatar_{person.Name.Replace(" ", "_")}.svg";
                File.WriteAllBytes(fileName, Convert.FromBase64String(photo));

                // Also save the Base64 string to a text file for inspection
                File.WriteAllText($"{fileName}.txt", photo);
            }
        }
    }

    // Modified TestDataGenerator to use the new PhotoGenerator
    public class TestDataGenerator
    {
        public static void GenerateTestFiles()
        {
            var cards = new List<BusinessCardModel>
        {
            new BusinessCardModel
            {
                Name = "John Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 15),
                Email = "john.doe@email.com",
                Phone = "1234567890",
                Photo = PhotoGenerator.GetSamplePhoto("John Doe", "Male"),
                Address = "123 Main St, City, Country"
            },
            new BusinessCardModel
            {
                Name = "Jane Smith",
                Gender = "Female",
                DateOfBirth = new DateTime(1985, 6, 22),
                Email = "jane.smith@email.com",
                Phone = "9876543210",
                Photo = PhotoGenerator.GetSamplePhoto("Jane Smith", "Female"),
                Address = "456 Oak Ave, Town, Country"
            }
        };

            GenerateCSV(cards, "test_business_cards.csv");
            GenerateXML(cards, "test_business_cards.xml");
        }

        private static void GenerateCSV(List<BusinessCardModel> cards, string fileName)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Name,Gender,DateOfBirth,Email,Phone,Photo,Address");

            foreach (var card in cards)
            {
                string escapedAddress = $"\"{card.Address.Replace("\"", "\"\"")}\"";
                string escapedPhoto = $"\"{card.Photo.Replace("\"", "\"\"")}\"";

                csv.AppendLine($"{card.Name},{card.Gender},{card.DateOfBirth:yyyy-MM-dd},{card.Email},{card.Phone},{escapedPhoto},{escapedAddress}");
            }

            File.WriteAllText(fileName, csv.ToString());
        }

        private static void GenerateXML(List<BusinessCardModel> cards, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<BusinessCardModel>));
            using (var writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, cards);
            }
        }
    }
}
