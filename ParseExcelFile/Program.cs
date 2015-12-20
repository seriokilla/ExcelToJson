using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Excel = Microsoft.Office.Interop.Excel;

namespace ParseExcelFile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (VerifyArgs(args) == false) return;
            
            var files = GetFilesFromFilespec(args[0]);
            foreach (var f in files)
            {
                var outfile = GetOutputFilePath(f);
                ProcessExcel(f, outfile, Convert.ToInt32(args[1]));
            }

            Console.WriteLine("Done");
        }

        private static bool VerifyArgs(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("{0} <inputfilespec> <nonzerobasedsheetindex>",
                    AppDomain.CurrentDomain.FriendlyName);
                return false;
            }
            return true;
        }

        private static List<string> GetFilesFromFilespec(string filespec)
        {
            var dir = Path.GetDirectoryName(filespec);
            var spec = Path.GetFileName(filespec);

            if (dir == null) return new List<string>();

            var files = Directory.GetFiles(dir, spec);
            return new List<string>(files);
        }

        private static string GetOutputFilePath(string inputFilePath)
        {
            var path = Path.GetDirectoryName(inputFilePath);
            var file = Path.GetFileNameWithoutExtension(inputFilePath);
            return path + @"\" + file + @".json";
        }

        private static void ProcessExcel(string inputfilepath, string outputfilepath, int nonzeroBasedSheetIndex)
        {
            var app = new Excel.Application();
            var book = app.Workbooks.Open(inputfilepath,
                0, true, 5, "", "", true,
                Excel.XlPlatform.xlWindows, "\t", false,
                false, 0, true, 1, 0);
            Thread.Sleep(2000);
            var sheet = book.Worksheets[nonzeroBasedSheetIndex];

            var range = sheet.UsedRange;
            var headerlist = new List<string>();

            for (var r = 1; r <= range.Rows.Count; r++)
            {
                var rowdict = new Dictionary<string, string>();

                for (var c = 1; c <= range.Columns.Count; c++)
                {
                    var val = range.Cells[r, c].Value;
                    var msg = (val == null) ? "null" : val.ToString();

                    if (r == 1)
                    {
                        headerlist.Add(msg);
                    }
                    else
                    {
                        rowdict.Add(headerlist[c - 1], msg);
                    }
                    //Console.WriteLine(msg);
                }
                if (r <= 1) continue;

                WriteJsonToFile(rowdict, outputfilepath);
            }

            app.Quit();
        }

        private static void WriteJsonToFile(Dictionary<string, string> rowdict, string outfilepath)
        {
            var json = JsonConvert.SerializeObject(rowdict, Formatting.None) + Environment.NewLine;
            Console.Write(".");
            File.AppendAllText(outfilepath, json);
        }
    }
}
