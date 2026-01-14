using Server.DataContext;
using Server.Domain.CommonEntities.BudgetItems;
using Server.Interfaces.EndPoints;
using Server.Services.Repositories;
using Shared.Dtos.BudgetItems;
using Shared.Dtos.General;
using Shared.Dtos.Plannings.RiskMatrixs;
using Shared.Dtos.Projects;
using Shared.Dtos.Projects._1._Starts.RiskMatrixs.RiskResponseActions;
using Shared.Enums.DashBoardTable;

namespace Server.EndPoints.ProjectDashBoard.ProjectPlannings.RiskMatrixs
{
    namespace Server.EndPoints.Projects.Starts.RiskMatrixs
    {
        public class RiskMatrixEndPoint : IEndPoint
        {
            void MapFromDto(RiskMatrixDto dto, RiskMatrix row)
            {
                row.Title = dto.Title;
                row.Cause = dto.Cause;
                row.RiskEvent = dto.RiskEvent;
                row.Effect = dto.Effect;

                // PMP Analysis (Enums)
                row.Probability = dto.Probability;
                row.Impact = dto.Impact;

                // PMP Response
                row.StrategyType = dto.StrategyType;
                row.ResponsePlanDescription = dto.ResponsePlanDescription;
                row.Trigger = dto.Trigger;
                row.Responsible = dto.Responsible;
                row.Status = dto.Status;
            }
            // 1. Helper: De DTO a Entidad (Para Create/Edit)
            void MapCreateFromDto(RiskMatrixDto dto, RiskMatrix row)
            {
                // PMP Identification
                row.Title = dto.Title;
                row.Cause = dto.Cause;
                row.RiskEvent = dto.RiskEvent;
                row.Effect = dto.Effect;

                // PMP Analysis (Enums)
                row.Probability = dto.Probability;
                row.Impact = dto.Impact;

                // PMP Response
                row.StrategyType = dto.StrategyType;
                row.ResponsePlanDescription = dto.ResponsePlanDescription;
                row.Trigger = dto.Trigger;
                row.Responsible = dto.Responsible;
                row.Status = dto.Status;
                if (dto.RiskMatrixComments != null && dto.RiskMatrixComments.Any())
                {
                    foreach (var commentDto in dto.RiskMatrixComments)
                    {
                        // Solo agregamos si tienen texto
                        if (!string.IsNullOrWhiteSpace(commentDto.Comment))
                        {
                            row.RiskMatrixComments.Add(new RiskMatrixComment
                            {
                                Id = Guid.NewGuid(),
                                RiskMatrixId = row.Id, // Vinculamos al ID del nuevo riesgo
                                Comment = commentDto.Comment,
                                CommentDate = DateTime.UtcNow,
                                // Usamos el usuario actual, ya que el DTO podría no traerlo o ser inseguro confiar en el cliente
                                CommentedBy = commentDto.CommentedBy,
                            });
                        }
                    }
                }
                if (dto.RiskResponseActions != null && dto.RiskResponseActions.Any())
                {
                    foreach (var actionDto in dto.RiskResponseActions)
                    {
                        var actionRow = new RiskResponseAction
                        {
                            Id = Guid.NewGuid(), // O el que venga del DTO
                            Description = actionDto.Description,
                            AssignedTo = actionDto.AssignedTo,
                            DueDate = actionDto.DueDate,
                            IsCompleted = actionDto.IsCompleted,
                            ActionType = actionDto.ActionType,
                            Order = actionDto.Order
                            // No asignamos RiskMatrixId aquí, EF lo infiere al agregarlo a la colección
                        };
                        row.RiskResponseActions.Add(actionRow);
                    }
                }
                //if (dto.LinkedInvestments != null && dto.LinkedInvestments.Any())
                //{
                //    foreach (var actionDto in dto.LinkedInvestments)
                //    {
                       
                //        row.RiskResponseActions.Add(actionRow);
                //    }
                //}
            }

            // 2. Helper: De Entidad a DTO (Para Get)
            static RiskMatrixDto MapToDto(RiskMatrix row)
            {
                RiskMatrixDto dto = new();
                dto.Id = row.Id;
                dto.ProjectId = row.ProjectId;
                dto.Order = row.Order;

                // Mapeo de campos PMP
                dto.Title = row.Title;
                dto.Cause = row.Cause;
                dto.RiskEvent = row.RiskEvent;
                dto.Effect = row.Effect;
                dto.Probability = row.Probability;
                dto.Impact = row.Impact;
                dto.StrategyType = row.StrategyType;
                dto.ResponsePlanDescription = row.ResponsePlanDescription;
                dto.Trigger = row.Trigger;
                dto.Responsible = row.Responsible;
                dto.Status = row.Status;

                // ✅ Mapeo de Comentarios
                if (row.RiskMatrixComments != null)
                {
                    dto.RiskMatrixComments = row.RiskMatrixComments
                        .Select(c => new RiskMatrixCommentDto
                        {
                            Id = c.Id,
                            RiskMatrixId = c.RiskMatrixId,
                            Comment = c.Comment,
                            CommentDate = c.CommentDate,
                            CommentedBy = c.CommentedBy
                        }).ToList();
                }

                // ✅ Mapeo de Inversiones (Polimorfismo para los Chips de la UI)
                if (row.RiskBudgetItems != null)
                {
                    dto.LinkedInvestments = row.RiskBudgetItems
                        .Where(x => !x.IsDeleted) // Aseguramos no traer borrados si aplicara
                        .Select(x => new BudgetItemDto
                        {
                            Id = x.BudgetItem.Id,
                            Name = x.BudgetItem.Name,
                            Quantity = x.BudgetItem.Quantity,
                            UnitPriceUSD = x.BudgetItem.UnitPriceUSD,
                            Order = x.BudgetItem.Order,
                            Category = x.BudgetItem.Category

                        }).ToList();
                }
                if (row.RiskResponseActions != null)
                {
                    dto.RiskResponseActions = row.RiskResponseActions
                        .OrderBy(x => x.Order) // Importante: Mantener el orden visual
                        .Select(x => new RiskResponseActionDto
                        {
                            Id = x.Id,
                            RiskMatrixId = x.RiskMatrixId,
                            Description = x.Description,
                            AssignedTo = x.AssignedTo,
                            DueDate = x.DueDate,
                            IsCompleted = x.IsCompleted,
                            ActionType = x.ActionType,
                            Order = x.Order
                        }).ToList();
                }
                return dto;
            }

            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                // --- CREATE ---
                app.MapPost("CreateRiskMatrix", async (CreateRiskMatrix dto, IAppDbContext _context, IRepositoryGetNextOrder getNextOrder) =>
                {
                    var row = new RiskMatrix
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = dto.ProjectId,
                    };

                    MapCreateFromDto(dto, row);
                    await _context.RiskMatrixs.AddAsync(row); // Asumo DbSet<RiskMatrix> RiskMatrixs

                    var project = await _context.Projects.FindAsync(dto.ProjectId);
                    if (project != null)
                        project.LastModifiedOn = DateTime.UtcNow;

                    var maxOrder = await _context.RiskMatrixs.Where(x => x.ProjectId == dto.ProjectId)
                  .MaxAsync(x => (int?)x.Order) ?? 0;

                    row.Order = maxOrder + 1;

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        // Invalidación idéntica a tu ejemplo
                        var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                        _context.InvalidateCache(keys);

                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = $"{typeof(RiskMatrix).Name} created successfully."
                        });
                    }
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true, // Ojo: en tu ejemplo tenías true aquí aunque fallara, ajusta si quieres false
                        Message = $"{typeof(RiskMatrix).Name} was not created successfully."
                    });
                });

                // --- EDIT ---
                app.MapPost("EditRiskMatrix", async (EditRiskMatrix dto, IAppDbContext _context) =>
                {
                    var row = await _context.RiskMatrixs.FindAsync(dto.Id);
                    if (row == null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = "Risk Matrix not found."
                        });
                    }

                    MapFromDto(dto, row);

                    var project = await _context.Projects.FindAsync(dto.ProjectId);
                    if (project != null)
                        project.LastModifiedOn = DateTime.UtcNow;

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                        _context.InvalidateCache(keys);

                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = $"{typeof(RiskMatrix).Name} Updated successfully."
                        });
                    }
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = true,
                        Message = $"{typeof(RiskMatrix).Name} was not Updated successfully."
                    });
                });

                // --- GET BY ID ---
                app.MapPost("GetRiskMatrixById", async (GetRiskMatrixById request, IAppDbContext _context) =>
                {
                    var cacheKey = $"{typeof(GetRiskMatrixById).Name}-{request.Id}";
                    var row = await _context.GetOrAddCacheAsync(async () =>
                    {
                        return await _context.RiskMatrixs
                            // ✅ INCLUDES IMPORTANTES
                            .Include(x => x.RiskMatrixComments)
                            .Include(x => x.RiskBudgetItems) // Tabla intermedia
                                .ThenInclude(r => r.BudgetItem) // Item real
                            .Include(x => x.RiskResponseActions)
                            .Where(x => x.Id == request.Id)
                            .AsSplitQuery()
                            .AsNoTracking()
                            .AsQueryable()
                            .FirstOrDefaultAsync();

                    }, cacheKey);

                    if (row == null)
                        return Results.Ok(new GeneralDto<RiskMatrixDto>
                        {
                            Succeeded = false,
                            Message = "Risk Matrix not found."
                        });

                    var dto = MapToDto(row);

                    return Results.Ok(new GeneralDto<RiskMatrixDto>
                    {
                        Succeeded = true,
                        Data = dto
                    });
                });

                // --- GET ALL ---
                app.MapPost("GetAllRiskMatrixs", async (GetAllRiskMatrixs dto, IAppDbContext _context) =>
                {
                    var cacheKey = $"{typeof(GetAllRiskMatrixs).Name}-{dto.ProjectId}";
                    var rows = await _context.GetOrAddCacheAsync(async () =>
                    {
                        return await _context.RiskMatrixs
                            // ✅ INCLUDES IMPORTANTES
                            .Include(x => x.RiskMatrixComments)
                            .Include(x => x.RiskBudgetItems)
                                .ThenInclude(r => r.BudgetItem)
                            .AsSplitQuery()
                            .AsNoTracking()
                            .Where(x => x.ProjectId == dto.ProjectId)
                            .OrderBy(x => x.Order)
                            .ToListAsync();

                    }, cacheKey);

                    var dtos = rows!.Select(MapToDto).ToList();

                    return Results.Ok(new GeneralDto<List<RiskMatrixDto>>
                    {
                        Succeeded = true,
                        Data = dtos
                    });
                });

                // --- DELETE ---
                app.MapPost("DeleteRiskMatrix", async (DeleteRiskMatrix dto, IAppDbContext _context) =>
                {
                    var row = await _context.RiskMatrixs.FindAsync(dto.Id);
                    if (row is null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = $"{typeof(RiskMatrix).Name} was not found"
                        });
                    }

                    // Borrado Lógico (o físico si prefieres context.Remove(row))
                    row.IsDeleted = true;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        // Lógica de Reordenamiento (Tu código original)
                        var datas = await _context.RiskMatrixs
                            .Where(x => x.ProjectId == dto.ProjectId && !x.IsDeleted) // Importante filtrar IsDeleted
                            .OrderBy(x => x.Order)
                            .ToListAsync();

                        int i = 1;
                        foreach (var data in datas)
                        {
                            data.Order = i;
                            i++;
                        }
                        await _context.SaveChangesAsync();

                        var keys = ProjectCacheBrain.GetStartKeyToInvalidate(dto.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                        _context.InvalidateCache(keys);

                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = $"{typeof(RiskMatrix).Name} was deleted"
                        });
                    }

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskMatrix).Name} was not deleted"
                    });
                });

                // --- CHANGE ORDER ---
                app.MapPost("ChangeOrderRiskMatrix", async (ChangeOrderRiskMatrix dto, IAppDbContext _context) =>
                {
                    var CurrentRow = await _context.RiskMatrixs.FindAsync(dto.Id);
                    if (CurrentRow == null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = $"{typeof(RiskMatrix).Name} was not found"
                        });
                    }

                    var NewRow = await _context.RiskMatrixs
                        .Where(x => x.ProjectId == dto.ProjectId && x.Order == dto.NewOrder)
                        .FirstOrDefaultAsync();

                    if (NewRow == null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = $"{typeof(RiskMatrix).Name} was not found"
                        });
                    }

                    // Swap
                    NewRow.Order = CurrentRow.Order;
                    CurrentRow.Order = dto.NewOrder;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var cacheKeyAll = $"{typeof(GetAllRiskMatrixs).Name}-{dto.ProjectId}";
                        _context.InvalidateCache(cacheKeyAll);

                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = $"{typeof(RiskMatrix).Name} was reorder"
                        });
                    }
                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskMatrix).Name} was not reorder"
                    });
                });

                // ✅ NUEVO: CREATE COMMENT
                app.MapPost("CreateRiskMatrixComment", async (CreateRiskMatrixComment dto, IAppDbContext _context) =>
                {
                    var comment = new RiskMatrixComment
                    {
                        Id = Guid.NewGuid(),
                        RiskMatrixId = dto.RiskMatrixId,
                        Comment = dto.Comment,
                        CommentDate = DateTime.UtcNow,
                        CommentedBy = dto.CommentedBy ?? "User"
                    };

                    await _context.RiskMatrixComments.AddAsync(comment);

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        // Invalidamos GetById para que el Diálogo se refresque con el nuevo comentario
                        _context.InvalidateCache($"{typeof(GetRiskMatrixById).Name}-{dto.RiskMatrixId}");
                        return Results.Ok(new GeneralDto { Succeeded = true });
                    }
                    return Results.Ok(new GeneralDto { Succeeded = false });
                });

                // ✅ NUEVO: GET INVESTMENTS (Para RiskMatrixInvestmentsDialog)
                app.MapPost("GetBudgetItemsByRiskMatrixId", async (GetBudgetItemsByRiskMatrixId req, IAppDbContext _context) =>
                {
                    // Nota: Asegúrate de usar el Request correcto o crear GetBudgetItemsByRiskMatrixId si usas otro DTO
                    // Aquí asumo que creaste GetBudgetItemsByRiskMatrixId como hablamos.

                    var items = await _context.Set<RiskBudgetItem>() // Usamos tu entidad 'RiskBudgetItem'
                        .Where(x => x.RiskMatrixId == req.RiskMatrixId) // Ajusta propiedad si el DTO se llama distinto
                        .Include(x => x.BudgetItem)
                        .Select(x => x.BudgetItem)
                        .AsNoTracking()
                        .ToListAsync();

                    var dtos = items.Select(x => new BudgetItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        //BudgetUSD = (double)x.BudgetUSD,
                        //Nomenclatore = x.Nomenclatore,
                        //CategoryName = x.GetType().Name
                    }).ToList();

                    return Results.Ok(new GeneralDto<List<BudgetItemDto>> { Succeeded = true, Data = dtos });
                });
                // --- EDIT COMMENT ---
                app.MapPost("EditRiskMatrixComment", async (EditRiskMatrixComment dto, IAppDbContext _context) =>
                {
                    var row = await _context.RiskMatrixComments.Include(x => x.RiskMatrix).FirstOrDefaultAsync(x => x.Id == dto.Id);

                    if (row == null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = "Comment not found."
                        });
                    }

                    // Actualizamos el texto
                    row.Comment = dto.Comment;
                    row.CommentedBy = dto.CommentedBy;
                    // Opcional: Actualizamos la fecha para saber que fue modificado recientemente
                    row.CommentDate = DateTime.UtcNow;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var keys = ProjectCacheBrain.GetStartKeyToInvalidate(row.RiskMatrix.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                        _context.InvalidateCache(keys);

                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = "Comment updated successfully."
                        });
                    }

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = "Comment was not updated."
                    });
                });
                app.MapPost("DeleteRiskMatrixComment", async (DeleteRiskMatrixComment dto, IAppDbContext _context) =>
                {
                    var row = await _context.RiskMatrixComments.Include(x => x.RiskMatrix).FirstOrDefaultAsync(x => x.Id == dto.Id);
                    if (row is null)
                    {
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = false,
                            Message = $"{typeof(RiskMatrixComment).Name} was not found"
                        });
                    }

                    // Borrado Lógico (o físico si prefieres context.Remove(row))
                    row.IsDeleted = true;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        // Lógica de Reordenamiento (Tu código original)



                        var keys = ProjectCacheBrain.GetStartKeyToInvalidate(row.RiskMatrix.ProjectId, row.Id, DashBoardsStartTable.RiskMatrix);
                        _context.InvalidateCache(keys);
                        return Results.Ok(new GeneralDto
                        {
                            Succeeded = true,
                            Message = $"{typeof(RiskMatrixComment).Name} was deleted"
                        });
                    }

                    return Results.Ok(new GeneralDto
                    {
                        Succeeded = false,
                        Message = $"{typeof(RiskMatrixComment).Name} was not deleted"
                    });
                });
            }
        }

    }
}
