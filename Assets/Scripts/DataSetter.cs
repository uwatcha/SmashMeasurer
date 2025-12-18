using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class DataSetter : MonoBehaviour
{
    private BarChart techCountChart;
    void Start()
    {
        List<DataEntry> datas = new List<DataEntry>(InputDataManager.Instance.GetInputDataList());
        Logger.LogElements("datas", datas.ConvertAll(d => d.techID));
        techCountChart = GameObject.FindWithTag("TechCountChart").GetComponent<BarChart>();
        
        SetTechCountData(datas);
    }

    private void SetTechCountData(List<DataEntry> datas)
    {
        // テックIDごとの出現回数を集計（0件も含む）
        var groupedCounts = datas
            .GroupBy(d => d.techID)
            .ToDictionary(g => g.Key, g => g.Count());

        var allTechCounts = TechIdNameDict.Dict
            .OrderByDescending(kv => kv.Key)
            .ToDictionary(kv => kv.Value, kv => groupedCounts.TryGetValue(kv.Key, out var count) ? count : 0);

        Logger.LogElements("techCounts", allTechCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        techCountChart.RemoveData();

        if (techCountChart.series.Count == 0)
        {
            techCountChart.AddSerie<Bar>("Tech");
        }

        foreach (var tech in allTechCounts)
        {
            Logger.Log($"Adding to chart: {tech.Key} with count {tech.Value}");
            techCountChart.AddYAxisData(tech.Key);
            techCountChart.AddData(0, tech.Value);
        }

        techCountChart.RefreshChart();
    }
}