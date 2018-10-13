using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class shipShop : PartModule
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

        public LoadCraftDialog craftBrowser = new LoadCraftDialog();
        public bool shipsInfo = false;
        public static bool MenuWindow = false;
        bool sellWindow = false;
        Vessel vesselSell = null;
        double vesselPrice = 0;
        Vector2 scrollPos = new Vector2();
        bool aboutWindow = false;

        void OnGUI()
        {
            if (MenuWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), menuWindow, "Ship shop2", HighLogic.Skin.window);
            }
            if (sellWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), SellWindow, "Sell ship", HighLogic.Skin.window);
            }
            if (aboutWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), AboutWindow, "About", HighLogic.Skin.window);
            }
        }

        void menuWindow(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("buy a new ship", HighLogic.Skin.button))
            {
                craftBrowser = LoadCraftDialog.Create(EditorFacility.VAB, HighLogic.CurrentGame.Title.Split(new string[] { " (" }, StringSplitOptions.None)[0], OnSelected, OnCancelled, false);
                MenuWindow = false;
            }
            if (GUILayout.Button("Sell a ship", HighLogic.Skin.button))
            {
                MenuWindow = false;
                sellWindow = true;
            }
            if (GUILayout.Button("Design a new ship", HighLogic.Skin.button))
            {
                MenuWindow = false;
                sellWindow = false;
                aboutWindow = false;
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.BACKUP);
                EditorDriver.StartEditor(EditorFacility.VAB);
            }
            if (GUILayout.Button("close", HighLogic.Skin.button))
            {
                MenuWindow = false;
                MainMenu.menuWindow = true;
            }

            GUILayout.EndVertical();
        }
        void SellWindow(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, HighLogic.Skin.scrollView);
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.vesselType != VesselType.EVA && vessel.vesselType != VesselType.Flag && vessel.vesselType != VesselType.SpaceObject && vessel != base.vessel && vessel.loaded == true)
                {
                    double price = 0;
                    foreach (Part part in vessel.Parts)
                    {
                        if (part.partInfo.name == "LargeTank") { price = price + 3000; }
                        else
                        {
                            if (part.partInfo.name == "SmallTank") { price = price + 1000; }
                            else
                            {
                                if (part.partInfo.name == "RadialOreTank") { price = price + 300; }
                                else
                                {
                                    price = price + part.partInfo.cost;
                                    foreach (PartResource resource in part.Resources)
                                    {
                                        price = price - resource.maxAmount * resource.info.unitCost;
                                    }
                                }
                            }
                        }

                    }
                    price = Math.Round(price, MidpointRounding.AwayFromZero);


                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(vessel.vesselName + "       price: " + price + "\n" + vessel.situation + " " + vessel.mainBody.bodyName, HighLogic.Skin.button))
                    {
                        if (vessel.GetCrewCount() != 0)
                        {
                            ScreenMessages.PostScreenMessage("there are still some crew in the vessel", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        }
                        else
                        {
                            vesselPrice = price;
                            vesselSell = vessel;
                            ScreenMessages.PostScreenMessage("Vessel selected", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        }
                    }
                    GUILayout.EndHorizontal();


                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sell", HighLogic.Skin.button))
            {
                if (vesselSell != null)
                {
                    Funding.Instance.AddFunds(vesselPrice, TransactionReasons.VesselRecovery);
                    foreach (Part part in vesselSell.parts)
                    {
                        foreach (PartResource resource in part.Resources)
                        {
                            resource.amount = 0;
                        }
                    }
                    vesselSell.Die();
                    ScreenMessages.PostScreenMessage("vessel selled", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("please select a vessel", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            if (GUILayout.Button("close", HighLogic.Skin.button)) { sellWindow = false; MenuWindow = true; }
            if (GUILayout.Button("about", HighLogic.Skin.button)) { aboutWindow = true; }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void AboutWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("To Sell a vessel , it: \n - Have to be near the station you are in \n - must not be a space station or a debris \n - must not contain crew \n WARNING: when you sell a vessel, the resources are not sold !");
            if (GUILayout.Button("close", HighLogic.Skin.button)) { aboutWindow = false; }
            GUILayout.EndVertical();
        }

        void OnSelected(string fullPath, LoadCraftDialog.LoadType Type)
        {
            Vector3 gpsPos = vessel.GetWorldPos3D();
            SpawnVesselFromCraftFile(fullPath);
            craftBrowser = null;
        }

        void OnCancelled()
        {
            craftBrowser = null;
        }


        void SpawnVesselFromCraftFile(string craftURL)
        {
            VesselData newData = new VesselData();

            newData.craftURL = craftURL;
            newData.body = FlightGlobals.currentMainBody;
            newData.orbiting = true;
            newData.flagURL = HighLogic.CurrentGame.flagURL;
            newData.owned = true;
            newData.vesselType = VesselType.Ship;
            SpawnVessel(newData);
        }


        void SpawnVessel(VesselData vesselData)
        {
            string gameDataDir = KSPUtil.ApplicationRootPath;
            vesselData.orbit = vessel.GetOrbit();
            vesselData.orbit.vel = vessel.orbit.vel;
            vesselData.orbit.meanAnomalyAtEpoch = vesselData.orbit.meanAnomalyAtEpoch + Math.Atan(UnityEngine.Random.Range(200, 300) / vesselData.orbit.semiMajorAxis);
            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            float lcHeight = 0;
            ConfigNode craftNode;
            ConfigNode currentShip = ShipConstruction.ShipConfig;
            shipConstruct = ShipConstruction.LoadShip(vesselData.craftURL);
            float dryCost = 0;
            float cost = 0;
            shipConstruct.GetShipCosts(out dryCost, out cost);
            cost = cost + dryCost;
            if (cost > Funding.Instance.Funds)
            {
                foreach (var p in FindObjectsOfType<Part>())
                {
                    if (!p.vessel)
                    {
                        Destroy(p.gameObject);
                    }
                }
                ScreenMessages.PostScreenMessage("not enought funds", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }
            craftNode = ConfigNode.Load(vesselData.craftURL);
            lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
            ShipConstruction.ShipConfig = currentShip;
            vesselData.name = shipConstruct.shipName;

            uint missionID = (uint)Guid.NewGuid().GetHashCode();
            uint launchID = HighLogic.CurrentGame.launchID++;

            foreach (Part p in shipConstruct.parts)
            {
                p.missionID = missionID;
                p.launchID = launchID;
                p.flightID = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
                p.temperature = 1.0;
            }
            ConfigNode empty = new ConfigNode();
            ProtoVessel dummyProto = new ProtoVessel(empty, null);
            Vessel dummyVessel = new Vessel();

            dummyVessel.parts = shipConstruct.parts;
            dummyProto.vesselRef = dummyVessel;

            foreach (Part p in shipConstruct.parts)
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
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(vesselData.name, VesselType.Ship, vesselData.orbit, 0, partNodes, additionalNodes);
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            vesselData.id = protoVessel.vesselRef.id;
            protoVessel.vesselRef.Load();
            Funding.Instance.AddFunds(-cost, TransactionReasons.Vessels);
            ScreenMessages.PostScreenMessage("vessel buy !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
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
