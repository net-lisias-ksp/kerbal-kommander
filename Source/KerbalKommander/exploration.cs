using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class exploration : PartModule
    {
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

        public static bool infoWindow = false;

        void OnGUI()
        {
            if (infoWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), InfoWindow, "Exploration informations", HighLogic.Skin.window);
            }
        }

        void InfoWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("type:                                           ", HighLogic.Skin.label);
            GUILayout.Label("price:", HighLogic.Skin.label);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("abandoned ship:                         ", HighLogic.Skin.label);
            GUILayout.Label("1000    ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (Funding.Instance.Funds >= 1000)
                {
                    List<string> ships = new List<string>
                    {
                        "GameData\\KerbalKommander\\Assets\\ships\\Broken-Kortana the asrtroid huntress.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-luna slave transport.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-DR1-LL3-R.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Kirk MKI.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Raptor S1.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-S-7 Scientific Vessel.craft",
                        "GameData\\KerbalKommander\\Assets\\ships\\broken-Slave-7.craft"
                    };
                    SpawnVesselFromCraftFile(ships.ElementAt(UnityEngine.Random.Range(0, ships.Count)), VesselType.Ship);
                    Funding.Instance.AddFunds(-1000, TransactionReasons.Cheating);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("not enough funds !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("abandoned Science Station:        ", HighLogic.Skin.label);
            GUILayout.Label("5000    ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (Funding.Instance.Funds >= 5000)
                {
                    Funding.Instance.AddFunds(-5000, TransactionReasons.Cheating);
                    SpawnVesselFromCraftFile("GameData\\KerbalKommander\\Assets\\ships\\Science Station.craft", VesselType.Ship);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("not enough funds !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Asteroid:                                    ", HighLogic.Skin.label);
            GUILayout.Label("1000    ", HighLogic.Skin.label);
            if (GUILayout.Button("Get Info", HighLogic.Skin.button))
            {
                if (Funding.Instance.Funds >= 1000)
                {
                    Funding.Instance.AddFunds(-1000, TransactionReasons.Cheating);

                    foreach (Vessel asteroid in FlightGlobals.Vessels)
                    {
                        if (asteroid.vesselType == VesselType.SpaceObject || asteroid.vesselType == VesselType.Unknown)
                        {
                            asteroid.Load();
                            SpawnAsteroid(asteroid);
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
                infoWindow = false;
                MainMenu.menuWindow = true;
            }
            GUILayout.EndVertical();
        }

        public static void SpawnVesselFromCraftFile(string craftURL, VesselType vesseltype)
        {
            VesselData newData = new VesselData();

            newData.craftURL = craftURL;
            newData.body = FlightGlobals.currentMainBody;
            newData.orbiting = true;
            newData.flagURL = HighLogic.CurrentGame.flagURL;
            newData.owned = true;
            newData.vesselType = vesseltype;
            SpawnVessel(newData);
        }

        static void SpawnVessel(VesselData vesselData)
        {
            string gameDataDir = KSPUtil.ApplicationRootPath;
            if (vesselData.vesselType == VesselType.Ship)
            {
                vesselData.name = "abandoned ship " + UnityEngine.Random.Range(1000, 10000);
            }
            else
            {
                vesselData.name = "abandoned science station " + UnityEngine.Random.Range(1000, 10000);
            }
            vesselData.orbit = FlightGlobals.ActiveVessel.GetOrbit();
            vesselData.orbit.inclination = UnityEngine.Random.Range(0, 360);
            vesselData.orbit.eccentricity = UnityEngine.Random.Range(0, 0.3f);
            vesselData.orbit.semiMajorAxis = UnityEngine.Random.Range(Convert.ToSingle(FlightGlobals.ActiveVessel.mainBody.Radius + FlightGlobals.ActiveVessel.mainBody.atmosphereDepth + 50000), Convert.ToSingle(FlightGlobals.ActiveVessel.mainBody.sphereOfInfluence));
            vesselData.orbit.LAN = UnityEngine.Random.Range(0, 360);
            vesselData.orbit.argumentOfPeriapsis = UnityEngine.Random.Range(0, 360);
            vesselData.orbit.meanAnomalyAtEpoch = 0;
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
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(vesselData.name, vesselData.vesselType, vesselData.orbit, 0, partNodes, additionalNodes);
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            vesselData.id = protoVessel.vesselRef.id;
            foreach (var p in FindObjectsOfType<Part>())
            {
                if (!p.vessel)
                {
                    Destroy(p.gameObject);
                }
            }
            if (vesselData.vesselType == VesselType.Ship)
            {
                setResourcesAmount(protoVessel.vesselRef);
            }
            if (vesselData.vesselType == VesselType.Station)
            {
                setScience(protoVessel.vesselRef);
            }
        }

        static void setResourcesAmount(Vessel vessel)
        {
            vessel.Load();
            int percentage = UnityEngine.Random.Range(0, 100);
            foreach (Part vPart in vessel.parts)
            {
                if (vPart.Modules.Contains("MultiTank"))
                {
                    foreach (PartResource resource in vPart.Resources)
                    {
                        if (resource.info.name == "Diamond")
                        {
                            resource.amount = resource.maxAmount * (Convert.ToDouble(percentage) / Convert.ToDouble(100));
                            break;
                        }
                    }
                }
                else
                {
                    foreach (PartResource resource in vPart.Resources)
                    {
                        resource.amount = resource.maxAmount * (Convert.ToDouble(percentage) / Convert.ToDouble(100));
                    }
                }
            }
        }

        static void setScience(Vessel vessel)
        {
            vessel.Load();
            foreach (Part vPart in vessel.parts)
            {
                foreach(PartResource resources in vPart.Resources)
                {
                    resources.amount = 0;
                }
                if (vPart.Modules.Contains("ModuleScienceConverter"))
                {
                    vPart.FindModuleImplementing<ModuleScienceConverter>().Lab.storedScience = Math.Abs(UnityEngine.Random.Range(200, 500));
                }
            }
        }

        public static void SpawnAsteroid(Vessel asteroidCopy)
        {
            VesselData vesselData = new VesselData();
            vesselData.body = FlightGlobals.currentMainBody;
            vesselData.orbiting = true;
            vesselData.flagURL = HighLogic.CurrentGame.flagURL;
            vesselData.owned = true;
            vesselData.vesselType = VesselType.SpaceObject;

            string gameDataDir = KSPUtil.ApplicationRootPath;
            vesselData.orbit = FlightGlobals.ActiveVessel.GetOrbit();
            vesselData.orbit.inclination = UnityEngine.Random.Range(0, 360);
            vesselData.orbit.eccentricity = UnityEngine.Random.Range(0, 0.3f);
            vesselData.orbit.semiMajorAxis = UnityEngine.Random.Range(Convert.ToSingle(FlightGlobals.ActiveVessel.mainBody.Radius + FlightGlobals.ActiveVessel.mainBody.atmosphereDepth + 50000), Convert.ToSingle(FlightGlobals.ActiveVessel.mainBody.sphereOfInfluence));
            vesselData.orbit.LAN = UnityEngine.Random.Range(0, 360);
            vesselData.orbit.argumentOfPeriapsis = UnityEngine.Random.Range(0, 360);

            vesselData.orbit.meanAnomalyAtEpoch = 0;
            ConfigNode[] partNodes;
            ConfigNode currentShip = ShipConstruction.ShipConfig;
            ShipConstruction.ShipConfig = currentShip;
            int i = 1;
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (v.vesselType == VesselType.Unknown || v.vesselType == VesselType.SpaceObject)
                {
                    i++;
                }
            }
            vesselData.name = "asteroid #"+ i;
            ConfigNode empty = new ConfigNode();
            ProtoVessel dummyProto = new ProtoVessel(empty, null);
            Vessel dummyVessel = new Vessel();
            dummyVessel.parts = asteroidCopy.Parts;
            dummyProto.vesselRef = dummyVessel;
            foreach (Part p in asteroidCopy.Parts)
            {
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
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(vesselData.name, vesselData.vesselType, vesselData.orbit, 0, partNodes, additionalNodes);
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
    }
}

