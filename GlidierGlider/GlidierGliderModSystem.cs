using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace GliderMod
{
    public class GlidierGliderModSystem : ModSystem
    {
        public static ModConfig Config;
        protected const string HarmonyId = "glidierglider";
        protected Harmony Harmony;

        public override void Start(ICoreAPI api)
        {
            TryToLoadConfig(api);
            
            Harmony = new Harmony(HarmonyId);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void Dispose()
        {

            if (Harmony != null)
            {
                Harmony.UnpatchAll(HarmonyId);
            }
        }
        
        private void TryToLoadConfig(ICoreAPI api) 
        {
            try
            {
                Config = api.LoadModConfig<ModConfig>("GlidierGlider.json");
                if (Config == null)
                {
                    Config = new ModConfig();
                }
                //Save a copy of the mod config.
                api.StoreModConfig<ModConfig>(Config, "GlidierGlider.json");
            }
            catch (Exception e)
            {
                Mod.Logger.Error("Could not load config! Loading default settings instead.");
                Mod.Logger.Error(e);
                Config = new ModConfig();
            }
        }
    }

    [HarmonyPatch(typeof(PModuleInAir), "ApplyFlying")]
    public class GG_PMIA_ApplyFlying
    {
        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ApplyFlying(PModuleInAir __instance, float dt, Entity entity, EntityPos pos, EntityControls controls) { }
    }

    [HarmonyPatch(typeof(PModulePlayerInAir), "ApplyFlying")]
    public class GG_PMPIA_ApplyFlying
    {
        static double NormalizeAngle(double angle)
        {
            angle = angle % (2 * Math.PI);
            if (angle < 0) angle += 2 * Math.PI;
            return angle;
        }

        static double LerpAngle(double a, double b, double t)
        {
            a = NormalizeAngle(a);
            b = NormalizeAngle(b);
            double diff = b - a;
            if (diff > Math.PI)
            {
                if (a > b)
                {
                    a += 2 * Math.PI;
                }
                else
                {
                    b += 2 * Math.PI;
                }
                diff = b - a;
            }
            
            return a + diff * t;
        }

        static double NormalizePitch(double pitch)
        {
            if (pitch > Math.PI / 2) pitch = Math.PI - pitch;
            return pitch;
        }

        static double LerpPitch(double a, double b, double t)
        {
            a = NormalizePitch(a);
            b = NormalizePitch(b);
            double diff = b - a;
            return NormalizePitch(a + diff * t);
        }

        static bool Prefix(PModulePlayerInAir __instance, float dt, Entity entity, EntityPos pos, EntityControls controls)
        {
            if (controls.Gliding)
            {
                Vec3f viewVector = pos.GetViewVector().Normalize();

                if (controls.GlideSpeed == 0)
                {
                    controls.GlideSpeed = pos.Motion.Length();
                }
                double glideFactor = viewVector.Y;


                controls.GlideSpeed = GameMath.Clamp(controls.GlideSpeed - (glideFactor * dt * GlidierGliderModSystem.Config.SpeedFactor), 
                    GlidierGliderModSystem.Config.SpeedMin, 
                    GlidierGliderModSystem.Config.SpeedMax);

                double yaw = Math.Atan2(pos.Motion.X, pos.Motion.Z);

                double motionLength = pos.Motion.Length();
                double pitch = motionLength == 0 ? 0d : Math.Asin(pos.Motion.Y / motionLength);

                double lerpYaw = LerpAngle(yaw, pos.Yaw, GlidierGliderModSystem.Config.HorizontalLerpFactor * dt);
                double lerpPitch = LerpPitch(pitch, Math.PI - pos.Pitch, GlidierGliderModSystem.Config.VerticalLerpFactor * dt);

                double cosPitch = Math.Cos(lerpPitch);

                Vec3d smoothed = new Vec3d(cosPitch * Math.Sin(lerpYaw), Math.Sin(lerpPitch), cosPitch * Math.Cos(lerpYaw)).Normalize();

                pos.Motion = smoothed.Mul(controls.GlideSpeed);
                pos.Motion.Y -= GlidierGliderModSystem.Config.FallSpeed * dt;
            }
            else
            {
                GG_PMIA_ApplyFlying.ApplyFlying(__instance, dt, entity, pos, controls);
            }
            return false;
        }


    }
}


