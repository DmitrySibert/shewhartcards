using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShewhartCards.Utilities
{
    class SubgroupsData
    {
        public SubgroupsData(int elementsQuantity, int deviationsQuantity)
        {
            this.elementsQuantity = elementsQuantity;
            this.diversionQuantity = deviationsQuantity;
            this.diversionProcent = (double)deviationsQuantity / elementsQuantity;
        }
        public int elementsQuantity;
        public int diversionQuantity;
        public double diversionProcent;
    }
}
