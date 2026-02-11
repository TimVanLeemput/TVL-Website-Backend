using FluentValidation;
using VotingPoll.Core.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class UpdatePollValidator : AbstractValidator<UpdatePollDto>
{
    public UpdatePollValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(3, 100);
        RuleFor(x => x.ClosesAt).NotEmpty();
    }
}