using Restaurante.IO.Api.Extensions;

namespace Restaurante.IO.Api.Results
{
    public class CustomErrorResult
    {
        public bool success { get; }

        public object data { get; }

        public CustomErrorResult(CustomResult customResult)
        {
            success = customResult.success;
            data = customResult.data;
        }
    }
}