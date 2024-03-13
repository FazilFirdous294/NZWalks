using FluentValidation;

namespace NZWalks.API.Validators
{
    public class UpdateWalkRequestValidator:AbstractValidator<Models.DTO.UpdateWalkRequest>
    {
        public UpdateWalkRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.length).GreaterThan(0);
        }
    }
}
