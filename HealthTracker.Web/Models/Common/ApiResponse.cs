namespace HealthTracker.Web.Models.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; } // This holds the User/Token object
        public string DisplayMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
