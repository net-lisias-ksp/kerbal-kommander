using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander.Tools
{
    public class VesselSpawner
    {

        public static Vessel SpawnVessel(string name ,string craftURL, string flagURL, VesselType vesselType, CelestialBody body, Orbit orbit, List<ProtoCrewMember> crewList)
        {
            string gameDataDir = KSPUtil.ApplicationRootPath;
            ConfigNode[] partNodes;
            ShipConstruct shipConstruct = null;
            float lcHeight = 0;
            ConfigNode craftNode;
            ConfigNode currentShip = ShipConstruction.ShipConfig;
            shipConstruct = ShipConstruction.LoadShip(craftURL);
            craftNode = ConfigNode.Load(craftURL);
            lcHeight = ConfigNode.ParseVector3(craftNode.GetNode("PART").GetValue("pos")).y;
            ShipConstruction.ShipConfig = currentShip;

            foreach (ProtoCrewMember crew in crewList)
            {
                Part part = shipConstruct.parts.Find(p => p.protoModuleCrew.Count < p.CrewCapacity);
                if (part != null)
                {
                    part.AddCrewmemberAt(crew, part.protoModuleCrew.Count);
                }
            }

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
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(name, VesselType.Ship, orbit, 0, partNodes, additionalNodes);
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            foreach (var p in UnityEngine.Object.FindObjectsOfType<Part>())
            {
                if (!p.vessel)
                {
                    UnityEngine.Object.Destroy(p.gameObject);
                }
            }
            return protoVessel.vesselRef;
        }
    }
}
