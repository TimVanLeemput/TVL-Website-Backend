using Microsoft.Extensions.Configuration;
using Resend;

namespace VotingPoll.Core.Services.EmailService;

public class EmailService
{
    private readonly IResend _resend;
    private readonly string _frontendUrl;

    public EmailService(IResend resend, IConfiguration configuration)
    {
        _resend = resend;
        _frontendUrl = configuration["App:FrontendUrl"]!;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string token)
    {
        string verifyUrl = $"{_frontendUrl}/verify?token={token}";

        EmailMessage message = new EmailMessage();
        message.From = "noreply@timvanleemput.com";
        message.To.Add(toEmail);
        message.Subject = "Verify your email — Tim Van Leemput";
        message.HtmlBody = $"""
            <p>Thanks for registering!</p>
            <p>Click the link below to verify your email address. The link expires in 24 hours.</p>
            <p><a href="{verifyUrl}">Verify my email</a></p>
            <p>If you didn't register, you can ignore this email.</p>
            """;

        await _resend.EmailSendAsync(message);
    }
}
