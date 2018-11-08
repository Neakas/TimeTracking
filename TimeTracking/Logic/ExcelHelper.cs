using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TimeTracking.Database;

namespace TimeTracking.Logic
{
    internal static class ExcelHelper
    {
        internal static void ExportTimeEntries(DateTime start, DateTime end, ExcelExportOptions options)
            => ExportTimeEntriesInternal(start, end, options);

        internal static void ExportTimeEntries(int month, int year, ExcelExportOptions options)
            => ExportTimeEntriesInternal(new DateTime(year, month, 1), new DateTime(year, month, DateTime.DaysInMonth(year, month)), options);

        internal static void ExportTimeEntries(int year, ExcelExportOptions options)
            => ExportTimeEntriesInternal(new DateTime(year, 1, 1), new DateTime(year, 12, 31), options);

        private static void ExportTimeEntriesInternal(DateTime start, DateTime end, ExcelExportOptions options)
        {
            #region Check ExcelExportOptions
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.SaveTo == null)
                throw new ArgumentNullException(nameof(options.SaveTo));
            #endregion CheckExcelExportOptions

            var fi = options.SaveTo;
            if (fi.Exists)
            {
                fi.Delete();
                fi.Refresh();
            }

            using (var excel = new ExcelPackage(fi))
            {
                // Row-Indicator
                int i = 1;

                #region ExcelWorksheet ws
                string title = $"Zeiten {start:dd.MM.yyyy} bis {end:dd.MM.yyyy}";
                var ws = excel.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                    ws = excel.Workbook.Worksheets.Add(title);
                else
                    ws.Name = title;
                #endregion ExcelWorksheet ws

                using (var context = new MSUtilityDBEntities())
                {
                    #region SQL - Load
                    const string sql = @"
                        SELECT D.[Day], K.Kunde, P.Project, ED.Content
                        FROM TTDates D
                        JOIN TTKundeEntry KE ON D.ID = KE.DateID
                        JOIN TTKunde K ON KE.KundeID = K.Id
                        JOIN TTProjectEntry PE ON KE.ID = PE.KundeEntryId
                        JOIN TTProject P ON PE.ProjectId = P.Id
                        JOIN TTEntryData ED ON PE.Id = ED.ProjectEntryId
                        WHERE D.[Day] >= @date1 AND D.[Day] <= @date2
                        ORDER BY 1, 2, 3, 4";

                    bool closeConn = false;

                    var conn = context.Database.Connection as SqlConnection;
                    if (conn == null)
                        throw new NullReferenceException("context.Database.Connection as SqlConnection == null");
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                        closeConn = true;
                    }

                    #endregion SQL - Load

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("date1", start);
                        cmd.Parameters.AddWithValue("date2", end);

                        // Header schreiben
                        ws.Cells[$"A{i}"].Value = "Datum";
                        ws.Cells[$"B{i}"].Value = "Kunde";
                        ws.Cells[$"C{i}"].Value = "Projekt";
                        ws.Cells[$"D{i}"].Value = "Erfassung";

                        // Rows schreiben
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                            {
                                i++;
                                ws.Cells[$"A{i}"].Value = $"{r.GetValue(0):dd.MM.yyyy}";
                                ws.Cells[$"B{i}"].Value = r.GetValue(1);
                                ws.Cells[$"C{i}"].Value = r.GetValue(2);
                                ws.Cells[$"D{i}"].Value = r.GetValue(3);
                            }
                    }

                    #region SQL - Unload
                    if (closeConn)
                        conn.Close();
                    #endregion SQL - Unload
                }

                #region Aufbereitungen / Formatierungen usw.
                // Titel-Zeile formatieren
                using (var headerRange = ws.Cells["A1:D1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(Color.White);
                }

                // Titel-Zeile fixieren
                ws.View.FreezePanes(2, 1);

                // AutoFilter für alle Spalten
                ws.Cells["A1:D1"].AutoFilter = true;

                // Columnsgrößen an Inhalt anpassen (bewusst zum Schluss!)
                ws.Cells.AutoFitColumns();
                #endregion Aufbereitungen / Formatierungen usw.

                excel.Save();
            }

            if (options.DirectOpen)
                Process.Start(fi.FullName);
        }
    }

    internal class ExcelExportOptions
    {
        internal bool DirectOpen { get; set; }
        internal FileInfo SaveTo { get; set; }

        internal static ExcelExportOptions Save(FileInfo saveTo) => new ExcelExportOptions { DirectOpen = false, SaveTo = saveTo };
        internal static ExcelExportOptions SaveAndOpen(FileInfo saveTo) => new ExcelExportOptions {DirectOpen = true, SaveTo = saveTo};
    }

    internal static class TestExport
    {
        internal static void ExportExcel()
        {
            var fi = new FileInfo(null /*ToDo*/);

            ExcelHelper.ExportTimeEntries(2018, 11, ExcelExportOptions.SaveAndOpen(fi));
        }
    }
}
