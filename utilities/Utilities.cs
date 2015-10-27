using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities {
    /// <summary>
    /// Compress a folder content with external 7Zip program.
    /// </summary>
    public class Zippit {
        // How to compress on 7z command line with C#
        // https://stackoverflow.com/questions/16052877/how-to-unzip-all-zip-file-from-folder-using-c-sharp-4-0-and-without-using-any-o

        // 7z command line examples
        // http://www.dotnetperls.com/7-zip-examples

        // If u don't put specific output folder, VS let the file on:
        // C:\[USER]\AppData\Local\VirtualStore\Program Files (x86)\7-Zip\[OUTPUTFILE]
        public static void MakeZip(string inFolder, string outFile) {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string zipCommand = "a -tzip \"{0}\" \"{1}\\*\"";
            string zipCompressorFolder = "7-Zip";
            string zipCompressorFileName = "7z.exe";
            string zipError = "Unable to compress file: {0}";

            Folders.CheckFileVersion(ref outFile);

            string zipCompressor = Path.Combine(appPath, zipCompressorFolder);
            string arguments = string.Format(zipCommand, outFile, inFolder);

            System.Diagnostics.Process zipCompressorProcess = Interacter.LaunchInShell(zipCompressor, zipCompressorFileName, arguments);

            while (!zipCompressorProcess.HasExited) { }
            if (zipCompressorProcess.ExitCode == 0) {
                // Directory.Delete(inFolder);
            }
            else {
                throw new Exception(string.Format(zipError, outFile));
            }
        }
    }

    /// <summary>
    /// It has Directory functions addons that aren't part of the Framework.
    /// </summary>
    public class Folders {
        public static void CheckIfNotExistAndCreate(string[] directoriesToCheck) {
            for (int i = 0; i < directoriesToCheck.Length; i++) {
                if (!Directory.Exists(directoriesToCheck[i]))
                    Directory.CreateDirectory(directoriesToCheck[i]);
            }
        }

        public static void CheckIfNotExistAndCreate(string directoryToCheck) {
            if (!Directory.Exists(directoryToCheck))
                Directory.CreateDirectory(directoryToCheck);
        }

        public static string[] MakeDataFolders(string dataPath, string[] folderNames) {

            string[] folderPaths = new string[folderNames.Length];

            for (int i = 0; i < folderNames.Length; i++) {
                folderPaths[i] = Path.Combine(dataPath, folderNames[i]);
            }

            Folders.CheckIfNotExistAndCreate(folderPaths);

            return folderPaths;
        }    

        /// <summary>
        /// Watchs for file creation on a folder.
        /// </summary>
        public static void Watch(DirectoryInfo path, string filter) {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path.FullName;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = filter;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e) {
            // Call cardiolizer man!!

            // It should listen like a service and call Cardiolizer when needed.
            // I'm wandering if should Cardiolizer call the Ambulatorio file mover or make another watchdog to handle it.

            // By now I'm gonna make just a for loop to search for files on the input folder. REALLY don't like it, but the other solution could take much more time 
            // to me to develop it. I'll do it soon.
            throw new NotImplementedException();
        }

        // How to copy folders with files in C#, seems it's not a framework function.
        // https://stackoverflow.com/questions/1974019/folder-copy-in-c-sharp
        public static void Copy(string source, string destination, bool recursive) {
            DirectoryInfo sourceDirectory = new DirectoryInfo(source);
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (FileInfo file in files) {
                string tempPath = Path.Combine(destination, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (recursive) {
                DirectoryInfo[] subDirectories = sourceDirectory.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories) {
                    string tempPath = Path.Combine(destination, subDirectory.Name);
                    Copy(subDirectory.FullName, tempPath, recursive);
                }
            }
        }

        public static void CheckFileVersion(ref string outFile) {
            int outFileVersion = 0;

            while (File.Exists(outFile)) {
                outFileVersion++;
                outFile = string.Format(outFile, "_" + outFileVersion.ToString());
            }
        }
    }

    public class Interacter {
        public static System.Diagnostics.Process LaunchInShell(string workingDirectory, string fileName, string arguments) {
            System.Diagnostics.ProcessStartInfo processInfo;
            processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.WorkingDirectory = workingDirectory;
            processInfo.FileName = fileName;
            processInfo.Arguments = arguments;
            processInfo.UseShellExecute = true;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(processInfo);

            /* Code	Meaning
             * 0	No error
             * 1	Warning (Non fatal error(s)). For example, one or more files were locked by some other application, so they were not compressed.
             * 2	Fatal error
             * 7	Command line error
             * 8	Not enough memory for operation
             * 255	User stopped the process
             */
            return process;
        }

        public static void GetCoolPaths(){
            string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string drive = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%");
            string homePath = Environment.ExpandEnvironmentVariables("%HOMEPATH%");
            string programFilesPath = Environment.ExpandEnvironmentVariables("%PROGRAMFILES%");
        }
    }

    public class DataFiller {
        public class dataItem {
            string _DataField;
            string _DataValue;

            public string DataField {
                get { return this._DataField; }
            }

            public string DataValue {
                get { return this._DataValue; }
            }

            public dataItem(string dataField, string dataValue) {
                _DataField = dataField;
                _DataValue = dataValue;
            }

            public dataItem(string dataField, int dataValue) {
                _DataField = dataField;
                _DataValue = dataValue.ToString();
            }

            public dataItem(string dataField, double dataValue) {
                _DataField = dataField;
                _DataValue = dataValue.ToString();
            }
        }

        public static void ReplaceFieldsWithValues(List<dataItem> dataList, string fileIn, string fileOut) {
            StringBuilder fileContent = new StringBuilder();

            using (StreamReader sr = new StreamReader(fileIn)) {
                fileContent.Append(sr.ReadToEnd());
            }

            foreach (dataItem dato in dataList) {
                fileContent.Replace(dato.DataField, dato.DataValue);
            }

            using (StreamWriter sw = new StreamWriter(fileOut)) {
                sw.Write(fileContent);
            }
        }

        public static List<dataItem> FillListWithRandomValues(string dummyFieldName) {
            List<dataItem> DataList = new List<dataItem>();
            Random randomNumber = new Random();

            for (int i = 0; i <= 40; i++) {
                DataList.Add(new dataItem(string.Format(dummyFieldName, i.ToString()), randomNumber.NextDouble() * 100));
            }

            return DataList;
        }
    }
}