using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Excel = Microsoft.Office.Interop.Excel;

namespace ParseExcelFile
{
    [Serializable]
    public class Config
    {
        public string FilePath;
        public int SheetIndex;
        public string OutFilePath;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            if (VerifyArgs(args) == false) return;
            var c = ReadConfig(args[0]);
            ProcessExcel(c);
        }

        private static bool VerifyArgs(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("{0} <input config xml>",
                    AppDomain.CurrentDomain.FriendlyName);
                return false;
            }
            return true;
        }

        private static Config ReadConfig(string confFilePath)
        {
            var reader = new XmlSerializer(typeof (Config));
            var file = new StreamReader(confFilePath);
            var c = (Config) reader.Deserialize(file);
            file.Close();
            return c;
        }

        private static void ProcessExcel(Config conf)
        {
            var app = new Excel.Application();
            var book = app.Workbooks.Open(conf.FilePath,
                0, true, 5, "", "", true,
                Excel.XlPlatform.xlWindows, "\t", false,
                false, 0, true, 1, 0);
            Thread.Sleep(2000);
            var sheet = book.Worksheets[conf.SheetIndex];

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
                        rowdict.Add(headerlist[c-1], msg);
                    }
                    //Console.WriteLine(msg);
                }
                if (r <= 1) continue;

                WriteJsonToFile(rowdict, conf.OutFilePath);
            }

            app.Quit();
            Console.WriteLine("Done");
        }

        private static void WriteJsonToFile(Dictionary<string, string> rowdict, string outfilepath)
        {
            var json = JsonConvert.SerializeObject(rowdict, Formatting.Indented) + Environment.NewLine;
            Console.Write(".");
            File.AppendAllText(outfilepath, json);
        }
    }
}
