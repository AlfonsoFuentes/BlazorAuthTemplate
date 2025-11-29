using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Pipings;

namespace Server.Domain.Configurations.BudgetItems.PipingTempates
{
    internal class IsometricConfig : IEntityTypeConfiguration<Pipe>
    {
        public void Configure(EntityTypeBuilder<Pipe> builder)
        {
    


          //  builder.HasOne(c => c.FluidCode).WithMany(t => t.Isometrics).HasForeignKey(x => x.FluidCodeId)
          //      .OnDelete(DeleteBehavior.NoAction);

          //  builder.HasOne(c => c.PipeTemplate).WithMany(t => t.Isometrics).HasForeignKey(x => x.PipeTemplateId)
          //.OnDelete(DeleteBehavior.NoAction);

          //  builder.HasMany(x => x.PipeItems)
          //.WithOne(t => t.Pipe)
          //.HasForeignKey(e => e.PipeId)
          //   .IsRequired()
          //   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
