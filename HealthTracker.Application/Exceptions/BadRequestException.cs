using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public IDictionary<string, string[]> ValidationErrors { get; set; }
        public BadRequestException(string message) : base(message)
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }
        public BadRequestException(string message, ValidationResult validationResult) : base(message)
        {
            ValidationErrors = validationResult.ToDictionary();
        }
    }
    //public class BadRequestException : Exception
    //{
    //    public string[] Errors { get; }

    //    public BadRequestException(string message)
    //        : base(message)
    //    {
    //        Errors = Array.Empty<string>();
    //    }

    //    public BadRequestException(string message, string[] errors)
    //        : base(message)
    //    {
    //        Errors = errors;
    //    }
    //}
}
