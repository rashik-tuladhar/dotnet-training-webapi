using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DlmsWebApi.Helpers
{
    public static class BookImageHelper
    {
        private static readonly string SecureKey = "SuperSecretLibrarySystemKey123!@#"; // Cryptographic key

        public static async Task<string?> UploadImageAsync(IFormFile? file, string webRootPath)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        public static string GetBookImageView(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                // A beautiful modern SVG placeholder book cover with a warm pastel linear gradient!
                return "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 150' width='100' height='150'><defs><linearGradient id='grad' x1='0%' y1='0%' x2='100%' y2='100%'><stop offset='0%' style='stop-color:%234f46e5;stop-opacity:1' /><stop offset='100%' style='stop-color:%2306b6d4;stop-opacity:1' /></linearGradient></defs><rect width='100' height='150' rx='8' fill='url(%23grad)'/><rect x='8' y='10' width='84' height='130' rx='4' fill='none' stroke='white' stroke-width='1.5' stroke-opacity='0.3'/><path d='M30 45 L70 45 M30 60 L70 60 M30 75 L55 75' stroke='white' stroke-width='3' stroke-linecap='round' stroke-opacity='0.8'/><path d='M35 110 L65 110' stroke='white' stroke-width='1.5' stroke-linecap='round' stroke-opacity='0.5'/></svg>";
            }
            return imageUrl;
        }

        public static string GenerateSecureImageUrl(string? imageUrl, IUrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return GetBookImageView(imageUrl); // Return the gorgeous default SVG placeholder
            }

            // Get the actual filename (e.g. "/uploads/guid_name.jpg" -> "guid_name.jpg")
            var fileName = Path.GetFileName(imageUrl);

            // Generate cryptographic HMAC-SHA256 signature
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecureKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(fileName));
                var signature = Convert.ToBase64String(hash);

                // Return the URL to our secure controller action
                return urlHelper.Action("ViewSecureImage", "Book", new { fileName = fileName, signature = signature }) ?? imageUrl;
            }
        }

        public static bool VerifySignature(string fileName, string signature)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(signature))
            {
                return false;
            }

            // Prevent path traversal by strictly validating that the name is a pure filename
            var cleanFileName = Path.GetFileName(fileName);
            if (cleanFileName != fileName)
            {
                return false; // Block traversal attempts (e.g. ../../etc/passwd)
            }

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecureKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(fileName));
                var expectedSignature = Convert.ToBase64String(hash);

                // Fixed-time comparison to completely prevent cryptographic timing attacks
                var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);
                var providedBytes = Encoding.UTF8.GetBytes(signature);

                if (expectedBytes.Length != providedBytes.Length)
                {
                    return false;
                }

                return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
            }
        }
    }
}