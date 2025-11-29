using Microsoft.EntityFrameworkCore.Query;
using Server.DataContext;
using Server.Interfaces;
using Shared.Dtos.General;
using System.Linq.Expressions;

namespace Server.Services
{
    //public interface IServerServices
    //{
    //    // ✅ CRUD básicos
    //    Task<GeneralDto> CreateAsync<T>(T entity, params Type[] additionalTypesToInvalidate) where T : class, IEntity;
    //    Task<GeneralDto> UpdateAsync<T>(T entity, params Type[] additionalTypesToInvalidate) where T : class, IEntity;

    //    // ✅ Consultas (sin caché, con tracking → para operaciones de escritura)
    //    Task<T?> FindAsync<T>(
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class, IEntity;

    //    // ✅ Consultas (con caché, sin tracking → para UI)
    //    Task<T?> GetByIdAsync<T>(Guid id,
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class, IEntity;

    //    Task<List<T>> GetAllAsync<T>(string CacheModifier = null!,
    //        Expression<Func<T, bool>>? filter = null,
    //         Expression<Func<T, object>> OrderBy = null!,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class, IEntity;

    //    Task<GeneralDto> OrderUpAsync<T>(Guid id) where T : class, IEntity;
    //    Task<GeneralDto> OrderDownAsync<T>(Guid id) where T : class, IEntity;
    //    Task<GeneralDto> DeleteRangeAsync<T>(List<Guid> ids, params Type[] additionalTypesToInvalidate) where T : class, IEntity;
    //    Task<GeneralDto> DeleteAsync<T>(Guid id, params Type[] additionalTypesToInvalidate) where T : class, IEntity;
    //    Task<bool> ValidateUniqueAsync<T, TValue>(
    //    TValue value,
    //    Expression<Func<T, TValue>> propertySelector,
    //    string CacheModifier = null!,
    //     Expression<Func<T, bool>>? filter = null,
    //    Guid? excludeId = null)
    //    where T : class, IEntity
    //    where TValue : notnull;
    //    Task<int> GetNextOrderAsync<T>(Expression<Func<T, bool>>? filter = null) where T : class, IEntity;
    //    Task ReindexOrderAsync<T>(string cachemodifier = null!, Expression<Func<T, bool>>? filter = null) where T : class, IEntity;
    //    void InvalidateCache(params string[] additionalTypes);
    //}

    //public class ServerServices : IServerServices
    //{
    //    private readonly IAppDbContext _context;
    //    private readonly ICache _cache;

    //    public ServerServices(IAppDbContext context, ICache cache)
    //    {
    //        _context = context;
    //        _cache = cache;
    //    }
    //    public async Task<int> GetNextOrderAsync<T>(Expression<Func<T, bool>>? filter = null) where T : class, IEntity
    //    {
    //        var getAll = await GetAllAsync(filter: filter);

    //        // ✅ Solo el cálculo de Order — confiamos en que el DbContext ya filtra por tenant y soft delete
    //        var maxOrder = getAll.Max(x => x.Order);

    //        return maxOrder + 1;
    //    }
    //    // ======================
    //    // ✅ CREATE
    //    // ======================
    //    public async Task<GeneralDto> CreateAsync<T>(T entity, params Type[] additionalTypesToInvalidate)
    //        where T : class, IEntity
    //    {
    //        try
    //        {
    //            await _context.Set<T>().AddAsync(entity);

    //            var result = await _context.SaveChangesAsync();

    //            if (result > 0)
    //            {
                  
    //                return new GeneralDto
    //                {
    //                    Suceeded = true,
    //                    Message = $"{typeof(T).Name} created successfully."
    //                };
    //            }

    //            return new GeneralDto
    //            {
    //                Suceeded = false,
    //                Message = $"{typeof(T).Name} was not created."
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto
    //            {
    //                Suceeded = false,
    //                Message = $"Error creating {typeof(T).Name}: {ex.Message}"
    //            };
    //        }
    //    }

    //    // ======================
    //    // ✅ UPDATE
    //    // ======================
    //    public async Task<GeneralDto> UpdateAsync<T>(T entity, params Type[] additionalTypesToInvalidate)
    //        where T : class, IEntity
    //    {
    //        try
    //        {

    //            var result = await _context.SaveChangesAsync();

    //            if (result > 0)
    //            {
                   
    //                return new GeneralDto
    //                {
    //                    Suceeded = true,
    //                    Message = $"{typeof(T).Name} updated successfully."
    //                };
    //            }

    //            return new GeneralDto
    //            {
    //                Suceeded = false,
    //                Message = $"{typeof(T).Name} was not updated."
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto
    //            {
    //                Suceeded = false,
    //                Message = $"Error updating {typeof(T).Name}: {ex.Message}"
    //            };
    //        }
    //    }

    //    // ======================
    //    // ✅ CONSULTAS (sin caché, con tracking → para operaciones de escritura)
    //    // ======================
    //    public async Task<T?> FindAsync<T>(
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class, IEntity
    //    {
    //        var query = BuildQuery(_context.Set<T>().AsTracking(), filter, includes);
    //        return await query.FirstOrDefaultAsync();
    //    }

    //    // ======================
    //    // ✅ CONSULTAS (con caché, sin tracking → para UI)
    //    // ======================
    //    public async Task<T?> GetByIdAsync<T>(Guid id,
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class, IEntity
    //    {
    //        var cacheKey = $"ById-{typeof(T).Name}-{id}";
    //        return await _cache.GetOrAddCacheAsync(async () =>
    //        {
    //            var query = BuildQuery(
    //                _context.Set<T>().AsNoTracking().AsSplitQuery(),
    //                filter,
    //                includes
    //            );
    //            return await query.FirstOrDefaultAsync(x => x.Id == id);
    //        }, cacheKey);
    //    }

    //    public async Task<List<T>> GetAllAsync<T>(string cachemodifier = null!,
    //Expression<Func<T, bool>>? filter = null,
    // Expression<Func<T, object>> OrderBy = null!,
    //Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //where T : class, IEntity
    //    {
    //        var cacheKey = $"All-{typeof(T).Name}";
    //        cacheKey = cachemodifier == null ? cacheKey : $"{cacheKey}-{cachemodifier}";
    //        // ✅ Cast explícito para coincidir con Func<Task<T?>>
    //        Func<Task<List<T>?>> factory = async () =>
    //        {
    //            var query = BuildQuery(
    //                _context.Set<T>().AsNoTracking().AsSplitQuery(),
    //                filter,
    //                includes
    //            );
    //            if (OrderBy != null)
    //            {
    //                query = query.OrderBy(OrderBy);
    //            }
    //            var result = await query.ToListAsync(); // sigue siendo List<T>, pero el tipo del delegado es List<T>?
    //            return result;
    //        };

    //        var result = await _cache.GetOrAddCacheAsync(factory, cacheKey);
    //        return result!; // ✅ ahora sí el ! es válido y el warning desaparece
    //    }

    //    // ======================
    //    // ✅ MÉTODOS PRIVADOS REUTILIZABLES
    //    // ======================
    //    private static IQueryable<T> BuildQuery<T>(
    //        IQueryable<T> query,
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
    //        where T : class
    //    {
    //        if (filter != null)
    //            query = query.Where(filter);
    //        if (includes != null)
    //            return includes(query);
    //        return query;
    //    }

    //    public void InvalidateCache(params string[] additionalTypes)
    //    {
           
    //        _cache.InvalidateCache(additionalTypes);
    //    }
    //    // ======================
    //    // ✅ REORDENAMIENTO
    //    // ======================

    //    private async Task<bool> SwapOrderAsync<T>(T current, T other)
    //where T : class, IEntity
    //    {
    //        var originalCurrentOrder = current.Order;
    //        var originalOtherOrder = other.Order;

    //        // Intercambiar
    //        current.Order = originalOtherOrder;
    //        other.Order = originalCurrentOrder;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            return true;
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            // Si otro hilo cambió los órdenes, revertimos y notificamos
    //            current.Order = originalCurrentOrder;
    //            other.Order = originalOtherOrder;
    //            return false;
    //        }
    //    }
    //    public async Task<GeneralDto> OrderUpAsync<T>(Guid id) where T : class, IEntity
    //    {
    //        try
    //        {
    //            var current = await _context.Set<T>().FindAsync(id);
    //            if (current == null)
    //                return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} not found." };

    //            var previous = await _context.Set<T>()
    //                .Where(x => x.Order == current.Order - 1)
    //                .FirstOrDefaultAsync();

    //            if (previous == null)
    //                return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} is already at the top." };

    //            // ✅ Intercambio seguro
    //            var success = await SwapOrderAsync(current, previous);
    //            if (!success)
    //                return new GeneralDto { Suceeded = false, Message = "Concurrent modification. Please retry." };

             
    //            return new GeneralDto { Suceeded = true, Message = $"{typeof(T).Name} moved up." };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto { Suceeded = false, Message = $"Error: {ex.Message}" };
    //        }
    //    }

    //    public async Task<GeneralDto> OrderDownAsync<T>(Guid id) where T : class, IEntity
    //    {
    //        try
    //        {
    //            var current = await _context.Set<T>().FindAsync(id);
    //            if (current == null)
    //                return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} not found." };

    //            var next = await _context.Set<T>()
    //                .Where(x => x.Order == current.Order + 1)
    //                .FirstOrDefaultAsync();

    //            if (next == null)
    //                return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} is already at the bottom." };

    //            // ✅ Intercambio seguro
    //            var success = await SwapOrderAsync(current, next);
    //            if (!success)
    //                return new GeneralDto { Suceeded = false, Message = "Concurrent modification. Please retry." };

            
    //            return new GeneralDto { Suceeded = true, Message = $"{typeof(T).Name} moved down." };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto { Suceeded = false, Message = $"Error: {ex.Message}" };
    //        }
    //    }
    //    public async Task ReindexOrderAsync<T>(string cachemodifier = null!,
    //Expression<Func<T, bool>>? filter = null) where T : class, IEntity
    //    {
    //        // 1. Obtener todos los IDs restantes, ordenados por Order
    //        var remaining = await GetAllAsync(cachemodifier, filter);


    //        // 2. Si no hay registros, nada que hacer
    //        if (remaining.Count == 0) return;

    //        var remianingQuery = remaining.Select(x => x.Id).ToList();
    //        // 3. Cargar las entidades para actualizar
    //        var entities = await _context.Set<T>()
    //            .Where(x => remianingQuery.Contains(x.Id))
    //            .OrderBy(x => x.Order)
    //            .ToListAsync();

    //        // 4. Reasignar Order secuencialmente
    //        for (int i = 0; i < entities.Count; i++)
    //        {
    //            entities[i].Order = i + 1;
    //        }
    //        var result = await _context.SaveChangesAsync();
            

    //    }
    //    public async Task<GeneralDto> DeleteAsync<T>(Guid id)
    //where T : class, IEntity
    //    {
    //        try
    //        {
    //            var entity = await _context.Set<T>().FindAsync(id);
    //            if (entity == null)
    //                return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} not found." };

    //            // ✅ Eliminar (soft o físico)
    //            if (entity is ISoftDeletable softEntity)
    //            {
    //                softEntity.IsDeleted = true;

    //            }
    //            else
    //            {
    //                _context.Set<T>().Remove(entity);
    //            }

    //            // ✅ Reindexar
    //            //await ReindexOrderAsync<T>();

    //            var result = await _context.SaveChangesAsync();
    //            if (result > 0)
    //            {
                   
    //                return new GeneralDto { Suceeded = true, Message = $"{typeof(T).Name} deleted and reordered." };
    //            }

    //            return new GeneralDto { Suceeded = false, Message = $"{typeof(T).Name} was not deleted." };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto { Suceeded = false, Message = $"Error: {ex.Message}" };
    //        }
    //    }
    //    public async Task<GeneralDto> DeleteRangeAsync<T>(List<Guid> ids)
    //where T : class, IEntity
    //    {
    //        if (ids == null || ids.Count == 0)
    //            return new GeneralDto { Suceeded = false, Message = "No IDs provided." };

    //        try
    //        {
    //            var entities = await _context.Set<T>()
    //                .Where(x => ids.Contains(x.Id))
    //                .ToListAsync();

    //            if (entities.Count == 0)
    //                return new GeneralDto { Suceeded = false, Message = "No records found to delete." };

    //            // ✅ Eliminar (soft o físico)
    //            foreach (var entity in entities)
    //            {
    //                if (entity is ISoftDeletable softEntity)
    //                {
    //                    softEntity.IsDeleted = true;

    //                }
    //                else
    //                {
    //                    _context.Set<T>().Remove(entity);
    //                }
    //            }

    //            // ✅ Reindexar
    //            //await ReindexOrderAsync<T>();

    //            var result = await _context.SaveChangesAsync();
    //            if (result > 0)
    //            {
                   
    //                return new GeneralDto { Suceeded = true, Message = $"{result} {typeof(T).Name}(s) deleted and reordered." };
    //            }

    //            return new GeneralDto { Suceeded = false, Message = "No records were deleted." };
    //        }
    //        catch (Exception ex)
    //        {
    //            return new GeneralDto { Suceeded = false, Message = $"Error: {ex.Message}" };
    //        }
    //    }
    //    public async Task<bool> ValidateUniqueAsync<T, TValue>(
    //TValue value,
    //Expression<Func<T, TValue>> propertySelector,
 
    // Expression<Func<T, bool>>? filter = null,
    //Guid? excludeId = null)
    //where T : class, IEntity
    //where TValue : notnull
    //    {
    //        try
    //        {

    //            var all = await GetAllAsync<T>(filter: filter);
    //            var getter = propertySelector.Compile();

    //            // ✅ Comparador inteligente: case-insensitive para strings, default para otros
    //            bool EqualsSafe(TValue a, TValue b)
    //            {
    //                if (a is string strA && b is string strB)
    //                    return string.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
    //                return EqualityComparer<TValue>.Default.Equals(a, b);
    //            }

    //            var exists = all.Any(x =>
    //                EqualsSafe(getter(x), value) && x.Id != excludeId);

    //            return !exists;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //}
}
