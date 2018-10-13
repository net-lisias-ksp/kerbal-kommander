using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;

namespace Kerbal_Kommander.missions
{
    public class Resource
    {
        public string name = null;
        public double unitCost = 0;
        public int ID = 0;
        public Resource(string resName, double cost, int resID)
        {
            name = resName;
            unitCost = cost;
            ID = resID;
        }
    }
    
    class TradingContracts : Contract
    {
        CelestialBody targetBody = null;
        public string name = null;
        public Resource resource = null;
        public Vessel station = null;
        public double resourceAmount = 0;
        public float reward = 0;
        public string synopsis = "";

        protected override bool Generate()
        {
            List<Resource> resList = new List<Resource>();
            resList.Add(new Resource("Steel", 2, 80208299));
            resList.Add(new Resource("Wood", 5, 2702029));
            resList.Add(new Resource("Diamond", 20, -975259340));
            resource = resList.ElementAt(UnityEngine.Random.Range(0, resList.Count));
            station = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            while (station.vesselType != VesselType.Station)
            {
                station = FlightGlobals.Vessels.ElementAt(UnityEngine.Random.Range(0, FlightGlobals.Vessels.Count));
            }
            resourceAmount = UnityEngine.Random.Range(5, 50) * 100;
            name = "Carry " + resourceAmount + " of " + resource.name + " to " + station.vesselName;
            synopsis = "The " + station.vesselName + " need " + resourceAmount + " of " + resource.name + ". \nYou have to dock with this station with a ship wich contain the needed resource.";
            reward = Convert.ToSingle(Math.Round((resourceAmount * resource.unitCost) * 2.5));

            targetBody = Planetarium.fetch.Home;
            this.AddParameter(new TradingParameter(name, resource, station, resourceAmount, reward));
            base.SetExpiry(5, 25);
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
            return TextGen.GenerateBackStories(Agent.Name, Agent.GetMindsetString(), "commerce", "trading", "Resources Transportation", new System.Random().Next());
        }
        protected override string GetSynopsys()
        {
            return synopsis;
        }
        protected override string MessageCompleted()
        {
            return "CONTRACT: \"" + name + "\" COMPLETE";
        }

        protected override void OnLoad(ConfigNode node)
        {
            name = node.GetValue("contractName");
            synopsis = node.GetValue("synopsis");
        }
        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractName", name);
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
    }

    public class TradingParameter : ContractParameter
    {

        private string contractName = null;
        private Resource resource = null;
        private Vessel station = null;
        private string resourceAmount = "0";
        private float reward = 0;
        private bool updated = false;
        private string stationName = "";

        public TradingParameter(string name, Resource resource, Vessel station, double resourceAmount, float reward)
        {
            this.contractName = name;
            this.resource = resource;
            this.station = station;
            stationName = station.vesselName;
            this.resourceAmount = resourceAmount.ToString();
            this.reward = reward;

        }
        public TradingParameter()
        {
        }


        protected override string GetHashString()
        {
            return "Carry " + resourceAmount + " of " + resource.name + " to " + stationName;
        }
        protected override string GetTitle()
        {
            return "Carry " + resourceAmount + " of " + resource.name + " to " + stationName;
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
            node.AddValue("station", stationName);
            node.AddValue("resourceAmount", resourceAmount);
            node.AddValue("contractName", contractName);
        }
        protected override void OnLoad(ConfigNode node)
        {
            List<Resource> resList = new List<Resource>();
            resList.Add(new Resource("Steel", 2, 80208299));
            resList.Add(new Resource("Wood", 5, 2702029));
            resList.Add(new Resource("Diamond", 20, -975259340));

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
            stationName = node.GetValue("station");
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.id.ToString() == stationName)
                    {
                        station = v;
                        station.id = v.id;
                    }
                }
            }
            resourceAmount = node.GetValue("resourceAmount");
            contractName = node.GetValue("contractName");
        }

        private void onPartCouple(GameEvents.FromToAction<Part, Part> action)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {
                if (action.to.vessel.id == station.id)
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
    
}
