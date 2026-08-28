// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Content.Shared.Siberia;
using Content.Shared.Siberia.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Server.Siberia.Sponsors;

public sealed class SponsorsManager
{
    [Dependency] private readonly IServerNetManager _netMgr = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private readonly HttpClient _httpClient = new();
    private readonly Dictionary<NetUserId, SponsorInfo> _cachedSponsors = new();

    private ISawmill _logger = default!;
    private string _apiUrl = string.Empty;

    public void Initialize()
    {
        _logger = Logger.GetSawmill("sponsors");
        _cfg.OnValueChanged(SCCVars.SponsorsApiUrl, s => _apiUrl = s, true);

        _netMgr.RegisterNetMessage<MsgSponsorInfo>();

        _netMgr.Connecting += OnConnecting;
        _netMgr.Connected += OnConnected;
        _netMgr.Disconnect += OnDisconnect;
    }

    public bool TryGetInfo(NetUserId userId, [NotNullWhen(true)] out SponsorInfo? sponsor)
    {
        return _cachedSponsors.TryGetValue(userId, out sponsor);
    }

    private async Task OnConnecting(NetConnectingArgs args)
    {
        var info = await LoadSponsorInfo(args.UserId);
        if (info?.Tier == null)
        {
            _cachedSponsors.Remove(args.UserId);
            return;
        }

        _cachedSponsors[args.UserId] = info;
    }

    private void OnConnected(object? sender, NetChannelArgs args)
    {
        var info = _cachedSponsors.TryGetValue(args.Channel.UserId, out var sponsor) ? sponsor : null;
        var msg = new MsgSponsorInfo { Info = info };
        _netMgr.ServerSendMessage(msg, args.Channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs args)
    {
        _cachedSponsors.Remove(args.Channel.UserId);
    }

    private async Task<SponsorInfo?> LoadSponsorInfo(NetUserId userId)
    {
        if (string.IsNullOrEmpty(_apiUrl))
            return null;

        var url = $"{_apiUrl}/sponsors/{userId.ToString()}";
        var response = await _httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorText = await response.Content.ReadAsStringAsync();
            _logger.Error(
                "Failed to get player sponsor info from API: [{StatusCode}] {Response}",
                response.StatusCode,
                errorText);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<SponsorInfo>();
    }
}
