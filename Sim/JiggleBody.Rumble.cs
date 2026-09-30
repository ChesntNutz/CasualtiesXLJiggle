using UnityEngine;

namespace CasualtiesJiggle
{
    public partial class JiggleBody
    {
        private float _rumbleLeft; // seconds remaining
        private float _rumbleDur; // total length of the current rumble (envelope shape)
        private float _rumbleAmp; // target-offset amplitude, world units
        private float _rumbleFreq = 6.5f;
        private float _rumblePhase;
        private float _rumbleSeed;
        private float _reboundT; // burp: delay until the belly springs back out
        private Vector2 _reboundDir;
        private float _reboundImp;
        private float _sloshSide = 1f;

        private static float BloatFactor(float bloat)
        {
            return Mathf.Clamp(0.5f + Mathf.Log10(Mathf.Max(1f, bloat)), 0.5f, 1.8f);
        }

        private bool CanRumble()
        {
            return _share != null
                && _body != null
                && _bellyIndex >= 0
                && _wobble > 0.001f
                && JiggleConfig.RumbleAmount.Value > 0f;
        }

        private void StartRumble(float amp, float duration, float freq)
        {
            // a fresh rumble never cuts a louder one that is still running
            float remaining =
                _rumbleLeft > 0f
                    ? _rumbleAmp * Mathf.Clamp01(_rumbleLeft / Mathf.Max(0.05f, _rumbleDur))
                    : 0f;
            _rumbleAmp = Mathf.Max(amp, remaining);
            _rumbleDur = _rumbleLeft = Mathf.Clamp(duration, 0.3f, 4f);
            _rumbleFreq = freq;
            _rumbleSeed = UnityEngine.Random.Range(0f, 100f);
        }

        /// Gurgle: the belly churns for the length of the sound.
        public void OnGurgle(float bloat, float duration)
        {
            if (!CanRumble())
                return;
            float amp =
                0.15f
                * BloatFactor(bloat)
                * Mathf.Min(_wobble, 1.5f)
                * JiggleConfig.RumbleAmount.Value;
            StartRumble(amp, duration, 6.5f);
        }

        /// Burp: the belly should hopefully if I'm not a dumbass tuck itself in and up as the ""air"" rises, then springs back out a moment
        public void OnBurp(float bloat)
        {
            if (!CanRumble())
                return;
            float f = BloatFactor(bloat) * Mathf.Min(_wobble, 2f) * JiggleConfig.RumbleAmount.Value;
            Vector2 inward = new Vector2(-1f, 0.35f).normalized;
            _sVel += inward * (f * 0.6f);
            SoftImpulseDir(inward, f);
            _reboundDir = -inward;
            _reboundImp = f * 1.1f;
            _reboundT = 0.18f;
            StartRumble(0.11f * f, 0.5f, 11f);
        }

        /// hehe funy slosh
        public void OnSlosh(float bloat)
        {
            if (!CanRumble())
                return;
            float f =
                0.35f
                * BloatFactor(bloat)
                * Mathf.Min(_wobble, 2f)
                * JiggleConfig.RumbleAmount.Value;
            Vector2 dir = new Vector2(_sloshSide, 0f);
            _sloshSide = -_sloshSide;
            _sVel += dir * (f * 0.6f);
            SoftImpulseDir(dir, f);
        }
    }
}
