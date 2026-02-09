using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.ApplicationConstants
{
    public class ApplicationConstant
    {
    }
    public class CommonMessage
    {
        public const string RegistrationSuccess = "Registration Success";
        public const string RegistrationFailed = "Registration Failed";

        public const string LoginSuccess = "Login Success";
        public const string LoginFailed = "Login Failed";

        public const string CreateOperationSuccess = "Record Created Successfully";
        public const string UpdateOperationSuccess = "Record Updated Successfully";
        public const string DeleteOperationSuccess = "Record Deleted Successfully";

        public const string CreateOperationFailed = "Create Operation Failed";
        public const string UpdateOperationFailed = "Update Operation Failed";
        public const string DeleteOperationFailed = "Delete Operation Failed";

        public const string RecordNotFound = "Record Not Found";
        public const string AlreadyRegistered = "Email already registered";
        public const string SystemError = "Something Went Wrong";
    }
}
