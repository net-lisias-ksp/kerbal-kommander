using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class crewHire : PartModule
    {
        public static bool hireCrewWindow = false;
        public Texture starTrue = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/starTrue", false);
        public Texture starFalse = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/starFalse", false);
        public Texture KerbalePortrait = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/KerbalePortrait", false);
        public Texture KerbalPortrait = GameDatabase.Instance.GetTexture("KerbalKommander/Assets/KerbalPortrait", false);
        public static List<ProtoCrewMember> crewToHire;

        void OnGUI()
        {
            if (hireCrewWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), DrawHireCrewWindow, "Crew Hire", HighLogic.Skin.window);
            }
        }

        void DrawHireCrewWindow(int windowID)
        {
            GUILayout.BeginVertical();


            foreach (ProtoCrewMember Crew in crewToHire)
            {
                GUILayout.BeginHorizontal();
                if (Crew.gender == ProtoCrewMember.Gender.Female) { GUILayout.Label(KerbalePortrait); }
                else { GUILayout.Label(KerbalPortrait); }
                GUILayout.Label(Crew.name + "\n" + Crew.trait, HighLogic.Skin.label);
                if (Crew.experienceLevel >= 1) { GUILayout.Label(starTrue); }
                else { GUILayout.Label(starFalse); }
                if (Crew.experienceLevel >= 2) { GUILayout.Label(starTrue); }
                else { GUILayout.Label(starFalse); }
                if (Crew.experienceLevel >= 3) { GUILayout.Label(starTrue); }
                else { GUILayout.Label(starFalse); }
                if (Crew.experienceLevel >= 4) { GUILayout.Label(starTrue); }
                else { GUILayout.Label(starFalse); }
                if (Crew.experienceLevel >= 5) { GUILayout.Label(starTrue); }
                else { GUILayout.Label(starFalse); }
                GUILayout.Label("Price: " + (Crew.experienceLevel * 10000 + 10000), HighLogic.Skin.label);
                if (GUILayout.Button("Hire", HighLogic.Skin.button))
                {
                    if (Funding.Instance.Funds < Crew.experienceLevel * 10000 + 10000)
                    {
                        ScreenMessages.PostScreenMessage("Not enough funds", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    }
                    else
                    {
                        foreach (Part CrewPart in vessel.Parts)
                        {
                            if (CrewPart.protoModuleCrew.Count < CrewPart.CrewCapacity)
                            {
                                Funding.Instance.AddFunds(-(Crew.experienceLevel * 10000 + 10000), TransactionReasons.CrewRecruited);

                                CrewPart.AddCrewmember(Crew);
                                crewToHire.Remove(Crew);
                                CrewPart.Actions.part.SpawnIVA();
                                CameraManager.Instance.SetCameraMap();
                                CameraManager.Instance.SetCameraFlight();

                                ScreenMessages.PostScreenMessage("Crew Added !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                break;
                            }
                        }
                    }

                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Refresh", HighLogic.Skin.button))
            {
                crewToHire.Clear();
                GenerateCrew();
            }
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                foreach (Part parts in vessel.Parts)
                {
                    parts.SpawnIVA();
                    if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                    CameraManager.Instance.SetCameraMap();
                    CameraManager.Instance.SetCameraFlight();
                }
                hireCrewWindow = false;
                MainMenu.menuWindow = true;
            }
            GUILayout.EndVertical();

        }

        public static void GenerateCrew()
        {
            int i;
            crewToHire = new List<ProtoCrewMember>();
            for (i = crewToHire.Count; i <= 5;)
            {
                ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.GetNewKerbal();
                crew.type = ProtoCrewMember.KerbalType.Crew;
                crew.experienceLevel = Math.Abs(UnityEngine.Random.Range(0, 6));
                FlightLog flight = new FlightLog();
                if (crew.experienceLevel >= 1)
                {
                    flight.AddEntry("Orbit,Kerbin");
                }
                if (crew.experienceLevel >= 2)
                {
                    flight.AddEntry("Land,Duna");
                }
                if (crew.experienceLevel >= 3)
                {
                    flight.AddEntry("Land,Ike");
                }
                if (crew.experienceLevel >= 4)
                {
                    flight.AddEntry("Land,Pol");
                }
                if (crew.experienceLevel >= 5)
                {
                    flight.AddEntry("Land,Eeloo");
                    flight.AddEntry("Land,Dres");
                }
                crew.careerLog.AddFlight();
                foreach (FlightLog.Entry entries in flight.Entries)
                {
                    crew.careerLog.AddEntry(entries);
                }
                crewToHire.Add(crew);
                i = crewToHire.Count;
            }
            hireCrewWindow = true;
        }
    }
}
