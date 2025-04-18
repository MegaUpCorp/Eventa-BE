using Appwrite;
using Appwrite.Models;
using FirebaseAdmin;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace Eventa_Services.Util
{
    public static class QRCodeUtility
    {
        public static async Task<string> GenerateAndUploadQRCodeAsync(string qrData, string fileName, ILogger logger, IConfiguration configuration)
        {
            try
            {
                // Generate QR code
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                using var qrBitmap = qrCode.GetGraphic(20);

                // Save QR code to a memory stream
                using var memoryStream = new MemoryStream();
                qrBitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Retrieve the Firebase Storage bucket name from configuration
                var bucketName = configuration["Firebase:StorageBucket"];
                if (string.IsNullOrEmpty(bucketName))
                {
                    throw new InvalidOperationException("Firebase Storage bucket is not configured.");
                }

                // Create a StorageClient
                var storageClient = StorageClient.Create();

                // Upload the QR code to Firebase Storage
                var objectName = $"qrcodes/{fileName}";
                await storageClient.UploadObjectAsync(
                    bucket: bucketName,
                    objectName: objectName,
                    contentType: "image/png",
                    source: memoryStream
                );

                // Return the public URL of the uploaded QR code
                return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating or uploading QR code");
                throw new InvalidOperationException("Failed to generate or upload QR code.", ex);
            }
        }
    }
}
