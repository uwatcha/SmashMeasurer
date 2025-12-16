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
        // テックIDごとの出現回数を集計
        var techCounts = datas
            .GroupBy(d => d.techID)
            .OrderBy(g => g.Key)
            .ToDictionary(g => TechIdNameDict.Dict[g.Key], g => g.Count());
        Logger.LogElements("techCounts", techCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());
        
        // チャートをクリア
        techCountChart.RemoveData();

        // シリーズがなければ追加
        if (techCountChart.series.Count == 0)
        {
            techCountChart.AddSerie<Bar>("Tech");
        }

        // データを追加
        foreach (var tech in techCounts)
        {
            Logger.Log($"Adding to chart: {tech.Key} with count {tech.Value}");
            techCountChart.AddYAxisData(tech.Key);
            techCountChart.AddData(0, tech.Value);
        }

        // チャートを更新
        techCountChart.RefreshChart();
    }
}