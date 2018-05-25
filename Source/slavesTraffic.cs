using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Experience;

namespace Kerbal_Kommander
{
    public class slavesTraffic : PartModule
    {
        Vector2 scrollPos = new Vector2();
        public static bool slaveWindow = false;
        public static double slavePrice;

        void OnGUI()
        {
            if (slaveWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), DrawGUISlaves, "Slaves Traffic", HighLogic.Skin.window);
            }
        }
        void DrawGUISlaves(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hello! When trafficking slaves, make sure the police don't find out!", HighLogic.Skin.label);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Male  " + "      Price: " + slavePrice.ToString(), HighLogic.Skin.label);
            if (GUILayout.Button("Buy Slave", HighLogic.Skin.button))
            {
                if (Funding.Instance.Funds < 1000)
                {
                    ScreenMessages.PostScreenMessage("Not enough eunds", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    Funding.Instance.AddFunds(-slavePrice, TransactionReasons.CrewRecruited);
                    ProtoCrewMember slave = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Tourist);
                    slave.ChangeName(CrewGenerator.GetRandomName(ProtoCrewMember.Gender.Male) + " (slave)");
                    slave.gender = ProtoCrewMember.Gender.Male;
                    slave.courage = 1;
                    slave.stupidity = 1;
                    slave.isBadass = true;
                    foreach (Part CrewPart in vessel.Parts)
                    {
                        if (CrewPart.protoModuleCrew.Count < CrewPart.CrewCapacity)
                        {
                            CrewPart.AddCrewmemberAt(slave, CrewPart.protoModuleCrew.Count);
                            CrewPart.SpawnIVA();
                            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
                            {
                                CameraManager.Instance.SetCameraMap();
                                CameraManager.Instance.SetCameraFlight();
                            }

                            break;
                        }
                    }

                    ScreenMessages.PostScreenMessage("New Slave Added (male)", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Female" + "      Price: " + slavePrice.ToString(), HighLogic.Skin.label);
            if (GUILayout.Button("Buy Slave", HighLogic.Skin.button))
            {
                if (Funding.Instance.Funds < 900)
                {
                    ScreenMessages.PostScreenMessage("Not enough funds", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    Funding.Instance.AddFunds(-slavePrice, TransactionReasons.CrewRecruited);
                    ProtoCrewMember slave = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Tourist);
                    slave.type = ProtoCrewMember.KerbalType.Tourist;
                    slave.ChangeName(CrewGenerator.GetRandomName(ProtoCrewMember.Gender.Female) + " (slave)");
                    slave.gender = ProtoCrewMember.Gender.Female;
                    slave.courage = 1;
                    slave.stupidity = 1;
                    slave.isBadass = true;
                    foreach (Part CrewPart in vessel.Parts)
                    {
                        if (CrewPart.protoModuleCrew.Count < CrewPart.CrewCapacity)
                        {
                            CrewPart.AddCrewmemberAt(slave, CrewPart.protoModuleCrew.Count);
                            CrewPart.SpawnIVA();
                            if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                            CameraManager.Instance.SetCameraMap();
                            CameraManager.Instance.SetCameraFlight();

                            break;
                        }
                    }
                    ScreenMessages.PostScreenMessage("New Slave Added (female)", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Sell Slaves:", HighLogic.Skin.label);

            scrollPos = GUILayout.BeginScrollView(scrollPos, HighLogic.Skin.scrollView);
            foreach (Part vPart in vessel.Parts)
            {
                foreach (ProtoCrewMember slaves in vPart.protoModuleCrew)
                {
                    if (slaves.type == ProtoCrewMember.KerbalType.Tourist)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name: " + slaves.name, HighLogic.Skin.label);
                        GUILayout.Label("Gender: " + slaves.gender.ToString());
                        GUILayout.Label("Price: " + slavePrice.ToString());
                        if (GUILayout.Button("Sell", HighLogic.Skin.button))
                        {
                            ScreenMessages.PostScreenMessage("Slave selled", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                            Funding.Instance.AddFunds(slavePrice, TransactionReasons.CrewRecruited);
                            vPart.protoModuleCrew.Remove(slaves);
                            vPart.DespawnIVA();
                            if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                            CameraManager.Instance.SetCameraMap();
                            CameraManager.Instance.SetCameraFlight();
                            vPart.SpawnIVA();
                            if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                            CameraManager.Instance.SetCameraMap();
                            CameraManager.Instance.SetCameraFlight();
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                slaveWindow = false;
                MainMenu.menuWindow = true;
                foreach (Part parts in vessel.Parts)
                {
                    parts.SpawnIVA();
                    if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Flight) { CameraManager.Instance.SetCameraFlight(); }
                    CameraManager.Instance.SetCameraMap();
                    CameraManager.Instance.SetCameraFlight();
                }

            }
            GUILayout.EndVertical();
        }

        public static void CheckPrice(string bodyName)
        {
            if (bodyName == "Kerbin") { slavePrice = 1000; }
            if (bodyName == "Minmus") { slavePrice = 1100; }
            if (bodyName == "Moho") { slavePrice = 2000; }
            if (bodyName == "Dres") { slavePrice = 1800; }
            if (bodyName == "Gilly") { slavePrice = 1200; }
            if (bodyName == "Jool") { slavePrice = 2000; }
            if (bodyName == "Bop") { slavePrice = 2200; }
            if (bodyName == "Tylo") { slavePrice = 2400; }
        }
    }
}
