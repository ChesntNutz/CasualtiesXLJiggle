using System;
using CasualtiesExtra;

namespace CasualtiesJiggle
{
    internal static class CasualtiesExtraApi
    {
        public static int GetWeightStage(Body body)
        {
            try
            {
                ExpieData.LocalExpieData local = ExpieData.ClientExpieData(body);
                return local != null ? local.WeightStage : -1;
            }
            catch
            {
                return -1;
            }
        }

        public static bool GetStuck(Body body)
        {
            try
            {
                ExpieData.LocalExpieData local = ExpieData.ClientExpieData(body);
                return local != null && local.stuck;
            }
            catch
            {
                return false;
            }
        }
    }
}
