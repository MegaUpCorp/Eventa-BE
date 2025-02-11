using Eventa_BusinessObject;
using Eventa_Services.Interfaces;
using Firebase.Storage;
using FirebaseAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly string _storageBucket;

        public FirebaseService(FirebaseApp firebaseApp, IOptions<FirebaseSetting> firebaseSettings)
        {
            _firebaseApp = firebaseApp ?? throw new ArgumentNullException(nameof(firebaseApp));
            _storageBucket = "hairsalon-588fe.appspot.com";
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            string uploadTask;

            // Check if the file is valid
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("One of the files is invalid.");
            }

            try
            {
                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var bucket = new FirebaseStorage(_storageBucket);

                // Upload the file stream to Firebase Storage
                using (var stream = file.OpenReadStream())
                {
                    uploadTask = await bucket.Child(fileName).PutAsync(stream);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            return uploadTask;
        }

    }
}
