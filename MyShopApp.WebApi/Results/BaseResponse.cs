using MyShopApp.WebApi.Filters;

namespace MyShopApp.WebApi.Results
{
    public class BaseResponse<TResult> : BaseResponse
    {
        public TResult? Result { get; set; }
    }

    public class BaseResponse
    {
        public ErrorInfo? Error { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(ErrorInfo error)
        {
            Error = error;
        }
    }
}
