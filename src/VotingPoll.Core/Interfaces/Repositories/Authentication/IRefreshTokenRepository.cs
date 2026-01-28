using VotingPoll.Core.Entities.Authentication;

namespace VotingPoll.Core.Interfaces.Repositories.Authentication;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken?> GetRefreshTokenByStringAsync(string refreshToken);
    public Task<RefreshToken> CreateAsync(RefreshToken? refreshToken);
}