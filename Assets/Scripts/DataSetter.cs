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
        SetUserTechRateChart(datas);
        SetAdvancedTechRateChart();
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
    private void SetUserTechRateChart(List<DataEntry> datas)
    {
        // スタイルを保持しつつデータだけクリア
        techRateChart.ClearSerieData();

        // 追加先Serieを名前で取得（Inspectorで用意された"User"を使用）
        var userSerie = techRateChart.GetSerie("User");
        if (userSerie == null)
        {
            Logger.Log("Serie 'User' が見つかりません。PieChartのInspectorでPieシリーズ 'User' を作成してください。");
            return;
        }

        // カテゴリごとに集計
        var categoryCounts = datas
            .GroupBy(d => TechCategoryDict.Dict[d.techID])
            .ToDictionary(g => g.Key, g => g.Count());

        Logger.LogElements("categoryCounts (User)", categoryCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        // Attack, Shield, Dodgeの順にデータを追加
        foreach (Categories category in System.Enum.GetValues(typeof(Categories)))
        {
            int count = categoryCounts.TryGetValue(category, out var value) ? value : 0;
            var label = CategoryNameDict.Dict.TryGetValue(category, out var name) ? name : category.ToString();
            Logger.Log($"Adding to pie chart (User): {label} with count {count}");
            techRateChart.AddData(userSerie.index, count, label);
        }

        techRateChart.RefreshChart();
    }

    private void SetAdvancedTechRateChart()
    {
        // ストックごとのフォルダから CSV を読み込み
        var twoStocksEntries = LoadAdvancedCsvEntriesFromFolder("2stocks");
        var threeStocksEntries = LoadAdvancedCsvEntriesFromFolder("3stocks");

        var entriesPerFile = new List<List<DataEntry>>();
        entriesPerFile.AddRange(twoStocksEntries);
        entriesPerFile.AddRange(threeStocksEntries);

        if (entriesPerFile.Count == 0)
        {
            Logger.Log("No valid data found in CSV TextAssets for pie chart");
            return;
        }

        // 追加先Serieを取得（優先: 名前"Advanced"、無ければ index 1）
        var advancedSerie = techRateChart.GetSerie("Advanced") ?? techRateChart.GetSerie(1);
        if (advancedSerie == null)
        {
            Logger.Log("Serie 'Advanced' が見つかりません。PieChartのInspectorでPieシリーズ 'Advanced' を作成してください。");
            return;
        }

        // ファイルごとのカテゴリ集計を保持
        var fileCategoryCountsList = new List<Dictionary<Categories, int>>();
        foreach (var entries in entriesPerFile)
        {
            var fileCategoryCounts = entries
                .GroupBy(d => TechCategoryDict.Dict[d.techID])
                .ToDictionary(g => g.Key, g => g.Count());
            fileCategoryCountsList.Add(fileCategoryCounts);
        }

        if (fileCategoryCountsList.Count == 0)
        {
            Logger.Log("No valid data found in CSV files for pie chart");
            return;
        }

        // 全カテゴリについて、ファイル間での平均を計算
        var averageCategoryCounts = new Dictionary<Categories, float>();
        foreach (Categories category in System.Enum.GetValues(typeof(Categories)))
        {
            var counts = fileCategoryCountsList
                .Select(dict => dict.TryGetValue(category, out var count) ? count : 0)
                .ToList();
            var average = (float)counts.Average();
            averageCategoryCounts[category] = Mathf.Round(average);
        }

        Logger.Log($"Calculated category averages from {fileCategoryCountsList.Count} files");

        // Attack, Shield, Dodgeの順にデータを追加
        foreach (Categories category in System.Enum.GetValues(typeof(Categories)))
        {
            var avgValue = (int)averageCategoryCounts[category];
            var label = CategoryNameDict.Dict.TryGetValue(category, out var name) ? name : category.ToString();
            Logger.Log($"Adding to pie chart (Advanced): {label} with count {avgValue}");
            techRateChart.AddData(advancedSerie.index, avgValue, label);
        }

        techRateChart.RefreshChart();
    }
    private void SetAdvancedTechCountData()
    {
        // ストックごとのフォルダから CSV を読み込み
        var twoStocksEntries = LoadAdvancedCsvEntriesFromFolder("2stocks");
        var threeStocksEntries = LoadAdvancedCsvEntriesFromFolder("3stocks");

        if (twoStocksEntries.Count == 0 && threeStocksEntries.Count == 0)
        {
            Logger.Log("No valid data found in 2stocks or 3stocks folders");
            return;
        }

        // テックIDごとの合計を計算
        var allTechIds = TechIdNameDict.Dict.Keys.ToList();
        var twoStocksCounts = new Dictionary<TechIDs, float>();
        var threeStocksCounts = new Dictionary<TechIDs, float>();

        // 2stocks の合計を計算（float で精度を保つ）
        foreach (var techId in allTechIds)
        {
            float count = 0;
            foreach (var entries in twoStocksEntries)
            {
                count += entries.Count(e => e.techID == techId);
            }
            twoStocksCounts[techId] = count;
        }

        // 3stocks の合計を計算
        foreach (var techId in allTechIds)
        {
            float count = 0;
            foreach (var entries in threeStocksEntries)
            {
                count += entries.Count(e => e.techID == techId);
            }
            threeStocksCounts[techId] = count;
        }

        Logger.Log($"2stocks files: {twoStocksEntries.Count}, 3stocks files: {threeStocksEntries.Count}");
        Logger.LogElements("2stocks counts", twoStocksCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());
        Logger.LogElements("3stocks counts", threeStocksCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        // 3stocks を 2/3 倍して、2stocks と合計
        int totalFiles = twoStocksEntries.Count + threeStocksEntries.Count;
        var normalizedCounts = new Dictionary<TechIDs, float>();

        foreach (var techId in allTechIds)
        {
            float twoStocksCount = twoStocksCounts[techId];
            float threeStocksScaled = threeStocksCounts[techId] * 2f / 3f;
            float combined = twoStocksCount + threeStocksScaled;
            normalizedCounts[techId] = combined / totalFiles;
        }

        Logger.LogElements("normalized averages", normalizedCounts.Select(kv => $"{kv.Key}: {kv.Value}").ToList());

        // 追加先Serieを取得（優先: 名前"Advanced"、無ければ index 0）
        var advancedSerie = techCountChart.GetSerie("Advanced") ?? techCountChart.GetSerie(0);
        if (advancedSerie == null)
        {
            Logger.Log("Serie 'Advanced' (index 0) が見つかりません。InspectorでBarシリーズ 'Advanced' を作成してください。");
            return;
        }

        // 正規化された平均値をグラフに追加（四捨五入は後で）
        foreach (var kv in TechIdNameDict.Dict.OrderByDescending(kv => kv.Key))
        {
            var avgValue = normalizedCounts.TryGetValue(kv.Key, out var avg) ? avg : 0f;
            Logger.Log($"Adding to chart (Advanced): {kv.Value} = {avgValue}");
            techCountChart.AddData(advancedSerie.index, avgValue);
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
            // 小数第一位で四捨五入して float で追加
            techCountChart.AddData(serieIndex, (float)System.Math.Round((double)tech.Value, 1));
        }
    }

    /// <summary>
    /// persistentDataPath/AdvancedPlayerDatas/{folderName} 配下の CSV ファイルを読み込み、各ファイルの DataEntry リストを返す
    /// </summary>
    private List<List<DataEntry>> LoadAdvancedCsvEntriesFromFolder(string folderName)
    {
        var result = new List<List<DataEntry>>();
        var folderPath = Path.Combine(Application.persistentDataPath, "AdvancedPlayerDatas", folderName);

        if (!Directory.Exists(folderPath))
        {
            Logger.Log($"Folder not found: {folderPath}");
            return result;
        }

        var csvPaths = Directory.GetFiles(folderPath, "*.csv");
        if (csvPaths.Length == 0)
        {
            Logger.Log($"No CSV files found in {folderPath}");
            return result;
        }

        foreach (var csvPath in csvPaths)
        {
            try
            {
                var entries = new List<DataEntry>();
                var lines = File.ReadAllLines(csvPath);
                bool isFirstLine = true;
                foreach (var raw in lines)
                {
                    var line = raw.Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // 1行目はメタデータ（studentID, experimentTimesCount）なのでスキップ
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    if (line.StartsWith("inputSecond")) continue; // header（念のため）
                    var parts = line.Split(',');
                    if (parts.Length < 2) continue;

                    if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var sec))
                        continue;
                    var techStr = parts[1].Trim();
                    if (!System.Enum.TryParse<TechIDs>(techStr, out var techId))
                        continue;
                    entries.Add(new DataEntry { inputSecond = sec, techID = techId });
                }

                result.Add(entries);
                Logger.Log($"Loaded {Path.GetFileName(csvPath)} from {folderName}: {entries.Count} entries");
            }
            catch (System.Exception ex)
            {
                Logger.Log($"Failed to parse {Path.GetFileName(csvPath)}: {ex.Message}");
            }
        }

        return result;
    }
}