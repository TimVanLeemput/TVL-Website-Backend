using FluentValidation;
using VotingPoll.Core.Models.DTOs.Authentication;

namespace VotingPoll.Core.Validation.AuthValidators;

public class RegisterRequestPasswordValidator : AbstractValidator<AuthDto.RegisterRequest>
{
    public RegisterRequestPasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 100)
            .WithMessage("At least 8 characters")
            .Matches(@"\d")
            .WithMessage("At least one digit")
            .Matches(@"[A-Z]")
            .WithMessage("At least one uppercase letter")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("At least one special character");
    }
}