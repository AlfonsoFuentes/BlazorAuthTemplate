using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DataContext;
using Server.Domain.CommonEntities.ProjectManagements;
using Server.Interfaces;
using Server.Interfaces.EndPoints;
using Server.Services;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Dtos.StakeHolders;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
namespace Server.EndPoints.StakeHolders
{

    public class StakeHolderEndPoints : IEndPoint
    {

        void MapFromDto(StakeHolderDto dto, StakeHolder row)
        {
            row.Name = dto.Name;
            row.Email = dto.Email;
            row.PhoneNumber = dto.PhoneNumber;
            row.Area = dto.Area;
            
        }
        static StakeHolderDto MapToDto(StakeHolder row)
        {
            StakeHolderDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.Email = row.Email;
            dto.PhoneNumber = row.PhoneNumber;
            dto.Area = row.Area;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateStakeHolder", async (CreateStakeHolder dto, IAppDbContext _context) =>
            {
                var row = new StakeHolder
                {
                    Id = Guid.NewGuid(),
                };
                MapFromDto(dto, row);
                await _context.StakeHolders.AddAsync(row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    
                    var cacheKeyAll = $"{typeof(GetAllStakeHolders).Name}";
                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(StakeHolder).Name} created successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(StakeHolder).Name} was not created successfully."
                });



            });


            // ✅ Editar
            app.MapPost("EditStakeHolder", async (EditStakeHolder dto, IAppDbContext _context) =>
            {


                var row = await _context.StakeHolders.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "StakeHolder not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyId = $"{typeof(GetStakeHolderById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllStakeHolders).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(StakeHolder).Name} updated successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(StakeHolder).Name} was not updated successfully."
                });

            });

            // ✅ Obtener por ID
            app.MapPost("GetStakeHolderById", async (GetStakeHolderById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetStakeHolderById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.StakeHolders
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
                        Message = $"{typeof(StakeHolder).Name} was not found"
                    });
                }

                var dto = MapToDto(row);
                return Results.Ok(new GeneralDto<StakeHolderDto>
                {
                    Succeeded = true,
                    Message = $"{typeof(StakeHolder).Name} was found!",
                    Data = dto


                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllStakeHolders", async (GetAllStakeHolders dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllStakeHolders).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.StakeHolders
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<StakeHolderDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteStakeHolder", async (DeleteStakeHolder dto, IAppDbContext _context) =>
            {
                var row = await _context.StakeHolders.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(StakeHolder).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var cacheKeyId = $"{typeof(GetStakeHolderById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllStakeHolders).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(StakeHolder).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(StakeHolder).Name} was not deleted"
                });
            });

            //// ✅ Eliminar varios
            //app.MapPost("DeleteGroupStakeHolder", async (DeleteGroupStakeHolder dto, IAppDbContext _context) =>
            //{
            //    return Results.Ok(await service.DeleteRangeAsync<StakeHolder>(dto.GroupIds));
            //});

            // ✅ Validar nombre único
            app.MapPost("ValidateStakeHolderName", async (ValidateStakeHolderName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllStakeHolders).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.StakeHolders
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);
        

                Func<StakeHolder, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });
            app.MapPost("ValidateStakeHolderEmail", async (ValidateStakeHolderEmail dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllStakeHolders).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.StakeHolders
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<StakeHolder, bool> predicate = x => dto.Id == Guid.Empty ? x.Email.Equals(dto.Email) : x.Id != dto.Id && x.Email.Equals(dto.Email);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "email is available." : "email already in use."
                };
            });



            // ✅ Eliminar uno

        }
    }
}

