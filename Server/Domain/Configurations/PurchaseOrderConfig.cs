namespace Server.Domain.Configurations
{
    internal class PurchaseOrderConfig : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(ci => ci.Id);

           
            builder.HasOne(c => c.Supplier).WithMany(t => t.PurchaseOrders).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(c => c.PurchaseOrderItems).WithOne(t => t.PurchaseOrder).HasForeignKey(e => e.PurchaseOrderId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }

    }
}
