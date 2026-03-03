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
            .WithMessage("Password must have at least 8 characters")
            .Matches(@"\d")
            .WithMessage("Password must contain at least one digit")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}