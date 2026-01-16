using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using System.Text;
namespace Server.EndPoints.Projects
{
    public class ProjectTechnicalRiskReportEndPoint : IEndPoint
    {
        byte[] CPLogo = null!;
        byte[] PMLogo = null!;
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            void GetImageData(IWebHostEnvironment host)
            {
                StringBuilder mesajes = new StringBuilder();
                var path = host.ContentRootPath;

                if (path == null)
                {
                    mesajes.Append("Server path not found");
                    path = host.WebRootPath;
                    if (path == null)
                    {
                        mesajes.Append("Web path not found");
                        Console.WriteLine(mesajes.ToString());
                        return;
                    }
                    else
                    {
                        mesajes.Append("Web path  found");
                        Console.WriteLine(mesajes.ToString());
                    }
                }
                else
                {
                    mesajes.Append("Server path found");
                    Console.WriteLine(path);
                }

                try
                {
                    var rutaImagen = Path.Combine(path, "Assets/CPLogo.PNG");
                    CPLogo = System.IO.File.ReadAllBytes(rutaImagen);

                    mesajes.Append($"CPLogo: created");
                    rutaImagen = Path.Combine(path, "Assets/PMLogo.PNG");
                    PMLogo = System.IO.File.ReadAllBytes(rutaImagen);
                    mesajes.Append($"PMLogo: created");
                    Console.WriteLine(mesajes.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
           
            // Dentro de ProjectExportEndPoint
            app.MapPost("ExportTechnicalRiskReport", async (ExportTechnicalRiskReport dto, IAppDbContext _context, [FromServices] IWebHostEnvironment host) =>
            {
                var cacheKey = $"{typeof(ExportTechnicalRiskReport).Name}-{dto.ProjectId}";
                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                        .AsSplitQuery()
                        .AsNoTracking()
                        .Include(x => x.Qualitys.OrderBy(x => x.Order))
                        .Include(x => x.KnownRisks.OrderBy(x => x.Order))
                        .Include(x => x.HazopNodes.OrderBy(x => x.Order))
                            .ThenInclude(n => n.Details.OrderBy(d => d.Order))
                        .Where(x => x.Id == dto.ProjectId)
                        .FirstOrDefaultAsync();
                }, cacheKey);

                if (project == null) return Results.Ok(new GeneralDto { Succeeded = false, Message = "Project not found" });

                GetImageData(host);

                // Generamos el reporte específico
                var pdfBytes = GenerateTechnicalReportBytes(project);

                return Results.Ok(new GeneralDto<Shared.Commons.FileResult>
                {
                    Succeeded = true,
                    Data = new Shared.Commons.FileResult
                    {
                        Data = pdfBytes,
                        ExportFileName = $"Technical Assessment - {project.Name}.pdf",
                        ContentType = Shared.Commons.FileResult.pdfContentType
                    }
                });
            });
        }
        byte[] GenerateTechnicalReportBytes(Project project)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Portrait());
                    page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // 1. Cabecera (Reutilizando tus logos)
                    page.Header().Element(header => RenderTechnicalHeader(header, project));

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // SECCIÓN A: CALIDAD
                        col.Item().Text("1. QUALITY ASSURANCE PLAN").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().PaddingBottom(10).LineHorizontal(1f);
                        col.Item().Table(table => RenderQualityTable(table, project));

                        // SECCIÓN B: MATRIZ DE RIESGOS (PMI)
                        col.Item().PaddingTop(20).Text("2. PROJECT RISK MATRIX").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().PaddingBottom(10).LineHorizontal(1f);
                        col.Item().Table(table => RenderRiskMatrixTable(table, project));

                        // SECCIÓN C: ESTUDIO HAZOP (SEGURIDAD DE PROCESO)
                        col.Item().PageBreak(); // El HAZOP suele ser largo, mejor empezar en página nueva
                        col.Item().Text("3. PROCESS HAZARD ANALYSIS (HAZOP)").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().PaddingBottom(10).LineHorizontal(1f);
                        RenderHazopSection(col, project);
                    });

                    page.Footer().AlignCenter().Text(x => {
                        x.Span("Technical Confidential Document - Page ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }
        void RenderTechnicalHeader(IContainer container, Project project)
        {
            container.Row(row =>
            {
                // Lado Izquierdo: Logo de la Empresa (CPLogo)
                if (CPLogo != null)
                {
                    row.ConstantItem(80).Image(CPLogo);
                }
                else
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("COLGATE-PALMOLIVE").FontSize(14).Bold().FontColor(Colors.Red.Medium);
                    });
                }

                // Centro: Título del Reporte y Nombre del Proyecto
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("TECHNICAL RISK & QUALITY ASSESSMENT").FontSize(14).Bold();
                    col.Item().AlignCenter().Text(project.Name).FontSize(11).SemiBold().FontColor(Colors.Grey.Darken2);
                    col.Item().AlignCenter().Text($"Generation Date: {DateTime.Now:MMMM dd, yyyy}").FontSize(8);
                });

                // Lado Derecho: Logo de Project Management (PMLogo)
                if (PMLogo != null)
                {
                    row.ConstantItem(80).Image(PMLogo);
                }
                else
                {
                    row.ConstantItem(80).AlignRight().Column(col =>
                    {
                        col.Item().Text("PMO OFFICE").FontSize(10).Bold();
                        col.Item().Text("Phase: START").FontSize(8);
                    });
                }
            });
        }
        void RenderQualityTable(TableDescriptor table, Project project)
        {
            table.ColumnsDefinition(columns => {
                columns.RelativeColumn(3); // Standard/Requirement
                columns.RelativeColumn(2); // Frequency
                columns.RelativeColumn(2); // Responsible
            });

            table.Header(header => {
                header.Cell().Element(HeaderStyle).Text("Quality Standard");
                header.Cell().Element(HeaderStyle).Text("Frequency");
                header.Cell().Element(HeaderStyle).Text("Responsible");
            });

            foreach (var q in project.Qualitys)
            {
                table.Cell().Element(RowStyle).Text(q.Name);
               
                table.Cell().Element(RowStyle).Text("QA Department");
            }
        }
        void RenderRiskMatrixTable(TableDescriptor table, Project project)
        {
            table.ColumnsDefinition(columns => {
                columns.RelativeColumn(4); // Risk
                columns.RelativeColumn(1); // Prob
                columns.RelativeColumn(1); // Imp
                columns.RelativeColumn(2); // Level
            });

            foreach (var r in project.RiskMatrixs)
            {
                table.Cell().Element(RowStyle).Text(r.Title);
                table.Cell().Element(RowStyle).AlignCenter().Text(r.Probability.ToString());
                table.Cell().Element(RowStyle).AlignCenter().Text(r.Impact.ToString());

                // Color basado en criticidad (Suponiendo Score = P * I)
                var score =(int) r.Probability * (int)r.Impact;
                var color = score > 15 ? Colors.Red.Lighten4 : score > 8 ? Colors.Yellow.Lighten4 : Colors.Green.Lighten4;

                table.Cell().Background(color).Element(RowStyle).AlignCenter().Text(r.RiskEvent).Bold();
            }
        }
        void RenderHazopSection(ColumnDescriptor col, Project project)
        {
            foreach (var node in project.HazopNodes)
            {
                col.Item().PaddingTop(10).Background(Colors.Grey.Lighten4).Padding(5).Column(nodeInfo =>
                {
                    nodeInfo.Item().Text($"NODE: {node.Name}").Bold().FontSize(10);
                    nodeInfo.Item().Text($"INTENT: {node.DesignIntent}").Italic().FontSize(9);
                });

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Deviation
                        columns.RelativeColumn(3); // Causes
                        columns.RelativeColumn(3); // Consequences
                        columns.RelativeColumn(3); // Recommendations
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Deviation");
                        header.Cell().Element(HeaderStyle).Text("Causes");
                        header.Cell().Element(HeaderStyle).Text("Consequences");
                        header.Cell().Element(HeaderStyle).Text("Recommendations");
                    });

                    foreach (var detail in node.Details)
                    {
                        table.Cell().Element(RowStyle).Text($"{detail.GuideWord} {detail.Parameter}");
                        table.Cell().Element(RowStyle).Text(detail.Causes);
                        table.Cell().Element(RowStyle).Text(detail.Consequences);
                        table.Cell().Element(RowStyle).Text(detail.Recommendations).SemiBold().FontColor(Colors.Blue.Medium);
                    }
                });

                col.Item().PaddingBottom(10);
            }
        }
        // Estilo para los encabezados de las tablas
        static IContainer HeaderStyle(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold().FontSize(9).FontColor(Colors.White))
                .Background(Colors.Blue.Darken3)
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .AlignCenter();
        }

        // Estilo para las filas de datos
        static IContainer RowStyle(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(4)
                .PaddingHorizontal(5)
                .DefaultTextStyle(x => x.FontSize(8));
        }

        // Estilo para los títulos de sección
        static void SectionTitleStyle(IContainer container, string title)
        {
            container
                .PaddingTop(15)
                .PaddingBottom(5)
                .BorderBottom(1)
                .BorderColor(Colors.Blue.Medium)
                .Text(title) // Aquí pasas el texto directamente
                .FontSize(12)
                .Bold()
                .FontColor(Colors.Blue.Medium);
        }
    }
}
