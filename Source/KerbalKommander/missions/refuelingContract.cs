using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;

namespace Kerbal_Kommander.missions
{
    
    class refuelingContract : Contract
    {
        CelestialBody targetBody = null;
        public string name = null;
        public Resource resource = null;
        public Vessel vessel = null;
        public double resourceAmount = 0;
        public float reward = 0;
        public string vesselName;
        public string synopsis = "";

        protected override bool Generate()
        {
            List<Resource> resList = new List<Resource>();
            resList.Add(new Resource("Steel", 2, 80208299));
            resList.Add(new Resource("Wood", 5, 2702029));
            resList.Add(new Resource("Diamond", 20, -975259340));
            resList.Add(new Resource("Liquid Fuel", 0.8, 374119730));
            resList.Add(new Resource("Oxydizer", 0.005, -1823983486));
            resList.Add(new Resource("Mono Propellant", 0.6, 2001413032));
            resList.Add(new Resource("Xenon Gas", 1.2, 1447111193));
            resource = resList.ElementAt(UnityEngine.Random.Range(0, resList.Count));
            resourceAmount = UnityEngine.Random.Range(5, 50) * 100;

            List<string> ships = new List<string>
            {
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\DR1-LL3-R_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\freighter large ( long range)_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\freighter medium ( long range)_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\freighter Medium_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\freighter small_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\Kirk MKI_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\Kortana the asrtroid huntress_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\luna slave transport_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\S-7 Scientific Vessel_refuel.craft",
                "GameData\\KerbalKommander\\Assets\\ships\\refuelContract\\Slave-7_refuel.craft",
           };

            vesselName = CrewGenerator.GetRandomName(ProtoCrewMember.Gender.Male);
            synopsis = vesselName + " ran out of fuel, he need " + resourceAmount + " of " + resource + " in exchange of a reward.\nYou just need to carry the resources to his ship and dock whith it.";
            vesselName = vesselName + "'s ship";
            CelestialBody body = FlightGlobals.Bodies[UnityEngine.Random.Range(1, FlightGlobals.Bodies.Count)];
            Orbit orbit = new Orbit(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 0.3f), UnityEngine.Random.Range(Convert.ToSingle(body.Radius + body.atmosphereDepth), Convert.ToSingle(body.sphereOfInfluence)), UnityEngine.Random.Range(0, 360), 0000000000000000, 0, HighLogic.CurrentGame.UniversalTime, body);
            vessel = Tools.VesselSpawner.SpawnVessel(vesselName, ships.ElementAt(UnityEngine.Random.Range(0, ships.Count)), HighLogic.CurrentGame.flagURL, VesselType.Ship, Planetarium.fetch.Home, orbit, new List<ProtoCrewMember>());


            name = "Refuel " + vessel.vesselName;
            reward = Convert.ToSingle(Math.Round(resourceAmount * resource.unitCost * 300));

            targetBody = Planetarium.fetch.Home;
            this.AddParameter(new refuelingParameter(resource, vessel, resourceAmount, reward));
            base.SetExpiry(25,100);
            base.SetScience(0f, targetBody);
            base.SetDeadlineYears(10f, targetBody);
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
            return name;
        }
        protected override string GetTitle()
        {
            return name;
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
            return "Refueling complete !";
        }

        protected override void OnLoad(ConfigNode node)
        {
            name = node.GetValue("name");
            vesselName = node.GetValue("vesselName");
            synopsis = node.GetValue("synopsis");
        }
        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("name", name);
            node.AddValue("vesselName", vesselName);
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
                if (vessel.vesselName == vesselName)
                {
                    foreach (Part part in vessel.parts)
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
    
    public class refuelingParameter : ContractParameter
    {

        public string ParamTitle = null;
        public Resource resource = null;
        public Vessel vessel = null;
        public string vesselID = "";
        public string resourceAmount = "0";
        public float reward = 0;
        public bool updated = false;

        public refuelingParameter(Resource resource, Vessel vessel, double resourceAmount, float reward)
        {
            this.resource = resource;
            this.vessel = vessel;
            vesselID = this.vessel.id.ToString();
            this.resourceAmount = resourceAmount.ToString();
            this.reward = reward;
            ParamTitle = "Carry " + resourceAmount + " of " + resource.name + " to " + vessel.name;
        }
        public refuelingParameter()
        {
        }


        protected override string GetHashString()
        {
            return "";
        }
        protected override string GetTitle()
        {
            return ParamTitle;
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
            node.AddValue("resource", resource.ID);
            node.AddValue("vessel", vesselID);
            node.AddValue("resourceAmount", resourceAmount);
            node.AddValue("ParamTitle", ParamTitle);
        }
        protected override void OnLoad(ConfigNode node)
        {
            ParamTitle = node.GetValue("ParamTitle");
            List<Resource> resList = new List<Resource>();
            resList.Add(new Resource("Steel", 2, 80208299));
            resList.Add(new Resource("Wood", 5, 2702029));
            resList.Add(new Resource("Diamond", 20, -975259340));
            resList.Add(new Resource("Liquid Fuel", 0.8, 374119730));
            resList.Add(new Resource("Oxydizer", 0.005, -1823983486));
            resList.Add(new Resource("Mono Propellant", 0.6, 2001413032));
            resList.Add(new Resource("Xenon Gas", 1.2, 1447111193));

            string i = node.GetValue("resource");
            foreach (Resource r in resList)
            {
                if (r.ID.ToString() == i)
                {
                    resource = r;

                    resource.ID = r.ID;
                    resource.name = r.name;
                    resource.unitCost = r.unitCost;
                }
            }
            resource = resList.ElementAt(1);
            vesselID = node.GetValue("vessel");
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.id.ToString() == vesselID)
                    {
                        vessel = v;
                        vessel.id = v.id;
                    }
                }
            }
            resourceAmount = node.GetValue("resourceAmount");

        }

        private void onPartCouple(GameEvents.FromToAction<Part, Part> action)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {
                if (action.to.vessel.id == vessel.id)
                {
                    double totalResAmount = 0;
                    foreach (Part p in action.from.vessel.parts)
                    {
                        foreach (PartResource res in p.Resources)
                        {
                            if (res.info.id == resource.ID)
                            {
                                totalResAmount = totalResAmount + res.amount;
                                break;
                            }
                        }
                    }
                    if (totalResAmount >= Convert.ToDouble(resourceAmount))
                    {
                        totalResAmount = Convert.ToDouble(resourceAmount);
                        foreach (Part p in action.from.vessel.parts)
                        {
                            foreach (PartResource res in p.Resources)
                            {
                                if (res.info.id == resource.ID)
                                {
                                    if (res.amount < totalResAmount)
                                    {
                                        totalResAmount = totalResAmount - res.amount;
                                        res.amount = 0;
                                    }
                                    else
                                    {
                                        res.amount = res.amount - totalResAmount;
                                        totalResAmount = 0;
                                    }
                                    break;
                                }
                            }
                        }
                        Debug.Log("contract complet");
                        base.SetComplete();
                    }
                    else
                    {
                        Debug.Log("not enough resources");
                    }
                }
            }
            else { base.SetIncomplete(); }
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

    public class undockingParameter : ContractParameter
    {
        public string vesselID = "";
        public Vessel vessel = null;

        public undockingParameter(Vessel vessel)
        {
            this.vessel = vessel;
            vesselID = vessel.id.ToString();
        }

        protected override string GetHashString()
        {
            return "Undock your ship from the cargo";
        }
        protected override string GetTitle()
        {
            return "Undock your ship from the cargo";
        }

        protected override void OnRegister()
        {
            GameEvents.onPartCouple.Add(OnDock);
            GameEvents.onPartUndock.Add(OnUnDock);
        }
        protected override void OnUnregister()
        {
            GameEvents.onPartCouple.Remove(OnDock);
            GameEvents.onPartUndock.Remove(OnUnDock);
        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("vessel", vesselID);
        }
        protected override void OnLoad(ConfigNode node)
        {
            vesselID = node.GetValue("vessel");
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.id.ToString() == vesselID)
                    {
                        vessel = v;
                        vessel.id = v.id;
                    }
                }
            }
        }

        private void OnUnDock(Part part)
        {
            if (part.vessel.id.ToString() == vesselID)
            {
                SetComplete();
            }
        }
        private void OnDock(GameEvents.FromToAction<Part, Part> action)
        {
            if (action.to == vessel || action.from == vessel)
            {
                SetIncomplete();
            }
        }
    }
}
