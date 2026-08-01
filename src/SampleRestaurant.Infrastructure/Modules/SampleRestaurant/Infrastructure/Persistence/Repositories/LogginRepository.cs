using WebApiCoreSeed.SampleRestaurant.Interfaces.Repository;
using WebApiCoreSeed.SampleRestaurant.Models;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Repository
{
    public class LogginRepository : Repository<LogginEntity>, ILogginRepository
    {
        public LogginRepository(SampleRestaurantDbContext context) : base(context) { }

    }
}