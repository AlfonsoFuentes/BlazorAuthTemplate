using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Domain.Configurations
{
    //internal class MeetingConfig : IEntityTypeConfiguration<Meeting>
    //{
    //    public void Configure(EntityTypeBuilder<Meeting> builder)
    //    {
    //        builder.HasKey(ci => ci.Id);

    //        builder.HasMany(x => x.MeetingAttendants)
    //            .WithOne(t => t.Meeting)
    //            .HasForeignKey(e => e.MeetingId)
    //            .IsRequired()
    //            .OnDelete(DeleteBehavior.Cascade);

    //        builder.HasMany(x => x.MeetingAgreements)
    //        .WithOne(t => t.Meeting)
    //        .HasForeignKey(e => e.MeetingId)
    //        .IsRequired()
    //        .OnDelete(DeleteBehavior.Cascade);
    //    }

    //}
}
