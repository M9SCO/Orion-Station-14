using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Configuration;

namespace Content.Shared.Siberia;

/// <summary>
///     Siberia console variable definitions.
/// </summary>
[CVarDefs]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class SCCVars
{
    /**
     * Sponsors
     */

    /// <summary>
    ///     URL of the sponsors API server.
    /// </summary>
    public static readonly CVarDef<string> SponsorsApiUrl =
        CVarDef.Create("sponsor.api_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     Comma-separated list of species IDs that require a sponsor subscription.
    ///     Players without a subscription (or without the species in AllowedSpecies) cannot pick these.
    /// </summary>
    public static readonly CVarDef<string> SponsorLockedSpecies =
        CVarDef.Create("sponsor.locked_species", "", CVar.REPLICATED);

    /*
     * Discord Auth
     */

    /// <summary>
    ///     Whether Discord linking is enabled.
    /// </summary>
    public static readonly CVarDef<bool> DiscordAuthEnabled =
        CVarDef.Create("discord_auth.enabled", false, CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord auth API server.
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiUrl =
        CVarDef.Create("discord_auth.api_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     Secret key for the Discord auth API server.
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiKey =
        CVarDef.Create("discord_auth.api_key", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
