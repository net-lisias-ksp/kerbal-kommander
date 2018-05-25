using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;

namespace Kerbal_Kommander.missions
{
    class rescueContracts : Contract
    {
        CelestialBody targetBody = null;
        public double reward = 0;
        public string contractName = "";
        public string vesselName = "";
        public string vesselID = "";
        public string synopsis = "";

        protected override bool Generate()
        {
            CelestialBody body = FlightGlobals.Bodies[UnityEngine.Random.Range(1, FlightGlobals.Bodies.Count)];
            Orbit orbit = new Orbit(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 0.3f), UnityEngine.Random.Range(Convert.ToSingle(body.Radius + body.atmosphereDepth), Convert.ToSingle(body.sphereOfInfluence)), UnityEngine.Random.Range(0, 360), 0000000000000000, 0, HighLogic.CurrentGame.UniversalTime, body);
            List<ProtoCrewMember> crewList = new List<ProtoCrewMember>();
            for (int i=0; i<UnityEngine.Random.Range(1, 3);)
            {
                crewList.Add(HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew));
                i++;
            }
            vesselName = crewList.ElementAt(0).name + "'s craft";


            vesselID = Tools.VesselSpawner.SpawnVessel(vesselName, "GameData\\KerbalKommander\\Assets\\ships\\Debris.craft", HighLogic.CurrentGame.flagURL, VesselType.Ship, Planetarium.fetch.Home, orbit, crewList ).id.ToString();
            synopsis = "Because of a catastrophic failure, the " + vesselName + " is tottaly unfunctional. You have to go to the broken ship, take the crews in your ship and bring them to the nearest station.";
            base.AddParameter(new rescueParameter(crewList.Count, crewList));
            contractName = "rescue " + crewList.Count + " crew around " + orbit.referenceBody.name + " in " + vesselName;
            reward = crewList.Count * orbit.semiMajorAxis;
            targetBody = Planetarium.fetch.Home;
            base.SetExpiry(25, 100);
            base.SetScience(0f, targetBody);
            base.SetDeadlineYears(10f, targetBody);
            base.SetReputation(0f, 0f, targetBody);
            base.SetFunds(0f, Convert.ToSingle(Math.Round(reward))/1000, 0f, targetBody);
            return true;
        }

        public override bool CanBeCancelled()
        {
            return true;
        }
        public override bool CanBeDeclined()
        {
            return true;
        }

        protected override string GetHashString()
        {
            return contractName;
        }
        protected override string GetTitle()
        {
            return contractName;
        }
        protected override string GetDescription()
        {
            return TextGen.GenerateBackStories(Agent.Name, Agent.GetMindsetString(), "", "", "", new System.Random().Next());
        }
        protected override string GetSynopsys()
        {
            return synopsis;
        }
        protected override string MessageCompleted()
        {
            return "Rescue complete !";
        }

        protected override void OnLoad(ConfigNode node)
        {
            contractName = node.GetValue("contractName");
            vesselID = node.GetValue("vesselID");
            synopsis = node.GetValue("synopsis");
        }
        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractName", contractName);
            node.AddValue("vesselID", vesselID);
            node.AddValue("synopsis", synopsis);
        }
        public override bool MeetRequirements()
        {
            if (HighLogic.CurrentGame.Parameters.SpaceCenter.CanLaunchAtPad == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnCancelled()
        {
            DespawnVesselDebris();
        }
        protected override void OnDeadlineExpired()
        {
            DespawnVesselDebris();
        }
        protected override void OnDeclined()
        {
            DespawnVesselDebris();
        }
        protected override void OnFinished()
        {
            DespawnVesselDebris();
        }
        protected override void OnOfferExpired()
        {
            DespawnVesselDebris();
        }
        public void DespawnVesselDebris()
        {
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.id.ToString() == vesselID)
                {
                    foreach(Part part in vessel.parts)
                    {
                        foreach (ProtoCrewMember crew in part.protoModuleCrew)
                        {
                            crew.KerbalRef.die();
                        }
                    }
                    vessel.Die();
                    break;
                }
            }
        }
    }

    public class rescueParameter : ContractParameter
    {

        public float reward = 0;
        public string kerbalsNumber = "0";
        private bool updated = false;
        public List<ProtoCrewMember> touristList = new List<ProtoCrewMember>();

        public rescueParameter(int kerbalsNumber, List<ProtoCrewMember> touristList)
        {
            this.kerbalsNumber = kerbalsNumber.ToString();
            this.touristList = touristList;

        }
        public rescueParameter()
        {
        }


        protected override string GetHashString()
        {
            return "Bring the lost kerbal into the nearest station";
        }
        protected override string GetTitle()
        {
            return "Bring the lost kerbal into the nearest station";
        }

        protected override void OnRegister()
        {
            this.disableOnStateChange = false;
            updated = false;
            if (Root.ContractState == Contract.State.Active)
            {
                GameEvents.onPartCouple.Add(onPartCouple);
                updated = true;
            }
            else { }
        }
        protected override void OnUnregister()
        {
            if (updated)
            {
                GameEvents.onPartCouple.Remove(onPartCouple);
            }
            else { }

        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("kerbalsNumber", kerbalsNumber);
            int i = 0;
            foreach (ProtoCrewMember c in touristList)
            {
                node.AddValue(i.ToString(), c.name);
                i++;
            }
        }
        protected override void OnLoad(ConfigNode node)
        {
            touristList = new List<ProtoCrewMember>();

            kerbalsNumber = node.GetValue("kerbalsNumber");
            for (int i = 0; i <= Convert.ToInt32(kerbalsNumber) - 1;)
            {
                string crewName = node.GetValue(i.ToString());
                foreach (ProtoCrewMember tourist in HighLogic.CurrentGame.CrewRoster.Crew)
                {
                    if (tourist.name == crewName)
                    {
                        touristList.Add(tourist);
                        break;
                    }
                }
                i++;
            }
        }

        private void onPartCouple(GameEvents.FromToAction<Part, Part> action)
        {
            
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {
                foreach (Part part in action.from.vessel.parts)
                {
                    if (part.Modules.Contains("MainMenu"))
                    {
                        testKerbals(action.from.vessel);
                        testKerbals(action.to.vessel);
                    }
                }
                foreach (Part part in action.to.vessel.parts)
                {
                    if (part.Modules.Contains("MainMenu"))
                    {
                        testKerbals(action.from.vessel);
                        testKerbals(action.to.vessel);
                    }
                }


            }
        }

        void testKerbals(Vessel ves)
        {
            foreach (Part p in ves.parts)
            {
                foreach(ProtoCrewMember tourist in p.protoModuleCrew)
                {
                    if (touristList.Contains(tourist))
                    {
                        p.protoModuleCrew.Remove(tourist);
                        tourist.KerbalRef.die();
                        tourist.seat.part.DespawnIVA();
                        if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                        CameraManager.Instance.SetCameraMap();
                        CameraManager.Instance.SetCameraFlight();
                        tourist.seat.part.SpawnIVA();
                        if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                        CameraManager.Instance.SetCameraMap();
                        CameraManager.Instance.SetCameraFlight();
                        touristList.Remove(tourist);
                    }
                }
                if (touristList.Count == 0)
                {
                    base.SetComplete();
                }
            }
        }

        public void flightReady()
        {
            base.SetIncomplete();
        }
        public void vesselChange(Vessel v)
        {
            base.SetIncomplete();
        }
    }
}
