using CsvHelper;
using LogAggregator;
using Sharprompt;
using System.Globalization;
using ClosedXML.Excel;

var path = Prompt.Input<string>("データ保管パスを入力してください。");

var children = Directory
    .GetDirectories(path)
    .Select(x => new DirectoryInfo(x))
    .Where(x => x.Name.First() < 'z')
    .Select((directoryInfo, index) => (DirectoryInfo: directoryInfo, Index:index));

using var workbook = new XLWorkbook();
var summaryWorksheet = workbook.Worksheets.Add("Summary");

summaryWorksheet.Cell(1, 1).Value = "製品名";
summaryWorksheet.Cell(1, 2).Value = "向き";
summaryWorksheet.Cell(1, 3).Value = "最小値";
summaryWorksheet.Cell(1, 4).Value = "平均値";
summaryWorksheet.Cell(1, 5).Value = "中央値";
summaryWorksheet.Cell(1, 6).Value = "最大値";

var summaries = new List<Summary>();
foreach (var child in children)
{
    var index = child.Index;
    var directoryInfo = child.DirectoryInfo;
    var productName = directoryInfo.Name.Substring(0, directoryInfo.Name.LastIndexOf(' '));
    var directionLabel = directoryInfo.Name.Substring(directoryInfo.Name.LastIndexOf(' ') + 1, 1);
    var direction = directionLabel switch
    {
        "F" => Direction.Front,
        "R" => Direction.Right,
        "L" => Direction.Left,
        "B" => Direction.Back,
        _ => throw new InvalidOperationException()
    };
    using var reader = new StreamReader(Path.Combine(directoryInfo.FullName, "summary.csv"));
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    var csvSummary = csv.GetRecords<CsvSummary>().Single(x => x.Name == "CABLE Output (VB-Audio Virtual Cable)");
    var summary = new Summary()
    {
        Name = productName,
        Direction = direction,
        Min = csvSummary.Min,
        Avg = csvSummary.Avg,
        Median = csvSummary.Median,
        Max = csvSummary.Max
    };
    summaries.Add(summary);

    summaryWorksheet.Cell(index + 2, 1).Value = productName;
    summaryWorksheet.Cell(index + 2, 2).Value = direction;
    summaryWorksheet.Cell(index + 2, 3).Value = csvSummary.Min;
    summaryWorksheet.Cell(index + 2, 4).Value = csvSummary.Avg;
    summaryWorksheet.Cell(index + 2, 5).Value = csvSummary.Median;
    summaryWorksheet.Cell(index + 2, 6).Value = csvSummary.Max;

}

var maxWorksheet = workbook.Worksheets.Add("Max");
maxWorksheet.Cell(1, 1).Value = "製品名";
maxWorksheet.Cell(1, 2).Value = "向き";
maxWorksheet.Cell(1, 3).Value = "最小値";
maxWorksheet.Cell(1, 4).Value = "平均値";
maxWorksheet.Cell(1, 5).Value = "中央値";
maxWorksheet.Cell(1, 6).Value = "最大値";

var productNames = summaries.Select(x => x.Name).Distinct();
foreach (var item in productNames.Select((productName, index) => (ProductName:productName, Index:index)))
{
    var maxAvg = summaries
    .Where(x => x.Name == item.ProductName)
    .MaxBy(x => x.Avg)!;
    var index = item.Index;

    maxWorksheet.Cell(index + 2, 1).Value = maxAvg.Name;
    maxWorksheet.Cell(index + 2, 2).Value = maxAvg.Direction;
    maxWorksheet.Cell(index + 2, 3).Value = maxAvg.Min;
    maxWorksheet.Cell(index + 2, 4).Value = maxAvg.Avg;
    maxWorksheet.Cell(index + 2, 5).Value = maxAvg.Median;
    maxWorksheet.Cell(index + 2, 6).Value = maxAvg.Max;
}

workbook.SaveAs(Path.Combine(path, "summary.xlsx"));