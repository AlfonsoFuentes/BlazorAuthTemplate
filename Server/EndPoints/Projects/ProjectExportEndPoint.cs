using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Server.DataContext;
using Server.Domain.CommonEntities;
using Server.Interfaces.EndPoints;
using Shared.Dtos.General;
using Shared.Dtos.Projects;
using Shared.Enums.ProjectDefinitionTypes;
using Shared.ExtensionsMethods;
using System.Text;
using Colors = QuestPDF.Helpers.Colors;
namespace Server.EndPoints.Projects
{
    public class ProjectCharterReportEndPoint : IEndPoint
    {
        byte[] CPLogo = null!;
        byte[] PMLogo = null!;
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("ExportProjectCharterReport", async (ExportProjectCharterReport dto, IAppDbContext _context, [FromServices] IWebHostEnvironment host) =>
            {
                var cacheKeyExportProjectCharterPDF = $"{typeof(ExportProjectCharterReport).Name}-{dto.ProjectId}";
                var project = await _context.GetOrAddCacheAsync(async () =>
                {
                    return await _context.Projects
                    .AsSplitQuery()
                    .AsNoTracking()
                    .AsQueryable()
                    .Include(x => x.StakeHolders).ThenInclude(x => x.RoleInsideProject!)
                    .Include(x => x.DefinitionItems.OrderBy(x => x.Type).ThenBy(x => x.Order))

                    .Include(x => x.Requirements.OrderBy(x => x.Order))
                     .Include(x => x.KnownRisks.OrderBy(x => x.Order))
                    //.Include(x => x.LearnedLessons.OrderBy(x => x.Order))    //TODO:Incluir LearnedLeason By Project
                    .Include(x => x.ExpertJudgements.OrderBy(x => x.Order)).ThenInclude(x => x.Expert!)
                    .Include(x => x.Qualitys.OrderBy(x => x.Order))
                    .Include(x => x.BudgetItems)
                    .Where(x => x.Id == dto.ProjectId)
                    .FirstOrDefaultAsync();

                }, cacheKeyExportProjectCharterPDF);

                if (project == null)
                    return Results.Ok(
                    new GeneralDto<Shared.Commons.FileResult>
                    {
                        Message = "Project not found",
                        Succeeded = false,
                    });
                GetImageData(host);
                var responsePDF = CreatePDF(project);
                return Results.Ok(
                    new GeneralDto<Shared.Commons.FileResult>
                    {
                        Message = "Project Charter Generated",
                        Succeeded = true,
                        Data = responsePDF,
                    });
            });

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
            Shared.Commons.FileResult CreatePDF(Project response)
            {
                byte[] pdfBytes = GenerateReportBytes(response);

                Shared.Commons.FileResult newresult = new()
                {
                    Data = pdfBytes,
                    ExportFileName = $"Project Charter {response.Name}.pdf",
                    ContentType = Shared.Commons.FileResult.pdfContentType,
                };

                return newresult;
            }
            byte[] GenerateReportBytes(Project response)
            {
                byte[] reportBytes;
                Document document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Size(PageSizes.Letter.Portrait());
                        page.MarginLeft(2f, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.MarginRight(2f, QuestPDF.Infrastructure.Unit.Centimetre); // Ajustar el margen derecho
                        page.PageColor(QuestPDF.Helpers.Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));
                        page.Header().Row(row =>
                        {
                            if (CPLogo == null)
                            {
                                row.ConstantItem(100).Column(col =>
                                {
                                    col.Item().AlignCenter().Text("Colgate Palmolive").FontColor("#422ef2").Bold().FontSize(16).Italic();

                                });
                            }
                            else
                            {
                                row.ConstantItem(100).Image(CPLogo);
                            }

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignCenter().Text("Confidential").FontSize(14);

                            });
                            if (PMLogo == null)
                            {

                                row.ConstantItem(100).Column(col =>
                                {
                                    col.Item().AlignCenter().Text("Project Management").FontColor("#422ef2").Bold().FontSize(16).Italic();

                                });
                            }
                            else
                            {
                                row.ConstantItem(100).Image(PMLogo);
                            }

                        });
                        page.Footer().AlignRight().Text(txt =>
                        {
                            txt.Span("Page ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" of ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingBottom(15).Column(col2 =>
                            {
                                col2.Item().Background(Colors.Grey.Lighten2).Text("Project Charter Statement").FontSize(20).AlignCenter();
                            });

                            col1.Item().Element((ele) => ProjectNameContent(ele, response));


                            col1.Item().Padding(10).Column(col2 =>
                            {
                                col2.Item().Text("A) StakeHolders").FontSize(10).Bold();
                            });
                            col1.Item().Element((ele) => StakeHoldersContent(ele, response));
                            col1.Item().Padding(10).Column(col2 =>
                            {
                                col2.Item().Text("B) Scope Management").FontSize(10).Bold();
                            });
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Background));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Objective));
                            col1.Item().Element((ele) => Requirements(ele, response));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Scope));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.AcceptanceCriteria));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Benefit));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Constraint));
                            col1.Item().Element((ele) => DefinitionItems(ele, response, ProjectDefinitionType.Assumption));
                            //col1.Item().Element((ele) => LearnedLessons(ele, response));
                            col1.Item().Element((ele) => KnownRisk(ele, response));
                            col1.Item().Element((ele) => ExpertJudgements(ele, response));


                            col1.Item().Element((ele) => Qualitys(ele, response));

                            col1.Item().Padding(10).Column(col2 =>
                            {
                                col2.Item().Text("C) Investment").FontSize(10).Bold();
                            });
                            col1.Item().Element((ele) => InvestmentContent(ele, response));


                            col1.Item().LineHorizontal(0.5f);
                            col1.Item().Element((ele) => SignContent(ele, response));
                        });

                    });
                });

                reportBytes = document.GeneratePdf();

                return reportBytes;
            }

            void ProjectNameContent(IContainer container, Project response)
            {
                container.Column(col1 =>
                {

                    col1.Item().PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Project Name: ").SemiBold().FontSize(20);
                            txt.Span(response.Name).SemiBold().FontSize(20);
                        });
                    });


                });
            }




            void StakeHoldersContent(IContainer container, Project response)
            {
                if (response.StakeHolders.Count == 0) return;
                container.Column(col1 =>
                {
                    col1.Item().TranslateX(15).PaddingBottom(5).Table(table => GetStakeHoldersTable(table, response));
                });
            }
            TableDescriptor GetStakeHoldersTable(TableDescriptor tabla, Project response)
            {
                tabla.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);


                });

                tabla.Header(header =>
                {
                    header.Cell()
                    .Padding(4).Text("Name").Bold();

                    header.Cell()
                   .Padding(4).Text("Role").Bold();




                });

                foreach (var expert in response.ExpertJudgements)
                {
                    if (expert.Expert != null)
                    {
                        tabla.Cell()
                            .Padding(4).Text(expert.Expert!.Name).FontSize(10);

                        tabla.Cell()
                        .Padding(4).Text("Expert").FontSize(10);
                    }

                }
                foreach (var stakeholder in response.StakeHolders)
                {
                    tabla.Cell()
                        .Padding(4).Text(stakeholder.Name).FontSize(10);

                    tabla.Cell()
                    .Padding(4).Text(stakeholder.RoleInsideProject!.Name).FontSize(10);


                }
                return tabla;
            }
            void InvestmentContent(IContainer container, Project response)
            {

                container.Column(col1 =>
                {

                    col1.Item().Padding(30).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Investment").FontSize(10).SemiBold();

                        });
                    });
                    col1.Item().Padding(10).Column(col2 =>
                    {
                        col2.Item().Text("Summary").FontSize(10).Bold();
                    });
                    col1.Item().Table(table => GetSummaryInvestmentTable(table, response));
                    col1.Item().Padding(10).Column(col2 =>
                    {
                        col2.Item().Text("Detailed").FontSize(10).Bold();
                    });
                    col1.Item().Table(table => GetInvestmentTable(table, response));
                });
            }
            TableDescriptor GetSummaryInvestmentTable(TableDescriptor tabla, Project response)
            {
                tabla.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);


                });

                tabla.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text("Item").Bold();

                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                   .Padding(4).Text("Budget,USD").Bold();




                });
                //var expenses = response.Expenses;
                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //   .Padding(4).Text("Expenses").FontSize(10);

                //var budgetExpenses = expenses.Sum(x => x.BudgetUSD);

                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //.Padding(4).Text(budgetExpenses.ToCurrencyCulture()).FontSize(10);

                //var capital = response.Capital;
                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //      .Padding(4).Text("Capital").FontSize(10);

                //var budgetCapital = capital.Sum(x => x.BudgetUSD);

                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //.Padding(4).Text(budgetCapital.ToCurrencyCulture()).FontSize(10);

                //var budgetAppropiation = response.BudgetItems.Sum(x => x.BudgetUSD);

                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //      .Padding(4).Text("Appropiation").FontSize(10);

                //tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //.Padding(4).Text(budgetAppropiation.ToCurrencyCulture()).FontSize(10);
                return tabla;
            }
            TableDescriptor GetInvestmentTable(TableDescriptor tabla, Project response)
            {
                tabla.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);


                });

                tabla.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text("Chapter").Bold();

                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                   .Padding(4).Text("Budget,USD").Bold();




                });
                //GetInvestmentItemTable<Alteration>(tabla, response.BudgetItems, "A");
                //GetInvestmentItemTable<Foundation>(tabla, response.BudgetItems, "B");
                //GetInvestmentItemTable<Structural>(tabla, response.BudgetItems, "C");
                //GetInvestmentItemTable<Equipment>(tabla, response.BudgetItems, "D");
                //GetInvestmentItemTable<Electrical>(tabla, response.BudgetItems, "E");
                //GetInvestmentItemTable<Pipe>(tabla, response.BudgetItems, "F");
                //GetInvestmentItemTable<Instrument>(tabla, response.BudgetItems, "G");
                //GetInvestmentItemTable<Painting>(tabla, response.BudgetItems, "I");
                //GetInvestmentItemTable<EHS>(tabla, response.BudgetItems, "K");
                //GetInvestmentItemTable<Tax>(tabla, response.BudgetItems, "L");
                //GetInvestmentItemTable<Testing>(tabla, response.BudgetItems, "N");
                //GetInvestmentItemTable<EngineeringDesign>(tabla, response.BudgetItems, "O");


                //var EngineeringSalary = response.BudgetItems.OfType<Engineering>().FirstOrDefault();
                //if (EngineeringSalary != null)
                //{
                //    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //   .Padding(4).Text($"O)-{EngineeringSalary.Name}").FontSize(10);

                //    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //    .Padding(4).Text(EngineeringSalary.BudgetUSD.ToCurrencyCulture()).FontSize(10);
                //}

                //var Contingency = response.BudgetItems.OfType<Contingency>().FirstOrDefault();
                //if (Contingency != null)
                //{
                //    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //   .Padding(4).Text($"P)-{Contingency.Name}").FontSize(10);

                //    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                //    .Padding(4).Text(Contingency.BudgetUSD.ToCurrencyCulture()).FontSize(10);
                //}


                return tabla;
            }
            //void GetInvestmentItemTable<T>(TableDescriptor tablaTableDescriptor, List<BudgetItem> budgetitems, string Letter) where T : BudgetItem
            //{
            //    var items = budgetitems.OfType<T>().ToList();
            //    if (items.Count > 0)
            //    {
            //        var budget = items.Sum(x => x.BudgetUSD);
            //        tablaTableDescriptor.Cell().Border(0.5f).BorderColor("#D9D9D9")
            //        .Padding(4).Text($"{Letter})-{typeof(T).Name}s").FontSize(10);
            //        tablaTableDescriptor.Cell().Border(0.5f).BorderColor("#D9D9D9")
            //        .Padding(4).Text(budget.ToCurrencyCulture()).FontSize(10);
            //    }

            //}

            void SignContent(IContainer container, Project response)
            {

                container.Column(col1 =>
                {

                    col1.Item().Padding(30).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Approvals").FontSize(10).SemiBold();

                        });
                    });
                    col1.Item().Table(table => GetSign(table, response));
                });
            }
            TableDescriptor GetSign(TableDescriptor tabla, Project response)
            {
                tabla.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);

                });

                tabla.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text("Name").Bold();

                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                   .Padding(4).Text("Role").Bold();

                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                   .Padding(4).Text("Sign").Bold();

                    header.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text("Sign date").Bold();


                });


                foreach (var expert in response.StakeHolders)
                {
                    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text(expert.Name).FontSize(10);

                    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text(expert.Area).FontSize(10);

                    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text(string.Empty).FontSize(10);

                    tabla.Cell().Border(0.5f).BorderColor("#D9D9D9")
                    .Padding(4).Text(string.Empty).FontSize(10);
                }

                return tabla;
            }
            void DefinitionItems(IContainer container, Project response, ProjectDefinitionType type)
            {
                var list = response.DefinitionItems.Where(x => x.Type == type).ToList();
                if (list.Count == 0) return;
                container.Column(col1 =>
                {

                    col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span(type.GetDescription()).FontSize(10).SemiBold();

                        });
                    });


                    col1.Item().Column(col2 =>
                    {

                        foreach (var row in list)
                        {
                            col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
                            {
                                txt.Span($"{row.Name}").FontSize(10);

                            });

                        }

                    });
                });
            }

            void KnownRisk(IContainer container, Project response)
            {
                if (response.KnownRisks.Count == 0) return;
                container.Column(col1 =>
                {

                    col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Known risks:").FontSize(10).SemiBold();

                        });
                    });


                    col1.Item().Column(col2 =>
                    {

                        foreach (var row in response.KnownRisks)
                        {
                            col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
                            {
                                txt.Span($"{row.Name}").FontSize(10);

                            });

                        }

                    });
                });
            }

            void Requirements(IContainer container, Project response)
            {
                if (response.Requirements.Count == 0) return;
                container.Column(col1 =>
                {

                    col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Requirements:").FontSize(10).SemiBold();

                        });
                    });


                    col1.Item().Column(col2 =>
                    {

                        foreach (var row in response.Requirements)
                        {
                            col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
                            {
                                txt.Span($"{row.Name}").FontSize(10);

                            });

                        }

                    });
                });
            }

            //void LearnedLessons(IContainer container, Project response)
            //{
            //    if (response.LearnedLessons.Count == 0) return;
            //    container.Column(col1 =>
            //    {

            //        col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
            //        {
            //            col2.Item().Text(txt =>
            //            {
            //                txt.Span("Learned Lessons:").FontSize(10).SemiBold();

            //            });
            //        });


            //        col1.Item().Column(col2 =>
            //        {

            //            foreach (var row in response.LearnedLessons)
            //            {
            //                col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
            //                {
            //                    txt.Span($"{row.Name}").FontSize(10);

            //                });

            //            }

            //        });
            //    });
            //}
            void ExpertJudgements(IContainer container, Project response)
            {
                if (response.ExpertJudgements.Count == 0) return;
                container.Column(col1 =>
                {

                    col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().TranslateX(15).Text(txt =>
                        {
                            txt.Span("Expert Judgements:").FontSize(10).SemiBold();

                        });
                    });


                    col1.Item().Column(col2 =>
                    {

                        foreach (var row in response.ExpertJudgements)
                        {
                            col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
                            {
                                txt.Span($"{row.Name}").FontSize(10);

                            });

                        }

                    });
                });
            }

            void Qualitys(IContainer container, Project response)
            {
                if (response.Qualitys.Count == 0) return;
                container.Column(col1 =>
                {

                    col1.Item().TranslateX(15).PaddingBottom(5).Column(col2 =>
                    {
                        col2.Item().Text(txt =>
                        {
                            txt.Span("Qualitys:").FontSize(10).SemiBold();

                        });
                    });


                    col1.Item().Column(col2 =>
                    {

                        foreach (var row in response.Qualitys)
                        {
                            col1.Item().TranslateX(20).PaddingBottom(5).ShowEntire().AlignLeft().Text(txt =>
                            {
                                txt.Span($"{row.Name}").FontSize(10);

                            });

                        }

                    });
                });
            }
        }

    }
}
