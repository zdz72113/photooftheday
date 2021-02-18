using DayPhoto.API.Infrastructure.Data;
using DayPhotos.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DayPhoto.API.Entities
{
    public class AppDbContext : DbContext
    {
        private string userName;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            this.userName = "*";
            //Database.EnsureCreated();
            //if (Database.GetPendingMigrations().Any())
            //{
            //    Database.Migrate();
            //}
        }

        public DbSet<SystemParameter> SystemParameters { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new SystemParameterConfiguration());
            modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.DetectChanges();

            var entities = ChangeTracker.Entries().Where(e => typeof(IAduitEntity).IsAssignableFrom(e.Entity.GetType()));
            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        {
                            (item.Entity as IAduitEntity).CreatedBy = this.userName;
                            (item.Entity as IAduitEntity).CreatedOn = DateTime.Now;
                            (item.Entity as IAduitEntity).UpdatedBy = this.userName;
                            (item.Entity as IAduitEntity).UpdatedOn = DateTime.Now;
                        }
                        break;
                    case EntityState.Modified:
                        {
                            (item.Entity as IAduitEntity).UpdatedBy = this.userName;
                            (item.Entity as IAduitEntity).UpdatedOn = DateTime.Now;
                        }
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            var entities = ChangeTracker.Entries().Where(e => typeof(IAduitEntity).IsAssignableFrom(e.Entity.GetType()));
            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        {
                            (item.Entity as IAduitEntity).CreatedBy = this.userName;
                            (item.Entity as IAduitEntity).CreatedOn = DateTime.Now;
                            (item.Entity as IAduitEntity).UpdatedBy = this.userName;
                            (item.Entity as IAduitEntity).UpdatedOn = DateTime.Now;
                        }
                        break;
                    case EntityState.Modified:
                        {
                            (item.Entity as IAduitEntity).UpdatedBy = this.userName;
                            (item.Entity as IAduitEntity).UpdatedOn = DateTime.Now;
                        }
                        break;
                }
            }

            return base.SaveChanges();
        }
    }
}
