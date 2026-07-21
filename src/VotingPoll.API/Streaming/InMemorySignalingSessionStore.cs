namespace VotingPoll.API.Streaming;

/// <inheritdoc cref="ISignalingSessionStore"/>
public sealed class InMemorySignalingSessionStore : ISignalingSessionStore
{
    private readonly object _lock = new();
    private SignalingSession? _current;

    public SignalingSession CreateOffer(string offerSdp)
    {
        lock (_lock)
        {
            _current = new SignalingSession { OfferSdp = offerSdp };
            return _current;
        }
    }

    public SignalingSession? GetCurrent()
    {
        lock (_lock)
        {
            return _current;
        }
    }

    public bool TrySetAnswer(string sessionId, string answerSdp)
    {
        lock (_lock)
        {
            if (_current == null || _current.SessionId != sessionId)
                return false;

            _current.AnswerSdp = answerSdp;
            return true;
        }
    }

    public bool TryAddIceCandidate(string sessionId, bool fromUnity, IceCandidateMessage candidate)
    {
        lock (_lock)
        {
            if (_current == null || _current.SessionId != sessionId)
                return false;

            // Unity's candidates queue up for the viewer to poll, and vice versa.
            (fromUnity ? _current.IceForViewer : _current.IceForUnity).Enqueue(candidate);
            return true;
        }
    }

    public IReadOnlyList<IceCandidateMessage> DrainIceCandidates(string sessionId, bool forUnity)
    {
        lock (_lock)
        {
            if (_current == null || _current.SessionId != sessionId)
                return Array.Empty<IceCandidateMessage>();

            System.Collections.Concurrent.ConcurrentQueue<IceCandidateMessage> queue =
                forUnity ? _current.IceForUnity : _current.IceForViewer;

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
            if (_current != null && _current.SessionId == sessionId)
                _current = null;
        }
    }
}
