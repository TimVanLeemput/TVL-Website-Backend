using Microsoft.AspNetCore.Mvc;
using VotingPoll.API.Filters;
using VotingPoll.API.Streaming;
using System.Linq;

namespace VotingPoll.API.Controllers;

/// <summary>
/// WebRTC signaling relay for the live-watch stream (headset view -> dashboard viewer).
/// Pure handshake plumbing: SDP offer/answer and trickled ICE candidates, held in memory
/// only. No training data touches this controller — see <see cref="TrainingSessionsController"/>
/// for that. Unity-facing actions use the same device-key auth as session upload; the
/// viewer (dashboard) side is unauthenticated for now, matching the dashboard's lack of a
/// live-watch consent/instructor-role system yet.
/// </summary>
[ApiController]
[Route("api/streaming")]
public class SignalingController : ControllerBase
{
    private readonly ISignalingSessionStore _store;

    public SignalingController(ISignalingSessionStore store)
    {
        _store = store;
    }

    /// <summary>Unity starts a stream by posting its SDP offer (optionally labeled, e.g. the procedure title).</summary>
    [RequireDeviceKey]
    [HttpPost("offer")]
    public ActionResult<object> PostOffer(SdpMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.Sdp))
            return BadRequest();

        SignalingSession session = _store.CreateOffer(message.Sdp, message.Label);
        return Ok(new { sessionId = session.SessionId });
    }

    /// <summary>The dashboard's live-sessions list: every stream currently live, newest first.</summary>
    [HttpGet("sessions")]
    public ActionResult<IReadOnlyList<LiveSessionSummary>> GetActiveSessions()
    {
        IReadOnlyList<LiveSessionSummary> summaries = _store.GetActive()
            .Select(s => new LiveSessionSummary { SessionId = s.SessionId, Label = s.Label, StartedAtUtc = s.CreatedAtUtc })
            .ToList();

        return Ok(summaries);
    }

    /// <summary>The viewer fetches a specific session's offer once it's picked from the live-sessions list.</summary>
    [HttpGet("offer")]
    public ActionResult<object> GetOffer(string sessionId)
    {
        SignalingSession? session = _store.Get(sessionId);
        if (session?.OfferSdp == null)
            return NoContent();

        return Ok(new { sessionId = session.SessionId, sdp = session.OfferSdp });
    }

    /// <summary>The viewer answers the offer.</summary>
    [HttpPost("answer")]
    public ActionResult PostAnswer(string sessionId, SdpMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.Sdp) || !_store.TrySetAnswer(sessionId, message.Sdp))
            return NotFound();

        return NoContent();
    }

    /// <summary>Unity polls for the viewer's answer.</summary>
    [RequireDeviceKey]
    [HttpGet("answer")]
    public ActionResult<object> GetAnswer(string sessionId)
    {
        SignalingSession? session = _store.Get(sessionId);
        if (session?.AnswerSdp == null)
            return NoContent();

        return Ok(new { sdp = session.AnswerSdp });
    }

    /// <summary>Unity trickles an ICE candidate for the viewer.</summary>
    [RequireDeviceKey]
    [HttpPost("ice/unity")]
    public ActionResult PostIceFromUnity(string sessionId, IceCandidateMessage message)
    {
        if (!_store.TryAddIceCandidate(sessionId, fromUnity: true, message))
            return NotFound();

        return NoContent();
    }

    /// <summary>The viewer trickles an ICE candidate for Unity.</summary>
    [HttpPost("ice/viewer")]
    public ActionResult PostIceFromViewer(string sessionId, IceCandidateMessage message)
    {
        if (!_store.TryAddIceCandidate(sessionId, fromUnity: false, message))
            return NotFound();

        return NoContent();
    }

    /// <summary>Unity drains ICE candidates trickled by the viewer.</summary>
    [RequireDeviceKey]
    [HttpGet("ice/unity")]
    public ActionResult<IReadOnlyList<IceCandidateMessage>> GetIceForUnity(string sessionId)
    {
        return Ok(_store.DrainIceCandidates(sessionId, forUnity: true));
    }

    /// <summary>The viewer drains ICE candidates trickled by Unity.</summary>
    [HttpGet("ice/viewer")]
    public ActionResult<IReadOnlyList<IceCandidateMessage>> GetIceForViewer(string sessionId)
    {
        return Ok(_store.DrainIceCandidates(sessionId, forUnity: false));
    }

    /// <summary>Unity ends the stream (watch toggle turned off, or app shutdown).</summary>
    [RequireDeviceKey]
    [HttpPost("stop")]
    public ActionResult Stop(string sessionId)
    {
        _store.Clear(sessionId);
        return NoContent();
    }
}
