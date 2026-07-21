namespace VotingPoll.API.Streaming;

/// <summary>
/// Holds exactly one active headset-to-viewer signaling session at a time — this app has
/// one trainee headset, so there is never a session to disambiguate between.
/// </summary>
public interface ISignalingSessionStore
{
    /// <summary>Starts a new session (replacing any previous one) with Unity's offer.</summary>
    SignalingSession CreateOffer(string offerSdp);

    /// <summary>The current session, if a headset has posted an offer.</summary>
    SignalingSession? GetCurrent();

    /// <summary>Stores the viewer's answer against the given session. False if the session is gone/mismatched.</summary>
    bool TrySetAnswer(string sessionId, string answerSdp);

    /// <summary>Queues a trickled ICE candidate for the other side. False if the session is gone/mismatched.</summary>
    bool TryAddIceCandidate(string sessionId, bool fromUnity, IceCandidateMessage candidate);

    /// <summary>Drains (removes and returns) the candidates waiting for one side.</summary>
    IReadOnlyList<IceCandidateMessage> DrainIceCandidates(string sessionId, bool forUnity);

    /// <summary>Ends the session if it matches the given id.</summary>
    void Clear(string sessionId);
}
