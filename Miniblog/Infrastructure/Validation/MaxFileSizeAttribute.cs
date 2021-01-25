using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.Infrastructure.Validation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long maxSize;
        public MaxFileSizeAttribute(long maxFileSize)
        {
            maxSize = maxFileSize;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile formFile)
            {
                if (formFile.Length > maxSize)
                {
                    string errorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : $"Maximum allowed file size is {maxSize / 1024 / 1024} MB";
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
