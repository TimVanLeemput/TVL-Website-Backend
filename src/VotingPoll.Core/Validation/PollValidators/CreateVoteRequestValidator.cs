using FluentValidation;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreateVoteRequestValidator : AbstractValidator<CreateVoteDto>
{
    public CreateVoteRequestValidator()
    {
        RuleFor(x => x.PollOptionId).NotNull().NotEmpty().GreaterThanOrEqualTo(0)
            .WithMessage("A option id is required");
    }
}