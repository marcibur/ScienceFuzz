using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScienceFuzz.Web.Pages.Tools
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidFileExtensions : ValidationAttribute
    {
        private readonly IEnumerable<string> _extensions;

        public ValidFileExtensions(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                    return ValidationResult.Success;

                case IFormFile file:
                    return ValidateFile(file);

                case IList<IFormFile> files:
                    return ValidateFiles(files);

                default:
                    return new ValidationResult("Property that attribute is used on is not of type IFormFile or IList<IFormFile>.");
            }
        }

        private ValidationResult ValidateFile(IFormFile file)
        {
            bool isValid = false;

            foreach (var extension in _extensions)
            {
                if (file.FileName.EndsWith(extension) == true)
                {
                    isValid = true;
                }
            }

            if (isValid)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"File '{file.FileName}' doesn't have the proper extension. Allowed extensions: {GetValidExtensions()}");
            }
        }

        private ValidationResult ValidateFiles(IList<IFormFile> images)
        {
            foreach (var image in images)
            {
                var errorMessage = ValidateFile(image)?.ErrorMessage;
                if (errorMessage != null)
                {
                    return new ValidationResult(errorMessage);
                }

                //bool isValid = false;

                //foreach (var extension in _imageExtensions)
                //{
                //    if (image.FileName.EndsWith(extension) == true)
                //    {
                //        isValid = true;
                //    }
                //}

                //if (isValid == false)
                //{
                //    return new ValidationResult($"File '{image.FileName}'  doesn't have the proper extension. Allowed extensions: {GetValidExtensions()}");
                //}
            }

            return ValidationResult.Success;
        }

        private string GetValidExtensions()
        {
            var stringBuilder = new StringBuilder();

            foreach (var extension in _extensions)
            {
                stringBuilder.AppendFormat("'.{0}', ", extension);
            }

            return stringBuilder.ToString();
        }
    }
}
