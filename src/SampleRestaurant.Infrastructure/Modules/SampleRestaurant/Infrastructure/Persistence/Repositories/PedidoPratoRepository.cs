using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class PedidoPratoRepository : Repository<PedidoPrato>, IPedidoPratoRepository
    {
        public PedidoPratoRepository(SampleRestaurantDbContext context) : base(context) { }

    }
}