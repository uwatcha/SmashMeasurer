using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class DataSetter : MonoBehaviour
{
    private BarChart techCountChart;
    private PieChart techRateChart;
    void Start()
    {
        List<DataEntry> datas = new List<DataEntry>(InputDataManager.Instance.GetInputDataList());
        Logger.LogElements("datas", datas.ConvertAll(d => d.techID));
        techCountChart = GameObject.FindWithTag("TechCountChart").GetComponent<BarChart>();
        techRateChart = GameObject.FindWithTag("TechRateChart").GetComponent<PieChart>();
        
        SetUserTechCountData(datas);
        SetAdvancedTechCountData();
        SetTechRateChart(datas);
    }

    private void SetUserTechCountData(List<DataEntry> datas)
    {
        // スタイルを保持しつつデータだけクリア
        techCountChart.ClearSerieData();
        techCountChart.ClearComponentData(); // 軸ラベル等のデータもクリア

        // 追加先Serieを名前で取得（Inspectorで用意された"User"を使用）
        var userSerie = techCountChart.GetSerie("User");
        if (userSerie == null)
        {
            Logger.Log("Serie 'User' が見つかりません。InspectorでBarシリーズ 'User' を作成してください。");
            return;
        }

        // shouldAddLabels=trueで初回ラベル追加実施
        AddTechCountDataToChart(datas, userSerie.index, "User", shouldAddLabels: true);
        techCountChart.RefreshChart();
    }
    private void SetTechRateChart(List<DataEntry> datas)
    {
        // カテゴリごとに集計
        var categoryCounts = datas
            .GroupBy(d => TechCategoryDict.Dict[d.techID])
            .ToDictionary(g => g.Key, g => g.Count());

        Logger.LogElements("categoryCounts", categoryCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        techRateChart.RemoveData();

        if (techRateChart.series.Count == 0)
        {
            techRateChart.AddSerie<Pie>("Category");
        }

        // Attack, Shield, Dodgeの順にデータを追加
        foreach (Categories category in System.Enum.GetValues(typeof(Categories)))
        {
            int count = categoryCounts.TryGetValue(category, out var value) ? value : 0;
            Logger.Log($"Adding to pie chart: {category} with count {count}");
            techRateChart.AddData(0, count, category.ToString());
        }

        techRateChart.RefreshChart();
    }
    private void SetAdvancedTechCountData()
    {
        var advancedDir = Path.Combine(Application.dataPath, "AdvancedPlayerDatas");
        if (!Directory.Exists(advancedDir))
        {
            Logger.Log($"Advanced data directory not found: {advancedDir}");
            return;
        }

        var csvPaths = Directory.GetFiles(advancedDir, "*.csv");
        if (csvPaths.Length == 0)
        {
            Logger.Log($"No CSV files found in {advancedDir}");
            return;
        }

        // ファイルごとのテックID集計を保持
        var fileCountsList = new List<Dictionary<TechIDs, int>>();

        foreach (var csvPath in csvPaths)
        {
            try
            {
                var entries = new List<DataEntry>();
                var lines = File.ReadAllLines(csvPath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("inputSecond")) continue; // header
                    var parts = line.Split(',');
                    if (parts.Length < 2) continue;

                    if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var sec))
                        continue;
                    var techStr = parts[1].Trim();
                    if (!System.Enum.TryParse<TechIDs>(techStr, out var techId))
                        continue;
                    entries.Add(new DataEntry { inputSecond = sec, techID = techId });
                }

                // このファイルのテックID集計
                var fileCounts = entries
                    .GroupBy(d => d.techID)
                    .ToDictionary(g => g.Key, g => g.Count());
                fileCountsList.Add(fileCounts);
                Logger.Log($"Loaded {csvPath}: {entries.Count} entries");
            }
            catch (System.Exception ex)
            {
                Logger.Log($"Failed to read {csvPath}: {ex.Message}");
            }
        }

        if (fileCountsList.Count == 0)
        {
            Logger.Log("No valid data found in CSV files");
            return;
        }

        // 全テックIDについて、ファイル間での平均を計算
        var allTechIds = TechIdNameDict.Dict.Keys.ToList();
        var averageCounts = new Dictionary<TechIDs, float>();

        foreach (var techId in allTechIds)
        {
            var counts = fileCountsList
                .Select(dict => dict.TryGetValue(techId, out var count) ? count : 0)
                .ToList();
            var average = (float)counts.Average();
            // 四捨五入して整数に
            averageCounts[techId] = Mathf.Round(average);
        }

        Logger.Log($"Calculated averages from {fileCountsList.Count} files");

        // 追加先Serieを取得（優先: 名前"Advanced"、無ければ index 0）
        var advancedSerie = techCountChart.GetSerie("Advanced") ?? techCountChart.GetSerie(0);
        if (advancedSerie == null)
        {
            Logger.Log("Serie 'Advanced' (index 0) が見つかりません。InspectorでBarシリーズ 'Advanced' を作成してください。");
            return;
        }

        // 平均値をTechIdNameDict の順序で直接グラフに追加（AddTechCountDataToChartをバイパス）
        foreach (var kv in TechIdNameDict.Dict.OrderByDescending(kv => kv.Key))
        {
            var avgValue = averageCounts.TryGetValue(kv.Key, out var avg) ? avg : 0;
            Logger.Log($"Adding to chart (Advanced): {kv.Value} = {avgValue}");
            techCountChart.AddData(advancedSerie.index, (int)avgValue);
        }

        techCountChart.RefreshChart();
    }

    /// <summary>
    /// 共通処理：DataEntry群をテックIDごとに集計し、指定されたSerieへ値を追加する
    /// </summary>
    private void AddTechCountDataToChart(List<DataEntry> datas, int serieIndex, string serieName, bool shouldAddLabels = true)
    {
        var groupedCounts = datas
            .GroupBy(d => d.techID)
            .ToDictionary(g => g.Key, g => g.Count());

        var allTechCounts = TechIdNameDict.Dict
            .OrderByDescending(kv => kv.Key)
            .ToDictionary(kv => kv.Value, kv => groupedCounts.TryGetValue(kv.Key, out var count) ? count : 0);

        Logger.LogElements($"techCounts ({serieName})", allTechCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        foreach (var tech in allTechCounts)
        {
            Logger.Log($"Adding to chart ({serieName}): {tech.Key} = {tech.Value}");
            if (shouldAddLabels)
            {
                techCountChart.AddYAxisData(tech.Key);
            }
            techCountChart.AddData(serieIndex, tech.Value);
        }
    }
}