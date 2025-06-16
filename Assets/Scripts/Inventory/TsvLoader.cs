using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
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

    /// <summary>
    /// StreamingAssets/Table 폴더에서 주어진 TSV 파일을 읽어 List<T>로 반환합니다. (동기 방식)
    /// </summary>
    public static List<T> LoadTable<T>(string tableName)
    {
        // 경로를 StreamingAssets로 고정
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Table");
        string filePath = Path.Combine(folderPath, tableName + ".tsv");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[TSVLoader] 파일이 존재하지 않습니다: {filePath}");
            return null;
        }

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, TsvConfig);

        return new List<T>(csv.GetRecords<T>());
    }
}