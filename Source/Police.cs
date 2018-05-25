using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class Police : PartModule
    {
        public bool policeGUI = false;
        private float lastFixedUpdate = 0.0f;
        private float logInterval = 20f;

        void FixedUpdate()
        {
            if ((Time.time - lastFixedUpdate) > logInterval)
            {
                lastFixedUpdate = Time.time;
                for (int i = FlightGlobals.ActiveVessel.Parts.Count - 1; i >= 0; --i)
                {
                    Part part = FlightGlobals.ActiveVessel.Parts[i];

                    for (int j = part.Resources.Count - 1; j >= 0; --j)
                    {
                        PartResource resource = part.Resources[j];
                        if (resource.resourceName == "Weapons" && resource.amount > 0)
                        {
                            resource.amount = 0;
                            policeGUI = true;
                        }
                    }

                    for (int j = part.protoModuleCrew.Count - 1; j >= 0; --j)
                    {
                        ProtoCrewMember crew = part.protoModuleCrew[j];
                        if (crew.type == ProtoCrewMember.KerbalType.Tourist && crew.isBadass == true)
                        {
                            removeSlaves();
                            policeGUI = true;
                            break;
                        }
                    }
                }
            }
        }

        void removeSlaves()
        {
            for (int i = FlightGlobals.ActiveVessel.Parts.Count - 1; i >= 0; --i)
            {
                Part part = FlightGlobals.ActiveVessel.Parts[i];
                restartForeach:
                for (int j = part.protoModuleCrew.Count - 1; j >= 0; --j)
                {
                    ProtoCrewMember crewMember = part.protoModuleCrew[j];
                    if (crewMember.type == ProtoCrewMember.KerbalType.Tourist && crewMember.isBadass == true)
                    {
                        part.protoModuleCrew.Remove(crewMember);
                        part.DespawnIVA();
                        part.SpawnIVA();
                        if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
                        {
                            CameraManager.Instance.SetCameraMap();
                            CameraManager.Instance.SetCameraFlight();
                        }
                        goto restartForeach;
                    }
                }
            }
            part.SpawnIVA();
            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.Flight)
            {
                CameraManager.Instance.SetCameraMap();
                CameraManager.Instance.SetCameraFlight();
            }
        }

        void OnGUI()
        {
            if (policeGUI == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 400) / 2, 700, 400), PoliceGUI, "Message from the police", HighLogic.Skin.window);
            }
        }

        void PoliceGUI(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("We have found illegal cargo in your ship!All illegal cargo has been taken and you have to pay a fine of $10, 000.");
            if (GUILayout.Button("pay", HighLogic.Skin.button))
            {
                Funding.Instance.AddFunds(-10000, TransactionReasons.Any);
                if (Funding.Instance.Funds < 0) { Funding.Instance.AddFunds(-Funding.Instance.Funds, TransactionReasons.Any); }
                policeGUI = false;
            }
            GUILayout.EndVertical();
        }

    }
}
