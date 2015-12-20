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
        static Config _config;
        public static void Main(string[] args)
        {
            if (VerifyArgs(args) == false) return;
            ReadConfig(args[0]);
            ProcessExcel();
        }

        private static void writeconfig()
        {
            var b = new Config()
            {
                FilePath = "mom.xlsx",
                SheetIndex = 3
            };
            var writer = new XmlSerializer(typeof(Config));
            var wfile = new StreamWriter(@"config.xml");
            writer.Serialize(wfile, b);
            wfile.Close();
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

        private static void ReadConfig(string confFilePath)
        {
            var reader = new XmlSerializer(typeof(Config));
            var file = new StreamReader(confFilePath);
            _config = (Config)reader.Deserialize(file);
            file.Close();
        }

        private static void ProcessExcel()
        {
            var app = new Excel.Application();
            var book = app.Workbooks.Open(_config.FilePath,
                    0, true, 5, "", "", true, 
                    Excel.XlPlatform.xlWindows, "\t", false, 
                    false, 0, true, 1, 0);

            //var sheet = (Excel.Worksheet)book.Worksheets.get_Item(_config.SheetIndex);
            var sheet = book.Worksheets[_config.SheetIndex];

            var range = sheet.UsedRange;

            for (var r = 1; r <= range.Rows.Count; r++)
            {
                for (var c = 1; c <= range.Columns.Count; c++)
                {
                    var val = range.Cells[r, c].Value2;
                    var msg = (val == null) ? "null" : val.ToString();
                    Console.WriteLine(msg);
                }
            }

            book.Close(true, null, null);
            app.Quit();
        }
    }
}
