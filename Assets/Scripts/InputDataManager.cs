using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class InputDataManager : DontDestroySingleton<InputDataManager>
{
  private List<DataEntry> inputDataList = new List<DataEntry>();

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
    //TODO: ファイル出力できるようにする
    //TODO: 上級者データをビルド後にOSで追加できるようにする
    StringBuilder csv = new StringBuilder();

    foreach (var data in inputDataList)
    {
      csv.AppendLine($"{data.inputSecond:F2},{data.techID}");
    }

    string fileName = $"input_data_{DateTime.Now:yyMMdd_HHmm}.csv";
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    File.WriteAllText(filePath, csv.ToString());

    Logger.Log($"Data saved to: {filePath}");
  }

  public void LogInputtedTechIDs()
  {
    Logger.LogElements($"Save datas", inputDataList.ConvertAll(data => data.techID));
  }
}