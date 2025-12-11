using Server.DataContext;

namespace Server.Services.Repositories
{
    public interface IRepositoryGetNextOrder
    {
        Task<int> GetNextOrderAsync<T>(string cache,Guid ProjectId) where T : class;
        
    }
    public class RepositoryGetNextOrder : IRepositoryGetNextOrder
    {
        private readonly IAppDbContext _context;
       
        public RepositoryGetNextOrder(IAppDbContext context)
        {
            _context = context;
         
        }
        public async Task<int> GetNextOrderAsync<T>(string cache, Guid ProjectId) where T : class
        {
            var rows = await _context.GetOrAddCacheAsync(async () =>
            {
                return await _context.Set<T>()
              .AsNoTracking()
              .AsQueryable()
              .ToListAsync();
            }, cache);
            if(rows == null || rows.Count == 0)
            {
                return 1;
            }
            var maxOrder = rows!
                .Where(x => (Guid)x.GetType().GetProperty("ProjectId")!.GetValue(x)! == ProjectId)
                .Select(x => (int)x.GetType().GetProperty("Order")!.GetValue(x)!)
                .DefaultIfEmpty(0)
                .Max();
            return maxOrder + 1;
        }
    }
}
