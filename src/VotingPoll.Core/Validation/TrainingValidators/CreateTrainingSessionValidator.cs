using System.Globalization;
using FluentValidation;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Infrastructure.Validation;

public class CreateTrainingSessionValidator : AbstractValidator<CreateTrainingSessionDto>
{
    public CreateTrainingSessionValidator()
    {
        RuleFor(x => x.SchemaVersion)
            .Equal(1)
            .WithMessage("Unsupported schemaVersion — this endpoint accepts version 1.");

        RuleFor(x => x.SessionId)
            .NotEmpty()
            .Must(sessionId => Guid.TryParse(sessionId, out _))
            .WithMessage("sessionId must be a GUID.");

        RuleFor(x => x.ProcedureId).NotEmpty().MaximumLength(128);
        RuleFor(x => x.ProcedureTitle).MaximumLength(256);

        RuleFor(x => x.StartedAtUtc)
            .NotEmpty()
            .Must(BeIso8601)
            .WithMessage("startedAtUtc must be an ISO-8601 timestamp.");

        RuleFor(x => x.CompletedAtUtc)
            .NotEmpty()
            .Must(BeIso8601)
            .WithMessage("completedAtUtc must be an ISO-8601 timestamp.");

        RuleFor(x => x)
            .Must(dto => !BeIso8601(dto.StartedAtUtc) || !BeIso8601(dto.CompletedAtUtc)
                         || Parse(dto.CompletedAtUtc) >= Parse(dto.StartedAtUtc))
            .WithMessage("completedAtUtc must not be before startedAtUtc.");

        RuleFor(x => x.DurationSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);

        RuleFor(x => x.Steps)
            .NotEmpty()
            .WithMessage("A session must contain at least one step.");

        RuleForEach(x => x.Steps).ChildRules(step =>
        {
            step.RuleFor(s => s.Index).GreaterThanOrEqualTo(0);
            step.RuleFor(s => s.DurationSeconds).GreaterThanOrEqualTo(0);
        });

        // Severity/ruleType are deliberately not whitelisted — future procedures add
        // new rule types and those must not bounce off the backend.
        RuleForEach(x => x.Errors).ChildRules(error =>
        {
            error.RuleFor(e => e.StepIndex).GreaterThanOrEqualTo(0);
            error.RuleFor(e => e.RuleType).NotEmpty();
            error.RuleFor(e => e.Severity).NotEmpty();
            error.RuleFor(e => e.ScoreDeduction).GreaterThan(0);
            error.RuleFor(e => e.AtSeconds).GreaterThanOrEqualTo(0);
        });

        RuleFor(x => x.Device).NotNull();
    }

    private static bool BeIso8601(string timestamp)
    {
        return DateTime.TryParse(timestamp, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out _);
    }

    private static DateTime Parse(string timestamp)
    {
        return DateTime.Parse(timestamp, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }
}
