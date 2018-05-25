using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    class objectsResearch : PartModule
    {
        public bool researchWindow = false;

        [KSPEvent(guiActive = true, guiName = "Research objects", guiActiveEditor = false, externalToEVAOnly = false, guiActiveUnfocused = true)]
        public void ActivateEvent()
        {
            researchWindow = true;
        }
        void OnGUI()
        {
            if (researchWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), window, "Research objects", HighLogic.Skin.window);
            }
        }
        void window(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Type:                                      ", HighLogic.Skin.label);
            GUILayout.Label("Price (science):", HighLogic.Skin.label);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Abandoned ship:                         ", HighLogic.Skin.label);
            GUILayout.Label("200     ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience >= 200)
                {
                    List<string> ships = new List<string>
                    {
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-DR1-LL3-R.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Kirk MKI.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Raptor S1.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-S-7 Scientific Vessel.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Slave-7.craft"
                    };
                    exploration.SpawnVesselFromCraftFile(ships.ElementAt(UnityEngine.Random.Range(0, ships.Count)), VesselType.Ship);
                    part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience = part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience - 200;
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Not enough science !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Abandoned Science Station:        ", HighLogic.Skin.label);
            GUILayout.Label("400     ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience >= 400)
                {
                    part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience = part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience - 400;
                    exploration.SpawnVesselFromCraftFile("GameData\\KerbalKommander\\Assets\\ships\\Science Station.craft", VesselType.Station);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Not enough science !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asteroid:                                    ", HighLogic.Skin.label);
            GUILayout.Label("200     ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience >= 200)
                {
                    part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience = part.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience - 200;

                    foreach (Vessel asteroid in FlightGlobals.Vessels)
                    {
                        if (asteroid.vesselType == VesselType.SpaceObject || asteroid.vesselType == VesselType.Unknown)
                        {
                            asteroid.Load();
                            exploration.SpawnAsteroid(asteroid);
                            break;
                        }
                    }
                }
                else
                {
                    ScreenMessages.PostScreenMessage("not enough funds !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("close", HighLogic.Skin.button))
            {
                researchWindow = false;
            }
            GUILayout.EndVertical();
        }
    }
}
