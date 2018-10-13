using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.UI.Screens.Flight;

namespace Kerbal_Kommander
{

    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class saveGenerator : MonoBehaviour
    {
        ApplicationLauncherButton appButton = null;
        public bool GUIvisible = false;
        bool GUIUpdate = false;
        bool validation = false;
        bool validation2 = false;
        string crewName = "Jebediah Kerman";
        string traitButton = "Pilot";
        ProtoCrewMember.Gender genderButton = ProtoCrewMember.Gender.Male;

        void Start()
        {
            DontDestroyOnLoad(this);
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(onGUIApplicationLauncherUnreadifying);
        }
        void OnGUIAppLauncherReady()
        {
            if (!appButton)
            {
                appButton = ApplicationLauncher.Instance.AddModApplication(openGUI, closeGUI, null, null, null, null, ApplicationLauncher.AppScenes.SPACECENTER, GameDatabase.Instance.GetTexture("KerbalKommander/Assets/mainMenu/icone", false));
            }
        }

        void OnGUIAppLauncherDestroyed()
        {
            if (appButton)
            {
                ApplicationLauncher.Instance.RemoveModApplication(appButton);
                appButton = null;
            }
        }

        void onGUIApplicationLauncherUnreadifying(GameScenes scenes)
        {
            if (appButton)
            {
                OnGUIAppLauncherDestroyed();
            }
            closeGUI();
        }

        void openGUI()
        {

            GUIvisible = true;
        }

        void closeGUI()
        {
            GUIvisible = false;
            validation = false;
        }

        public void OnGUI()
        {
            if (GUIvisible)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), generatorWindow, "Station Menu", HighLogic.Skin.window);
            }
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (FlightGlobals.ActiveVessel.vesselName == "Kerbin Station" && HighLogic.CurrentGame.Parameters.SpaceCenter.CanGoToMissionControl == false)
                {
                    GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), crewWindow, "Creat your KERBAL", HighLogic.Skin.window);
                }
            }
            if (GUIUpdate)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 100) / 2, (Screen.height - 60) / 2, 150, 60), UpdateWindow, "", HighLogic.Skin.window);
            }
        }

        void UpdateWindow(int windowID)
        {
            if (GUILayout.Button("Done !", HighLogic.Skin.button))
            {
                for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; --i)
                {
                    Vessel station = FlightGlobals.Vessels[i];
                    if (station.vesselType == VesselType.Station)
                    {
                        setResourcesAmount(station);
                    }
                }
                GUIUpdate = false;
            }
        }

        void crewWindow(int windowID)
        {
            GUILayout.Label("Create a crew:    ", HighLogic.Skin.label);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:   ", HighLogic.Skin.label);
            crewName = GUILayout.TextField(crewName, HighLogic.Skin.textField);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gender: ", HighLogic.Skin.label);
            if (GUILayout.Button(genderButton.ToString(), HighLogic.Skin.button))
            {
                if (genderButton == ProtoCrewMember.Gender.Male)
                {
                    genderButton = ProtoCrewMember.Gender.Female;
                }
                else
                {
                    genderButton = ProtoCrewMember.Gender.Male;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("trait:      ", HighLogic.Skin.label);
            if (GUILayout.Button(traitButton, HighLogic.Skin.button))
            {
                if (traitButton == "Pilot")
                {
                    traitButton = "Engineer";
                }
                else
                {
                    if (traitButton == "Scientist")
                    {
                        traitButton = "Pilot";
                    }
                    else
                    {
                        traitButton = "Scientist";
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("create your kerbal", HighLogic.Skin.button))
            {
                for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; --i)
                {
                    Vessel station = FlightGlobals.Vessels[i];
                    if (station.vesselType == VesselType.Station)
                    {
                        setResourcesAmount(station);
                    }
                }
                generateCrew(traitButton, genderButton);
                HighLogic.CurrentGame.Parameters.SpaceCenter.CanGoToMissionControl = true;
            }
        }






        void generatorWindow(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Begin A Kerbal Kommander Game", HighLogic.Skin.button))
            {
                validation = true;
            }
            if (validation)
            {
                GUILayout.Label("This Will destroy all the ships and reset the funds !", HighLogic.Skin.label);
                GUILayout.Label("Are you sure you want to do this ?", HighLogic.Skin.label);

                if (GUILayout.Button("yes", HighLogic.Skin.button))
                {
                    if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                    {
                        ScreenMessages.PostScreenMessage("Save Generation...", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        generateSave(false);
                    }
                    else
                    {
                        ScreenMessages.PostScreenMessage("You have to be in carrer mode !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    }
                }
                if (GUILayout.Button("Cancel", HighLogic.Skin.button))
                {
                    validation = false;
                }
            }
            GUILayout.Label("", HighLogic.Skin.label);
            if (GUILayout.Button("Update The Game", HighLogic.Skin.button))
            {
                validation2 = true;
            }
            if (validation2)
            {
                GUILayout.Label("This Will destroy all the ships which are docked with stations and kill all the kerbals which are in the stations.", HighLogic.Skin.label);
                GUILayout.Label("Are you sure you want to do this ?", HighLogic.Skin.label);

                if (GUILayout.Button("yes", HighLogic.Skin.button))
                {
                    ScreenMessages.PostScreenMessage("Save Generation...", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    generateSave(true);
                }
                if (GUILayout.Button("Cancel", HighLogic.Skin.button))
                {
                    validation2 = false;
                }
            }
            if (GUILayout.Button("Update The Game to 0.3", HighLogic.Skin.button))
            {
                HighLogic.CurrentGame.Parameters.SpaceCenter.CanGoToMissionControl = true;
                SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Kerbol observation station.craft", FlightGlobals.fetch.bodies.ElementAt(0), 600000000, "Kerbol observation station");
                SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Deep space research station 1.craft", FlightGlobals.fetch.bodies.ElementAt(0), 150000000000, "Terminus");
                HighLogic.CurrentGame.Parameters.Difficulty.EnableCommNet = true;
                ScreenMessages.PostScreenMessage("DONE !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            GUILayout.EndVertical();
        }

        void generateSave(bool isUpdate)
        {
            #region PARAMETERS
            GameParameters Params = new GameParameters();
            //FLIGHT
            Params.Flight.CanQuickSave = true;
            Params.Flight.CanQuickLoad = true;
            Params.Flight.CanAutoSave = true;
            Params.Flight.CanUseMap = true;
            Params.Flight.CanSwitchVesselsNear = true;
            Params.Flight.CanSwitchVesselsFar = false;
            Params.Flight.CanTimeWarpHigh = true;
            Params.Flight.CanTimeWarpLow = true;
            Params.Flight.CanEVA = true;
            Params.Flight.CanIVA = true;
            Params.Flight.CanBoard = true;
            Params.Flight.CanRestart = false;
            Params.Flight.CanLeaveToEditor = false;
            Params.Flight.CanLeaveToTrackingStation = false;
            Params.Flight.CanLeaveToSpaceCenter = true;
            Params.Flight.CanLeaveToMainMenu = false;

            //EDITOR
            Params.Editor.CanSave = true;
            Params.Editor.CanLoad = true;
            Params.Editor.CanStartNew = true;
            Params.Editor.CanLaunch = false;
            Params.Editor.CanLeaveToSpaceCenter = true;
            Params.Editor.CanLeaveToMainMenu = false;

            //TRACKINGSTATION
            Params.TrackingStation.CanFlyVessel = true;
            Params.TrackingStation.CanAbortVessel = false;
            Params.TrackingStation.CanLeaveToSpaceCenter = true;
            Params.TrackingStation.CanLeaveToMainMenu = false;

            //SPACECENTER
            Params.SpaceCenter.CanGoInVAB = true;
            Params.SpaceCenter.CanGoInSPH = true;
            Params.SpaceCenter.CanGoInTrackingStation = true;
            Params.SpaceCenter.CanLaunchAtPad = false;
            Params.SpaceCenter.CanLaunchAtRunway = false;
            Params.SpaceCenter.CanGoToAdmin = false;
            Params.SpaceCenter.CanGoToAstronautC = false;
            Params.SpaceCenter.CanGoToMissionControl = false;
            Params.SpaceCenter.CanGoToRnD = false;
            Params.SpaceCenter.CanSelectFlag = true;
            Params.SpaceCenter.CanLeaveToMainMenu = true;

            //DIFFICULTY
            Params.Difficulty.AutoHireCrews = false;
            Params.Difficulty.MissingCrewsRespawn = true;
            Params.Difficulty.BypassEntryPurchaseAfterResearch = true;
            Params.Difficulty.AllowStockVessels = false;
            Params.Difficulty.IndestructibleFacilities = true;
            Params.Difficulty.ResourceAbundance = 1;
            Params.Difficulty.ReentryHeatScale = 1;
            Params.Difficulty.EnableCommNet = true;

            //CAREER
            Params.Career.StartingFunds = 20000;
            Params.Career.StartingScience = 0;
            Params.Career.StartingReputation = 0;
            Params.Career.FundsGainMultiplier = 1;
            Params.Career.RepGainMultiplier = 1;
            Params.Career.ScienceGainMultiplier = 1;
            Params.Career.RepLossMultiplier = 1;
            Params.Career.RepLossDeclined = 1;
            Params.Career.FundsLossMultiplier = 1;

            HighLogic.CurrentGame.Parameters = Params;

            ScenarioUpgradeableFacilities.Instance.CheatFacilities();


            if (!isUpdate)
            {
                Funding.Instance.AddFunds(20000 - Funding.Instance.Funds, TransactionReasons.Cheating);
                ResearchAndDevelopment.Instance.AddScience(-ResearchAndDevelopment.Instance.Science, TransactionReasons.Cheating);
                Reputation.Instance.AddReputation(-Reputation.Instance.reputation, TransactionReasons.Cheating);
                startfor:
                for (int i = HighLogic.CurrentGame.CrewRoster.Crew.ToList().Count - 1; i >= 0;)
                {
                    ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.ToList()[i];
                    HighLogic.CurrentGame.CrewRoster.Remove(crew);
                    goto startfor;
                }
            }
            ResearchAndDevelopment.Instance.CheatTechnology();
            ProgressTracking.Instance.CheatProgression();

            #endregion

            if (!isUpdate)
            {
                FlightGlobals.Vessels.Clear();
            }
            #region spawn stations
            //Kerbin
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\big station.craft", FlightGlobals.fetch.bodies.ElementAt(1), 200000, "Kerbin Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 7.craft", FlightGlobals.fetch.bodies.ElementAt(2), 200000, "Mun Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 2.craft", FlightGlobals.fetch.bodies.ElementAt(3), 100000, "Minmus Station");
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 10.craft", FlightGlobals.fetch.bodies.ElementAt(3), 15000, "Minmus Pirate Station");

            //Duna
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 4.craft", FlightGlobals.fetch.bodies.ElementAt(6), 200000, "Duna Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 3.craft", FlightGlobals.fetch.bodies.ElementAt(7), 200000, "Ike Station");

            //Eve
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 9.craft", FlightGlobals.fetch.bodies.ElementAt(5), 300000, "Eve Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 4.craft", FlightGlobals.fetch.bodies.ElementAt(13), 50000, "Gilly Station");
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 2.craft", FlightGlobals.fetch.bodies.ElementAt(13), 15000, "Gilly Pirate Station");

            //Moho
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 1.craft", FlightGlobals.fetch.bodies.ElementAt(4), 200000, "Moho Pirate Station");

            //Dres
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\big pirate station.craft", FlightGlobals.fetch.bodies.ElementAt(15), 200000, "Dres Pirate Station");

            //Eeloo
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 8_1.craft", FlightGlobals.fetch.bodies.ElementAt(16), 150000, "Eeloo Station");

            //Jool
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Space station 2_0.craft", FlightGlobals.fetch.bodies.ElementAt(8), 55000000, "Jool Station");
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Pirate Station 2_0.craft", FlightGlobals.fetch.bodies.ElementAt(8), 3500000, "Jool Pirate Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 6.craft", FlightGlobals.fetch.bodies.ElementAt(9), 300000, "Laythe Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 3.craft", FlightGlobals.fetch.bodies.ElementAt(10), 300000, "Vall Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 5.craft", FlightGlobals.fetch.bodies.ElementAt(12), 400000, "Tylo Station");
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 1.craft", FlightGlobals.fetch.bodies.ElementAt(12), 1000000, "Tylo Pirate Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Medium Outpost 5.craft", FlightGlobals.fetch.bodies.ElementAt(11), 100000, "Bop Pirate Station");

            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Small Outpost 1.craft", FlightGlobals.fetch.bodies.ElementAt(14), 75000, "Pol Station");

            //sun
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Kerbol observation station.craft", FlightGlobals.fetch.bodies.ElementAt(0), 600000000, "Kerbol observation station");
            SpawnVessel("GameData\\KerbalKommander\\Assets\\stations\\Deep space research station 1.craft", FlightGlobals.fetch.bodies.ElementAt(0), 150000000000, "Terminus");

            #endregion

            GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (isUpdate)
                {
                    if (vessel.vesselType == VesselType.EVA || vessel.vesselType == VesselType.Ship)
                    {
                        FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(vessel));
                        GUIUpdate = true;
                        break;
                    }
                }
                else
                {
                    if (vessel.vesselName == "Kerbin Station")
                    {
                        FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(vessel));
                        break;
                    }
                }
            }
        }

        private class VesselData
        {
            public string name = null;
            public Guid? id = null;
            public string craftURL = null;
            public AvailablePart craftPart = null;
            public string flagURL = null;
            public VesselType vesselType = VesselType.Ship;
            public CelestialBody body = null;
            public Orbit orbit = null;
            public bool orbiting = false;
            public bool owned = false;
            public VesselData() { }
            public VesselData(VesselData vd)
            {
                name = vd.name;
                id = vd.id;
                craftURL = vd.craftURL;
                craftPart = vd.craftPart;
                flagURL = vd.flagURL;
                vesselType = vd.vesselType;
                body = vd.body;
                orbit = vd.orbit;
                orbiting = vd.orbiting;
                owned = vd.owned;
            }
        }



        void SpawnVessel(string craftURL, CelestialBody orbitmainBodyName, double altitude, string stationName)
        {


            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselName == stationName)
                {
                    vessel.Die();
                    break;
                }
            }
            VesselData vesselData = new VesselData();

            vesselData.craftURL = craftURL;
            vesselData.orbiting = true;
            vesselData.flagURL = HighLogic.CurrentGame.flagURL;
            vesselData.owned = true;
            string gameDataDir = KSPUtil.ApplicationRootPath;
            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            float lcHeight = 0;
            ConfigNode craftNode;
            ConfigNode currentShip = ShipConstruction.ShipConfig;
            shipConstruct = ShipConstruction.LoadShip(vesselData.craftURL);
            craftNode = ConfigNode.Load(vesselData.craftURL);
            lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
            ShipConstruction.ShipConfig = currentShip;
            ConfigNode empty = new ConfigNode();
            ProtoVessel dummyProto = new ProtoVessel(empty, null);
            Vessel dummyVessel = new Vessel();
            dummyVessel.parts = shipConstruct.parts;
            dummyProto.vesselRef = dummyVessel;
            uint missionID = (uint)Guid.NewGuid().GetHashCode();
            uint launchID = HighLogic.CurrentGame.launchID++;

            foreach (Part p in shipConstruct.parts)
            {
                p.missionID = missionID;
                p.launchID = launchID;
                p.flightID = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                p.temperature = 1.0;
                dummyProto.protoPartSnapshots.Add(new ProtoPartSnapshot(p, dummyProto));
            }
            foreach (ProtoPartSnapshot p in dummyProto.protoPartSnapshots)
            {
                p.storePartRefs();
            }
            List<ConfigNode> partNodesL = new List<ConfigNode>();
            foreach (var snapShot in dummyProto.protoPartSnapshots)
            {
                ConfigNode node = new ConfigNode("PART");
                snapShot.Save(node);
                partNodesL.Add(node);
            }
            partNodes = partNodesL.ToArray();
            ConfigNode[] additionalNodes = new ConfigNode[0];
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(stationName, VesselType.Station, new Orbit(0, 0, altitude + orbitmainBodyName.Radius, 0, 0, 0, 0, orbitmainBodyName), 0, partNodes, additionalNodes);
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            vesselData.id = protoVessel.vesselRef.id;
            foreach (var p in FindObjectsOfType<Part>())
            {
                if (!p.vessel)
                {
                    Destroy(p.gameObject);
                }
            }
        }
        void setResourcesAmount(Vessel vessel)
        {
            vessel.Load();
            for (int i = vessel.parts.Count - 1; i >= 0; --i)
            {
                Part vPart = vessel.parts[i];
                for (int j = vPart.Resources.Count - 1; j >= 0; --j)
                {
                    PartResource resources = vPart.Resources[j];
                    if (vPart.name != "pirateStationScreen" && vPart.name != "stationScreen")
                    {
                        resources._flowState = false;
                    }
                    resources.amount = 0;
                }
            }
        }

        void generateCrew(string kerbalTrait, ProtoCrewMember.Gender gender)
        {
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
            while (crew.trait != kerbalTrait)
            {
                crew = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);
            }
            crew.ChangeName(crewName);
            crew.type = ProtoCrewMember.KerbalType.Crew;
            crew.gender = gender;
            crew.experienceLevel = 5;
            FlightLog flight = new FlightLog();
            flight.AddEntry("Land,Eeloo");
            flight.AddEntry("Land,Dres");
            flight.AddEntry("Land,Pol");
            flight.AddEntry("Land,Gilly");
            flight.AddEntry("Land,Tylo");
            flight.AddEntry("Land,Bop");
            flight.AddEntry("Land,Vall");
            flight.AddEntry("Land,Laythe");
            crew.careerLog.AddFlight();
            for (int i = flight.Entries.Count - 1; i >= 0; --i)
            {
                FlightLog.Entry entries = flight.Entries[i];
                crew.careerLog.AddEntry(entries);
            }
            for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; --i)
            {
                Vessel vessel = FlightGlobals.Vessels[i];
                if (vessel.vesselName == "Kerbin Station")
                {
                    for (int j = vessel.Parts.Count - 1; j >= 0; --j)
                    {
                        Part part = vessel.Parts[j];
                        if (part.protoModuleCrew.Count< part.CrewCapacity)
                        {
                            part.AddCrewmember(crew);
                            part.Actions.part.SpawnIVA();
                            CameraManager.Instance.SetCameraMap();
                            CameraManager.Instance.SetCameraFlight();
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}