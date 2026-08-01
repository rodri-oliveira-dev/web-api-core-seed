using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PratoRepository : Repository<Prato>, IPratoRepository
    {
        public PratoRepository(SampleRestaurantDbContext context) : base(context) { }

    }
}