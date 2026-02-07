namespace MyShopApp.WebApi.Filters
{
    public class ErrorInfo
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ErrorInfo(string message)
        {
            Message = message;
        }

        public ErrorInfo(string code, string message) : this(message)
        { 
            Code = code;
        }
    }
}
