using BlueChallenge.Api.Model.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueChallenge.Api.Data.Configurations;

public class ScheduleModelConfiguration : IEntityTypeConfiguration<ScheduleModel>
{
    public void Configure(EntityTypeBuilder<ScheduleModel> builder)
    {
        builder.HasKey(schedule => schedule.Id);

        builder.OwnsOne(schedule => schedule.DateRange);
        builder.OwnsOne(schedule => schedule.HourRange);

        builder.HasOne(schedule => schedule.User)
               .WithMany()
               .HasForeignKey("UserId");
    }
}
