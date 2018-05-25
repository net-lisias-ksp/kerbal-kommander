using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class asteroid : PartModule
    {

        Vector2 scrollPos = new Vector2();
        public static bool asteroidGUI = false;

        void OnGUI()
        {
            if (asteroidGUI == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 400) / 2, 700, 400), AsteroidGUI, "Asteroid", HighLogic.Skin.window);
            }
        }

        void AsteroidGUI(int windowID)
        {
            double asteroidPrice = 0;
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, HighLogic.Skin.scrollView);

            foreach (Vessel vessels in FlightGlobals.Vessels)
            {
                if (vessels.vesselType == VesselType.SpaceObject && vessels.loaded == true)
                {
                    if (vessels.DiscoveryInfo.objectSize == UntrackedObjectClass.A) { asteroidPrice = 5000; }
                    if (vessels.DiscoveryInfo.objectSize == UntrackedObjectClass.B) { asteroidPrice = 10000; }
                    if (vessels.DiscoveryInfo.objectSize == UntrackedObjectClass.C) { asteroidPrice = 20000; }
                    if (vessels.DiscoveryInfo.objectSize == UntrackedObjectClass.D) { asteroidPrice = 35000; }
                    if (vessels.DiscoveryInfo.objectSize == UntrackedObjectClass.E) { asteroidPrice = 50000; }

                    if (GUILayout.Button("name: " + vessels.vesselName + "\n" + "type: " + vessels.DiscoveryInfo.objectSize + "\n price: " + asteroidPrice, HighLogic.Skin.button))
                    {
                        if (vessels.parts.Count >= 1)
                        {
                            FlightGlobals.ForceSetActiveVessel(part.vessel);
                            Funding.Instance.AddFunds(asteroidPrice, TransactionReasons.VesselRecovery);
                            vessels.Die();
                            ScreenMessages.PostScreenMessage("Asteroid Selled", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        }
                        else
                        {
                            ScreenMessages.PostScreenMessage("There is still a ship on the asteroid", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("close", HighLogic.Skin.button))
            {
                asteroidGUI = false;
                MainMenu.menuWindow = true;
            }
            GUILayout.EndVertical();
        }

    }
}
