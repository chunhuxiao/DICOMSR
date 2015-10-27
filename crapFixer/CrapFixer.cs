using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapFixer
{
    public class CrapFixer
    {
                /// <summary>
        /// Formats path with "Ambulatorio" crap Report sorting way.
        /// Default path: \\UNC\xx-xxx\xxxxxxxx.doc
        /// </summary>
        /// <param name="fileName">Name of report to format path.</param>
        /// <returns></returns>
        public static string AmbulatorioReportPath(string UNC, string fileName) {
            string splittedFileName = fileName.Split('.')[0];
            string reportFolder = string.Format("{0}-{1}", splittedFileName.Substring(0, 2), splittedFileName.Substring(2, 3));
            string reportPath = Path.Combine(UNC, reportFolder, fileName);
            return reportPath;
        }
    }
}
