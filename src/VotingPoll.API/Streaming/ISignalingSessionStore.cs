namespace VotingPoll.API.Streaming;

/// <summary>
/// Holds every active headset-to-viewer signaling session. In practice usually zero or
/// one (one headset), but kept keyed rather than singular so the dashboard's live-sessions
/// list and multiple simultaneous streams aren't precluded.
/// </summary>
public interface ISignalingSessionStore
{
    /// <summary>Starts a new session with Unity's offer, replacing any prior session(s) --
    /// there is only ever one headset, so an earlier session still on record at this point
    /// is stale (its stream ended without a clean /stop, e.g. the app was killed).</summary>
    SignalingSession CreateOffer(string offerSdp, string? label);

    /// <summary>A specific session, if it exists.</summary>
    SignalingSession? Get(string sessionId);

    /// <summary>Every currently active session, newest first -- the dashboard's live-sessions list.</summary>
    IReadOnlyList<SignalingSession> GetActive();

    /// <summary>Stores the viewer's answer against the given session. False if the session is gone/mismatched.</summary>
    bool TrySetAnswer(string sessionId, string answerSdp);

    /// <summary>Queues a trickled ICE candidate for the other side. False if the session is gone/mismatched.</summary>
    bool TryAddIceCandidate(string sessionId, bool fromUnity, IceCandidateMessage candidate);

    /// <summary>Drains (removes and returns) the candidates waiting for one side.</summary>
    IReadOnlyList<IceCandidateMessage> DrainIceCandidates(string sessionId, bool forUnity);

    /// <summary>Ends the session if it exists.</summary>
    void Clear(string sessionId);
}
