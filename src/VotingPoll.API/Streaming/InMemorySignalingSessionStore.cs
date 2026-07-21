namespace VotingPoll.API.Streaming;

/// <inheritdoc cref="ISignalingSessionStore"/>
public sealed class InMemorySignalingSessionStore : ISignalingSessionStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, SignalingSession> _sessions = new();

    public SignalingSession CreateOffer(string offerSdp, string? label)
    {
        SignalingSession session = new() { OfferSdp = offerSdp, Label = label };

        lock (_lock)
        {
            // Exactly one device key exists (TrainingApi:DeviceKey), i.e. exactly one headset
            // can ever be streaming -- a new offer always means the previous stream ended
            // (or was abandoned, e.g. the app got killed rather than toggling watch off), so
            // any prior sessions are stale and would otherwise pile up in the live-sessions list.
            _sessions.Clear();
            _sessions[session.SessionId] = session;
        }

        return session;
    }

    public SignalingSession? Get(string sessionId)
    {
        lock (_lock)
        {
            return _sessions.GetValueOrDefault(sessionId);
        }
    }

    public IReadOnlyList<SignalingSession> GetActive()
    {
        lock (_lock)
        {
            return _sessions.Values.OrderByDescending(s => s.CreatedAtUtc).ToList();
        }
    }

    public bool TrySetAnswer(string sessionId, string answerSdp)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out SignalingSession? session))
                return false;

            session.AnswerSdp = answerSdp;
            return true;
        }
    }

    public bool TryAddIceCandidate(string sessionId, bool fromUnity, IceCandidateMessage candidate)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out SignalingSession? session))
                return false;

            // Unity's candidates queue up for the viewer to poll, and vice versa.
            (fromUnity ? session.IceForViewer : session.IceForUnity).Enqueue(candidate);
            return true;
        }
    }

    public IReadOnlyList<IceCandidateMessage> DrainIceCandidates(string sessionId, bool forUnity)
    {
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out SignalingSession? session))
                return Array.Empty<IceCandidateMessage>();

            System.Collections.Concurrent.ConcurrentQueue<IceCandidateMessage> queue =
                forUnity ? session.IceForUnity : session.IceForViewer;

            List<IceCandidateMessage> drained = new();
            while (queue.TryDequeue(out IceCandidateMessage? candidate))
                drained.Add(candidate);

            return drained;
        }
    }

    public void Clear(string sessionId)
    {
        lock (_lock)
        {
            _sessions.Remove(sessionId);
        }
    }
}
