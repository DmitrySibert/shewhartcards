using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShewhartCards.Utilities;

namespace ShewhartCards
{
    public class ShewhartCardsBuilder
    {
        private static double[] A1 = { 0, 0, 2.121, 1.732, 1.5, 1.342, 1.225, 1.134, 1.061, 1, 0.949, 0.905, 0.866, 0.832, 0.802, 0.775, 0.75, 0.728, 0.707, 0.688, 0.671, 0.655, 0.64, 0.626, 0.612, 0.6 };
        private static double[] A2 = { 0, 0, 1.88, 1.023, 0.729, 0.577, 0.483, 0.419, 0.373, 0.337, 0.308 };
        private static double[] D3 = { 0, 0, 0, 0, 0, 0, 0, 0.076, 0.136, 0.184, 0.223 };
        private static double[] D4 = { 3.267, 2.575, 2.282, 2.115, 2.004, 1.924, 1.864, 1.816, 1.777 };

        public enum AlternativeCardType
        {
            pCard,
            npCard,
            cCard,
            uCard
        }

        public enum QuantitativeCardType
        {
            xCard,
            rCard
        }

        private static ShewhartCard buildShewhartCard(double ucl, double lcl, double cl, double[] averageValues)
        {
            ShewhartCard shewhartCard = new ShewhartCard();
            shewhartCard.setValues(averageValues);
            shewhartCard.setCentralLine(cl);
            shewhartCard.setLowerLine(lcl);
            shewhartCard.setUpperLine(ucl);
            return shewhartCard;
        }

        public static IShewhartCard getAlternativeCard(AlternativeCardType type, SubgroupsData[] subgroupsData)
        {
            IShewhartCard shewhartCard = null;
            switch (type)
            {
                case AlternativeCardType.pCard:
                    shewhartCard = pCardEqualSubgroups(subgroupsData);
                    break;
                case AlternativeCardType.npCard:
                    shewhartCard = npCard(subgroupsData);
                    break;
                case AlternativeCardType.cCard:
                    shewhartCard = cCard(subgroupsData);
                    break;
                case AlternativeCardType.uCard:
                    shewhartCard = uCard(subgroupsData);
                    break;
            }
            return shewhartCard;
        }

        public static IShewhartCard getQuantitativeCard(QuantitativeCardType type, double[][] values)
        {
            IShewhartCard shewhartCard = null;
            switch (type)
            {
                case QuantitativeCardType.xCard:
                    shewhartCard = xCard(values);
                    break;
                case QuantitativeCardType.rCard:
                    shewhartCard = rCard(values);
                    break;
            }
            return shewhartCard;
        }


        /// <summary>
        /// "Доля дефектных единиц продукции".
        /// Метод строит контрольную карту Шухарта вида "p-карта" для подгрупп с одинаковым числом элементов
        /// Стандартные значения не заданы/
        /// </summary>
        /// <param name="subgroupsData">Массив подгрупп с колличеством элементов в каждой подгруппе, числом и процентом отклонений</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет 
        /// </returns>
        public static IShewhartCard pCardEqualSubgroups(SubgroupsData[] subgroupsData)
        {
            double p = 0; //Central line
            int allElementsNumber = 0;
            double[] values = new double[subgroupsData.Length];
            int i = 0;
            foreach (SubgroupsData elem in subgroupsData)
            {
                values[i] = elem.diversionProcent;
                p += elem.diversionQuantity;
                allElementsNumber += elem.elementsQuantity;
                i++;
            }
            p /= (double)allElementsNumber;
            double sigma = Math.Sqrt(p * (1 - p) / subgroupsData[0].elementsQuantity);
            double ucl = p + 3 * sigma;//upper line
            double lcl = p - 3 * sigma;//low line
            if (lcl < 0)
            {
                lcl = 0;
            }
            IShewhartCard shewhartsCard = buildShewhartCard(ucl, lcl, p, values);
            return shewhartsCard;
        }

        /// <summary>
        /// "Доля дефектных единиц продукции".
        /// Метод строит контрольную карту Шухарта вида "p-карта" для подгрупп с различным числом элементов
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="subgroupsData">Массив подгрупп с колличеством элементов в каждой подгруппе, числом и процентом отклонений</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет 
        /// </returns>
        public static IShewhartCard pCardNotEqualSubgroups(SubgroupsData[] subgroupsData)
        {
            double[] normalizePValues = new double[subgroupsData.Length]; //нормированные значения доли несоответствующих единиц
            double p = 0; //Central line
            int allElementsNumber = 0;
            foreach (SubgroupsData elem in subgroupsData)
            {
                p += elem.diversionQuantity;
                allElementsNumber += elem.elementsQuantity;
            }
            p /= (double)allElementsNumber;
            double sigma;
            for (int i = 0; i < subgroupsData.Length; i++)
            {
                sigma = Math.Sqrt(p * (1 - p) / subgroupsData[i].elementsQuantity);
                normalizePValues[i] = (subgroupsData[i].diversionProcent - p) / sigma;
            }
            double cl = 0;
            double ulc = 3;//upper line
            double lcl = -3;//low line
            IShewhartCard shewhartCard = buildShewhartCard(ulc, lcl, cl, normalizePValues);
            return shewhartCard;
        }

        /// <summary>
        /// "Число дефектных единиц продукции".
        /// Метод строит контрольную карту Шухарта вида "np-карта" для подгрупп с одинаковым числом элементов.
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="subgroupsData">Массив подгрупп с колличеством элементов в каждой подгруппе, числом и процентом отклонений</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard npCard(SubgroupsData[] subgroupsData)
        {
            double p = 0;
            double np = 0; //Central line
            int allElementsNumber = 0;
            int allSubgroupsNumber = 0;
            foreach (SubgroupsData elem in subgroupsData)
            {
                p += elem.diversionQuantity;
                allElementsNumber += elem.elementsQuantity;
                allSubgroupsNumber++;
            }
            np = p;
            p /= (double)allElementsNumber;
            np /= (double)allSubgroupsNumber;
            double sigma = Math.Sqrt(np * (1 - p));
            double ulc = np + 3 * sigma;//upper line
            double lcl = np - 3 * sigma;//low line
            double[] quantityOfRejects = new double[subgroupsData.Length];
            for (int i = 0; i < quantityOfRejects.Length; i++)
            {
                quantityOfRejects[i] = subgroupsData[i].diversionQuantity;
            }
            IShewhartCard shewhartCard = buildShewhartCard(ulc, lcl, np, quantityOfRejects);
            return shewhartCard;
        }

        /// <summary>
        /// "Число дефектов".
        /// Метод строит контрольную карту Шухарта вида "c-карта" для подгрупп с одинаковым числом элементов.
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="subgroupsData">Массив подгрупп с колличеством элементов в каждой подгруппе, числом и процентом отклонений</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard cCard(SubgroupsData[] subgroupsData)
        {
            double c = 0; //центральная линия
            foreach (SubgroupsData elem in subgroupsData)
            {
                c += elem.diversionQuantity;
            }
            c /= subgroupsData.Length;
            double sigma = Math.Sqrt(c);
            double ucl = c + 3 * sigma;//upper line
            double lcl = c - 3 * sigma;//low line
            double[] quantityOfRejects = new double[subgroupsData.Length];
            for (int i = 0; i < quantityOfRejects.Length; i++)
            {
                quantityOfRejects[i] = subgroupsData[i].diversionQuantity;
            }
            IShewhartCard shewhartCard = buildShewhartCard(ucl, lcl, c, quantityOfRejects);
            return shewhartCard;
        }

        /// <summary>
        /// "Доля дефектов на единицу продукции".
        /// Метод строит контрольную карту Шухарта вида "u-карта" для подгрупп с одинаковым числом элементов.
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="subgroupsData">Массив подгрупп с колличеством элементов в каждой подгруппе, числом и процентом отклонений</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard uCard(SubgroupsData[] subgroupsData)
        {
            double u = 0; //центральная линия
            foreach (SubgroupsData elem in subgroupsData)
            {
                u += elem.diversionQuantity;
            }
            u /= subgroupsData[0].elementsQuantity * subgroupsData.Length;
            double sigma = Math.Sqrt(u / subgroupsData[0].elementsQuantity);
            double ucl = u + 3 * sigma;//upper line
            double lcl = u - 3 * sigma;//low line
            double[] procentOfRejects = new double[subgroupsData.Length];
            for (int i = 0; i < procentOfRejects.Length; i++)
            {
                procentOfRejects[i] = subgroupsData[i].diversionProcent;
            }
            IShewhartCard shewhartCard = buildShewhartCard(ucl, lcl, u, procentOfRejects);
            return shewhartCard;
        }

        private static double calculateR(double[] values)
        {
            double max, min;
            max = 0;
            min = values[0];
            foreach (double x in values)
            {
                if (x > max)
                {
                    max = x;
                }
                if (x < min)
                {
                    min = x;
                }
            }
            return max - min;
        }

        /// <summary>
        /// "Карта среднего"
        /// Метод строит карту Шухарта вида "X-карта" значений для подгрупп с одинаковым числом элементов.
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="groupValues">Массив подгрупп со значениями для каждой подгруппы</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard xCard(double[][] groupValues)
        {
            double x = 0; //центральная линия
            double r = 0; //среднее значение размаха для всех подгрупп
            double[] averageValues = new double[groupValues.Length];
            for (int i = 0; i < groupValues.Length; i++)
            {
                double average = 0; //среднее значение для подгруппы
                for (int j = 0; j < groupValues[i].Length; j++)
                {
                    average += groupValues[i][j];
                }
                r += calculateR(groupValues[i]);
                averageValues[i] = average / groupValues[i].Length;
                x += averageValues[i];
            }
            r /= groupValues.Length;
            x /= groupValues.Length;
            double ucl = x + A2[groupValues[0].Length] * r;
            double lcl = x - A2[groupValues[0].Length] * r;
            return buildShewhartCard(ucl, lcl, x, averageValues);
        }

        /// <summary>
        /// "Карта среднего"
        /// Метод строит карту Шухарта вида "X-карта" значений для подгрупп с одинаковым числом элементов.
        /// Стандартные значения заданы.
        /// </summary>
        /// <param name="groupValues">Массив подгрупп со значениями для каждой подгруппы</param>
        /// <param name="x">Стандартное среднее значение</param>
        /// <param name="sigma">Стандартное отклонение</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard xCard(double[][] groupValues, double x, double sigma)
        {
            double[] averageValues = new double[groupValues.Length];
            for (int i = 0; i < groupValues.Length; i++)
            {
                double average = 0; //среднее значение для подгруппы
                for (int j = 0; j < groupValues[i].Length; j++)
                {
                    average += groupValues[i][j];
                }
                averageValues[i] = average / groupValues[i].Length;
                x += averageValues[i];
            }
            double ucl = x + A1[groupValues[0].Length] * sigma;
            double lcl = x - A1[groupValues[0].Length] * sigma;
            return buildShewhartCard(ucl, lcl, x, averageValues);
        }

        /// <summary>
        /// "Карта размахов"
        /// Метод строит карту Шухарта вида "R-карта" для подгрупп с одинаковым числом элементов.
        /// Стандартные значения не заданы.
        /// </summary>
        /// <param name="groupValues">Массив подгрупп со значениями для каждой подгруппы</param>
        /// <returns>true - если процесс находится в управляемом состоянии
        ///          false - если нет
        /// </returns>
        public static IShewhartCard rCard(double[][] groupValues)
        {
            double r = 0; //среднее значение размаха для всех подгрупп
            double[] averageValues = new double[groupValues.Length];
            for (int i = 0; i < groupValues.Length; i++)
            {
                averageValues[i] = calculateR(groupValues[i]);
                r += averageValues[i];
            }
            r /= groupValues.Length;
            double ucl = D4[groupValues[0].Length - 2] * r;
            double lcl = D3[groupValues[0].Length - 2] * r;
            return buildShewhartCard(ucl, lcl, r, averageValues);
        }
    }
}
