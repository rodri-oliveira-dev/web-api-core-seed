namespace Restaurante.IO.Api.Extensions
{
    public class CustomResult
    {

        public CustomResult(bool success, object data)
        {
            this.success = success;
            this.data = data;
        }
        public bool success { get; }

        /// <summary>
        /// Objeto de retorno da chamada
        /// </summary>
        public object data { get; }

    }
}