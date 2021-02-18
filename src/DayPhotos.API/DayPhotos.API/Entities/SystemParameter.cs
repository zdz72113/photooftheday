using DayPhoto.API.Common;
using DayPhoto.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DayPhoto.API.Entities
{
    public partial class SystemParameter : BaseEntity<Guid>, IAduitEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Remark { get; set; }
        public bool IsUserMaintanance { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class SystemParameterConfiguration : IEntityTypeConfiguration<SystemParameter>
    {
        public void Configure(EntityTypeBuilder<SystemParameter> builder)
        {
            builder.Property(t => t.Key).HasMaxLength(50).IsRequired();
            builder.Property(t => t.Value).HasMaxLength(50).IsRequired();
            builder.Property(t => t.Remark).HasMaxLength(500);

            builder.Property(t => t.CreatedBy).HasMaxLength(50);
            builder.Property(t => t.UpdatedBy).HasMaxLength(50);

            //builder.HasData(new SystemParameter
            //{
            //    Id = Guid.Parse(Constants.SystemParameter.ThreadBeforeDrillTolerantValueId),
            //    Key = "ThreadBeforeDrillTolerantValue",
            //    Value = "0",
            //    Remark = "ThreadBeforeDrillTolerantValue",
            //    IsUserMaintanance = true,
            //    CreatedBy = "System",
            //    CreatedOn = DateTime.MinValue,
            //    UpdatedBy = "System",
            //    UpdatedOn = DateTime.MinValue
            //});
        }
    }
}
