using Server.Domain.CommonEntities;

namespace Server.Domain.Configurations
{
    //internal class AppConfig : IEntityTypeConfiguration<App>
    //{
    //    public void Configure(EntityTypeBuilder<App> builder)
    //    {

    //    }
    //}
    internal class ProjectConfig : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasMany(x => x.AcceptanceCriterias)
           .WithOne(t => t.Project)
           .HasForeignKey(e => e.ProjectId)
            .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(ci => ci.Id);
            builder.HasMany(x => x.OtherTasks)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(ci => ci.Id);
            builder.HasMany(x => x.Assumptions)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);


            builder.HasKey(ci => ci.Id);
            builder.HasMany(x => x.MonitoringLogs)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BackGrounds)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
             .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Bennefits)
           .WithOne(t => t.Project)
           .HasForeignKey(e => e.ProjectId)
            .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Objectives)
        .WithOne(t => t.Project)
        .HasForeignKey(e => e.ProjectId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Constrainsts)
    .WithOne(t => t.Project)
    .HasForeignKey(e => e.ProjectId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Deliverables)
          .WithOne(t => t.Project)
          .HasForeignKey(e => e.ProjectId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ExpertJudgements)
       .WithOne(t => t.Project)
       .HasForeignKey(e => e.ProjectId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.KnownRisks)
           .WithOne(t => t.Project)
           .HasForeignKey(e => e.ProjectId)
            .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.LearnedLessons)
        .WithOne(t => t.Project)
        .HasForeignKey(e => e.ProjectId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

         //   builder.HasMany(x => x.Meetings)
         //.WithOne(t => t.Project)
         //.HasForeignKey(e => e.ProjectId)
         //   .IsRequired()
         //   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Acquisitions)
       .WithOne(t => t.Project)
       .HasForeignKey(e => e.ProjectId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);


           

            builder.HasMany(x => x.Requirements)
     .WithOne(t => t.Project)
     .HasForeignKey(e => e.ProjectId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Scopes)
         .WithOne(t => t.Project)
         .HasForeignKey(e => e.ProjectId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Qualitys)
       .WithOne(t => t.Project)
       .HasForeignKey(e => e.ProjectId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Cascade);




            builder.HasMany(x => x.Resources)
         .WithOne(t => t.Project)
         .HasForeignKey(e => e.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Communications)
      .WithOne(t => t.Project)
      .HasForeignKey(e => e.ProjectId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BudgetItems)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

           // builder.HasMany(x => x.BasicEngineeringItems)
           //.WithOne(t => t.Project)
           //.HasForeignKey(e => e.ProjectId)
           //   .IsRequired()
           //   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.StakeHolders)
                  .WithMany(e => e.Projects);

          

            builder.HasMany(x => x.PurchaseOrders)
          .WithOne(t => t.Project)
          .HasForeignKey(e => e.ProjectId)
           .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
