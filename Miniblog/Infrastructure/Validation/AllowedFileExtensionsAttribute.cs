using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Web.Infrastructure.Validation
{
    public class AllowedFileExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;
        public AllowedFileExtensionsAttribute(params string[] extensions)
        {
            if (extensions?.Any() ?? default)
                this.extensions = extensions;
            else
                this.extensions = new string[] { };
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile formFile)
            {
                string fileExtension = Path.GetExtension(formFile.FileName).TrimStart('.');
                if (extensions.Any() && !extensions.Contains(fileExtension))
                {
                    string errorMessage = !string.IsNullOrWhiteSpace(ErrorMessage) ? ErrorMessage : "This file extension is not allowed";
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
