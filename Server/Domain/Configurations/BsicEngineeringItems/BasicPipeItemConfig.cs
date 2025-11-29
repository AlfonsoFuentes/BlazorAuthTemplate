namespace Server.Domain.Configurations.BsicEngineeringItems
{
    //internal class BasicPipeItemConfig : IEntityTypeConfiguration<BasicPipeItem>
    //{
    //    public void Configure(EntityTypeBuilder<BasicPipeItem> builder)
    //    {
    //        builder.HasMany(x => x.IsometricItems)
    //          .WithOne(t => t.BasicPipeItem)
    //          .HasForeignKey(e => e.BasicPipeItemId)
    //             .IsRequired()
    //             .OnDelete(DeleteBehavior.Cascade);


    //        builder.HasOne(c => c.FluidCode).WithMany(t => t.BasicPipeItems).HasForeignKey(x => x.BasicFluidCodeId)
    //            .OnDelete(DeleteBehavior.NoAction);

    //        builder.HasOne(c => c.PipeTemplate).WithMany(t => t.BasicPipeItems).HasForeignKey(x => x.BasicPipeTemplateId)
    //      .OnDelete(DeleteBehavior.NoAction);

    //    }
    //}
}
