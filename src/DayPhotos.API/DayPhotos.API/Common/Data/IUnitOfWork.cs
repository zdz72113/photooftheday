using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DayPhoto.API.Infrastructure.Data
{
    public interface IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        Task<int> SaveChangesAsync();

        int SaveChanges();
    }
}
