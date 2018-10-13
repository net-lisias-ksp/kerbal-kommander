using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class trading : PartModule
    {
        public static double priceLiqFuelS;
        public static double priceLiqFuelB;
        public static double priceOxidizerS;
        public static double priceOxidizerB;
        public static double priceMonoS;
        public static double priceMonoB;
        public static double priceXenonS;
        public static double priceXenonB;
        public static double priceOreS;
        public static double priceOreB;
        public static double priceSteelB;
        public static double priceSteelS;
        public static double priceDiamondB;
        public static double priceDiamondS;
        public static double priceWoodB;
        public static double priceWoodS;
        public static double priceWeaponB;
        public static double priceWeaponS;
        short BuyResources = 0;
        short SellResource = 0;
        public static bool DrawGUIWindow = false;
        bool tradWindowBool = false;
        string resourceName;
        double priceBuy;
        double priceSell;
        int ResourceID;
        Vector2 scrollPos = new Vector2();
        double mass;
        public List<PartResource> ModResources = null;

        static void setPriceKer()
        {
            priceLiqFuelB = 0.8;
            priceLiqFuelS = 0.7;
            priceOxidizerB = 0.18;
            priceOxidizerS = 0.08;
            priceOreB = 0.02;
            priceOreS = 0.01;
            priceXenonB = 4;
            priceXenonS = 3.5;
            priceMonoB = 1.2;
            priceMonoS = 1;
            priceSteelB = 2;
            priceSteelS = 1.6;
            priceDiamondB = 20;
            priceDiamondS = 19;
            priceWoodB = 5;
            priceWoodS = 4.5;
            priceWeaponB = 50;
            priceWeaponS = 40;
        }
        static void setPriceMun()
        {
            priceLiqFuelB = 0.5;
            priceLiqFuelS = 0.4;
            priceOxidizerB = 0.1;
            priceOxidizerS = 0.05;
            priceOreB = 0.01;
            priceOreS = 0.001;
            priceXenonB = 5;
            priceXenonS = 4;
            priceMonoB = 1;
            priceMonoS = 0.8;
            priceSteelB = 1.2;
            priceSteelS = 1;
            priceDiamondB = 15;
            priceDiamondS = 14;
            priceWoodB = 7;
            priceWoodS = 6.5;
            priceWeaponB = 45;
            priceWeaponS = 38;
        }
        static void setPriceMinmus()
        {
            priceLiqFuelB = 0.5;
            priceLiqFuelS = 0.4;
            priceOxidizerB = 0.1;
            priceOxidizerS = 0.05;
            priceOreB = 0.01;
            priceOreS = 0.001;
            priceXenonB = 5;
            priceXenonS = 4;
            priceMonoB = 1;
            priceMonoS = 0.8;
            priceSteelB = 1.2;
            priceSteelS = 1;
            priceDiamondB = 13;
            priceDiamondS = 12;
            priceWoodB = 7;
            priceWoodS = 6.5;
            priceWeaponB = 45;
            priceWeaponS = 38;
        }
        static void setPriceDuna()
        {
            priceLiqFuelB = 1;
            priceLiqFuelS = 0.9;
            priceOxidizerB = 0.2;
            priceOxidizerS = 0.18;
            priceOreB = 0.01;
            priceOreS = 0;
            priceXenonB = 5;
            priceXenonS = 4.5;
            priceMonoB = 1.5;
            priceMonoS = 1.4;
            priceSteelB = 1;
            priceSteelS = 0.8;
            priceDiamondB = 17;
            priceDiamondS = 15;
            priceWoodB = 8;
            priceWoodS = 7.9;
            priceWeaponB = 60;
            priceWeaponS = 55;
        }
        static void setPriceIke()
        {
            priceLiqFuelB = 0.8;
            priceLiqFuelS = 0.7;
            priceOxidizerB = 0.18;
            priceOxidizerS = 0.16;
            priceOreB = 0.005;
            priceOreS = 0;
            priceXenonB = 4.5;
            priceXenonS = 3;
            priceMonoB = 1.3;
            priceMonoS = 1.1;
            priceSteelB = 0.8;
            priceSteelS = 0.7;
            priceDiamondB = 14;
            priceDiamondS = 10;
            priceWoodB = 12;
            priceWoodS = 11;
            priceWeaponB = 55;
            priceWeaponS = 53;
        }
        static void setPriceEve()
        {
            priceLiqFuelB = 2;
            priceLiqFuelS = 1.8;
            priceOxidizerB = 0.5;
            priceOxidizerS = 0.4;
            priceOreB = 0.11;
            priceOreS = 0.1;
            priceXenonB = 6;
            priceXenonS = 4;
            priceMonoB = 2;
            priceMonoS = 1.2;
            priceSteelB = 4;
            priceSteelS = 3.8;
            priceDiamondB = 22;
            priceDiamondS = 15;
            priceWoodB = 12;
            priceWoodS = 11;
            priceWeaponB = 30;
            priceWeaponS = 28;
        }
        static void setPriceGilly()
        {
            priceLiqFuelB = 2;
            priceLiqFuelS = 1.8;
            priceOxidizerB = 0.5;
            priceOxidizerS = 0.4;
            priceOreB = 0.09;
            priceOreS = 0.08;
            priceXenonB = 6;
            priceXenonS = 4;
            priceMonoB = 2;
            priceMonoS = 1.2;
            priceSteelB = 3.8;
            priceSteelS = 3.7;
            priceDiamondB = 21;
            priceDiamondS = 11;
            priceWoodB = 14;
            priceWoodS = 12;
            priceWeaponB = 27.5;
            priceWeaponS = 25;
        }
        static void setPriceMoho()
        {
            priceLiqFuelB = 3;
            priceLiqFuelS = 2.5;
            priceOxidizerB = 2;
            priceOxidizerS = 1.5;
            priceOreB = 0.2;
            priceOreS = 0.17;
            priceXenonB = 8;
            priceXenonS = 7;
            priceMonoB = 3;
            priceMonoS = 2.5;
            priceSteelB = 5;
            priceSteelS = 4.3;
            priceDiamondB = 25;
            priceDiamondS = 16;
            priceWoodB = 30;
            priceWoodS = 25;
            priceWeaponB = 23;
            priceWeaponS = 15;
        }
        static void setPriceDres()
        {
            priceLiqFuelB = 0.8;
            priceLiqFuelS = 0.7;
            priceOxidizerB = 0.2;
            priceOxidizerS = 0.18;
            priceOreB = 0.01;
            priceOreS = 0;
            priceXenonB = 4.5;
            priceXenonS = 4;
            priceMonoB = 1;
            priceMonoS = 0.9;
            priceSteelB = 0.7;
            priceSteelS = 0.6;
            priceDiamondB = 15;
            priceDiamondS = 12;
            priceWoodB = 20;
            priceWoodS = 18;
            priceWeaponB = 40;
            priceWeaponS = 38;
        }
        static void setPriceEeloo()
        {
            priceLiqFuelB = 3.5;
            priceLiqFuelS = 3.2;
            priceOxidizerB = 2.2;
            priceOxidizerS = 1.8;
            priceOreB = 0.3;
            priceOreS = 0.2;
            priceXenonB = 8.5;
            priceXenonS = 7.8;
            priceMonoB = 3.5;
            priceMonoS = 3.2;
            priceSteelB = 6;
            priceSteelS = 5.5;
            priceDiamondB = 30;
            priceDiamondS = 25;
            priceWoodB = 18;
            priceWoodS = 15;
            priceWeaponB = 50;
            priceWeaponS = 48;
        }
        static void setPriceJool()
        {
            priceLiqFuelB = 1.1;
            priceLiqFuelS = 0.9;
            priceOxidizerB = 0.25;
            priceOxidizerS = 0.21;
            priceOreB = 0.04;
            priceOreS = 0.03;
            priceXenonB = 4.6;
            priceXenonS = 4;
            priceMonoB = 1.8;
            priceMonoS = 1.5;
            priceSteelB = 2.3;
            priceSteelS = 1.8;
            priceDiamondB = 23;
            priceDiamondS = 21;
            priceWoodB = 6;
            priceWoodS = 5;
            priceWeaponB = 50;
            priceWeaponS = 40;
        }
        static void setPriceLaythe()
        {
            priceLiqFuelB = 1;
            priceLiqFuelS = 0.9;
            priceOxidizerB = 0.2;
            priceOxidizerS = 0.18;
            priceOreB = 0.03;
            priceOreS = 0.02;
            priceXenonB = 4.1;
            priceXenonS = 3.8;
            priceMonoB = 1.6;
            priceMonoS = 1.4;
            priceSteelB = 2;
            priceSteelS = 1.8;
            priceDiamondB = 20;
            priceDiamondS = 19.5;
            priceWoodB = 5;
            priceWoodS = 4.5;
            priceWeaponB = 45;
            priceWeaponS = 40;
        }
        static void setPriceVall()
        {
            priceLiqFuelB = 0.9;
            priceLiqFuelS = 0.8;
            priceOxidizerB = 0.18;
            priceOxidizerS = 0.12;
            priceOreB = 0.025;
            priceOreS = 0.02;
            priceXenonB = 4;
            priceXenonS = 3.8;
            priceMonoB = 1.4;
            priceMonoS = 1.3;
            priceSteelB = 1.9;
            priceSteelS = 1.75;
            priceDiamondB = 19.7;
            priceDiamondS = 19.5;
            priceWoodB = 7;
            priceWoodS = 6;
            priceWeaponB = 45;
            priceWeaponS = 40;
        }
        static void setPriceBop()
        {
            priceLiqFuelB = 0.5;
            priceLiqFuelS = 0.4;
            priceOxidizerB = 0.1;
            priceOxidizerS = 0.05;
            priceOreB = 0.01;
            priceOreS = 0.001;
            priceXenonB = 3.2;
            priceXenonS = 3.1;
            priceMonoB = 1;
            priceMonoS = 0.8;
            priceSteelB = 1.2;
            priceSteelS = 1;
            priceDiamondB = 13;
            priceDiamondS = 12;
            priceWoodB = 9;
            priceWoodS = 6;
            priceWeaponB = 45;
            priceWeaponS = 38;
        }
        static void setPriceTylo()
        {
            priceLiqFuelB = 1.5;
            priceLiqFuelS = 1.4;
            priceOxidizerB = 0.4;
            priceOxidizerS = 0.3;
            priceOreB = 0.06;
            priceOreS = 0.05;
            priceXenonB = 5;
            priceXenonS = 4.8;
            priceMonoB = 2.2;
            priceMonoS = 2.1;
            priceSteelB = 3;
            priceSteelS = 2.7;
            priceDiamondB = 25;
            priceDiamondS = 24;
            priceWoodB = 6;
            priceWoodS = 5;
            priceWeaponB = 50;
            priceWeaponS = 40;
        }
        static void setPricePol()
        {
            priceLiqFuelB = 0.5;
            priceLiqFuelS = 0.4;
            priceOxidizerB = 0.1;
            priceOxidizerS = 0.05;
            priceOreB = 0.01;
            priceOreS = 0.001;
            priceXenonB = 3.2;
            priceXenonS = 3.1;
            priceMonoB = 1;
            priceMonoS = 0.8;
            priceSteelB = 1.2;
            priceSteelS = 1;
            priceDiamondB = 13;
            priceDiamondS = 12;
            priceWoodB = 9;
            priceWoodS = 6;
            priceWeaponB = 45;
            priceWeaponS = 38;
        }
        static void setPriceSunHigh()
        {
            priceLiqFuelB = 6;
            priceLiqFuelS = 5;
            priceOxidizerB = 2;
            priceOxidizerS = 1.5;
            priceOreB = 0.8;
            priceOreS = 0.7;
            priceXenonB = 12;
            priceXenonS = 11;
            priceMonoB = 8;
            priceMonoS = 7;
            priceSteelB = 15;
            priceSteelS = 14;
            priceDiamondB = 50;
            priceDiamondS = 45;
            priceWoodB = 23;
            priceWoodS = 22;
            priceWeaponB = 70;
            priceWeaponS = 65;
        }
        static void setPriceSunLow()
        {
            priceLiqFuelB = 5.5;
            priceLiqFuelS = 5;
            priceOxidizerB = 1.8;
            priceOxidizerS = 1.5;
            priceOreB = 0.6;
            priceOreS = 0.5;
            priceXenonB = 10;
            priceXenonS = 8;
            priceMonoB = 7;
            priceMonoS = 6;
            priceSteelB = 13;
            priceSteelS = 12.5;
            priceDiamondB = 48;
            priceDiamondS = 47;
            priceWoodB = 21;
            priceWoodS = 20;
            priceWeaponB = 60;
            priceWeaponS = 58;
        }
        public static void checkPrice(string refBodyName, double SMA)
        {
            ScreenMessages.PostScreenMessage(refBodyName, 5.0f, ScreenMessageStyle.UPPER_CENTER);
            if (refBodyName == "Kerbin") { setPriceKer(); }
            if (refBodyName == "Mun") { setPriceMun(); }
            if (refBodyName == "Minmus") { setPriceMinmus(); }
            if (refBodyName == "Duna") { setPriceDuna(); }
            if (refBodyName == "Ike") { setPriceIke(); }
            if (refBodyName == "Eve") { setPriceEve(); }
            if (refBodyName == "Gilly") { setPriceGilly(); }
            if (refBodyName == "Moho") { setPriceMoho(); }
            if (refBodyName == "Dres") { setPriceDres(); }
            if (refBodyName == "Eeloo") { setPriceEeloo(); }
            if (refBodyName == "Jool") { setPriceJool(); }
            if (refBodyName == "Laythe") { setPriceLaythe(); }
            if (refBodyName == "Vall") { setPriceVall(); }
            if (refBodyName == "Bop") { setPriceBop(); }
            if (refBodyName == "Tylo") { setPriceTylo(); }
            if (refBodyName == "Pol") { setPricePol(); }
            if (refBodyName == "Sun" && SMA > 1000000000) { setPriceSunHigh(); }
            if (refBodyName == "Sun" && SMA < 1000000000) { setPriceSunLow(); }
        }
        void OnGUI()
        {
            if (DrawGUIWindow == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 410) / 2, 700, 410), DrawGUI, "trading", HighLogic.Skin.window);
            }
            if (tradWindowBool == true)
            {
                GUI.Window(GetInstanceID(), new Rect((Screen.width - 700) / 2, (Screen.height - 400) / 2, 710, 410), tradWindow, resourceName, HighLogic.Skin.window);
            }
        }


        void DrawGUI(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, HighLogic.Skin.scrollView);
            if (GUILayout.Button("Liquid Fuel", HighLogic.Skin.button))
            {
                resourceName = "Liquid Fuel";
                priceBuy = priceLiqFuelB;
                priceSell = priceLiqFuelS;
                ResourceID = 374119730;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Oxidizer", HighLogic.Skin.button))
            {
                resourceName = "Oxydizer";
                priceBuy = priceOxidizerB;
                priceSell = priceOxidizerS;
                ResourceID = -1823983486;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Mono Propellant", HighLogic.Skin.button))
            {
                resourceName = "Mono Propellant";
                priceBuy = priceMonoB;
                priceSell = priceMonoS;
                ResourceID = 2001413032;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Xenon Gas", HighLogic.Skin.button))
            {
                resourceName = "Xenon Gas";
                priceBuy = priceXenonB;
                priceSell = priceXenonS;
                ResourceID = 1447111193;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Ore", HighLogic.Skin.button))
            {
                resourceName = "Ore";
                priceBuy = -1;
                priceSell = priceOreS;
                ResourceID = 79554;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Steel", HighLogic.Skin.button))
            {
                resourceName = "Steel";
                priceBuy = priceSteelB;
                priceSell = priceSteelS;
                ResourceID = 80208299;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (GUILayout.Button("Diamond", HighLogic.Skin.button))
            {
                resourceName = "Diamond";
                priceBuy = priceDiamondB;
                priceSell = priceDiamondS;
                ResourceID = -975259340;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;

            }

            if (GUILayout.Button("Wood", HighLogic.Skin.button))
            {
                resourceName = "Wood";
                priceBuy = priceWoodB;
                priceSell = priceWoodS;
                ResourceID = 2702029;
                tradWindowBool = true;
                DrawGUIWindow = false;
                BuyResources = 0;
            }
            if (part.partInfo.name == "pirateStationScreen")
            {
                GUILayout.Label("Illegal products:", HighLogic.Skin.label);
                if (GUILayout.Button("Weapons", HighLogic.Skin.button))
                {
                    resourceName = "Weapons";
                    priceBuy = priceWeaponB;
                    priceSell = priceWeaponS;
                    ResourceID = -1406985801;
                    tradWindowBool = true;
                    DrawGUIWindow = false;
                    BuyResources = 0;
                }
            }
            vesselResources();
            if (ModResources.Count > 1)
            {
                GUILayout.Label("Mods:", HighLogic.Skin.label);
                foreach (PartResource resource in ModResources)
                {
                    if (resource.resourceName != "ElectricCharge" && resource.resourceName != "LiquidFuel" && resource.resourceName != "Oxidizer" && resource.resourceName != "MonoPropellant" && resource.resourceName != "XenonGas" && resource.resourceName != "Ore" && resource.resourceName != "Steel" && resource.resourceName != "Diamond" && resource.resourceName != "Wood" && resource.resourceName != "Weapons" && resource.resourceName != "SolidFuel" && resource.resourceName != "IntakeAir")
                    {
                        if (GUILayout.Button(resource.resourceName, HighLogic.Skin.button))
                        {

                            resourceName = resource.resourceName;
                            priceSell = resource.info.unitCost;
                            priceBuy = resource.info.unitCost;
                            ResourceID = resource.info.id;
                            ConfigNode c = new ConfigNode("RESSOURCE");
                            c.AddValue("name", resourceName);
                            c.AddValue("amount", 0);
                            c.AddValue("maxAmount", 100000);
                            part.AddResource(c);

                            tradWindowBool = true;
                            DrawGUIWindow = false;
                            BuyResources = 0;
                        }
                    }
                }
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("close", HighLogic.Skin.button))
            {
                DrawGUIWindow = false;
                MainMenu.menuWindow = true;
            }



            GUILayout.EndVertical();
        }
        void vesselResources()
        {
            bool ex = false;
            ModResources.Clear();
            foreach (Part part in vessel.parts)
            {
                foreach (PartResource resource in part.Resources)
                {
                    ex = false;
                    if (resource.resourceName != "LiquidFuel" && resource.resourceName != "Oxidizer" && resource.resourceName != "MonoPropellant" && resource.resourceName != "XenonGas" && resource.resourceName != "Ore" && resource.resourceName != "Steel" && resource.resourceName != "Diamond" && resource.resourceName != "Wood" && resource.resourceName != "ElectricCharge" && resource.resourceName != "SolidFuel")
                    {
                        foreach (PartResource res in ModResources)
                        {
                            if (res.resourceName == resource.resourceName)
                            {
                                ex = true;
                            }
                        }
                        if (ex == false) { ModResources.Add(resource); }
                    }
                }
            }
        }

        void tradWindow(int windowID)
        {
            double funds = Funding.Instance.Funds;

            GUILayout.BeginVertical();
            float maxS = Convert.ToSingle(mass);

            if (priceBuy == -1) { goto IfCantBuy; }

            GUILayout.BeginHorizontal();
            mass = part.Resources.Get(ResourceID).amount;
            GUILayout.Label("Buy " + resourceName, HighLogic.Skin.label);
            GUILayout.Label("Price: " + priceBuy, HighLogic.Skin.label);
            float max = Convert.ToSingle(funds / priceBuy);

            GUILayout.Label("Quantity: " + BuyResources, HighLogic.Skin.label);

            GUILayout.Label("Prix Total: " + BuyResources * priceBuy, HighLogic.Skin.label);
            if (GUILayout.Button("Buy", HighLogic.Skin.button))
            {
                if (BuyResources * priceBuy <= funds)
                {
                    Funding.Instance.AddFunds(-(BuyResources * priceBuy), TransactionReasons.Cheating);
                    part.TransferResource(ResourceID, BuyResources);
                    ScreenMessages.PostScreenMessage("Done !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Not Enough Money !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                BuyResources = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (max < 10000)
            {

                BuyResources = (short)GUILayout.HorizontalSlider(BuyResources, 0, Math.Abs(max), HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb);
            }
            else
            {
                BuyResources = (short)GUILayout.HorizontalSlider(BuyResources, 0, 10000, HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Fill Vessel", HighLogic.Skin.button))
            {
                //price = 0;
                foreach (Part part in vessel.parts)
                {
                    if (part.name != "pirateStationScreen" && part.name != "stationScreen")
                    {
                        foreach (PartResource resource in part.Resources)
                        {
                            if (resource.info.id == ResourceID && resource._flowState)
                            {
                                if ((resource.maxAmount - resource.amount) * priceBuy >= Funding.Instance.Funds)
                                {
                                    ScreenMessages.PostScreenMessage("Not Enough Funds To Fill Everything", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                    resource.amount = resource.amount + Math.Abs(Funding.Instance.Funds / priceBuy);
                                    Funding.Instance.AddFunds(-Funding.Instance.Funds, TransactionReasons.Cheating);
                                    break;
                                }
                                else
                                {
                                    //price = price + (resource.maxAmount - resource.amount) * priceBuy;
                                    Funding.Instance.AddFunds(-(resource.maxAmount - resource.amount) * priceBuy, TransactionReasons.Any);
                                    resource.amount = resource.maxAmount;
                                    break;
                                }
                            }
                        }
                    }
                }
                ScreenMessages.PostScreenMessage("Vessel Filled !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }

            IfCantBuy:
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sell " + resourceName, HighLogic.Skin.label);
            GUILayout.Label("Price: " + priceSell, HighLogic.Skin.label);


            GUILayout.Label("Quantity: " + SellResource, HighLogic.Skin.label);
            GUILayout.Label("Prix Total: " + SellResource * priceSell, HighLogic.Skin.label);
            if (GUILayout.Button("Sell", HighLogic.Skin.button))
            {
                if (mass >= SellResource)
                {
                    Funding.Instance.AddFunds((SellResource * priceSell), TransactionReasons.Cheating);
                    part.TransferResource(ResourceID, -SellResource);
                    ScreenMessages.PostScreenMessage("Done !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Not Enough Resources !", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                SellResource = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (maxS > 10000)
            {
                SellResource = (short)GUILayout.HorizontalSlider(SellResource, 0, 10000, HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb);
            }
            else
            {

                SellResource = (short)GUILayout.HorizontalSlider(SellResource, 0, Math.Abs(maxS), HighLogic.Skin.horizontalSlider, HighLogic.Skin.horizontalSliderThumb);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Sell All", HighLogic.Skin.button))
            {
                foreach (Part part in vessel.parts)
                {
                    foreach (PartResource resource in part.Resources)
                    {
                        if (resource.info.id == ResourceID)
                        {
                            Funding.Instance.AddFunds(resource.amount * priceSell, TransactionReasons.Cheating);
                            resource.amount = 0;
                        }
                    }
                }
            }
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                tradWindowBool = false;
                DrawGUIWindow = true;
            }
            GUILayout.EndVertical();
        }
    }
}
