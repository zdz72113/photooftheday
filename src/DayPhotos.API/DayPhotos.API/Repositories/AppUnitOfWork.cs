using DayPhoto.API.Entities;
using DayPhoto.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Repositories
{
    public class AppUnitOfWork : UnitOfWork<AppDbContext>
    {
        public AppUnitOfWork(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
