namespace MissionG3_TwitterWeb_API.logic.Common
{
    public static class CommonMessage
    {
        public static string CurrentURL { get; set; }
        public static string AppURL { get; set; }
        public static string DefaultErrorMessage { get; set; } = "An error occurred while processing your request.";
        public static string SomethingWentWrong { get; set; } = "Something went wrong. Please refresh the page or try again later.";        
    }
}
