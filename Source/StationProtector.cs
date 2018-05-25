using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    class StationProtector : PartModule
    {
        public void Update()
        {
            if (vessel.loaded == true && CheatOptions.IgnoreMaxTemperature == false || CheatOptions.NoCrashDamage == false || CheatOptions.UnbreakableJoints == false)
            {
                enableProtection();
            }
        }

        public void enableProtection()
        {
            CheatOptions.IgnoreMaxTemperature = true;
            CheatOptions.NoCrashDamage = true;
            CheatOptions.UnbreakableJoints = true;
        }
    }
}
