using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Server.Spreader;

/// <summary>
/// Handles entities that spread out when they reach the relevant growth level.
/// </summary>
[RegisterComponent]
public sealed partial class CrystalMassComponent : Component
{

    [DataField] 
    public SoundSpecifier DustSound = new SoundPathSpecifier("/Audio/_EinsteinEngines/Supermatter/supermatter.ogg");

    /// <summary>
    /// At level 3 spreading can occur; prior to that we have a chance of increasing our growth level and changing our sprite.
    /// </summary>
    [DataField]
    public int GrowthLevel = 1;

    /// <summary>
    /// Chance to spread whenever an edge spread is possible.
    /// </summary>
    [DataField]
    public float SpreadChance = 1f;

    /// <summary>
    /// How much damage is required to reduce growth level
    /// </summary>
    [DataField]
    public float GrowthHealth = 10.0f;

    /// <summary>
    /// How much damage is required to prevent growth
    /// </summary>
    [DataField]
    public float GrowthBlock = 20.0f;

    /// <summary>
    /// How much the kudzu heals each tick
    /// </summary>
    [DataField]
    public DamageSpecifier? DamageRecovery = null;

    [DataField]
    public float GrowthTickChance = 1f;

    /// <summary>
    /// number of sprite variations for kudzu
    /// </summary>
    [DataField]
    public int SpriteVariants = 7;

    /// <summary>
    /// What spawns in the place of an unfortunate entity that got removed by the crystal.
    /// </summary>
    [DataField]
    public string CollisionResultPrototype = "Ash";
}
