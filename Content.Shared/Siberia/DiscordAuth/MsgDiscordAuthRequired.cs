// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Siberia.DiscordAuth;

/// <summary>
/// Server sends this to the client on connect if Discord authorization is required.
/// </summary>
public sealed class MsgDiscordAuthRequired : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public string AuthUrl = string.Empty;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        AuthUrl = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(AuthUrl);
    }
}
