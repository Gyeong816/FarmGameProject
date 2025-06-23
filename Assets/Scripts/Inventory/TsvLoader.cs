  using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Unity.Profiling;
using UnityEngine;

public static class TsvLoader
{
    private static readonly CsvConfiguration TsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        Mode = CsvMode.NoEscape,
        HasHeaderRecord = true,
        MissingFieldFound = null,
        HeaderValidated = null,
    };


    public static async Task<List<T>> LoadTableAsync<T>(string tableName)
    {
       
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Table");
        string filePath = Path.Combine(folderPath, tableName + ".tsv");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[TSVLoader] 파일이 존재하지 않습니다: {filePath}");
            return null;
        }

        using StreamReader reader = new StreamReader(filePath);
        using CsvReader csv = new CsvReader(reader, TsvConfig);

        var records = new List<T>();
        
        await foreach (var record in csv.GetRecordsAsync<T>())
        {
            records.Add(record);
        }
        return records;
    }
}