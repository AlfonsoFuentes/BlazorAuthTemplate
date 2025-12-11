using Azure.Core;
using Server.DataContext;
using Server.Services;
using Shared.Dtos.Starts.Contingencys;
using Shared.Dtos.Starts.Engineerings;
using Shared.Dtos.Starts.Foundations;
using Shared.Dtos.Starts.Investments;
using Shared.Dtos.Starts.Taxs;
namespace Server.Services.Repositories
{
    public interface IRepositoryInvestmentCalculation
    {
        Task CalculateEngineeringTotalCost(Guid ProjectId);
        Task<double> GetCapitalBudget(Guid ProjectId);
        Task<double> GetCapitalBudgetForTaxes(Guid ProjectId);
        Task<double> GetContingencyPercentage(Guid ProjectId);
        Task<double> GetEngineeringPercentage(Guid ProjectId);
    }
    public class RepositoryInvestmentCalculation : IRepositoryInvestmentCalculation
    {
        private readonly IAppDbContext _context;

        public RepositoryInvestmentCalculation(IAppDbContext context)
        {
            this._context = context;
        }
        public async Task CalculateEngineeringTotalCost(Guid ProjectId)
        {
            var engineering = await _context.EngineeringSalarys.OrderBy(x => x.Order).FirstOrDefaultAsync(x => x.ProjectId == ProjectId);
            var contingency = await _context.Contingencys.OrderBy(x => x.Order).FirstOrDefaultAsync(x => x.ProjectId == ProjectId);
            var taxes = await _context.Taxes.OrderBy(x => x.Order).FirstOrDefaultAsync(x => x.ProjectId == ProjectId);



            var project = await _context.Projects
                 .Include(x => x.BudgetItems)

               .AsSplitQuery()
               .AsNoTracking()
               .AsQueryable()
               .FirstOrDefaultAsync(x => x.Id == ProjectId);

            if (project == null)
            {
                return;
            }
            var budgetUSD = project.CapitalWOTaxesBudgetUSD;
            if (taxes != null)
            {
                taxes.BudgetUSD = budgetUSD * (taxes.Percentage / 100);
                budgetUSD += taxes.BudgetUSD;
            }
            double totalengineringcontingency = contingency?.Percentage ?? 0;
            totalengineringcontingency += engineering?.Percentage ?? 0;

            if (contingency != null)
            {
                contingency.BudgetUSD = budgetUSD * contingency.Percentage / (100 - totalengineringcontingency);
            }
            if (engineering != null)
            {
                engineering.BudgetUSD = budgetUSD * (engineering.Percentage / (100 - totalengineringcontingency));
            }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var cacheKeyGetInvestmentById = $"{typeof(GetInvestmentById).Name}-{ProjectId}"; //TODO: cuando se pueda tiparlo mejor
                var cacheKeyContingencyAll = $"{typeof(GetAllContingencys).Name}{ProjectId}";
                var cacheKeyEngineeringAll = $"{typeof(GetAllEngineeringSalarys).Name}{ProjectId}";
                var cacheKeyTaxAll = $"{typeof(GetAllTaxs).Name}{ProjectId}";
                var cacheKeyContingencyId = $"{typeof(GetContingencyById).Name}-{contingency?.Id}";
                var cacheKeyEngineeringId = $"{typeof(GetEngineeringSalaryById).Name}-{engineering?.Id}";
                var cacheKeyTaxId = $"{typeof(GetTaxById).Name}-{taxes?.Id}";


                _context.InvalidateCache(cacheKeyGetInvestmentById, cacheKeyContingencyAll, cacheKeyEngineeringAll, cacheKeyTaxAll, cacheKeyContingencyId, cacheKeyEngineeringId, cacheKeyTaxId);
            }
        }
        public async Task<double> GetCapitalBudget(Guid ProjectId)
        {
            var cacheKeyGetInvestmentById = $"{typeof(GetInvestmentById).Name}-{ProjectId}"; //TODO: cuando se pueda tiparlo mejor
            var project = await _context.GetOrAddCacheAsync(async () =>
            {
                return await _context.Projects
                .Include(x => x.BudgetItems)

              .AsSplitQuery()
              .AsNoTracking()
              .AsQueryable()
              .FirstOrDefaultAsync(x => x.Id == ProjectId);

            }, cacheKeyGetInvestmentById);
            return project?.CapitalBudgetUSD ?? 0;
        }
        public async Task<double> GetCapitalBudgetForTaxes(Guid ProjectId)
        {
            var cacheKeyGetInvestmentById = $"{typeof(GetInvestmentById).Name}-{ProjectId}"; //TODO: cuando se pueda tiparlo mejor
            var project = await _context.GetOrAddCacheAsync(async () =>
            {
                return await _context.Projects
                .Include(x => x.BudgetItems)

              .AsSplitQuery()
              .AsNoTracking()
              .AsQueryable()
              .FirstOrDefaultAsync(x => x.Id == ProjectId);

            }, cacheKeyGetInvestmentById);
            return project?.CapitalWOTaxesBudgetUSD ?? 0;
        }

       public async Task<double> GetEngineeringPercentage(Guid ProjectId)
        {
            var cacheKey = $"{typeof(GetAllEngineeringSalarys).Name}{ProjectId}";
            var engineering = await _context.GetOrAddCacheAsync(async () =>
            {
                return await _context.EngineeringSalarys
                .OrderBy(x => x.Order)
              .AsSplitQuery()
              .AsNoTracking()
              .AsQueryable()
              .Where(x => x.ProjectId == ProjectId)
              .ToListAsync();
            }, cacheKey);
            return engineering?.FirstOrDefault()?.Percentage ?? 0;
        }
      public  async Task<double> GetContingencyPercentage(Guid ProjectId)
        {
            var cacheKey = $"{typeof(GetAllContingencys).Name}{ProjectId}";
            var contingencies = await _context.GetOrAddCacheAsync(async () =>
            {
                return await _context.Contingencys
                .OrderBy(x => x.Order)
              .AsSplitQuery()
              .AsNoTracking()
              .AsQueryable()
              .Where(x => x.ProjectId == ProjectId)
              .ToListAsync();
            }, cacheKey);
            return contingencies?.FirstOrDefault()?.Percentage ?? 0;
        }
    }

}

