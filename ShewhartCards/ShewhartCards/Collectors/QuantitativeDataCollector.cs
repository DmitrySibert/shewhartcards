using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ShewhartCards.Services;

namespace ShewhartCards.Collector
{
    class QuantitativeDataCollector
    {

        private string currentSelectionName;
        private long currentSelectionId;
        public int quantityInSubgroup
        {
            get;
            private set;
        }
        private ArrayList valuesBuffer;
        private DataStorageService dataStorageService;

        public QuantitativeDataCollector()
        {
            dataStorageService = new DataStorageService();
        }

        public void beginCollecting(string selectionName, int quantityInSubgroup)
        {
            currentSelectionName = selectionName;
            valuesBuffer = new ArrayList();
            this.quantityInSubgroup = quantityInSubgroup;
            currentSelectionId = dataStorageService.addSelection(selectionName);
        }

        public QuantitativeDataCollector setValue(double value)
        {
            valuesBuffer.Add(value);
            if (valuesBuffer.Count == quantityInSubgroup)
            {
                long currentSubgroupId = dataStorageService.addSubgroup(currentSelectionId, quantityInSubgroup);
                double[] values = (double[])valuesBuffer.ToArray(typeof(double));
                valuesBuffer.Clear();
                dataStorageService.addValues(currentSubgroupId, values);
            }
            return this;
        }

        public void clear(string selectionName)
        {
            dataStorageService.deleteSelection(selectionName);
        }

        public double[] getSelectionsValues()
        {
            return dataStorageService.getSelectionsValues(currentSelectionName);
        }

        public void removeValueAndItsGroup(string selectionName, double value)
        {
            dataStorageService.deleteValueAndItsGroups(selectionName, value);
        }

        public IShewhartCard getShewhartCard(ShewhartCardsBuilder.QuantitativeCardType cardType)
        {
            double[] values = getSelectionsValues();
            int groupQuantity = values.Length / quantityInSubgroup;
            double[][] groupsOfValues = new double[groupQuantity][];
            int valuesCounter = 0;
            for (int i = 0; i < groupQuantity; i++)
            {
                groupsOfValues[i] = new double[quantityInSubgroup];
                for (int j = 0; j < quantityInSubgroup; j++)
                {
                    groupsOfValues[i][j] = values[valuesCounter];
                    valuesCounter++;
                }
            }
            return ShewhartCardsBuilder.getQuantitativeCard(cardType, groupsOfValues);
        }
    }
}
