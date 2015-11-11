using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

// ----------------------
// TODO

// Replace methods.
//
// 1) Get DCM File.
// 2) Extract DATA.
// 3) Copy template to temp folder named as DCM file.
// 4) Replace Values from that temp folder's xmls.
// 5) Compress files to .odt then change extencion to .doc
//
// ----------------------

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

            tempPath = Path.Combine(dataPath, tempDir);
            // tempContentFilePath = ""; // Path.Combine(tempFullPath, tempContentFileName); // Path.Combine(tempPath, tempContentFileName);
        
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

        public static void MakeReport(FileInfo fileIn)
        {
            ReportMaker report = new ReportMaker();
            report.LoadDCMFile(fileIn.FullName, Config.SRStartString);

            int accessionNumber = report.GetAccessionNumber();
            string tempFullPath = Path.Combine(Config.tempPath, accessionNumber.ToString());

            Utilities.Folders.CheckIfNotExistAndCreate(tempFullPath);
            Utilities.Folders.Copy(Config.templatePath, tempFullPath, true);

            foreach (string contentFile in Config.contentFiles)
            {
                report.ReplaceValuesOnFile(contentFile);
            }

            FileInfo fileOut =
                new FileInfo(CrapFixer.CrapFixer.AmbulatorioReportPath(
                    Config.fileOutPath,
                    accessionNumber + ".doc"));

            if (fileOut.Exists) { fileOut.Delete(); }

            Console.WriteLine("Creating report: {0}", fileOut);
            Utilities.Zippit.MakeZip(fileIn.FullName, fileOut.FullName);

            Directory.Delete(fileIn.Directory.FullName, true);
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

            string[] folderPaths = new string[] 
            { 
                Config.dataInPath, 
                Config.templatePath, 
                Config.tempPath 
            };
            
            Utilities.Folders.CheckIfNotExistAndCreate(folderPaths);

            // First check for files already in the input folder
            DirectoryInfo sourceDir =  new DirectoryInfo(Config.dataInPath);
            foreach (FileInfo file in sourceDir.EnumerateFiles(Config.fileFilter))
            {
                MakeReport(file);   
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

    class ReportMaker
    {
        const string dcmNotLoaded = "DCM File not loaded.";

        bool dcmLoaded = false;
        List<StructuredReport.Meassurements.item> meassurementList;

        public void LoadDCMFile(string dcmFilePath, string sSRBegin)
        {
            meassurementList = StructuredReport.StructuredReport.ExtractDCMData(dcmFilePath, sSRBegin);
            dcmLoaded = true;
        }

        public int GetAccessionNumber()
        {
            if (!dcmLoaded) throw new Exception(dcmNotLoaded);

            StructuredReport.Meassurements.item item = meassurementList.Find(x => x.id == "AccessionNumber");
            return int.Parse(item.value);
        }

        public void ReplaceValuesOnFile(string file)
        {
            ReplaceValuesOnFile(file, file);
        }

        public void ReplaceValuesOnFile(string fileIn, string fileOut)
        {
            if (!dcmLoaded) throw new Exception(dcmNotLoaded);

            StringBuilder fileContent = new StringBuilder();

            using (StreamReader sr = new StreamReader(fileIn))
            {
                fileContent.Append(sr.ReadToEnd());
            }

            foreach (StructuredReport.Meassurements.item item in meassurementList)
            {
                try
                {
                    fileContent.Replace(item.id, item.value + item.units);
                }
                catch { }
            }

            using (StreamWriter sw = new StreamWriter(fileOut))
            {
                sw.Write(fileContent);
            }
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
            FileInfo file = new FileInfo(e.FullPath);
            Cardiolizer.MakeReport(file);
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