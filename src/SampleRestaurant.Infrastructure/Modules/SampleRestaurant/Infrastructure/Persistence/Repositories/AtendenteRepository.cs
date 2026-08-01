using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class AtendenteRepository : Repository<Atendente>, IAtendenteRepository
    {
        public AtendenteRepository(SampleRestaurantDbContext context) : base(context) { }

    }
}