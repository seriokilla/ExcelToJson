using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ParseExcelFile.Test
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void DirectoryInfoTest()
        {
            var filepath = @"E:\Logs\iPlannerAlerts";
            var filename = "*.csv";

            var filespec = filepath + @"\" + filename;
            var path = Path.GetDirectoryName(filespec);
            Assert.AreEqual(path, filepath);

            var f = Path.GetFileName(filespec);
            Assert.AreEqual(f, filename);
        }

        [Test]
        public void TestFileExtension()
        {
            var filepath = @"c:\temp\file.txt";
            var f = Path.GetFileNameWithoutExtension(filepath);

        }
    }
}
