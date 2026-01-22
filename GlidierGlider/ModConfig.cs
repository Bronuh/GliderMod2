
using System.ComponentModel;

namespace GliderMod;

// TODO: write XML docs for every member, so AutoConfigLib can generate proper descriptions
public class ModConfig
{
    /// <summary>
    /// SAMPLE_TEXT
    /// </summary>
    [Category("Base settings")]
    public double SpeedMin = 0.0001f;
    [Category("Base settings")]
    public float SpeedFactor = 0.25f;
    [Category("Base settings")]
    public float SpeedMax = 1.5f;

    [Category("Base settings")]
    public float FallSpeed = 0.5f;

    [Category("Base settings")]
    public float VerticalLerpFactor = 25f;
    [Category("Base settings")]
    public float HorizontalLerpFactor = 40f;

    /// <summary>
    /// Allows controlled acceleration (sprint key) or deceleration (sneak key) while flying.
    /// </summary>
    [Category("Cheats")]
    public bool EnableJetMode = false;

    /// <summary>
    /// Controls how quickly the flight speed changes when using jet mode.
    /// </summary>
    [Category("Cheats")]
    public double JetModeFixedGlideFactor = 0.2;
}