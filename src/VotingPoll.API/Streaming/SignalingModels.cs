namespace VotingPoll.API.Streaming;

/// <summary>An SDP offer or answer body. <see cref="Label"/> is only meaningful on the offer (Unity -> relay).</summary>
public sealed class SdpMessage
{
    public string Sdp { get; set; } = string.Empty;
    public string? Label { get; set; }
}

/// <summary>Summary row for the dashboard's live-sessions list.</summary>
public sealed class LiveSessionSummary
{
    public string SessionId { get; set; } = string.Empty;
    public string? Label { get; set; }
    public DateTime StartedAtUtc { get; set; }
}

/// <summary>One trickled ICE candidate, mirroring the browser/Unity.WebRTC wire shape.</summary>
public sealed class IceCandidateMessage
{
    public string Candidate { get; set; } = string.Empty;
    public string? SdpMid { get; set; }
    public int? SdpMLineIndex { get; set; }
}
