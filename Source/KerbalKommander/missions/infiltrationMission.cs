using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;
using Contracts.Parameters;

namespace Kerbal_Kommander.missions
{
    class infiltrationContract : Contract
    {
        CelestialBody targetBody = null;
        public string name = null;
        public Vessel vessel = null;
        public float reward = 0;
        public string vesselName;
        public string synopsis = "";
        public string vesselID = "";

        protected override bool Generate()
        {
            
            CelestialBody body = FlightGlobals.Bodies[UnityEngine.Random.Range(1, FlightGlobals.Bodies.Count)];
            Orbit orbit = new Orbit(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 0.3f), UnityEngine.Random.Range(Convert.ToSingle(body.Radius + body.atmosphereDepth), Convert.ToSingle(body.sphereOfInfluence)), UnityEngine.Random.Range(0, 360), 0, 0, HighLogic.CurrentGame.UniversalTime, body);
            vesselName = orbit.referenceBody.bodyName + "'s Ennemy station " + UnityEngine.Random.Range(101, 999);
            vessel = Tools.VesselSpawner.SpawnVessel(vesselName, "GameData\\KerbalKommander\\Assets\\ships\\mothership.craft", HighLogic.CurrentGame.flagURL, VesselType.Ship, Planetarium.fetch.Home, orbit, new List<ProtoCrewMember>());
            vesselID = vessel.id.ToString();
            name = "Infiltrate an enemy station around " + body.bodyName;
            synopsis = "There is an enemy base in orbit around " + body.bodyName + " called " + vesselName + ".\nYou need to go in the station to desactivate the generator.\nFirst, you need to enter in the station. But be carefull, if you approach the station with a ship of more than 5t in a range of 5-6 km, they will spot you and the mission will be failed.\nWhen you will be in the station, you just need to wait a bit, your kerbal will desactivate the generator.\nThen, you will have to quickly flee because the station will explode.\nGood luck !";
            targetBody = Planetarium.fetch.Home;
            
            AddParameter(new infiltrationParameter(vessel));
            AddParameter(new discretionParameter(vessel));
            AddParameter(new evasionParameter(vessel));

            reward = Convert.ToSingle(1000 * UnityEngine.Random.Range(20, 45));
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
            return "Infiltration mission complete !";
        }

        protected override void OnLoad(ConfigNode node)
        {
            name = node.GetValue("name");
            vesselID = node.GetValue("vesselID");
            synopsis = node.GetValue("synopsis");
        }
        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("name", name);
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
            ScreenMessages.PostScreenMessage("Contract Complet !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        }
        protected override void OnOfferExpired()
        {
            DespawnVesselDebris();
        }
        public void DespawnVesselDebris()
        {
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                foreach (Part part in vessel.parts)
                {
                    foreach (ProtoCrewMember crew in part.protoModuleCrew)
                    {
                        crew.KerbalRef.die();
                    }
                }
                if (vessel.id.ToString() == vesselID)
                {
                    vessel.Die();
                    break;
                }
            }
        }
    }

    public class infiltrationParameter : ContractParameter
    {

        public string ParamTitle = null;
        public Vessel vessel = null;
        public string vesselID = "";
        public bool updated = false;
        public static int successCounter = 0;

        public infiltrationParameter(Vessel vessel)
        {
            this.vessel = vessel;
            vesselID = this.vessel.id.ToString();
            ParamTitle = "Infiltrate the " + vessel.vesselName + " ennemy Base";
            successCounter = 0;
        }
        public infiltrationParameter()
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
                GameEvents.onCrewBoardVessel.Add(onCrewBoardVessel);
                updated = true;
            }
            else { }

        }
        protected override void OnUnregister()
        {
            if (updated)
            {
                GameEvents.onCrewBoardVessel.Remove(onCrewBoardVessel);
            }
            else { }

        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("vessel", vesselID);
            node.AddValue("ParamTitle", ParamTitle);
        }
        protected override void OnLoad(ConfigNode node)
        {
            successCounter = 0;
            ParamTitle = node.GetValue("ParamTitle");
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

        private void onCrewBoardVessel(GameEvents.FromToAction<Part, Part> action)
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {
                if (action.to.vessel.id == vessel.id)
                {
                    ScreenMessages.PostScreenMessage("Your Kerbal is trying to desactivate the generator", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            else { base.SetIncomplete(); }
        }
        protected override void OnUpdate()
        {
            if (this.Root.ContractState == Contract.State.Active)
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    if (FlightGlobals.ready && !PauseMenu.isOpen)
                    {
                        if (vessel.GetCrewCount() > 0 || successCounter >= 500)
                        {
                            successCounter++;
                            if (successCounter <= 500)
                                ScreenMessages.PostScreenMessage(((successCounter * 100) / 500).ToString(), 0.05f);
                        }
                        else
                            successCounter = 0;
                    }
                    if (successCounter == 500)
                    {
                        ScreenMessages.PostScreenMessage("Generator Desactivated");
                        base.SetComplete();
                    }
                    if (successCounter >= 1500)
                    {
                        ScreenMessages.PostScreenMessage("Enemy Station exploded !");
                        for (int i = vessel.parts.Count - 1; i >= 0; i--)
                            vessel.parts[i].explode();
                    }
                }
            }
        }
    }

    public class discretionParameter : ContractParameter
    {
        public string ParamTitle = null;
        public string vesselID = null;
        public Vessel vessel = null;
        public bool updated = false;

        public discretionParameter(Vessel vessel)
        {
            this.vessel = vessel;
            vesselID = vessel.id.ToString();
            ParamTitle = "Be discreet: don't approach the station with a too big vessel";
        }
        public discretionParameter()
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
                GameEvents.onVesselLoaded.Add(onVesselLoaded);
                updated = true;
            }
            else { }

        }
        protected override void OnUnregister()
        {
            if (updated)
            {
                GameEvents.onVesselLoaded.Remove(onVesselLoaded);
            }
            else { }

        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("vessel", vesselID);
            node.AddValue("ParamTitle", ParamTitle);
        }
        protected override void OnLoad(ConfigNode node)
        {
            ParamTitle = node.GetValue("ParamTitle");

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
        private void onVesselLoaded(Vessel loadedVessel)
        {
            ScreenMessages.PostScreenMessage("vessel Loaded", 10f, ScreenMessageStyle.UPPER_CENTER);
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel)
            {
                if (vessel.loaded)
                {
                    foreach (Vessel v in FlightGlobals.Vessels)
                    {
                        
                        if (v.loaded == true && v.GetTotalMass() > 5 && v != vessel)
                        {
                            SetFailed();
                            ScreenMessages.PostScreenMessage(v.vesselName + "has been spotted :(", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                            goto end;
                        }
                    }
                    SetComplete();
                    end:;
                }
                else
                {
                    SetComplete();
                }
            }
        }
    }

    public class evasionParameter : ContractParameter
    {
        public string ParamTitle = null;
        public string vesselID = null;
        public Vessel vessel = null;
        public bool updated = false;

        public evasionParameter(Vessel vessel)
        {
            this.vessel = vessel;
            vesselID = vessel.id.ToString();
            ParamTitle = "flee after having infiltrate the station";
        }
        public evasionParameter()
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
                updated = true;
            }
            else { }

        }
        protected override void OnUnregister()
        {
            if (updated)
            {
            }
            else { }

        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("vessel", vesselID);
            node.AddValue("ParamTitle", ParamTitle);
        }
        protected override void OnLoad(ConfigNode node)
        {
            ParamTitle = node.GetValue("ParamTitle");

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
        protected override void OnUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (infiltrationParameter.successCounter >= 1500)
                {
                    int crews = 0;
                    foreach (Part p in vessel.parts)
                    {
                        crews = crews + p.protoModuleCrew.Count;
                    }
                    if (crews == 0)
                    {
                        SetComplete();
                    }
                    else
                    {
                        this.SetFailed();
                    }
                }
            }
        }
    }
}
