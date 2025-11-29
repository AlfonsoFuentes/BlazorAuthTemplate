namespace Server.Domain.Configurations.BudgetItems
{
    internal class TaxesItemConfig : IEntityTypeConfiguration<TaxesItem>
    {
        public void Configure(EntityTypeBuilder<TaxesItem> builder)
        {


            builder.HasOne(c => c.Selected)
                .WithMany(t => t.TaxesSelecteds)
                .HasForeignKey(x => x.SelectedId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
