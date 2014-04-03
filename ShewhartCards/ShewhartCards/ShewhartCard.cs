using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShewhartCards
{
    interface IShewhartCard
    {
        double getUpperLine();
        double getLowerLine();
        int getGroupQuantity();
        double[] getValues();
        bool isInStateOfStaticalControl();
        bool isInSpecialState();
    }

    class ShewhartCard : IShewhartCard
    {

        private double upperLine;
        private double lowerLine;
        private double centralLine;
        private double[] values;

        public double getUpperLine()
        {
            return upperLine;
        }

        public double getLowerLine()
        {
            return lowerLine;
        }

        public double getCentralLine()
        {
            return centralLine;
        }

        public int getGroupQuantity()
        {
            return values.Length;
        }

        public double[] getValues()
        {
            double[] tmp = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                tmp[i] = values[i];
            }
            return tmp;
        }

        public void setValues(double[] values)
        {
            this.values = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                this.values[i] = values[i];
            }
        }

        public void setUpperLine(double ucl)
        {
            upperLine = ucl;
        }

        public void setLowerLine(double lcl)
        {
            lowerLine = lcl;
        }

        public void setCentralLine(double cl)
        {
            centralLine = cl;
        }

        public bool isInStateOfStaticalControl()
        {
            foreach (double value in this.values)
            {
                if ((value <= this.lowerLine) || (value >= this.upperLine))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isInSpecialState()
        {
            return true;
        }
    }
}
