namespace Server.Domain.Configurations.BsicEngineeringItems
{
    //internal class BasicEngineeringItemConfig : IEntityTypeConfiguration<BasicEngineeringItem>
    //{
    //    public void Configure(EntityTypeBuilder<BasicEngineeringItem> builder)
    //    {
    //        builder.HasMany(x => x.Nozzles)
    //          .WithOne(t => t.BasicEngineeringItem)
    //          .HasForeignKey(e => e.BasicEngineeringItemId)
    //             .IsRequired()
    //             .OnDelete(DeleteBehavior.Cascade);

    //        //builder
    //        //    .HasMany(m => m.BudgetItemNewGanttTasks) // Un hito tiene un padre
    //        //    .WithOne(m => m.SelectedBasicEngineeringItem) // Un padre puede tener muchos subhitos
    //        //    .HasForeignKey(m => m.SelectedBasicEngineeringItemId) // Clave foránea
    //        //    .OnDelete(DeleteBehavior.Restrict); // Evita la eliminación en cascada para evitar problemas



    //        //builder.HasMany(x => x.PurchaseOrderItems)
    //        //    .WithOne(x => x.BasicEngineeringItem)
    //        //    .HasForeignKey(x => x.BasicEngineeringItemId)
    //        //    .OnDelete(DeleteBehavior.Restrict);

    //    }
    //}
}
