using CsvHelper;
using LogAggregator;
using Sharprompt;
using System.Globalization;
using ClosedXML.Excel;

var path = Prompt.Input<string>("データ保管パスを入力してください。");

var location =
    path.Split("\\").Last() == "Local"
        ? Location.Local
        : Location.Remote;

using var workbook = new XLWorkbook();

AddDetail(workbook, path, location);

var summaries = AddSummaries(workbook, path, location);
AddMax(workbook, summaries);

workbook.SaveAs(Path.Combine(path, "summary.xlsx"));

static List<Summary> AddSummaries(XLWorkbook xlWorkbook, string path, Location location)
{
    var children = Directory
        .GetDirectories(path)
        .Select(x => new DirectoryInfo(x))
        .Where(x => x.Name.First() < 'z')
        .Select((directoryInfo, index) => (DirectoryInfo: directoryInfo, Index: index));

    var summaryWorksheet = xlWorkbook.Worksheets.Add("Summary");

    summaryWorksheet.Cell(1, 1).Value = "製品名";
    summaryWorksheet.Cell(1, 2).Value = "向き";
    summaryWorksheet.Cell(1, 3).Value = "最小値";
    summaryWorksheet.Cell(1, 4).Value = "平均値";
    summaryWorksheet.Cell(1, 5).Value = "中央値";
    summaryWorksheet.Cell(1, 6).Value = "最大値";

    var list = new List<Summary>();
    foreach (var child in children)
    {
        var index = child.Index;
        var directoryInfo = child.DirectoryInfo;
        var productName = directoryInfo.Name.Substring(0, directoryInfo.Name.LastIndexOf('_'));
        productName =
            productName.Substring(
                productName.LastIndexOf('_') + 1,
                productName.Length - productName.LastIndexOf('_') - 1);
        var directionLabel = directoryInfo.Name.Substring(directoryInfo.Name.LastIndexOf('_') + 1, 1);
        var direction = directionLabel switch
        {
            "F" => Direction.Front,
            "R" => Direction.Right,
            "L" => Direction.Left,
            "B" => Direction.Back,
            "S" => Direction.Speak,
            _ => throw new InvalidOperationException()
        };
        using var reader = new StreamReader(Path.Combine(directoryInfo.FullName, "summary.csv"));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var systemName =
            location == Location.Local
                ? Microphone.FindByProductName(productName).SystemName
                : "CABLE Output (VB-Audio Virtual Cable)";

        var csvSummary = csv.GetRecords<CsvSummary>().Single(x => x.Name.Contains(systemName));
        var summary = new Summary(
            productName,
            direction,
            csvSummary.Min,
            csvSummary.Avg,
            csvSummary.Median,
            csvSummary.Max);
        list.Add(summary);

        summaryWorksheet.Cell(index + 2, 1).Value = productName;
        summaryWorksheet.Cell(index + 2, 2).Value = direction.ToString();
        summaryWorksheet.Cell(index + 2, 3).Value = csvSummary.Min;
        summaryWorksheet.Cell(index + 2, 4).Value = csvSummary.Avg;
        summaryWorksheet.Cell(index + 2, 5).Value = csvSummary.Median;
        summaryWorksheet.Cell(index + 2, 6).Value = csvSummary.Max;
    }

    return list;
}

static void AddMax(XLWorkbook workbook1, List<Summary> summaries)
{
    var maxWorksheet = workbook1.Worksheets.Add("Max");
    maxWorksheet.Cell(1, 1).Value = "製品名";
    maxWorksheet.Cell(1, 2).Value = "向き";
    maxWorksheet.Cell(1, 3).Value = "最小値";
    maxWorksheet.Cell(1, 4).Value = "平均値";
    maxWorksheet.Cell(1, 5).Value = "中央値";
    maxWorksheet.Cell(1, 6).Value = "最大値";

    var productNames = summaries.Select(x => x.Name).Distinct();
    foreach (var item in productNames.Select((productName, index) => (ProductName: productName, Index: index)))
    {
        var maxAvg = summaries
            .Where(x => x.Name == item.ProductName)
            .Where(x => x.Direction is not Direction.Speak)
            .MaxBy(x => x.Avg)!;
        var index = item.Index;

        maxWorksheet.Cell(index + 2, 1).Value = maxAvg.Name;
        maxWorksheet.Cell(index + 2, 2).Value = maxAvg.Direction.ToString();
        maxWorksheet.Cell(index + 2, 3).Value = maxAvg.Min;
        maxWorksheet.Cell(index + 2, 4).Value = maxAvg.Avg;
        maxWorksheet.Cell(index + 2, 5).Value = maxAvg.Median;
        maxWorksheet.Cell(index + 2, 6).Value = maxAvg.Max;
    }
}

static void AddDetail(XLWorkbook workbook, string path, Location location)
{
    var children = Directory
        .GetDirectories(path)
        .Select(x => new DirectoryInfo(x))
        .Where(x => x.Name.First() < 'z')
        .Select((directoryInfo, index) => (DirectoryInfo: directoryInfo, Index: index));
    List<MicrophoneRecord> microphones = new();

    foreach (var child in children)
    {
        var directoryInfo = child.DirectoryInfo;
        var productName = directoryInfo.Name.Substring(0, directoryInfo.Name.LastIndexOf('_'));
        productName =
            productName.Substring(
                productName.LastIndexOf('_') + 1,
                productName.Length - productName.LastIndexOf('_') - 1);
        var directionLabel = directoryInfo.Name.Substring(directoryInfo.Name.LastIndexOf('_') + 1, 1);
        var direction = directionLabel switch
        {
            "F" => Direction.Front,
            "R" => Direction.Right,
            "L" => Direction.Left,
            "B" => Direction.Back,
            "S" => Direction.Speak,
            _ => throw new InvalidOperationException()
        };

        var systemName =
            location == Location.Local
                ? Microphone.FindByProductName(productName).SystemName
                : "CABLE Output (VB-Audio Virtual Cable)";

        using var reader = new StreamReader(Path.Combine(directoryInfo.FullName, "detail.csv"));
        string header = reader.ReadLine()!;
        string[] columns = header.Split(",");
        int columnIndex = 1;
        for (; columnIndex < columns.Length; columnIndex++)
        {
            if (columns[columnIndex].Contains(systemName))
            {
                break;
            }
        }

        MicrophoneRecord microphoneRecord = new(productName, direction);
        for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
        {
            string[] cells = line.Split(",");
            microphoneRecord.Decibels.Add(double.Parse(cells[columnIndex]));
        }
        microphones.Add(microphoneRecord);
    }

    var worksheet = workbook.Worksheets.Add("Detail");
    int maxRecordCount = microphones.Max(x => x.Decibels.Count);
    // ヘッダー出力
    worksheet.Cell(1, 1).Value = "No.";
    for (int i = 0; i < microphones.Count; i++)
    {
        var microphone = microphones[i];
        worksheet.Cell(1, i + 2).Value = $"{microphone.ProductName}_{microphone.Direction.ToString()}";
    }

    for (int rowCount = 0; rowCount < maxRecordCount; rowCount++)
    {
        worksheet.Cell(rowCount + 2, 1).Value = rowCount + 1;
        for (int columnCount = 0; columnCount < microphones.Count; columnCount++)
        {
            var microphone = microphones[columnCount];
            var value =
                rowCount < microphone.Decibels.Count
                    ? microphone.Decibels[rowCount]
                    : -84d;
            worksheet.Cell(rowCount + 2, columnCount + 2).Value = value;
        }
    }
}