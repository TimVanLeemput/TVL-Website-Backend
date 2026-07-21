namespace VotingPoll.API.Streaming;

/// <summary>An SDP offer or answer body.</summary>
public sealed class SdpMessage
{
    public string Sdp { get; set; } = string.Empty;
}

/// <summary>One trickled ICE candidate, mirroring the browser/Unity.WebRTC wire shape.</summary>
public sealed class IceCandidateMessage
{
    public string Candidate { get; set; } = string.Empty;
    public string? SdpMid { get; set; }
    public int? SdpMLineIndex { get; set; }
}
