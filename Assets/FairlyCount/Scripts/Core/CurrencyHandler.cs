using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount.Core
{
    public static class CurrencyHandler
    {
        public static float Convert(Currency to, Currency from, float amount)
        {
            return amount * GetConvertRatio(to, from);
        }

        public static float GetConvertRatio(Currency to, Currency from)
        {
            if(to == from)
            {
                return 1f;
            }

            return 1f;
        }
    }
}
