using Eventa_BusinessObject;
using Eventa_Services.Interfaces;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace Eventa_Services.Implements
{
    public class FirebaseService : IFirebaseService
    {
        private readonly string _storageBucket;

        public FirebaseService(IOptions<FirebaseSetting> firebaseSettings)
        {
            var settings = firebaseSettings.Value;

            // Set the storage bucket from the settings
            _storageBucket = settings.StorageBucket;

            if (string.IsNullOrEmpty(_storageBucket))
            {
                throw new InvalidOperationException("Firebase Storage bucket is not configured.");
            }
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            try
            {
                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Create a StorageClient using the service account key
                var storageClient = StorageClient.Create(GoogleCredential.FromFile("Helper/firebase-adminsdk.json"));

                // Upload the file to Firebase Storage
                using var stream = file.OpenReadStream();
                await storageClient.UploadObjectAsync(
                    bucket: _storageBucket,
                    objectName: fileName,
                    contentType: file.ContentType,
                    source: stream
                );

                // Return the public URL of the uploaded file
                return $"https://firebasestorage.googleapis.com/v0/b/{_storageBucket}/o/{Uri.EscapeDataString(fileName)}?alt=media";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}", ex);
            }
        }
    }
}
