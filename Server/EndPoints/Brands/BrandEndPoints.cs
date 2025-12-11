using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.Brands;
using Shared.Dtos.General;
namespace Server.EndPoints.Brands
{

    public class BrandEndPoints : IEndPoint
    {

        void MapFromDto(BrandDto dto, Brand row)
        {
            row.Name = dto.Name;

        }
        static BrandDto MapToDto(Brand row)
        {
            BrandDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;

            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateBrand", async (CreateBrand dto, IAppDbContext _context) =>
            {
                var row = new Brand
                {
                    Id = Guid.NewGuid(),
                };
                MapFromDto(dto, row);
                await _context.Brands.AddAsync(row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyAll = $"{typeof(GetAllBrands).Name}";
                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Brand).Name} created successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Brand).Name} was not created successfully."
                });



            });


            // ✅ Editar
            app.MapPost("EditBrand", async (EditBrand dto, IAppDbContext _context) =>
            {


                var row = await _context.Brands.FindAsync(dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Brand not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyId = $"{typeof(GetBrandById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllBrands).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Brand).Name} created successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Brand).Name} was not created successfully."
                });

            });

            // ✅ Obtener por ID
            app.MapPost("GetBrandById", async (GetBrandById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetBrandById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Brands
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable()
                  .FirstOrDefaultAsync(x => x.Id == request.Id);

                }, cacheKey);



                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Brand).Name} was not found"
                    });
                }

                var dto = MapToDto(row);
                return Results.Ok(new GeneralDto<BrandDto>
                {
                    Succeeded = true,
                    Message = $"{typeof(Brand).Name} was found!",
                    Data = dto


                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllBrands", async (GetAllBrands dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllBrands).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Brands
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<BrandDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteBrand", async (DeleteBrand dto, IAppDbContext _context) =>
            {
                var row = await _context.Brands.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Brand).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var cacheKeyId = $"{typeof(GetBrandById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllBrands).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Brand).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Brand).Name} was not deleted"
                });
            });

            //// ✅ Eliminar varios
            //app.MapPost("DeleteGroupBrand", async (DeleteGroupBrand dto, IAppDbContext _context) =>
            //{
            //    return Results.Ok(await service.DeleteRangeAsync<Brand>(dto.GroupIds));
            //});

            // ✅ Validar nombre único
            app.MapPost("ValidateBrandName", async (ValidateBrandName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllBrands).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Brands
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<Brand, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });

           

        }
    }
}

