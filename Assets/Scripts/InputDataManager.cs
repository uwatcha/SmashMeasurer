using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class InputDataManager : DontDestroySingleton<InputDataManager>
{
  private List<DataEntry> inputDataList = new List<DataEntry>();
  private string studentID = "";
  private int experimentTimesCount = 0;

  public string StudentID
  {
    set => studentID = value;
  }
  public int ExperimentTimesCount
  {
    set => experimentTimesCount = value;
  }

  public void AddData(TechIDs techID)
  {
    DataEntry entry = new DataEntry();
    entry.techID = techID;
    entry.inputSecond = Timer.Instance.GetElapsedTime();
    inputDataList.Add(entry);
    Logger.Log($"Added Data: TechID={entry.techID}, InputSecond={entry.inputSecond:F2}");
  }

  public ReadOnlyArray<DataEntry> GetInputDataList()
  {
    return new ReadOnlyArray<DataEntry>(inputDataList.ToArray());
  }

  public void RemoveLastData()
  {
    if (inputDataList.Count > 0)
    {
      inputDataList.RemoveAt(inputDataList.Count - 1);
    }
  }

  public void SaveData()
  {
    //TODO: 上級者データをビルド後にOSで追加できるようにする
    StringBuilder csv = new StringBuilder();

    // 1行目: メタデータ（studentID, experimentTimesCount）
    csv.AppendLine($"{studentID},{experimentTimesCount}");

    // 2行目以降: inputSecond, techID
    foreach (var data in inputDataList)
    {
      csv.AppendLine($"{data.inputSecond:F2},{data.techID}");
    }

    string fileName = $"{studentID}_{experimentTimesCount}times_{DateTime.Now:yyMMdd_HHmm}.csv";
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    File.WriteAllText(filePath, csv.ToString());

    Logger.Log($"Data saved to: {filePath}");
  }

  public void LogInputtedTechIDs()
  {
    Logger.LogElements($"Save datas", inputDataList.ConvertAll(data => data.techID));
  }
}