using LazyCache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Server.Domain.CommonEntities;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Domain.Identities;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Claims;

namespace Server.DataContext
{
    public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
    {

        private readonly IAppCache _cache;
        string _tenantId = string.Empty;
        private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();
        public AppDbContext(DbContextOptions<AppDbContext> options, IAppCache cache, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _cache = cache;
            _tenantId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
           ?? "default";
        }


        public DbSet<GeneralLearnedLesson> GeneralLearnedLessons { get; set; } = null!;
        public DbSet<RiskMatrix> RiskMatrixs { get; set; } = null!;
        public DbSet<RiskMatrixComment> RiskMatrixComments { get; set; } = null!;
        public DbSet<OtherTask> OtherTasks { get; set; } = null!;
        public DbSet<ProjectDefinitionItem> ProjectDefinitionItems { get; set; }
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<BudgetItem> BudgetItems { get; set; } = null!;
        public DbSet<StakeHolder> StakeHolders { get; set; } = null!;
        public DbSet<HazopNode> HazopNodes { get; set; }
        public DbSet<HazopDetail> HazopDetails { get; set; }
        public DbSet<KnownRisk> KnownRisks { get; set; }

        public DbSet<MonitoringLog> MonitoringLogs { get; set; } = null!;

        public DbSet<Requirement> Requirements { get; set; } = null!;

        public DbSet<ExpertJudgement> ExpertJudgements { get; set; } = null!;
        public DbSet<RoleInsideProject> RoleInsideProjects { get; set; } = null!;

        public DbSet<LearnedLesson> LearnedLessons { get; set; } = null!;

        public DbSet<Brand> Brands { get; set; } = null!;


        public DbSet<Quality> Qualitys { get; set; } = null!;
        public DbSet<Communication> Communications { get; set; } = null!;
        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<Acquisition> Acquisitions { get; set; } = null!;

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
        public DbSet<PurchaseOrderItemReceived> PurchaseOrderItemReceiveds { get; set; } = null!;

        public DbSet<GanttTask> GanttTasks { get; set; }
        public DbSet<GanttDependency> GanttDependencys { get; set; }
        public DbSet<DelayCause> DelayCauses { get; set; }
        public DbSet<TaskChangeLog> TaskChangeLogs { get; set; }
        public DbSet<RiskResponseAction> RiskResponseActions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            ConfigureDatatTypes(builder);

            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            TenantedQueryFilter(builder);

        }
        void TenantedQueryFilter(ModelBuilder builder)
        {
            builder.Entity<Project>().HasQueryFilter(x => x.TenantId == _tenantId);
        }
        void ConfigureDatatTypes(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
           .SelectMany(t => t.GetProperties())
           .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            try
            {

                var entittes = ChangeTracker.Entries<IEntity>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();


                foreach (var row in entittes)
                {
                    if (row.State == EntityState.Added)
                    {
                        row.Entity.CreatedOn = DateTime.Now;

                        if (row.Entity.IsTennanted)
                        {
                            var entity = row.Entity as ITennant;
                            entity!.TenantId = _tenantId;
                        }

                    }

                }


                var result = await base.SaveChangesAsync();

                return result; // ✅ devuelve el valor real
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
            }
            return 0;
        }
        public async Task<T?> GetOrAddCacheAsync<T>(Func<Task<T?>> addItemFactory, string key, bool isTenanted = false) where T : class
        {

            var tenantPart = isTenanted ? $"-{_tenantId}" : "";

            var finalKey = $"{key}{tenantPart}";

            _cacheKeys.TryAdd(finalKey, 0);
            return await _cache.GetOrAddAsync(finalKey, addItemFactory); // ✅ Ahora acepta T?
        }
        public void InvalidateCache(params string[] keysToRemove)
        {


            foreach (var key in keysToRemove)
            {
                _cacheKeys.TryRemove(key, out _);
                _cache.Remove(key);
            }
        }
    }
}
