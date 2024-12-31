using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._EinsteinEngines.CCVar;

[CVarDefs]
public sealed partial class EinsteinCCVars : CVars
{
    // TODO: Move the rest of the announcer code to _EinsteinEngines

    /*
        * Announcers
        */

    /// <summary>
    ///     Weighted list of announcers to choose from
    /// </summary>
    public static readonly CVarDef<string> AnnouncerList =
        CVarDef.Create("announcer.list", "RandomAnnouncers", CVar.REPLICATED);

    /// <summary>
    ///     Optionally force set an announcer
    /// </summary>
    public static readonly CVarDef<string> Announcer =
        CVarDef.Create("announcer.announcer", "", CVar.SERVERONLY);

    /// <summary>
    ///     Optionally blacklist announcers
    ///     List of IDs separated by commas
    /// </summary>
    public static readonly CVarDef<string> AnnouncerBlacklist =
        CVarDef.Create("announcer.blacklist", "", CVar.SERVERONLY);

    /// <summary>
    ///     Changes how loud the announcers are for the client
    /// </summary>
    public static readonly CVarDef<float> AnnouncerVolume =
        CVarDef.Create("announcer.volume", 0.5f, CVar.ARCHIVE | CVar.CLIENTONLY);

    /// <summary>
    ///     Disables multiple announcement sounds from playing at once
    /// </summary>
    public static readonly CVarDef<bool> AnnouncerDisableMultipleSounds =
        CVarDef.Create("announcer.disable_multiple_sounds", false, CVar.ARCHIVE | CVar.CLIENTONLY);



    #region Contests System

    /// <summary>
    ///     The MASTER TOGGLE for the entire Contests System.
    ///     ALL CONTESTS BELOW, regardless of type or setting will output 1f when false.
    /// </summary>
    public static readonly CVarDef<bool> DoContestsSystem =
        CVarDef.Create("contests.do_contests_system", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     Contest functions normally include an optional override to bypass the clamp set by max_percentage.
    ///     This CVar disables the bypass when false, forcing all implementations to comply with max_percentage.
    /// </summary>
    public static readonly CVarDef<bool> AllowClampOverride =
        CVarDef.Create("contests.allow_clamp_override", true, CVar.REPLICATED | CVar.SERVER);
    /// <summary>
    ///     Toggles all MassContest functions. All mass contests output 1f when false
    /// </summary>
    public static readonly CVarDef<bool> DoMassContests =
        CVarDef.Create("contests.do_mass_contests", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     Toggles all StaminaContest functions. All stamina contests output 1f when false
    /// </summary>
    public static readonly CVarDef<bool> DoStaminaContests =
        CVarDef.Create("contests.do_stamina_contests", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     Toggles all HealthContest functions. All health contests output 1f when false
    /// </summary>
    public static readonly CVarDef<bool> DoHealthContests =
        CVarDef.Create("contests.do_health_contests", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     Toggles all MindContest functions. All mind contests output 1f when false.
    ///     MindContests are not currently implemented, and are awaiting completion of the Psionic Refactor
    /// </summary>
    public static readonly CVarDef<bool> DoMindContests =
        CVarDef.Create("contests.do_mind_contests", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     Toggles all MoodContest functions. All mood contests output 1f when false.
    /// </summary>
    public static readonly CVarDef<bool> DoMoodContests =
        CVarDef.Create("contests.do_mood_contests", true, CVar.REPLICATED | CVar.SERVER);

    /// <summary>
    ///     The maximum amount that Mass Contests can modify a physics multiplier, given as a +/- percentage
    ///     Default of 0.25f outputs between * 0.75f and 1.25f
    /// </summary>
    public static readonly CVarDef<float> MassContestsMaxPercentage =
        CVarDef.Create("contests.max_percentage", 0.25f, CVar.REPLICATED | CVar.SERVER);

    #endregion

    #region Glimmer System

    /// <summary>
    ///    Whether glimmer is enabled.
    /// </summary>
    public static readonly CVarDef<bool> GlimmerEnabled =
        CVarDef.Create("glimmer.enabled", true, CVar.REPLICATED);

    /// <summary>
    ///     Passive glimmer drain per second.
    ///     Note that this is randomized and this is an average value.
    /// </summary>
    public static readonly CVarDef<float> GlimmerLostPerSecond =
        CVarDef.Create("glimmer.passive_drain_per_second", 0.1f, CVar.SERVERONLY);

    /// <summary>
    ///     Whether random rolls for psionics are allowed.
    ///     Guaranteed psionics will still go through.
    /// </summary>
    public static readonly CVarDef<bool> PsionicRollsEnabled =
        CVarDef.Create("psionics.rolls_enabled", true, CVar.SERVERONLY);

    /// <summary>
    ///     Whether height & width sliders adjust a character's Fixture Component
    /// </summary>
    public static readonly CVarDef<bool> HeightAdjustModifiesHitbox =
        CVarDef.Create("heightadjust.modifies_hitbox", true, CVar.SERVERONLY);

    /// <summary>
    ///     Whether height & width sliders adjust a player's max view distance
    /// </summary>
    public static readonly CVarDef<bool> HeightAdjustModifiesZoom =
        CVarDef.Create("heightadjust.modifies_zoom", false, CVar.SERVERONLY);

    /// <summary>
    ///     Whether height & width sliders adjust a player's bloodstream volume.
    /// </summary>
    /// <remarks>
    ///     This can be configured more precisely by modifying BloodstreamAffectedByMassComponent.
    /// </remarks>
    public static readonly CVarDef<bool> HeightAdjustModifiesBloodstream =
        CVarDef.Create("heightadjust.modifies_bloodstream", true, CVar.SERVERONLY);

    /// <summary>
    ///     Enables station goals
    /// </summary>
    public static readonly CVarDef<bool> StationGoalsEnabled =
        CVarDef.Create("game.station_goals", true, CVar.SERVERONLY);

    /// <summary>
    ///     Chance for a station goal to be sent
    /// </summary>
    public static readonly CVarDef<float> StationGoalsChance =
        CVarDef.Create("game.station_goals_chance", 0.1f, CVar.SERVERONLY);

    #endregion

}
