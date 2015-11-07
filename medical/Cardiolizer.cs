using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cardiolizer {
    struct Config
    {
        public const string
            folderDir = "Cardiolizer",
            dataInDir = "DCM_IN",
            fileFilter = "SR*",
            fileOutDir = "DOC_OUT",
            fileOutName = "{0}.doc",
            templateDir = "ODT_TEMPLATE",
            contentFileName = "content.xml",
            tempDir = "ODT_TEMP",
            tempContentFileName = "content.xml",
            SRStartString = "<Report>";

        public const string[] contentFiles = { "content.xml", "styles.xml" };

        public static string
            fileOutPath = "",
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            dataPath = Path.Combine(appDataPath, folderDir),
            dataInPath = Path.Combine(dataPath, dataInDir),
            templatePath = Path.Combine(dataPath, templateDir),

            // Deprecate
            contentFilePath = Path.Combine(templatePath, contentFileName),
            // ---------

            tempPath = Path.Combine(dataPath, tempDir),
            tempFullPath = "", // Path.Combine(Config.tempPath, new FileInfo(sourceFiles[i]).Name);
            tempContentFilePath = ""; // Path.Combine(tempFullPath, tempContentFileName); // Path.Combine(tempPath, tempContentFileName);
        
        public static bool
            watch = false,
            erase = false;
    }

    /// <summary>
    /// Checks for input SR DCM files and creates DOC files with content of them.
    /// </summary>
    class Cardiolizer
    {
        static void WriteUsage()
        {
            Console.WriteLine("Usage: Cardiolizer -i <input_path> -o <output_path> <-w|--watch> <-e|--erase>");
        }
        
        static void Main(string[] args)
        {
            // string[] args = {"-i", @"C:\Users\Juan Pelotas\AppData\Roaming\Cardiolizer\DCM_IN", "-o", @"\\172.100.1.201\Ambulatorio$", "-w", "-e" };

            if (args.Length == 0)
            {
                WriteUsage();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-h" || args[i] == "--help")
                {
                    WriteUsage();
                    return;
                }

                if (args[i] == "-i" || args[i] == "--input")
                {
                    if (!string.IsNullOrWhiteSpace(args[i + 1]))
                    {
                        Config.dataInPath = args[i + 1];
                        i++;
                    }
                }


                if (args[i] == "-o" || args[i] == "--output")
                {
                    if (!string.IsNullOrWhiteSpace(args[i + 1]))
                    {
                        Config.fileOutPath = args[i + 1];
                        i++;
                    }
                }

                if (args[i] == "-w" || args[i] == "--watch")
                {
                    Config.watch = true;            
                }

                if (args[i] == "-e" || args[i] == "--erase")
                {
                    Config.erase = true;
                }
            }

            Console.WriteLine("App Data Path: \"{0}\"", Config.appDataPath);
            Console.WriteLine("Data Path: \"{0}\"", Config.dataPath);
            Console.WriteLine("Data In dir: \"{0}\"", Config.dataInDir);
            Console.WriteLine("Template dir: \"{0}\"", Config.templateDir);
            // Console.WriteLine("Content File Path: \"{0}\"", Config.contentFilePath);
            Console.WriteLine("Temp: \"{0}\"", Config.tempDir);

            foreach (string contentFileTemp in Config.contentFiles)
            {
                Console.WriteLine("Temp Content File: \"{0}\"", contentFileTemp);
            }
            
            Console.WriteLine("Out Path: \"{0}\"", Config.fileOutPath);

            string[] folderPaths = new string[] { Config.dataInPath, Config.templatePath, Config.tempPath };
            Utilities.Folders.CheckIfNotExistAndCreate(folderPaths);

            // First check for files already in the input folder
            string[] sourceFiles = Directory.GetFiles(Config.dataInPath, Config.fileFilter);
            for (int i = 0; i < sourceFiles.Length; i++)
            {
                Config.tempFullPath = Path.Combine(Config.tempPath, new DirectoryInfo(sourceFiles[i]).Name);
                Report report = new Report();
                Utilities.Folders.CheckIfNotExistAndCreate(Config.tempFullPath);
                Utilities.Folders.Copy(Config.templatePath, Config.tempFullPath, true);
                report.MakeReportFromSRDCM(sourceFiles[i]);
            }

            // Then start watching if applies
            if (Config.watch)
            {
                Watcher SRWatcher = new Watcher();
                SRWatcher.Watch(new DirectoryInfo(Config.dataInPath), Config.fileFilter, new DirectoryInfo(Config.fileOutPath));
                Console.WriteLine("Entering watch_mode. Press Q to quit.");
                while (Console.ReadKey().Key != ConsoleKey.Q) ;
            }

            return;
        }
    }

    class Report
    {
        public void MakeReportFromSRDCM(string inPath, string outPath, string[] files)
        {
            // **
            // Here I made a mistake. Cause I've mixed too many tasks on just one function,
            // what creates confusion.
            // Next step would be reordering task by task on its properly function.
            // 
            // Read SR DCM and make a list.
            // Replace values on XML with that list.
            // ZIP the files on a DOC file.
            // Delete them all.
            // **

            string content = StructuredReport.StructuredReport.Read(srdcm, Config.SRStartString);
            List<StructuredReport.Meassurements.item> meassurementsList = new List<StructuredReport.Meassurements.item>();
            meassurementsList = StructuredReport.StructuredReport.FillList(content);

            foreach (StructuredReport.Meassurements.item item in meassurementsList)
            {
                if (item.units != null)
                {
                    StructuredReport.Meassurements.Units.TryGetValue(item.units, out item.units);
                }
            }

            int accessionNumber = 0;
            foreach (string file in files)
            {

                string reportContentFilePath = Path.Combine(inPath, file);
                string reportTempContentFilePath = Path.Combine(outPath, file);

                Console.WriteLine("Replacing values on: {0}", file);
                int accessionNumberTemp =
                    StructuredReport.StructuredReport.ReplaceFieldsWithValues
                    (
                        meassurementsList,
                        reportContentFilePath,
                        reportTempContentFilePath
                    );

                // if (accessionNumber != 0) accessionNumber = accessionNumberTemp;
                if (accessionNumberTemp > 0 && accessionNumberTemp != null) accessionNumber = accessionNumberTemp;
            }

            FileInfo fileOut = new FileInfo(
                CrapFixer.CrapFixer.AmbulatorioReportPath(Config.fileOutPath, accessionNumber + ".doc"));
            if (fileOut.Exists) { fileOut.Delete(); }

            Console.WriteLine("Creating report: {0}", fileOut.FullName);
            Utilities.Zippit.MakeZip(Config.tempFullPath, fileOut.FullName);

            Directory.Delete(Config.tempFullPath, true);
            // File.Delete(file);
        }
    }

    class Watcher
    {
        DirectoryInfo _destPath;

        /// <summary>
        /// Watchs for file creation on a folder.
        /// </summary>
        public void Watch(DirectoryInfo path, string filter, DirectoryInfo destPath)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path.FullName;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(OnModify);
            watcher.EnableRaisingEvents = true;

            _destPath = destPath;
        }

        private void OnModify(object sender, FileSystemEventArgs e)
        {
            while (IsFileLocked(e.FullPath))
            {
                Thread.Sleep(500);
            }
            Console.WriteLine("Trying to create new report: {0}", e.FullPath);
            Config.tempFullPath = Path.Combine(Config.tempPath, new DirectoryInfo(e.FullPath).Name);
            Report report = new Report();
            Utilities.Folders.CheckIfNotExistAndCreate(Config.tempFullPath);
            Utilities.Folders.Copy(Config.templatePath, Config.tempFullPath, true);
            report.MakeReportFromSRDCM(e.FullPath);
        }

        private bool IsFileLocked(string file)
        {
            const long ERROR_SHARING_VIOLATION = 0x20;
            const long ERROR_LOCK_VIOLATION = 0x21;

            //check that problem is not in destination file
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }
    }
}

// Link divertido http://www.forosdelweb.com/f14/desafios-programacion-1052704/