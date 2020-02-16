using Microsoft.AspNetCore.Mvc;

namespace Restaurante.IO.Api.Extensions.Authorization
{
    public class CustomError
    {
        public bool success { get; }

        public object data { get; }

        public CustomError(CustomResult customResult)
        {
            success = customResult.success;
            data = customResult.data;
        }
    }
}