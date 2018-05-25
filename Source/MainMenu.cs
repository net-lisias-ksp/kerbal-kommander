using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class MainMenu : PartModule
    {
        public static bool menuWindow = false;
        public static Texture CrewHireButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/CrewHireButton", false);
        public static Texture AsteroidButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/AsteroidButton", false);
        public static Texture ShipShopButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/ShipShopButton", false);
        public static Texture SlaveTrafficButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/SlaveTrafficButton", false);
        public static Texture TradingButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/TradingButton", false);
        public static Texture ExplorationButton = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/ExplorationButton", false);
        public static Texture Contract = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/Contract", false);
        [KSPEvent(guiActive = true, guiName = "open station menu", guiActiveEditor = false, externalToEVAOnly = false, guiActiveUnfocused = true)]
        public void ActivateEvent()
        {
            menuWindow = true;
        }
        void OnGUI()
        {
            if (menuWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), mainMenuWindow, "Station Menu", HighLogic.Skin.window);
            }
        }
        void mainMenuWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(TradingButton, HighLogic.Skin.button))
            {
                trading.checkPrice(vessel.orbit.referenceBody.name, vessel.orbit.semiMajorAxis);
                menuWindow = false;
                trading.DrawGUIWindow = true;
            }
            if (GUILayout.Button(CrewHireButton, HighLogic.Skin.button))
            {
                menuWindow = false;
                crewHire.GenerateCrew();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(AsteroidButton, HighLogic.Skin.button))
            {
                menuWindow = false;
                asteroid.asteroidGUI = true;
            }
            
            if (GUILayout.Button(ShipShopButton, HighLogic.Skin.button))
            {
                menuWindow = false;
                shipShop.MenuWindow = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(ExplorationButton, HighLogic.Skin.button))
            {
                menuWindow = false;
                exploration.infoWindow = true;
            }
            if (GUILayout.Button(Contract, HighLogic.Skin.button))
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.BACKUP);
                HighLogic.LoadScene(GameScenes.SPACECENTER);
                menuWindow = false;
            }
            if (part.partInfo.name == "pirateStationScreen")
            {
                if (GUILayout.Button(SlaveTrafficButton, HighLogic.Skin.button))
                {
                    slavesTraffic.CheckPrice(vessel.mainBody.name);
                    menuWindow = false;
                    slavesTraffic.slaveWindow = true;
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("close", HighLogic.Skin.button)) { menuWindow = false; }
            GUILayout.EndVertical();
        }
    }
}
