using Server.Domain.CommonEntities.BudgetItems;
using Shared.Enums.ProjectNeedTypes;

namespace Server.Domain.CommonEntities
{
    public class Project : Entity, ITennant
    {

        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public override bool IsTennanted => true;

        public DateTime? StartDate { get; set; }


        public int ProjectNeedType { get; set; }
        public int CostCenter { get; set; }
        public int Focus { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public ProjectStatusEnum StatusEnum => ProjectStatusEnum.GetType(Status);
        public string ProjectNumber { get; set; } = string.Empty;
        public List<ProjectDefinitionItem> DefinitionItems { get; set; } = new();
        public double PercentageEngineering { get; set; }
        public double PercentageContingency { get; set; }
        public double PercentageTaxProductive { get; set; }
        public bool IsProductiveAsset { get; set; } = true;
        public int LastVisitedPhase { get; set; } = 1;
        #region Start Management
        public List<HazopNode> HazopNodes { get; set; } = new();
        //public List<Objective> Objectives { get; set; } = new();
        public List<Requirement> Requirements { get; set; } = new();
        //public List<Scope> Scopes { get; set; } = new();
        //public List<AcceptanceCriteria> AcceptanceCriterias { get; set; } = new();
        //public List<Bennefit> Bennefits { get; set; } = new();
        //public List<Constrainst> Constrainsts { get; set; } = new();
        //public List<Assumption> Assumptions { get; set; } = new();
        public List<LearnedLesson> LearnedLessons { get; set; } = new();   //Esta no debe ser dependiente del projecto
        public List<ExpertJudgement> ExpertJudgements { get; set; } = new();
        public List<KnownRisk> KnownRisks { get; set; } = new();
        public List<Quality> Qualitys { get; set; } = new();
        public List<BudgetItem> BudgetItems { get; set; } = new();
        public List<StakeHolder> StakeHolders { get; } = [];
        public List<RiskMatrix> RiskMatrixs { get; set; } = new();
        #endregion
        #region Timeline
        //public List<Deliverable> Deliverables { get; set; } = new();
        public virtual ICollection<GanttTask> GanttTasks { get; set; } = new List<GanttTask>();

        #endregion


        #region Communication
        public List<Communication> Communications { get; set; } = new();
        #endregion
        #region Resources
        public List<Resource> Resources { get; set; } = new();
        #endregion

        public List<OtherTask> OtherTasks { get; set; } = new();
        //public List<Meeting> Meetings { get; set; } = new();
        public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
        [NotMapped]
        public List<PurchaseOrder> CapitalPurchaseOrders => PurchaseOrders == null || PurchaseOrders.Count == 0 ? new() :
            PurchaseOrders.Where(x => !x.IsAlteration).ToList();
        [NotMapped]
        public List<PurchaseOrder> AlterationPurchaseOrders => PurchaseOrders == null || PurchaseOrders.Count == 0 ? new() :
            PurchaseOrders.Where(x => x.IsAlteration).ToList();
        public List<Acquisition> Acquisitions { get; set; } = new();

        public List<MonitoringLog> MonitoringLogs { get; set; } = new();

        [NotMapped]
        public List<BudgetItem> Expenses => BudgetItems == null || BudgetItems.Count == 0 ? new() : BudgetItems.Where(x => x.IsExpense).ToList();

        [NotMapped]
        public List<BudgetItem> Capital => BudgetItems == null || BudgetItems.Count == 0 ? new() : BudgetItems.Where(x => x.IsCapital).ToList();

        [NotMapped]
        public List<BudgetItem> Appropiation => [.. Expenses, .. Capital];
        [NotMapped]
        public decimal CapitalBudgetUSD => Capital == null || Capital.Count == 0 ? 0 : Capital.Sum(x => x.BudgetUSD);

        [NotMapped]
        public decimal ExpensesBudgetUSD => Expenses == null || Expenses.Count == 0 ? 0 : Expenses.Sum(x => x.BudgetUSD);


        //[NotMapped]
        //public List<BasicEquipmentItem> BasicEquipmentItems => BasicEngineeringItems == null || BasicEngineeringItems.Count == 0 ? new() : BasicEngineeringItems.OfType<BasicEquipmentItem>().ToList();
        //[NotMapped]
        //public List<BasicInstrumentItem> BasicInstrumentItems => BasicEngineeringItems == null || BasicEngineeringItems.Count == 0 ? new() : BasicEngineeringItems.OfType<BasicInstrumentItem>().ToList();
        //[NotMapped]
        //public List<BasicValveItem> BasicValveItems => BasicEngineeringItems == null || BasicEngineeringItems.Count == 0 ? new() : BasicEngineeringItems.OfType<BasicValveItem>().ToList();
        //[NotMapped]
        //public List<BasicPipeItem> BasicPipeItem => BasicEngineeringItems == null || BasicEngineeringItems.Count == 0 ? new() : BasicEngineeringItems.OfType<BasicPipeItem>().ToList();
        //[NotMapped]
        //public List<BasicEngineeringItem> ProcessDiagramComponents => [.. BasicEquipmentItems, .. BasicInstrumentItems, .. BasicValveItems, .. BasicPipeItem];

    }
    internal class ProjectConfig : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

            //   builder.HasMany(x => x.AcceptanceCriterias)
            //.WithOne(t => t.Project)
            //.HasForeignKey(e => e.ProjectId)
            // .IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.OtherTasks)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.HazopNodes)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);



            builder.HasMany(x => x.MonitoringLogs)
            .WithOne(t => t.Project)
            .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.BackGrounds)
            //.WithOne(t => t.Project)
            //.HasForeignKey(e => e.ProjectId)
            // .IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

            // builder.HasMany(x => x.Bennefits)
            //.WithOne(t => t.Project)
            //.HasForeignKey(e => e.ProjectId)
            // .IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.RiskMatrixs)
           .WithOne(t => t.Project)
           .HasForeignKey(e => e.ProjectId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);
            //    builder.HasMany(x => x.Objectives)
            //.WithOne(t => t.Project)
            //.HasForeignKey(e => e.ProjectId)
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

            //     builder.HasMany(x => x.Constrainsts)
            //     .WithOne(t => t.Project)
            //     .HasForeignKey(e => e.ProjectId)
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.DefinitionItems)
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

            //   builder.HasMany(x => x.Scopes)
            //.WithOne(t => t.Project)
            //.HasForeignKey(e => e.ProjectId)
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

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



            builder.HasMany(e => e.StakeHolders)
                  .WithMany(e => e.Projects);



            builder.HasMany(x => x.PurchaseOrders)
          .WithOne(t => t.Project)
          .HasForeignKey(e => e.ProjectId)
           .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

            // ✅ AGREGAR ESTO: Relación con la nueva tabla GanttTask
            // Asegúrate de tener public List<GanttTask> GanttTasks { get; set; } en tu clase Project
            builder.HasMany(x => x.GanttTasks)
               .WithOne(t => t.Project)
               .HasForeignKey(e => e.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        }

    }

}
