using Microsoft.AspNetCore.WebUtilities;

namespace Restaurante.IO.Api.Resources
{
    internal static class HttpErrorMessages
    {
        public static string RetornaMensagemErro(int statusCode)
        {
            return statusCode switch
            {
                400 => "O pedido não pode ser cumprido devido à erro de sintaxe.",
                401 => "A chamada precisa ser efetuada por um usuario autenticado.",
                403 => "O usuário esta autenticado, mas o não possui permissão para executar essa ação.",
                404 => "A página solicitada não pôde ser encontrada, mas pode estar disponível novamente no futuro.",
                405 => "Foi feita uma solicitação de uma página usando um método de solicitação não suportado por essa página.",
                _ => ReasonPhrases.GetReasonPhrase(statusCode)
            };
        }
    }
}