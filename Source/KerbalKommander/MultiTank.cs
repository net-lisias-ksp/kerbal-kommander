using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kerbal_Kommander
{
    public class MultiTank : PartModule
    {
        [KSPField]
        public double TankAmount;



        void Update()
        {
            double TotalAmount = 0;
            for (int i = part.Resources.Count - 1; i >= 0; --i)
            {
                PartResource resource = part.Resources[i];
                TotalAmount = TotalAmount + resource.amount;
            }
            for (int i = part.Resources.Count - 1; i >= 0; --i)
            {
                PartResource resource = part.Resources[i];
                resource.maxAmount = TankAmount - TotalAmount + resource.amount;
            }
        }
    }
}
