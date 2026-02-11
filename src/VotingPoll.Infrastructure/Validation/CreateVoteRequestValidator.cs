using FluentValidation;
using VotingPoll.Core.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreateVoteRequestValidator : AbstractValidator<CreateVoteDto>
{
    public CreateVoteRequestValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("A poll user id is required");
        RuleFor(x => x.PollOptionId).NotNull().NotEmpty().WithMessage("A option id is required");
    }
}