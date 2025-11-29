using CllientMudBlazor.Templates;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.Dtos.General;
using Shared.Dtos.Starts.AcceptanceCriterias;

namespace CllientMudBlazor.Pages.Projects.Starts.AcceptanceCriterias
{
    public partial class AcceptanceCriteria
    {
        string Title => "Acceptance Criterias";

        [Parameter]
        public Guid ProjectId { get; set; }
        [Parameter]
        public EventCallback GetAllParent { get; set; }

        List<AcceptanceCriteriaDto> Items = new();
        string nameFilter = string.Empty;
        Func<AcceptanceCriteriaDto, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);

        public List<AcceptanceCriteriaDto> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();
        async Task OnNameFilter(string namefilter)
        {
            nameFilter = namefilter;
            await Task.Delay(1);
        }
        protected override async Task OnParametersSetAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var result = await HttpService.PostAsync<GetAllAcceptanceCriterias, GeneralDto<List<AcceptanceCriteriaDto>>>(new GetAllAcceptanceCriterias()
            {
                ProjectId = ProjectId,
            });
            if (result.Suceeded)
            {
                Items = result.Data.OrderBy(x=>x.Order).ToList();
                if (GetAllParent.HasDelegate) await GetAllParent.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task Add()
        {
            CreateAcceptanceCriteria dto = new()
            {
                ProjectId = ProjectId,
            };
            var parameters = new DialogParameters<AcceptanceCriteriaDialog>
            {
                { x => x.Model, dto},
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<AcceptanceCriteriaDialog>("Add Acceptance Criteria", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(AcceptanceCriteriaDto dto)
        {
            var parameters = new DialogParameters<AcceptanceCriteriaDialog>
        {

            { x => x.Model, dto},
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<AcceptanceCriteriaDialog>("Edit Acceptance Criteria", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAll();
            }
        }
        public async Task Delete(AcceptanceCriteriaDto dto)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {dto.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteAcceptanceCriteria request = new()
                {
                    Id = dto.Id,
                    ProjectId = ProjectId,


                };
                var resultDelete = await HttpService.PostAsync<DeleteAcceptanceCriteria, GeneralDto>(request);
                if (resultDelete.Suceeded)
                {
                    await GetAll();



                }

            }

        }

        public async Task OrderUp(AcceptanceCriteriaDto dto)
        {
            ChangeOrderAcceptanceCriteria neworder = new()
            {
                Id = dto.Id,
              
                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order - 1;
            var result = await HttpService.PostAsync<ChangeOrderAcceptanceCriteria, GeneralDto>(neworder);
            if(result.Suceeded)
            {
                await GetAll();
            }


        }
        public async Task OrderDown(AcceptanceCriteriaDto dto)
        {
            ChangeOrderAcceptanceCriteria neworder = new()
            {
                Id = dto.Id,
      
                ProjectId = ProjectId,
            };
            neworder.NewOrder = dto.Order + 1;
            var result = await HttpService.PostAsync<ChangeOrderAcceptanceCriteria, GeneralDto>(neworder);
            if (result.Suceeded)
            {
                await GetAll();
            }
        }
    }
}
