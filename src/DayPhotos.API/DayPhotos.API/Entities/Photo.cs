using DayPhoto.API.Infrastructure.Data;
using DayPhotos.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhotos.API.Entities
{
    public partial class Photo : BaseEntity<Guid>
    {
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Url2 { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public PhotoSource Source { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.Property(t => t.Url).HasMaxLength(200).IsRequired();
            builder.Property(t => t.Url2).HasMaxLength(200);
            builder.Property(t => t.Title).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.Copyright).HasMaxLength(200);
        }
    }
}
