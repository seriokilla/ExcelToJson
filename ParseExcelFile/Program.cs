using System;
using System.IO;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace ParseExcelFile
{
    [Serializable]
    public class Config
    {
        public string FilePath;
        public int SheetIndex;
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
            var reader = new XmlSerializer(typeof(Config));
            var file = new StreamReader(confFilePath);
            var c = (Config)reader.Deserialize(file);
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

            var sheet = book.Worksheets[conf.SheetIndex];

            var range = sheet.UsedRange;

            for (var r = 1; r <= range.Rows.Count; r++)
            {
                for (var c = 1; c <= range.Columns.Count; c++)
                {
                    var val = range.Cells[r, c].Value;
                    var msg = (val == null) ? "null" : val.ToString();
                    Console.WriteLine(msg);
                }
            }

            book.Close(true, null, null);
            app.Quit();
        }
    }
}
