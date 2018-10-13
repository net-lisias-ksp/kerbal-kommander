using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class scienceRecover :MonoBehaviour
    {
        private float lastFixedUpdate = 0.0f;
        private float logInterval = 50f;
        void Update()
        {
            if (HighLogic.CurrentGame.Parameters.SpaceCenter.CanGoToAstronautC == false)
            {
                Funding.Instance.AddFunds(ResearchAndDevelopment.Instance.Science * 20, TransactionReasons.Any);
                ResearchAndDevelopment.Instance.AddScience(-ResearchAndDevelopment.Instance.Science, TransactionReasons.Any);
                if (Reputation.CurrentRep != 0)
                    Reputation.Instance.AddReputation(-Reputation.CurrentRep, TransactionReasons.Any);
            }
        }
        void FixedUpdate()
        {
            if (HighLogic.CurrentGame.Parameters.SpaceCenter.CanGoToAstronautC == false && HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if ((Time.time - lastFixedUpdate) > logInterval)
                {
                    lastFixedUpdate = Time.time;
                    if (CheatOptions.IgnoreMaxTemperature == true || CheatOptions.NoCrashDamage == true || CheatOptions.UnbreakableJoints == true)
                    {
                        CheatOptions.IgnoreMaxTemperature = false;
                        CheatOptions.NoCrashDamage = false;
                        CheatOptions.UnbreakableJoints = false;
                    }
                }
            }
        }
    }
}
