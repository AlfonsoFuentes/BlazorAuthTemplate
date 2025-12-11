using CllientMudBlazor.Services.HttPServives;
using FluentValidation;
using Shared.Dtos.Brands;
using Shared.Dtos.StakeHolders;
using Shared.Dtos.Starts.ExpertJudgements;

namespace CllientMudBlazor.Pages.Projects.Starts.ExpertJudgements
{
    public class ExpertJudgementValidator : AbstractValidator<ExpertJudgementDto>
    {
        private IHttpServices Service;
        public ExpertJudgementValidator(IHttpServices service)
        {
            Service = service;
            RuleFor(x => x.Name).NotEmpty().WithMessage("Expert concept must be defined!");

            RuleFor(x => x.Expert).NotNull().WithMessage("Expert must be defined!");

            //RuleFor(x => x.Expert).MustAsync(ReviewIfExpertExist)
            //    .When(x => x.Expert != null)
            //    .WithMessage(x => $"{x.Expert!.Name} already exist");



        }
        async Task<bool> ReviewIfExpertExist(ExpertJudgementDto request, StakeHolderDto? name, CancellationToken cancellationToken)
        {
            ValidateExpertJudgementName validate = new()
            {
                ExpertId = request.Expert != null ? request.Expert.Id : Guid.Empty,
                Id = request.Id,
                ProjectId = request.ProjectId,


            };
            var response = await Service.PostForValidationAsync(validate);
            return !response;
        }
    }
}
