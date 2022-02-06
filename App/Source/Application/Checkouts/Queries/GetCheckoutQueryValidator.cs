using FluentValidation;

namespace CloudSeedApp;

public class GetCheckoutQueryValidator : AbstractValidator<GetCheckoutQuery>
{
    public GetCheckoutQueryValidator()
    {
        RuleFor(v => v.CheckoutId)
            .NotEmpty();
    }
}