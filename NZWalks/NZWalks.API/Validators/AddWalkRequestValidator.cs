using FluentValidation;


namespace NZWalks.API.Validators
{
    public class AddWalkRequestValidator: AbstractValidator<Models.DTO.AddWalkRequest>
    {
        public AddWalkRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.length).GreaterThan(0);
        }
    }
}
