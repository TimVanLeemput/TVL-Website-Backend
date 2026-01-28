using VotingPoll.Core.Entities.Authentication;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;

public interface ITokenService
{
    public Task<string> GenerateAccessToken(User? user);

    public Task<RefreshToken> GenerateRefreshToken();
}