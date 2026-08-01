using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class MesaRepository : Repository<Mesa>, IMesaRepository
    {
        public MesaRepository(SampleRestaurantDbContext context) : base(context) { }

    }
}