using FluentValidation;
using VotingPoll.Core.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreateVoteRequestValidator : AbstractValidator<CreateVoteDto>
{
    public CreateVoteRequestValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("A poll user id is required");
        RuleFor(x => x.UserId).Length(2, 100).WithMessage("User id must be between 2 and 100 characters");
        RuleFor(x => x.PollOptionId).NotNull().NotEmpty().GreaterThanOrEqualTo(0)
            .WithMessage("A option id is required");
    }
}