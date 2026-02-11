using FluentValidation;
using VotingPoll.Core.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreatePollOptionDtoValidator : AbstractValidator<CreatePollOptionDto>
{
    public CreatePollOptionDtoValidator()
    {
        RuleFor(x => x.PollOptionName).NotEmpty().WithMessage("A poll option name is required");
    }
}