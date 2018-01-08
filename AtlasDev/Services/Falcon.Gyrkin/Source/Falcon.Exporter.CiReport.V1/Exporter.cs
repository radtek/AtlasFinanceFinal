using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Atlas.Common.Attributes;
using DevExpress.Spreadsheet;
using Expressions;
using Falcon.Exporter.CiReport.Infrastructure.Attributes;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces;
using Falcon.Exporter.CiReport.Infrastructure.Interfaces.Models;
using Falcon.Exporter.CiReport.Infrastructure.Structures.Reports;
using Serilog;

namespace Falcon.Exporter.CiReport.V1
{
  public class Exporter : IExporter
  {
    private const int DefaultRowHeight = 60;
    private const string StartExpression = "{{";
    private const string EndExpression = "}}";
    public byte[] ExportCiReport(IExportCiReportModel exportCiReportModel, ILogger logger)
    {
      using (var workbook = new Workbook())
      {
        try
        {
          foreach (var worksheet in exportCiReportModel.Worksheets.OrderBy(c => c.Order))
          {
            var newWorksheet = workbook.Worksheets.Add(worksheet.Description.Replace(':', ' ')
              .Replace('\\', ' ')
              .Replace('/', ' ')
              .Replace('?', ' ')
              .Replace('[', '(')
              .Replace(']', ')'));

            newWorksheet.Cells[0, 0].Value = string.Format("Report extracted on {0} - For Period {1} - {2}",
              DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), exportCiReportModel.StartDate.ToString("dd MMM yyyy"),
              exportCiReportModel.EndDate.ToString("dd MMM yyyy"));

            if (worksheet.CiReports != null && worksheet.CiReports.Count > 0)
            {
              PopulateCiReportWorksheet(newWorksheet, worksheet.CiReports);
              if (worksheet.VarianceStatus != null)
              {
                PopulateVarianceWorksheet(newWorksheet, worksheet.VarianceStatus, worksheet.CiReports.Count + 3);
              }
            }

            if (worksheet.PossibleHandovers != null && worksheet.PossibleHandovers.Count > 0)
            {
              PopulatePossibleHandoverWorksheet(newWorksheet, worksheet.PossibleHandovers);
            }

            if (worksheet.CiReportScores != null && worksheet.CiReportScores.Count > 0)
            {
              PopulateCompuscanProductWorksheet(newWorksheet, worksheet.CiReportScores);
            }

            if (worksheet.BranchesLowMeans != null && worksheet.BranchesLowMeans.Count > 0)
            {
              PopulateLowMeanWorksheet(newWorksheet, worksheet.BranchesLowMeans);
            }

            if (worksheet.LastBranchSyncStatuses != null && worksheet.LastBranchSyncStatuses.Count > 0)
            {
              PopulateBranchSyncStatus(newWorksheet, worksheet.LastBranchSyncStatuses);
            }

            if (worksheet.CiReportBranchSummaries != null && worksheet.CiReportBranchSummaries.Count > 0)
            {
              PopulateBranchSummary(newWorksheet, worksheet.CiReportBranchSummaries);
            }
          }

          if (workbook.Worksheets.Count > 1)
            workbook.Worksheets.RemoveAt(0);

          workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[0];

          workbook.EndUpdate();
        }
        catch (Exception ex)
        {
          logger.Error(string.Format("CI REPORT EXPORTER (V1): {0} - {1}", ex.Message, ex.StackTrace));
        }
        return workbook.SaveDocument(DocumentFormat.Xlsx);
      }
    }

    private static void PopulateCiReportWorksheet(Worksheet worksheet,
      ICollection<Infrastructure.Structures.Reports.CiReport> reportSales)
    {
      PopulateWorksheet(worksheet, reportSales, 2);
    }

    private static void PopulateVarianceWorksheet(Worksheet worksheet,
      VarianceStatus varianceStatus, int startRowIndex = 0)
    {
      PopulateWorksheet(worksheet, new List<VarianceStatus> { varianceStatus }, startRowIndex, 3);
    }

    private static void PopulatePossibleHandoverWorksheet(Worksheet worksheet,
      ICollection<PossibleHandover> reportPossibleHandovers)
    {
      PopulateWorksheet(worksheet, reportPossibleHandovers, 27);
    }

    private static void PopulateCompuscanProductWorksheet(Worksheet worksheet,
      ICollection<CiReportScore> reportCompuscanProducts)
    {
      PopulateWorksheet(worksheet, reportCompuscanProducts, 15);
    }

    private static void PopulateLowMeanWorksheet(Worksheet worksheet,
      ICollection<CiReportLowMean> ciReportLowMeans)
    {
      PopulateWorksheet(worksheet, ciReportLowMeans, 0);
    }

    private static void PopulateBranchSyncStatus(Worksheet worksheet,
      ICollection<CiReportBranchSyncStatus> branchSyncStatuses)
    {
      PopulateWorksheet(worksheet, branchSyncStatuses, 0);
    }

    private static void PopulateBranchSummary(Worksheet worksheet,
      ICollection<CiReportBranchSummary> branchSummaries)
    {
      PopulateWorksheet(worksheet, branchSummaries, 0);
    }

    private static void PopulateWorksheet<T>(Worksheet worksheet, ICollection<T> data, int startingRowIndex,
      int startingColumnIndex = 0)
    {
      if (ShowHeader<T>())
      {
        ImportIntoWorksheet(worksheet, BuildHeader(data), startingRowIndex++, startingColumnIndex);
      }

      foreach (var row in data)
      {
        ImportIntoWorksheet(worksheet, BuildValueRow(row), startingRowIndex++, startingColumnIndex);
      }
    }

    private static void ImportIntoWorksheet(Worksheet worksheet, List<Cell> rowData, int rowIndex,
      int startingColumnIndex)
    {
      for (var colIndex = 0; colIndex < rowData.Count; colIndex++)
      {
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Value = rowData[colIndex].Value;
        if (rowData[colIndex].CenterAlign)
        {
          worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Alignment.Horizontal =
            SpreadsheetHorizontalAlignment.Center;
        }
        else if (rowData[colIndex].RightAlign)
        {
          worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Alignment.Horizontal =
            SpreadsheetHorizontalAlignment.Right;
        }
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].NumberFormat = rowData[colIndex].NumberFormat;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].ColumnWidth = rowData[colIndex].ColumnWidth;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Font.Color = rowData[colIndex].FontColor;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Font.Bold = rowData[colIndex].Bold;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Font.Italic = rowData[colIndex].Italic;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].FillColor = rowData[colIndex].BackgroundColor;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].RowHeight = rowData[colIndex].RowHeight;
        worksheet.Cells[rowIndex, colIndex + startingColumnIndex].Borders.SetAllBorders(rowData[colIndex].FontColor,
          rowData[colIndex].BorderLineStyle);
      }
    }

    private static dynamic GetValueFromProperty<T>(PropertyInfo property, T obj)
    {
      var tempValue = property.GetValue(obj, null);
      if (tempValue == null)
        return null;

      if (property.PropertyType == typeof(decimal))
      {
        return (float)tempValue;
      }
      return tempValue;
    }

    private static dynamic GetDisplayFromProperty<T>(PropertyInfo property, T obj)
    {
      var tempValue = property.GetValue(obj, null);
      var value = tempValue == null ? string.Empty : tempValue.ToString();

      var formatAttributes = (FormatAttribute[])property.GetCustomAttributes(typeof(FormatAttribute), false);
      if (formatAttributes.Length <= 0)
        return value;

      if (property.PropertyType == typeof(int))
      {
        value = int.Parse(value).ToString(formatAttributes[0].Format);
      }
      else if (property.PropertyType == typeof(long))
      {
        value = long.Parse(value).ToString(formatAttributes[0].Format);
      }
      else if (property.PropertyType == typeof(DateTime))
      {
        value = DateTime.Parse(value).ToString(formatAttributes[0].Format);
      }
      else if (property.PropertyType == typeof(float))
      {
        value = float.Parse(value).ToString(formatAttributes[0].Format);
      }
      else if (property.PropertyType == typeof(decimal))
      {
        value = decimal.Parse(value).ToString(formatAttributes[0].Format);
      }

      return value;
    }

    private static object GetValueFromPropertyWithoutFormatting<T>(PropertyInfo property, T obj)
    {
      return property.GetValue(obj, null);
    }

    private static List<Cell> BuildValueRow<T>(T row)
    {
      var rowHeight = DefaultRowHeight;
      var defaultRowHeightAttributes =
        (RowHeightAttribute[]) (typeof (T)).GetCustomAttributes(typeof (RowHeightAttribute), false);
      if (defaultRowHeightAttributes.Length > 0)
      {
        rowHeight = defaultRowHeightAttributes[0].RowHeight;
      }

      var rows = new List<Cell>();
      var properties = typeof (T).GetProperties();

      foreach (var property in properties)
      {
        var orderAttributes = (OrderAttribute[]) property.GetCustomAttributes(typeof (OrderAttribute), false);
        if (orderAttributes.Length <= 0)
          continue;

        var cell = new Cell
        {
          Order = orderAttributes[0].Order,
          Value = GetValueFromProperty(property, row)
        };

        var detailFormatAttributes =
          (DetailFormatAttribute[]) property.GetCustomAttributes(typeof (DetailFormatAttribute), false);
        if (detailFormatAttributes.Length > 0)
        {
          cell.BackgroundColor = detailFormatAttributes[0].BackgroundColor;
          cell.Bold = detailFormatAttributes[0].Bold;
          cell.FontColor = detailFormatAttributes[0].FontColor;
          cell.Italic = detailFormatAttributes[0].Italic;
          cell.RightAlign = detailFormatAttributes[0].RightAlign;
          cell.CenterAlign = detailFormatAttributes[0].Center;
        }

        var numberFormatAttribute = (FormatAttribute[]) property.GetCustomAttributes(typeof (FormatAttribute), false);
        if (numberFormatAttribute.Length > 0)
        {
          cell.NumberFormat = numberFormatAttribute[0].Format;
        }

        var conditionalBackgroundColorAttributes =
          (ConditionalBackgroundColorAttribute[])
            property.GetCustomAttributes(typeof (ConditionalBackgroundColorAttribute), false);
        if (conditionalBackgroundColorAttributes.Length > 0)
        {
          foreach (var conditionalBackgroundColorAttribute in conditionalBackgroundColorAttributes)
          {
            var condition = conditionalBackgroundColorAttribute.Condition;
            foreach (var p in properties)
            {
              condition = condition.Replace(string.Format("{0}{1}{2}", StartExpression, p.Name, EndExpression),
                string.Format("{0}", GetValueFromPropertyWithoutFormatting(p, row)));
            }
            try
            {
              var expression = new DynamicExpression(condition, ExpressionLanguage.Csharp);
              if (Convert.ToBoolean(expression.Invoke()))
              {
                cell.BackgroundColor = conditionalBackgroundColorAttribute.BackgroundColor;
                break;
              }
            }
            catch (Exception)
            {
              // ignored
            }
          }
        }

        var columnWidthAttributes =
          (ColumnWidthAttribute[]) property.GetCustomAttributes(typeof (ColumnWidthAttribute), false);
        if (columnWidthAttributes.Length > 0)
        {
          cell.ColumnWidth = columnWidthAttributes[0].Width;
        }

        var rowHeightAttributes =
          (RowHeightAttribute[]) property.GetCustomAttributes(typeof (RowHeightAttribute), false);
        cell.RowHeight = rowHeightAttributes.Length > 0 ? rowHeightAttributes[0].RowHeight : rowHeight;

        rows.Add(cell);
      }

      return rows.OrderBy(t => t.Order).ToList();
    }

    private static bool ShowHeader<T>()
    {
      var orderAttributes = (NoHeaderAttribute[])typeof(T).GetCustomAttributes(typeof(NoHeaderAttribute), false);
      return orderAttributes.Length == 0;
    }

    private static List<Cell> BuildHeader<T>(ICollection<T> data)
    {
      var headers = new List<Cell>();

      foreach (var property in typeof(T).GetProperties())
      {
        var orderAttributes = (OrderAttribute[])property.GetCustomAttributes(typeof(OrderAttribute), false);
        if (orderAttributes.Length <= 0)
          continue;

        var cell = new Cell
        {
          Order = orderAttributes[0].Order,
        };

        var descriptionAttributes =
          (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descriptionAttributes.Length > 0)
        {
          cell.Value = GetHeaderTextByExpression(descriptionAttributes[0].Description, data);
        }

        var headerFormatAttributes =
          (HeaderFormatAttribute[])property.GetCustomAttributes(typeof(HeaderFormatAttribute), false);
        if (headerFormatAttributes.Length > 0)
        {
          cell.BackgroundColor = headerFormatAttributes[0].BackgroundColor;
          cell.Bold = headerFormatAttributes[0].Bold;
          cell.FontColor = headerFormatAttributes[0].FontColor;
          cell.Italic = headerFormatAttributes[0].Italic;
        }

        var columnWidthAttributes =
          (ColumnWidthAttribute[])property.GetCustomAttributes(typeof(ColumnWidthAttribute), false);
        if (columnWidthAttributes.Length > 0)
        {
          cell.ColumnWidth = columnWidthAttributes[0].Width;
        }

        var detailFormatAttributes =
          (DetailFormatAttribute[])property.GetCustomAttributes(typeof(DetailFormatAttribute), false);
        if (detailFormatAttributes.Length > 0)
        {
          cell.RightAlign = detailFormatAttributes[0].RightAlign;
          cell.CenterAlign = detailFormatAttributes[0].Center;
        }

        headers.Add(cell);
      }

      return headers.ToList();
    }

    private static Tuple<string, string> GetHeaderExpression(string headerText)
    {
      if (headerText.Contains(StartExpression) && headerText.Contains(EndExpression))
      {
        var expressionText =
          headerText.Substring(headerText.IndexOf(StartExpression, StringComparison.Ordinal) + StartExpression.Length,
            headerText.IndexOf(EndExpression, StringComparison.Ordinal) -
            (headerText.IndexOf(StartExpression, StringComparison.Ordinal) + StartExpression.Length));
        return new Tuple<string, string>(string.Format("{0}{1}{2}", StartExpression, expressionText, EndExpression),
          expressionText.Trim());
      }
      return new Tuple<string, string>(string.Empty, string.Empty);
    }

    private static string GetHeaderTextByExpression<T>(string headerText, ICollection<T> data)
    {
      if (data.FirstOrDefault() == null)
        return headerText;
      var expressionText = GetHeaderExpression(headerText);
      if (!string.IsNullOrWhiteSpace(expressionText.Item1))
      {
        var property = typeof(T).GetProperties().FirstOrDefault(p => expressionText.Item2 == p.Name);
        if (property != null)
        {
          headerText = headerText.Replace(expressionText.Item1, GetDisplayFromProperty(property, data.FirstOrDefault()));
        }
      }
      return headerText;
    }

    private class Cell
    {
      public int Order;
      public int RowHeight = 60;
      public int ColumnWidth = 300;
      public dynamic Value = string.Empty;
      public Color FontColor = Color.Black;
      public Color BackgroundColor = Color.White;
      public string NumberFormat = "";
      public bool Bold;
      public bool Italic;
      public bool CenterAlign;
      public bool RightAlign;
      public BorderLineStyle BorderLineStyle = BorderLineStyle.Thin;
    }
  }
}