using System.Collections.Concurrent;

namespace VotingPoll.API.Streaming;

/// <summary>
/// One headset-to-viewer WebRTC handshake in progress. In-memory only, no persistence —
/// this is signaling plumbing for the live-watch stream, not training data.
/// </summary>
public sealed class SignalingSession
{
    public string SessionId { get; } = Guid.NewGuid().ToString("N");

    /// <summary>Trainee-facing label for the dashboard's live-sessions list, e.g. the procedure title.</summary>
    public string? Label { get; set; }

    public string? OfferSdp { get; set; }
    public string? AnswerSdp { get; set; }
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Candidates gathered by Unity, waiting for the viewer to poll them.</summary>
    public ConcurrentQueue<IceCandidateMessage> IceForViewer { get; } = new();

    /// <summary>Candidates gathered by the viewer, waiting for Unity to poll them.</summary>
    public ConcurrentQueue<IceCandidateMessage> IceForUnity { get; } = new();
}
