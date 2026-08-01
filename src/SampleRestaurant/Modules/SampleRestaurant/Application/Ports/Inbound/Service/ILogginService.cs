using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces.Service
{
    public interface ILogginService : IDisposable
    {
        Task<bool> Adicionar(LogginEntity mesa);
    }
}