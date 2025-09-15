namespace EasyGames.Models
{
    // ErrorViewModel to hold error information for the error view
    public class ErrorViewModel
    {
        // Unique identifier for the request
        public string? RequestId { get; set; } // Request ID for tracking errors

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Indicates if RequestId should be shown
    }
}
