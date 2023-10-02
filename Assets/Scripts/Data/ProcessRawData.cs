using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ProcessRawData : MonoBehaviour
{
    public TextAsset SourceRawData;
    public TextAsset TargetProcessedData;

    private string lastRawDataPath;
    private string targetProcessedDataPath;

    void Start()
    {
#if UNITY_EDITOR
        lastRawDataPath = Path.Combine(Application.dataPath, "LastRawDataVersion.txt");
        targetProcessedDataPath = UnityEditor.AssetDatabase.GetAssetPath(TargetProcessedData);
#else
        lastRawDataPath = Path.Combine(Application.persistentDataPath, "LastRawDataVersion.txt");
        targetProcessedDataPath = Path.Combine(
            Application.persistentDataPath,
            "TargetProcessedData.json"
        );
#endif

        string lastRawDataVersion = File.Exists(lastRawDataPath)
            ? File.ReadAllText(lastRawDataPath)
            : "";

        if (lastRawDataVersion != SourceRawData.text)
        {
            ProcessData(targetProcessedDataPath);
            File.WriteAllText(lastRawDataPath, SourceRawData.text); // Save the current raw data version
        }
    }

    void ProcessData(string targetFilePath)
    {
        List<RawDataModel> rawDataList = JsonUtility
            .FromJson<RawDataModelList>($"{{\"items\":{SourceRawData.text}}}")
            .items;

        List<RawDataModel> orderedList = rawDataList
            .OrderBy(item => item.domain)
            .ThenBy(item => item.cluster)
            .ThenBy(item => item.standardid)
            .ToList();

        string processedJson = JsonUtility.ToJson(
            new RawDataModelList { items = orderedList },
            true
        );
        File.WriteAllText(targetFilePath, processedJson);
    }
}
