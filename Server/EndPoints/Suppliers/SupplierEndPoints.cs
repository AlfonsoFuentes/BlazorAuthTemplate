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
using Shared.Dtos.Suppliers;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
namespace Server.EndPoints.Suppliers
{

    public class SupplierEndPoints : IEndPoint
    {

        void MapFromDto(SupplierDto dto, Supplier row)
        {
            row.Name = dto.Name;
            row.VendorCode = dto.VendorCode;
            row.TaxCodeLD = dto.TaxCodeLD;
            row.NickName = dto.NickName;
            row.TaxCodeLP = dto.TaxCodeLP;
            row.PhoneNumber = dto.PhoneNumber;
            row.ContactName = dto.ContactName;
            row.Address = dto.Address;
            row.ContactEmail = dto.ContactEmail;
            row.SupplierCurrency = dto.SupplierCurrency.Id;
        }
        static SupplierDto MapToDto(Supplier row)
        {
            SupplierDto dto = new();
            dto.Id = row.Id;
            dto.Name = row.Name;
            dto.VendorCode = row.VendorCode;
            dto.TaxCodeLD = row.TaxCodeLD;
            dto.NickName = row.NickName;
            dto.TaxCodeLP = row.TaxCodeLP;
            dto.PhoneNumber = row.PhoneNumber;
            dto.ContactName = row.ContactName;
            dto.Address = row.Address;
            dto.ContactEmail = row.ContactEmail;
            dto.SupplierCurrency = row.SupplierCurrencyEnum;
            return dto;

        }
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            // ✅ Crear
            app.MapPost("CreateSupplier", async (CreateSupplier dto, IAppDbContext _context) =>
            {
                var row = new Supplier
                {
                    Id = Guid.NewGuid(),
                };
                MapFromDto(dto, row);
                await _context.Suppliers.AddAsync(row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyAll = $"{typeof(GetAllSuppliers).Name}";
                    _context.InvalidateCache(cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Supplier).Name} created successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Supplier).Name} was not created successfully."
                });



            });


            // ✅ Editar
            app.MapPost("EditSupplier", async (EditSupplier dto, IAppDbContext _context) =>
            {


                var row = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (row == null)
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Supplier not found."
                    });
                MapFromDto(dto, row);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var cacheKeyId = $"{typeof(GetSupplierById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllSuppliers).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Supplier).Name} created successfully."
                    });


                }
                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Supplier).Name} was not created successfully."
                });

            });

            // ✅ Obtener por ID
            app.MapPost("GetSupplierById", async (GetSupplierById request, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetSupplierById).Name}-{request.Id}";
                var row = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Suppliers
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
                        Message = $"{typeof(Supplier).Name} was not found"
                    });
                }

                var dto = MapToDto(row);
                return Results.Ok(new GeneralDto<SupplierDto>
                {
                    Succeeded = true,
                    Message = $"{typeof(Supplier).Name} was found!",
                    Data = dto


                });
            });

            // ✅ Obtener todos
            app.MapPost("GetAllSuppliers", async (GetAllSuppliers dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllSuppliers).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Suppliers
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                var dtos = rows!.Select(MapToDto).ToList();

                return Results.Ok(new GeneralDto<List<SupplierDto>>
                {
                    Succeeded = true,
                    Data = dtos
                });
            });
            app.MapPost("DeleteSupplier", async (DeleteSupplier dto, IAppDbContext _context) =>
            {
                var row = await _context.Suppliers.FindAsync(dto.Id);
                if (row is null)
                {
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(Supplier).Name} was not found"
                    });
                }
                row.IsDeleted = true;
                if (await _context.SaveChangesAsync() > 0)
                {
                    var cacheKeyId = $"{typeof(GetSupplierById).Name}-{dto.Id}";
                    var cacheKeyAll = $"{typeof(GetAllSuppliers).Name}";
                    _context.InvalidateCache(cacheKeyId, cacheKeyAll);
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(Supplier).Name} was deleted"
                    });
                }

                return Results.Ok(new GeneralDto
                {
                    Succeeded = false,
                    Message = $"{typeof(Supplier).Name} was not deleted"
                });
            });

            //// ✅ Eliminar varios
            //app.MapPost("DeleteGroupSupplier", async (DeleteGroupSupplier dto, IAppDbContext _context) =>
            //{
            //    return Results.Ok(await service.DeleteRangeAsync<Supplier>(dto.GroupIds));
            //});

            // ✅ Validar nombre único
            app.MapPost("ValidateSupplierName", async (ValidateSupplierName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllSuppliers).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Suppliers
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);
        

                Func<Supplier, bool> predicate = x => dto.Id == Guid.Empty ? x.Name.Equals(dto.Name) : x.Id != dto.Id && x.Name.Equals(dto.Name);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });

            // ✅ Validar nick único (similar)
            app.MapPost("ValidateSupplierNickName", async (ValidateSupplierNickName dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllSuppliers).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Suppliers
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<Supplier, bool> predicate = x => dto.Id == Guid.Empty ? x.NickName.Equals(dto.NickName) : x.Id != dto.Id && x.NickName.Equals(dto.NickName);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "Name is available." : "Name already in use."
                };
            });

            // ✅ Validar vendor code único (similar)
            app.MapPost("ValidateSupplierVendorCode", async (ValidateSupplierVendorCode dto, IAppDbContext _context) =>
            {
                var cacheKey = $"{typeof(GetAllSuppliers).Name}";
                var rows = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Suppliers
                  .AsSplitQuery()
                  .AsNoTracking()
                  .AsQueryable().ToListAsync();

                }, cacheKey);


                Func<Supplier, bool> predicate = x => dto.Id == Guid.Empty ? x.VendorCode.Equals(dto.VendorCode) : x.Id != dto.Id && x.VendorCode.Equals(dto.VendorCode);

                var isUnique = rows!.Any(predicate);

                return new GeneralDto<bool>
                {
                    Succeeded = true,
                    Data = isUnique,
                    Message = isUnique ? "VendorCode is available." : "VendorCode already in use."
                };
            });

            // ✅ Eliminar uno

        }
    }
}

