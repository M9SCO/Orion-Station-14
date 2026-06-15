using System.IO;
using System.Text.Json.Serialization;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Siberia.Sponsors;

/// <summary>
/// Server sends sponsor info to client on connect if the user is a sponsor.
/// </summary>
public sealed class MsgSponsorInfo : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public SponsorInfo? Info;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var isSponsor = buffer.ReadBoolean();
        buffer.ReadPadBits();

        if (!isSponsor)
            return;

        var length = buffer.ReadVariableInt32();
        var stream = new MemoryStream();
        buffer.ReadAlignedMemory(stream, length);
        serializer.DeserializeDirect(stream, out Info);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Info != null);
        buffer.WritePadBits();

        if (Info == null)
            return;

        var stream = new MemoryStream();
        serializer.SerializeDirect(stream, Info);
        buffer.WriteVariableInt32((int) stream.Length);
        buffer.Write(stream.AsSpan());
    }
}

[Serializable, NetSerializable]
public sealed class SponsorInfo
{
    [JsonPropertyName("tier")]
    public int? Tier { get; set; }

    [JsonPropertyName("oocColor")]
    public string? OOCColor { get; set; }

    [JsonPropertyName("priorityJoin")]
    public bool HavePriorityJoin { get; set; }

    [JsonPropertyName("extraSlots")]
    public int ExtraSlots { get; set; }

    [JsonPropertyName("allowedMarkings")]
    public string[] AllowedMarkings { get; set; } = Array.Empty<string>();

    // Siberia - new fields

    [JsonPropertyName("allowedSpecies")]
    public string[] AllowedSpecies { get; set; } = Array.Empty<string>();

    [JsonPropertyName("rolePriority")]
    public bool RolePriority { get; set; }
}
