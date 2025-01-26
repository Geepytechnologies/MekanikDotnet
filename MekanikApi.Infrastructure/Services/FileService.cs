using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Npgsql.BackendMessages;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    
    public class FileService
    {
        public static GenericTypeResponse<ImageDetailsDTo> UploadImageToCloudinary(IFormFile file)
        {
            Cloudinary cloudinary = new(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            if (file == null || file.Length == 0)
                return new GenericTypeResponse<ImageDetailsDTo>
                {
                    StatusCode = 404,
                    Message = "File is missing"
                };

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "mekanik",
                Transformation = new Transformation().Width(400).Height(400).Crop("fill")
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            if (uploadResult.StatusCode != HttpStatusCode.OK)
                return new GenericTypeResponse<ImageDetailsDTo>
                {
                    StatusCode = 400,
                    Message = "Upload failed",
                };
            var imageId = uploadResult.PublicId;
            var imageUrl = uploadResult.SecureUrl.ToString();

            var imageDetails = new ImageDetailsDTo
            {
                Id = imageId,
                Url = imageUrl,
            };

            return new GenericTypeResponse<ImageDetailsDTo>
            {
                StatusCode = 200,
                Message = "Image Uploaded",
                Result = imageDetails
            };
        }

        public static DeletionResult DeleteImage(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return null;
            }

            Cloudinary cloudinary = new(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;


            var deletionParams = new DeletionParams(publicId);

            var result = cloudinary.Destroy(deletionParams);
            return result;
        }
    }
}
