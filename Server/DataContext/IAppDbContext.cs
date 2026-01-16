using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.BudgetItems;

namespace Server.DataContext
{
    public interface IAppDbContext
    {
        DbSet<T> Set<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<OtherTask> OtherTasks { get; set; } 
      
        DbSet<Project> Projects { get; set; } 
      
        DbSet<StakeHolder> StakeHolders { get; set; } 
       
        DbSet<KnownRisk> KnownRisks { get; set; }

        DbSet<MonitoringLog> MonitoringLogs { get; set; } 
      
        DbSet<Requirement> Requirements { get; set; } 
  
        DbSet<ExpertJudgement> ExpertJudgements { get; set; } 
        DbSet<RoleInsideProject> RoleInsideProjects { get; set; } 

        DbSet<LearnedLesson> LearnedLessons { get; set; }
   
        DbSet<Brand> Brands { get; set; } 
  
        DbSet<Quality> Qualitys { get; set; } 
        DbSet<Communication> Communications { get; set; } 
        DbSet<Resource> Resources { get; set; } 
        DbSet<Acquisition> Acquisitions { get; set; } 

        DbSet<PurchaseOrder> PurchaseOrders { get; set; } 
        DbSet<Supplier> Suppliers { get; set; } 
        DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } 
        DbSet<PurchaseOrderItemReceived> PurchaseOrderItemReceiveds { get; set; }
        DbSet<RiskMatrix> RiskMatrixs { get; set; }
        DbSet<RiskMatrixComment> RiskMatrixComments { get; set; }
       
        DbSet<GeneralLearnedLesson> GeneralLearnedLessons { get; set; }
        DbSet<GanttTask> GanttTasks { get; set; }
        DbSet<GanttDependency> GanttDependencys { get; set; }

         DbSet<ProjectDefinitionItem> ProjectDefinitionItems { get; set; }

        DbSet<DelayCause> DelayCauses { get; set; }
        DbSet<TaskChangeLog> TaskChangeLogs { get; set; }
        DbSet<RiskResponseAction> RiskResponseActions { get; set; }
        DbSet<BudgetItem> BudgetItems { get; set; }
        DbSet<HazopNode> HazopNodes { get; set; }

        Task<T?> GetOrAddCacheAsync<T>(Func<Task<T?>> addItemFactory, string key, bool IsTenanted = false) where T : class; // T sigue siendo class, pero el resultado puede ser null
        void InvalidateCache(params string[] types);
    }
}
