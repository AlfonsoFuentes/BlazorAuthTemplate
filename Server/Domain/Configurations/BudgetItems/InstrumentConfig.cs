using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Instruments;

namespace Server.Domain.Configurations.BudgetItems
{
    internal class InstrumentConfig : IEntityTypeConfiguration<Instrument>
    {
        public void Configure(EntityTypeBuilder<Instrument> builder)
        {
         //   builder.HasOne(x => x.InstrumentTemplate)
         //        .WithMany(t => t.Instruments)
         //        .HasForeignKey(e => e.InstrumentTemplateId)

         //.OnDelete(DeleteBehavior.NoAction);

           // builder.HasMany(x => x.InstrumentItems)
           //.WithOne(t => t.Instrument)
           //.HasForeignKey(e => e.InstrumentId)
           //   .IsRequired()
           //   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
