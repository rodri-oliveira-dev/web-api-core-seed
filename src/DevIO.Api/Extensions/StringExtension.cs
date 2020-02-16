using System;

namespace Restaurante.IO.Api.Extensions
{
    public static class StringExtension
    {
        public static bool IsBase64String(this string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return false;
            }

            var buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}