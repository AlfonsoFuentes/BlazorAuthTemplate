namespace Server.Domain.Configurations.BudgetItems
{
    internal class TaxConfig : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {

            builder.HasMany(x => x.TaxesItems)
          .WithOne(t => t.TaxItem)
          .HasForeignKey(e => e.TaxItemId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
