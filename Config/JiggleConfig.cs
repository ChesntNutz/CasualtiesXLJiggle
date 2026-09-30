using BepInEx.Configuration;

namespace CasualtiesJiggle
{
    public static class JiggleConfig
    {
        // Master
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> DebugEnabled;
        public static ConfigEntry<float> Intensity; // global multiplier for everything

        // Spring feel, stiffness, damping, max squash
        public static ConfigEntry<float> BellyStiffness; // spring stiffness (omega^2); lower = wobblier
        public static ConfigEntry<float> Damping; // how quickly wobble settles (per second)
        public static ConfigEntry<float> MaxSquash; // hard clamp on belly displacement, world units
        public static ConfigEntry<float> MaxOffset; // clamp on other limbs' share of the spring
        public static ConfigEntry<float> InertiaGain; // how strongly acceleration drives the spring vs gravity
        public static ConfigEntry<float> GravitySag; // constant downward droop multiplier
        public static ConfigEntry<float> FallLift; // upward belly billow while falling
        public static ConfigEntry<float> FallSagFade; // standing droop cancelled while falling
        public static ConfigEntry<float> AccelSmoothTime; // smoothing of the body acceleration input
        public static ConfigEntry<float> AccelClamp; // acceleration clamp (teleport/collision safety)

        // Belly mesh (the localized deformation)
        public static ConfigEntry<bool> MeshBelly; // false = transform-only fallback (old look)
        public static ConfigEntry<int> MeshResolution; // grid subdivisions per axis
        public static ConfigEntry<float> BellyRegionX; // belly region in the DownTorso sprite,
        public static ConfigEntry<float> BellyRegionY; // normalized (0..1, origin bottom-left,
        public static ConfigEntry<float> BellyRegionW; // x to the right = character front,
        public static ConfigEntry<float> BellyRegionH; // y up). Only this area deforms.
        public static ConfigEntry<float> BellyFalloff; // smooth falloff margin around the region
        public static ConfigEntry<float> BellyBulge; // vertical bulge from horizontal squash

        // Soft body
        public static ConfigEntry<bool> SoftBody;
        public static ConfigEntry<float> SoftStiffness;
        public static ConfigEntry<float> SoftHomePull;
        public static ConfigEntry<float> SoftDamping;
        public static ConfigEntry<float> SoftMaxDisp;

        // Impulses
        public static ConfigEntry<float> FootstepImpulse;
        public static ConfigEntry<float> LandingImpulse;
        public static ConfigEntry<float> JumpImpulse;
        public static ConfigEntry<float> EatImpulse;
        public static ConfigEntry<float> RumbleAmount; // stomach rumble on gurgles / burps / sloshes

        // Stage coupling
        public static ConfigEntry<bool> ScaleWithWeight; // jiggle scales with WeightStage, OFF at stage 0
        public static ConfigEntry<bool> BellyBreathing; // subtle idle breathing on the belly
        public static ConfigEntry<bool> NeutralizeStuckScale; // undo CasualtiesExtra's stuck torso girdle

        // Environment
        public static ConfigEntry<float> WallSquash; // belly compression against walls when fat
        public static ConfigEntry<float> WallSquashMax; // max flatten, fraction of sprite width
        public static ConfigEntry<float> WallSquashDepth; // world-unit overlap that counts as full press

        public static void BindAll(ConfigFile config)
        {
            Enabled = config.Bind(
                "General",
                "Enabled",
                true,
                "Master toggle. Restart not required, applies to new bodies."
            );
            Intensity = config.Bind(
                "General",
                "Intensity",
                1f,
                new ConfigDescription(
                    "Global jiggle multiplier.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );

            BellyStiffness = config.Bind(
                "Springs",
                "BellyStiffness",
                26f,
                new ConfigDescription(
                    "Spring stiffness of the belly (omega squared). Lower = wobblier.",
                    new AcceptableValueRange<float>(5f, 120f)
                )
            );
            Damping = config.Bind(
                "Springs",
                "Damping",
                2.0f,
                new ConfigDescription(
                    "How fast wobble settles. Critical damping is 2*sqrt(BellyStiffness) (~10 here); lower = bouncier. ~2 gives a jelly feel.",
                    new AcceptableValueRange<float>(0.5f, 25f)
                )
            );
            MaxSquash = config.Bind(
                "Springs",
                "MaxSquash",
                0.8f,
                new ConfigDescription(
                    "Maximum belly displacement from the spring, in world units (game sprites are 8 PPU: 0.125 = 1 sprite pixel, 0.8 = 6.4 px).",
                    new AcceptableValueRange<float>(0.02f, 2f)
                )
            );
            MaxOffset = config.Bind(
                "Springs",
                "MaxOffset",
                0.25f,
                new ConfigDescription(
                    "Maximum positional share other limbs get from the spring, world units.",
                    new AcceptableValueRange<float>(0.05f, 2f)
                )
            );
            InertiaGain = config.Bind(
                "Springs",
                "InertiaGain",
                1.0f,
                new ConfigDescription(
                    "Multiplier on the whole (gravity - acceleration) drive. 1 = free fall cancels the sag exactly; higher = exaggerated inertia response.",
                    new AcceptableValueRange<float>(0f, 3f)
                )
            );
            GravitySag = config.Bind(
                "Springs",
                "GravitySag",
                1f,
                new ConfigDescription(
                    "Constant downward droop (belly sag) multiplier.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            FallLift = config.Bind(
                "Springs",
                "FallLift",
                1.0f,
                new ConfigDescription(
                    "Upward belly billow while falling, full-weight-range scale. 0 = off. At stage 8 and full fall speed the belly rides ~MaxSquash up.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            FallSagFade = config.Bind(
                "Springs",
                "FallSagFade",
                1.0f,
                new ConfigDescription(
                    "How much of the standing droop is cancelled while falling (airborne, descending). 1 = weightless fall.",
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
            AccelSmoothTime = config.Bind(
                "Springs",
                "AccelSmoothTime",
                0.06f,
                new ConfigDescription(
                    "Exponential smoothing time of the body acceleration input, seconds.",
                    new AcceptableValueRange<float>(0.01f, 0.3f)
                )
            );
            AccelClamp = config.Bind(
                "Springs",
                "AccelClamp",
                45f,
                new ConfigDescription(
                    "Body acceleration clamp, world units/s^2. Collisions and teleports can never exceed this.",
                    new AcceptableValueRange<float>(5f, 200f)
                )
            );

            MeshBelly = config.Bind(
                "Belly",
                "MeshBelly",
                true,
                "Deform the belly through a mesh (localized to the belly region — chest and hips never move). False = transform-only fallback (whole DownTorso sprite moves)."
            );
            MeshResolution = config.Bind(
                "Belly",
                "MeshResolution",
                12,
                new ConfigDescription(
                    "Grid subdivisions per axis of the belly mesh.",
                    new AcceptableValueRange<int>(4, 24)
                )
            );
            BellyRegionX = config.Bind(
                "Belly",
                "BellyRegionX",
                0.45f,
                new ConfigDescription(
                    "Belly region left edge, normalized (0..1) inside the DownTorso sprite (x right = character front).",
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
            BellyRegionY = config.Bind(
                "Belly",
                "BellyRegionY",
                0.05f,
                new ConfigDescription(
                    "Belly region bottom edge, normalized (0..1) inside the DownTorso sprite.",
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
            BellyRegionW = config.Bind(
                "Belly",
                "BellyRegionW",
                0.55f,
                new ConfigDescription(
                    "Belly region width, normalized.",
                    new AcceptableValueRange<float>(0.05f, 1f)
                )
            );
            BellyRegionH = config.Bind(
                "Belly",
                "BellyRegionH",
                0.60f,
                new ConfigDescription(
                    "Belly region height, normalized.",
                    new AcceptableValueRange<float>(0.05f, 1f)
                )
            );
            BellyFalloff = config.Bind(
                "Belly",
                "BellyFalloff",
                0.18f,
                new ConfigDescription(
                    "Smooth falloff margin around the belly region, normalized. Deformation is zero outside it.",
                    new AcceptableValueRange<float>(0.03f, 0.5f)
                )
            );
            BellyBulge = config.Bind(
                "Belly",
                "BellyBulge",
                0.5f,
                new ConfigDescription(
                    "Vertical bulge of the flesh when the belly is compressed horizontally.",
                    new AcceptableValueRange<float>(0f, 2f)
                )
            );

            SoftBody = config.Bind(
                "Soft",
                "SoftBody",
                true,
                "Per-vertex soft body on the belly mesh (verlet points + neighbor springs, WgMod-style). False = milestone-1 single-spring mesh deformation."
            );
            SoftStiffness = config.Bind(
                "Soft",
                "SoftStiffness",
                0.35f,
                new ConfigDescription(
                    "Neighbor spring strength (fraction of the length error corrected per step). Higher = firmer flesh, more wave propagation.",
                    new AcceptableValueRange<float>(0.05f, 1f)
                )
            );
            SoftHomePull = config.Bind(
                "Soft",
                "SoftHomePull",
                0.35f,
                new ConfigDescription(
                    "Pull of each point toward its driven home position (shape matching). Higher = snappier, less wobble.",
                    new AcceptableValueRange<float>(0.05f, 1f)
                )
            );
            SoftDamping = config.Bind(
                "Soft",
                "SoftDamping",
                2.0f,
                new ConfigDescription(
                    "Per-point velocity damping per second. Lower = jellier.",
                    new AcceptableValueRange<float>(0.5f, 10f)
                )
            );
            SoftMaxDisp = config.Bind(
                "Soft",
                "SoftMaxDisp",
                0.8f,
                new ConfigDescription(
                    "Per-vertex displacement clamp from home, world units (8 PPU: 0.125 = 1 sprite pixel).",
                    new AcceptableValueRange<float>(0.05f, 2f)
                )
            );

            FootstepImpulse = config.Bind(
                "Impulses",
                "FootstepImpulse",
                1f,
                new ConfigDescription(
                    "Belly kick on each footstep.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            LandingImpulse = config.Bind(
                "Impulses",
                "LandingImpulse",
                1f,
                new ConfigDescription(
                    "Belly compress on landing from a fall.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            JumpImpulse = config.Bind(
                "Impulses",
                "JumpImpulse",
                1f,
                new ConfigDescription(
                    "Belly lag kick on jumping.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            EatImpulse = config.Bind(
                "Impulses",
                "EatImpulse",
                1f,
                new ConfigDescription(
                    "Belly pulse on eating (the weight-gain 'Jiggle(amount)' hook).",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );

            RumbleAmount = config.Bind(
                "Impulses",
                "RumbleAmount",
                1f,
                new ConfigDescription(
                    "Stomach rumble: the belly churns on CasualtiesExtra's gurgles, tucks in and springs back on burps, and slaps sideways on sloshes. 0 = off. Scales with bloating and weight stage.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );

            ScaleWithWeight = config.Bind(
                "Weight",
                "ScaleWithWeight",
                true,
                "Jiggle amplitude scales with the weight gain mod's WeightStage / weightOffset. Stage 0 = no jiggle."
            );
            BellyBreathing = config.Bind(
                "Weight",
                "BellyBreathing",
                true,
                "Subtle idle breathing on the belly, scales with weight."
            );
            NeutralizeStuckScale = config.Bind(
                "Weight",
                "NeutralizeStuckScale",
                true,
                "Undo CasualtiesExtra's whole-torso 'stuck' scaling (limbs 1+2 x0.9/x0.75 when wedged at stage >= 2) so only the belly squashes against walls."
            );

            WallSquash = config.Bind(
                "Weight",
                "WallSquash",
                1f,
                new ConfigDescription(
                    "Belly compression against walls when fat.",
                    new AcceptableValueRange<float>(0f, 5f)
                )
            );
            WallSquashMax = config.Bind(
                "Weight",
                "WallSquashMax",
                0.3f,
                new ConfigDescription(
                    "Maximum wall compression as a fraction of the belly sprite's width.",
                    new AcceptableValueRange<float>(0.05f, 0.6f)
                )
            );
            WallSquashDepth = config.Bind(
                "Weight",
                "WallSquashDepth",
                1.2f,
                new ConfigDescription(
                    "How deep (world units) the sprite must press INTO the wall before compression reaches its maximum. Contact-only: nothing deforms before the sprite edge touches the wall.",
                    new AcceptableValueRange<float>(0.1f, 5f)
                )
            );
        }
    }
}
