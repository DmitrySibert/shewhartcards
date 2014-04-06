using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShewhartCards.Services;
using ShewhartCards.Utilities;

namespace ShewhartCards.Collector
{
    public class AlternativeDataCollector
    {
        private string currentSelectionName;
        private long currentSelectionId;

        private DataStorageService dataStorageService;

        public AlternativeDataCollector()
        {
            dataStorageService = new DataStorageService();
        }

        public void beginCollecting(string selectionName)
        {
            currentSelectionName = selectionName;
            currentSelectionId = dataStorageService.addSelection(selectionName);
        }

        public AlternativeDataCollector setValue(int quantityInSubgroup, int rejectQuantity)
        {
            long currentSubgroupId = dataStorageService.addSubgroup(currentSelectionId, quantityInSubgroup);
            double[] value = new double[1];
            value[0] = rejectQuantity;
            dataStorageService.addValues(currentSubgroupId, value);
            return this;
        }

        public int[][] getGroupsOfDeviations()
        {
            return dataStorageService.getSelectionsGroupsOfDeviations(currentSelectionName);
        }

        public void clear(string selectionName)
        {
            dataStorageService.deleteSelection(selectionName);
        }

        public IShewhartCard getShewhartCard(ShewhartCardsBuilder.AlternativeCardType cardType)
        {
            int[][] values = getGroupsOfDeviations();
            SubgroupsData[] groupsOfValues = new SubgroupsData[values.Length];
            for (int i = 0; i < groupsOfValues.Length; i++)
            {
                groupsOfValues[i] = new SubgroupsData(values[i][0], values[i][1]);
            }
            return ShewhartCardsBuilder.getAlternativeCard(cardType, groupsOfValues);
        }

    }
}