using System;
using System.Threading.Tasks;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Repository
{
    public interface ILogginRepository : IDisposable
    {
        Task<int> Registrar(LogginEntity log);
    }
}
