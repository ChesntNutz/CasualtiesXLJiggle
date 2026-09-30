namespace CasualtiesJiggle
{
    public sealed class SoftProfile
    {
        public string Name;
        public int LimbIndex;

        private bool _regionFromConfig;

        // Region that may deform, normalized 0..1 inside the sprite (x right, y UP), plus the
        // smooth falloff margin and the vertical bulge gain. Ignored when RegionFromConfig.
        private float _regionX;
        private float _regionY;
        private float _regionW;
        private float _regionH;
        private float _falloff = 0.1f;
        private float _bulge;

        // Multipliers over the global Soft* cfg values.
        private float _stiffnessMul = 1f; // neighbor springs (higher = firmer)
        private float _homePullMul = 1f; // pull back to rest shape (higher = snaps back faster)
        private float _dampingMul = 1f; // higher = settles faster
        private float _maxDispMul = 1f; // cap on how far a point may travel from home
        public float DriveMul = 1f; // how much of the gravity/accel drive reaches this mesh
        public float ImpulseMul = 1f; // footstep/jump/landing/eat kicks
        public float OverflowMul = 1f; // "fat spilling around an obstacle" strength

        public bool Collide = true;

        ///Approximate sprite pixels per grid cell (resolution adapts to sprite size).
        public const float CellPx = 5f;

        public float Stiffness => JiggleConfig.SoftStiffness.Value * _stiffnessMul;
        public float HomePull => JiggleConfig.SoftHomePull.Value * _homePullMul;
        public float Damping => JiggleConfig.SoftDamping.Value * _dampingMul;
        public float MaxDisp => JiggleConfig.SoftMaxDisp.Value * _maxDispMul;
        public float RumbleMul = 2f;

        public void GetRegion(
            out float x,
            out float y,
            out float w,
            out float h,
            out float falloff,
            out float bulge
        )
        {
            if (_regionFromConfig)
            {
                x = JiggleConfig.BellyRegionX.Value;
                y = JiggleConfig.BellyRegionY.Value;
                w = JiggleConfig.BellyRegionW.Value;
                h = JiggleConfig.BellyRegionH.Value;
                falloff = JiggleConfig.BellyFalloff.Value;
                bulge = JiggleConfig.BellyBulge.Value;
                return;
            }
            x = _regionX;
            y = _regionY;
            w = _regionW;
            h = _regionH;
            falloff = _falloff;
            bulge = _bulge;
        }

        public static readonly SoftProfile Belly = new()
        {
            Name = "belly",
            LimbIndex = 2,
            _regionFromConfig = true,
        };

        public static readonly bool ChestEnabled = false;

        public static readonly SoftProfile Chest = new()
        {
            Name = "chest",
            LimbIndex = 1,
            _regionFromConfig = false,
            _regionX = 0.30f,
            _regionY = 0.35f,
            _regionW = 0.55f,
            _regionH = 0.40f,
            _falloff = 0.12f,
            _bulge = 0.5f,
            _stiffnessMul = 1.6f,
            _homePullMul = 1.3f,
            _dampingMul = 1.3f,
            _maxDispMul = 0.5f,
            DriveMul = 0.5f,
            ImpulseMul = 0.5f,
            OverflowMul = 0.5f,
            Collide = true,
        };
    }
}
