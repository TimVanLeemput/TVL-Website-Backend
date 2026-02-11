using FluentValidation;
using VotingPoll.Core.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreatePollDtoValidator : AbstractValidator<CreatePollDto>
{
    public CreatePollDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(3, 100);

        RuleFor(x => x.AllPollOptions)
            .NotEmpty()
            .Must(options => options != null && options.Count >= 2 && options.Count <= 10)
            .WithMessage("A poll must have at least 2 options");

        RuleForEach(x => x.AllPollOptions)
            .SetValidator(new CreatePollOptionDtoValidator());
    }
}