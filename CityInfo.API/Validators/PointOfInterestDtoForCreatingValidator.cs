using CityInfo.API.Modles;
using FluentValidation;

namespace CityInfo.API.Validators
{
    public class PointOfInterestDtoForCreatingValidator : AbstractValidator<PointOfInterestDtoForCreating>
    {
        public PointOfInterestDtoForCreatingValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("The name is required")
                .MaximumLength(50);
            RuleFor(e => e.Description).MaximumLength(200);
        }
    }
}
