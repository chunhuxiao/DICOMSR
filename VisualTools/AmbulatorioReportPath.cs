using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VisualTools {
    public partial class AmbulatorioReportPath : Form {
        public AmbulatorioReportPath() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            ReportLoad();
        }

        private void button2_Click(object sender, EventArgs e) {
            ReportSave();
        }

        private void ReportLoad() {
            FileDialog fileDialog = new OpenFileDialog();
            DialogResult result = fileDialog.ShowDialog();
            label4.Text = result.ToString();

            // I use FileInfo to extract fileName, cause FileDialog things that a FileName is its full path...
            FileInfo file = new FileInfo(fileDialog.FileName);
            string UNC = textBox1.Text;

            label1.Text = file.FullName;

            string fullPath = CrapFixer.CrapFixer.AmbulatorioReportPath(file.Name, UNC);

            textBox2.Text = fullPath;
        }

        private void ReportSave() {
            FileInfo fileSource = new FileInfo(label1.Text);
            FileInfo fileTarget = new FileInfo(textBox2.Text);
            try {
                if (!Directory.Exists(fileTarget.DirectoryName)) {
                    Console.WriteLine("Creating directory: {0}", fileTarget.DirectoryName);
                    Directory.CreateDirectory(fileTarget.DirectoryName);
                }

                fileSource.CopyTo(fileTarget.FullName);

                MessageBox.Show("Copiado correctamente.");
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Excepción al copiar el archivo");
            }
        }
    }
}
