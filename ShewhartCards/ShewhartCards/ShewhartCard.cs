using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShewhartCards
{
    public interface IShewhartCard
    {
        double getUpperLine();
        double getLowerLine();
        int getGroupQuantity();
        double[] getValues();
        bool isInStateOfStaticalControl();
        bool isInSpecialState();
    }

    public class ShewhartCard : IShewhartCard
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

        private bool isTendencyOfSix() 
        {
            int countUp = 1, countDown = 1;
            double prev = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (prev > values[i])
                {
                    countUp++;
                }
                else
                {
                    countUp = 1;
                }
                if (prev < values[i])
                {
                    countDown++;
                }
                else
                {
                    countDown = 1;
                }
                if (countDown == 6 || countUp == 6)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isIncDecTendency()
        {
            bool isUp;
            int begin = 0;
            int count = 2;
            if (values.Length < 14)
            {
                return false;
            }
            begin = findIncDecTendencyBegin(0, out isUp);
            if (begin == 0)
            {
                return false;
            }
            for (int i = begin; i < values.Length - 1; i++)
            {
                if (values[i] > values[i + 1])
                {
                    if (isUp)
                    {
                        count++;
                        isUp = !isUp;
                    }
                    else
                    {
                        i = findIncDecTendencyBegin(i, out isUp);
                        if (i == 0)
                        {
                            return false;
                        }
                        count = 2;
                    }
                }
            }
            if (count == 14)
            {
                return true;
            }
            return false;
        }

        private int findIncDecTendencyBegin(int begin, out bool startTendency)
        {
            int result = 0;
            startTendency = false;
            for (int i = begin; i < values.Length - 1; i++)
            {
                if (values[i] > values[i + 1])
                {
                    startTendency = false;
                    result = i + 1;
                }
                else if (values[i] < values[i + 1])
                {
                    startTendency = true;
                    result = i + 1;
                }
            }
            return result; 
        }

        private bool isTwoValuesFromThreeinAZone()
        {
            double sigma = (upperLine - centralLine) / 3;
            double firstZoneATop = upperLine;
            double firstZoneABottom = upperLine - sigma;
            double secondZoneATop = lowerLine + sigma;
            double secondZoneABottom = lowerLine;
            for (int i = 0; i < values.Length - 2; i++)
            {
                if ((isInZone(firstZoneATop, firstZoneABottom, values[i]) && isInZone(firstZoneATop, firstZoneABottom, values[i + 1]) && !isInZone(firstZoneATop, firstZoneABottom, values[i + 2])) ||
                    (isInZone(firstZoneATop, firstZoneABottom, values[i]) && !isInZone(firstZoneATop, firstZoneABottom, values[i + 1]) && isInZone(firstZoneATop, firstZoneABottom, values[i + 2])) ||
                    (!isInZone(firstZoneATop, firstZoneABottom, values[i]) && isInZone(firstZoneATop, firstZoneABottom, values[i + 1]) && isInZone(firstZoneATop, firstZoneABottom, values[i + 2])))
                {
                    return true;
                }
                if ((isInZone(secondZoneATop, secondZoneABottom, values[i]) && isInZone(secondZoneATop, secondZoneABottom, values[i + 1]) && !isInZone(secondZoneATop, secondZoneABottom, values[i + 2])) ||
                    (isInZone(secondZoneATop, secondZoneABottom, values[i]) && !isInZone(secondZoneATop, secondZoneABottom, values[i + 1]) && isInZone(secondZoneATop, secondZoneABottom, values[i + 2])) ||
                    (!isInZone(secondZoneATop, secondZoneABottom, values[i]) && isInZone(secondZoneATop, secondZoneABottom, values[i + 1]) && isInZone(secondZoneATop, secondZoneABottom, values[i + 2])))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isFourValuesFromFiveInBZone()
        {
            double sigma = (upperLine - centralLine) / 3;
            double firstZoneBTop = upperLine - sigma;
            double firstZoneBBottom = upperLine - 2 * sigma;
            double secondZoneBTop = lowerLine + 2 * sigma;
            double secondZoneBBottom = lowerLine + sigma;
            for (int i = 0; i < values.Length - 4; i++)
            {
                if ((isInZone(firstZoneBTop, firstZoneBBottom, values[i]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 1]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 2]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 3]) && !isInZone(firstZoneBTop, firstZoneBBottom, values[i + 4])) ||
                    (isInZone(firstZoneBTop, firstZoneBBottom, values[i]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 1]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 2]) && !isInZone(firstZoneBTop, firstZoneBBottom, values[i + 3]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 4])) ||
                    (isInZone(firstZoneBTop, firstZoneBBottom, values[i]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 1]) && !isInZone(firstZoneBTop, firstZoneBBottom, values[i + 2]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 3]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 4])) ||
                    (isInZone(firstZoneBTop, firstZoneBBottom, values[i]) && !isInZone(firstZoneBTop, firstZoneBBottom, values[i + 1]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 2]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 3]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 4])) ||
                    (!isInZone(firstZoneBTop, firstZoneBBottom, values[i]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 1]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 2]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 3]) && isInZone(firstZoneBTop, firstZoneBBottom, values[i + 4])))
                {
                    return true;
                }
                if ((isInZone(secondZoneBTop, secondZoneBBottom, values[i]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 1]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 2]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 3]) && !isInZone(secondZoneBTop, secondZoneBBottom, values[i + 4])) ||
                    (isInZone(secondZoneBTop, secondZoneBBottom, values[i]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 1]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 2]) && !isInZone(secondZoneBTop, secondZoneBBottom, values[i + 3]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 4])) ||
                    (isInZone(secondZoneBTop, secondZoneBBottom, values[i]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 1]) && !isInZone(secondZoneBTop, secondZoneBBottom, values[i + 2]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 3]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 4])) ||
                    (isInZone(secondZoneBTop, secondZoneBBottom, values[i]) && !isInZone(secondZoneBTop, secondZoneBBottom, values[i + 1]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 2]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 3]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 4])) ||
                    (!isInZone(secondZoneBTop, secondZoneBBottom, values[i]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 1]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 2]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 3]) && isInZone(secondZoneBTop, secondZoneBBottom, values[i + 4])))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isFifteenValuesInCZone()
        {
            double sigma = (upperLine - centralLine) / 3;
            double zoneCTop = centralLine + sigma;
            double zoneCBottom = centralLine - sigma;
            int count = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (isInZone(zoneCTop, zoneCBottom, values[i]))
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == 15)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isEightValuesOutOfCZone()
        {
            double sigma = (upperLine - centralLine) / 3;
            double zoneCTop = centralLine + sigma;
            double zoneCBottom = centralLine - sigma;
            int count = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (!isInZone(zoneCTop, zoneCBottom, values[i]))
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == 8)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isInZone(double zoneTop, double zoneBottom, double value)
        {
            return value <= zoneTop && value >= zoneBottom;
        }

        private bool isNineValuesOnOneSide()
        {
            int aboveCount = 0;
            int belowCount = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] >= centralLine)
                {
                    aboveCount++;
                }
                else
                {
                    aboveCount = 0;
                }
                if (values[i] <= centralLine)
                {
                    belowCount++;
                }
                else
                {
                    belowCount = 0;
                }
                if (belowCount == 9 || aboveCount == 9)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
