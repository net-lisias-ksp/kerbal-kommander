using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;

namespace Kerbal_Kommander.missions
{
    class TourismContracts : Contract
    {
        CelestialBody targetBody = null;
        public float reward = 0;
        public Vessel station1 = null;
        public Vessel station2 = null;
        public int kerbalsNumber = 0;
        public string contractName = "";
        public string synopsis = "";
        public List<ProtoCrewMember> touristList = new List<ProtoCrewMember>();

        public int getCrewSeatNumber(Vessel vessel)
        {
            int CrewSeatNumber = 0;
            foreach (Part p in vessel.parts)
            {
                CrewSeatNumber = CrewSeatNumber + (p.CrewCapacity - p.protoModuleCrew.Count);
            }
            return 4;
        }
        public void setReward(Vessel v1, Vessel v2, int kerNum)
        {
            reward = kerbalsNumber * 5000 * Math.Abs(setBodyValue(v1)-setBodyValue(v2));
        }
        public float setBodyValue(Vessel v)
        {
            int baseValue = v.orbit.referenceBody.flightGlobalsIndex;
            float returnValue = 0;
            if (baseValue == 1) { returnValue = 5; goto end;}
            if (baseValue == 2) { returnValue = 5.5f; goto end; }
            if (baseValue == 3) { returnValue = 6.5f; goto end; }
            if (baseValue == 4) { returnValue = 1; goto end; }
            if (baseValue == 5) { returnValue = 3; goto end; }
            if (baseValue == 6) { returnValue = 7; goto end; }
            if (baseValue == 7) { returnValue = 8; goto end; }
            if (baseValue == 8) { returnValue = 12; goto end; }
            if (baseValue == 9) { returnValue = 12.5f; goto end; }
            if (baseValue == 10) { returnValue = 13; goto end; }
            if (baseValue == 11) { returnValue = 13.5f; goto end; }
            if (baseValue == 12) { returnValue = 14; goto end; }
            if (baseValue == 13) { returnValue = 4; goto end; }
            if (baseValue == 14) { returnValue = 13.25f; goto end; }
            if (baseValue == 15) { returnValue = 12; goto end; }
            if (baseValue == 0 && v.orbit.semiMajorAxis < 1000000000) { returnValue = -1; goto end; }
            if (baseValue == 0 && v.orbit.semiMajorAxis >= 1000000000) { returnValue = 18; goto end; }
            end:
            return returnValue;
        }
        protected override bool Generate()
        {
            List<ProtoCrewMember> touristList = new List<ProtoCrewMember>();
            station1 = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            while (station1.vesselType != VesselType.Station)
            {
                station1 = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            }
            station2 = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            while (station2.vesselType != VesselType.Station || station2 == station1)
            {
                station2 = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            }
            kerbalsNumber = UnityEngine.Random.Range(1, getCrewSeatNumber(station1));
            touristList.Clear();
            int i;
            for (i = 1; i <= kerbalsNumber;)
            {
                i++;
                ProtoCrewMember Tourist = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Tourist);
                Tourist.courage = 1;
                Tourist.stupidity = 1;
                Tourist.isBadass = false;
                touristList.Add(Tourist);
            }
            contractName = "Ferry " + kerbalsNumber + " tourist from " + station1.name + " to " + station2.name;
            synopsis = "This " + kerbalsNumber + " kerbals want to go from " + station1.vesselName + " to " + station2.vesselName + ".\nIf your ferry them to their destination, you'll get a good payoff.";
            setReward(station1, station2, kerbalsNumber);
            base.AddParameter(new TourismParameter(contractName, station1, station2, kerbalsNumber, touristList));

            targetBody = Planetarium.fetch.Home;
            base.SetExpiry(25, 100);
            base.SetScience(0f, targetBody);
            base.SetDeadlineYears(20f, targetBody);
            base.SetReputation(0f, 0f, targetBody);
            base.SetFunds(0f, reward, 0f, targetBody);
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
            return "Kerbals arrived at destination !";
        }

        protected override void OnLoad(ConfigNode node)
        {
            contractName = node.GetValue("contractName");
            synopsis = node.GetValue("synopsis");
            for (int i = 0; i <= Convert.ToInt32(kerbalsNumber) - 1;)
            {
                string crewName = node.GetValue(i.ToString());
                foreach (ProtoCrewMember tourist in HighLogic.CurrentGame.CrewRoster.Tourist)
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
        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractName", contractName);
            node.AddValue("synopsis", synopsis);
            int i = 0;
            foreach (ProtoCrewMember c in touristList)
            {
                node.AddValue(i.ToString(), c.name);
                i++;
            }
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
            KillKerbals();
        }
        protected override void OnDeadlineExpired()
        {
            
            KillKerbals();
        }
        void KillKerbals()
        {
            foreach (ProtoCrewMember tourist in touristList)
            {
                tourist.Die();
            }
        }
    }

    public class TourismParameter : ContractParameter
    {

        public float reward = 0;
        public Vessel station1 = null;
        public Vessel station2 = null;
        public string kerbalsNumber = "0";
        public string contractName = "";
        private bool updated = false;
        private string station1Name = "";
        private string station2Name = "";

        public List<ProtoCrewMember> touristList = new List<ProtoCrewMember>();

        public TourismParameter(string name, Vessel station1, Vessel station2, int kerbalsNumber, List<ProtoCrewMember> touristList)
        {
            this.contractName = name;
            this.station1 = station1;
            station1Name = station1.vesselName;
            this.station2 = station2;
            station2Name = station2.vesselName;
            this.kerbalsNumber = kerbalsNumber.ToString();
            this.touristList = touristList;
        }
        public TourismParameter()
        {
        }


        protected override string GetHashString()
        {
            return contractName;
        }
        protected override string GetTitle()
        {
            return contractName;
        }

        protected override void OnRegister()
        {
            this.disableOnStateChange = false;
            updated = false;
            if (Root.ContractState == Contract.State.Active)
            {
                GameEvents.onPartCouple.Add(onPartCouple);
                GameEvents.onVesselChange.Add(onVesselIsLoaded);
                updated = true;
            }
            else { }
        }
        protected override void OnUnregister()
        {
            if (updated)
            {
                GameEvents.onPartCouple.Remove(onPartCouple);
                GameEvents.onVesselChange.Remove(onVesselIsLoaded);
            }
            else { }

        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractName", contractName);
            node.AddValue("station1", station1Name);
            node.AddValue("station2", station2Name);
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
            contractName = node.GetValue("contractName");

            kerbalsNumber = node.GetValue("kerbalsNumber");
            station1Name = node.GetValue("station1");
            station2Name = node.GetValue("station2");
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.vesselName == station1Name)
                    {
                        station1 = v;
                    }
                    if (v.vesselName == station2Name)
                    {
                        station2 = v;
                    }
                }
            }
            for (int i = 0; i <= Convert.ToInt32(kerbalsNumber) - 1;)
            {
                string crewName = node.GetValue(i.ToString());
                foreach (ProtoCrewMember tourist in HighLogic.CurrentGame.CrewRoster.Tourist)
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

        private void onVesselIsLoaded(Vessel vessel)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (touristList.ElementAt(1).rosterStatus != ProtoCrewMember.RosterStatus.Assigned && station1Name == FlightGlobals.ActiveVessel.vesselName)
                {
                    foreach (ProtoCrewMember tourist in touristList)
                    {
                        foreach (Part p in FlightGlobals.ActiveVessel.parts)
                        {
                            if (p.CrewCapacity > 0 && tourist.rosterStatus != ProtoCrewMember.RosterStatus.Assigned)
                            {
                                p.AddCrewmemberAt(tourist, p.protoModuleCrew.Count);
                                tourist.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
                                p.SpawnIVA();
                                if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                                CameraManager.Instance.SetCameraMap();
                                CameraManager.Instance.SetCameraFlight();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void onPartCouple(GameEvents.FromToAction<Part, Part> action)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {

                if (action.to.vessel.id == station2.id)
                {
                    foreach (ProtoCrewMember tourist in touristList)
                    {
                        if (!checkTourist(tourist, action.from.vessel, action.to.vessel))
                        {
                            base.SetIncomplete();
                            goto end;
                        }
                    }
                    base.SetComplete();
                    foreach (ProtoCrewMember tourist in touristList)
                    {
                        tourist.Die();
                        tourist.seat.part.DespawnIVA();
                        if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                        CameraManager.Instance.SetCameraMap();
                        CameraManager.Instance.SetCameraFlight();
                        tourist.seat.part.SpawnIVA();
                        if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                        CameraManager.Instance.SetCameraMap();
                        CameraManager.Instance.SetCameraFlight();
                    }
                }
            }
            else { base.SetIncomplete(); }
            end:;
        }

        public void flightReady()
        {
            base.SetIncomplete();
        }
        public void vesselChange(Vessel v)
        {
            base.SetIncomplete();
        }

        bool checkTourist(ProtoCrewMember touristToCheck, Vessel v1, Vessel v2)
        {
            foreach (Part p in v1.parts)
            {
                foreach (ProtoCrewMember crew in p.protoModuleCrew)
                {
                    if (crew.name == touristToCheck.name)
                    {
                        return true;
                    }
                }

            }
            foreach (Part p in v2.parts)
            {
                foreach (ProtoCrewMember crew in p.protoModuleCrew)
                {
                    if (crew.name == touristToCheck.name)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }
}
