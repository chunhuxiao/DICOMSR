using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StructuredReport
{
    public static class Meassurements
    {
        public class item
        {
            string _id;
            string _description;
            string _xPath;
            public string value;
            public string units;

            public item(string id, string description, string xPath)
            {
                _id = id;
                _description = description;
                _xPath = xPath;
            }

            public string description
            {
                get { return _description; }
            }

            public string xPath
            {
                get { return _xPath; }
            }

            public string id
            {
                get { return _id; }
            }
        }

        public static class GetList
        {
            public static List<item> Toshiba()
            {
                List<item> items = new List<item>();

                // PatientData
                items.Add(new item("AccessionNumber", "Número de Acceso", "/Report/PIMS/AccessionNumber"));
                items.Add(new item("PatientName", "Nombre", "/Report/PIMS/PatientName"));
                //xmlItems.Add(new xmlItem("PatientWeight", "Peso", "/Report/PIMS/PatientWeight"));
                items.Add(new item("PatientWeight", "Peso", "/Report/App[@Name='Common']/Group[@Name='PatientGroup']/LabeledMeas[@Name='MCR_BODY_WEIGHT']/RawMeas/ComponentMeas[@Name='MCR_WEIGHT']"));
                items.Add(new item("", "Ritmo", ""));
                //xmlItems.Add(new xmlItem("PatientHeight", "Altura", "/Report/PIMS/PatientHeight"));
                items.Add(new item("PatientHeight", "Altura", "/Report/App[@Name='Common']/Group[@Name='PatientGroup']/LabeledMeas[@Name='MCR_BODY_HEIGHT']/RawMeas/ComponentMeas[@Name='MCR_HEIGHT']"));
                items.Add(new item("PatientAge", "Edad", "/Report/PIMS/PatientAge"));
                items.Add(new item("BSA", "Superficie corporal", "/Report/App[@Name='Common']/Group[@Name='PatientGroup']/LabeledMeas[@Name='BSA']/RawMeas/ComponentMeas[@Name='MCR_BSA']"));
                items.Add(new item("PatientDOB", "Fechad de Nacimiento", "/Report/PIMS/PatientDOB"));

                // StudyData
                //xmlItems.Add(new xmlItem("M_LV_diam_d_Alias", "Diámetro Telediastólico del VI", "/Report/App[@Name='Echo']/Group[@Name='AllAggregateNodes']/AggregateNode[@Name='M_LV_Diam_d_Alias']"));
                items.Add(new item("M_LPC_IVStoLVPW_d", "Diámetro Telediastólico del VI", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/LabeledMeas[@Name='M_LPC_IVStoLVPW_d']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE_2']"));
                items.Add(new item("B_LV_A4MOD_s", "Diámetro Telesistólico del VI, slice 13", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='LV_MOD_Simpson']/LabeledMeas[@Name='B_LV_A4MOD_s']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE_13']"));
                items.Add(new item("M_LPC_IVStoLVPW_d", "Grosor TIV (Tabique Interventricular)", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/LabeledMeas[@Name='M_LPC_IVStoLVPW_d']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE_1']"));
                items.Add(new item("M_LVPW_Thick_d_Alias", "Grosor PP (Pared posterior)", "/Report/App[@Name='Echo']/Group[@Name='AllAggregateNodes']/AggregateNode[@Name='M_LVPW_Thick_d_Alias']"));
                //items.Add(new item("M_LPC_IVStoLVPW_d", "Grosor PP (Pared posterior)", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/LabeledMeas[@Name='M_LPC_IVStoLVPW_d']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE_3']"));
                items.Add(new item("M_LA_Diam_s", "Aurícula Izquierda", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='AorticValve_M']/LabeledMeas[@Name='M_LA_Diam_s']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE']"));
                items.Add(new item("M_Ao_Diam_d", "Raíz Aórtica", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='AorticValve_M']/LabeledMeas[@Name='M_Ao_Diam_d']/RawMeas/ComponentMeas[@Name='MCR_DISTANCE']"));
                items.Add(new item("", "Ventrículo Derecho", ""));
                //xmlItems.Add(new xmlItem("LV_EF_A4MOD", "Fracción de acortamiento del VI", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='LV_MOD_Simpson']/Calc[@Name='LV_EF_A4MOD']"));
                items.Add(new item("M_LV_FS", "Fracción de acortamiento del VI", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Calc[@Name='M_LV_FS']"));
                items.Add(new item("M_LV_EF_Teichholtz", "Fracción de eyección de Teichholtz", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/Calc[@Name='M_LV_EF_Teichholtz']"));
                items.Add(new item("LV_EF_A4MOD", "Fracción de eyección de Simpson", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='LV_MOD_Simpson']/Calc[@Name='LV_EF_A4MOD']"));
                items.Add(new item("M_LV_Mass_d_AVCube", "Masa del VI", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/Calc[@Name='M_LV_Mass_d_AVCube']"));
                items.Add(new item("M_LV_MI_d_AVCube", "Indice de masa", "/Report/App[@Name='Echo']/Group[@Name='Mmode']/Group[@Name='LV_M']/Calc[@Name='M_LV_MI_d_AVCube']"));

                // DopplerData
                // Mitral Valve
                items.Add(new item("D_MV_EPeakVmax_DCTPHT_d", "E Vel", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='MitralValve_B']/LabeledMeas[@Name='D_MV_EPeakVmax_DCTPHT_d']/RawMeas/ComponentMeas[@Name='MCR_VELOCITY']"));
                items.Add(new item("D_MV_APeakVmax_d", "A Vel", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='MitralValve_B']/LabeledMeas[@Name='D_MV_APeakVmax_d']/RawMeas/ComponentMeas[@Name='MCR_VELOCITY']"));
                items.Add(new item("MV_EARatio", "E/A", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='MitralValve_B']/Calc[@Name='MV_EARatio']"));
                // Aortic Valve
                items.Add(new item("MCR_PEAK_SYSTOLE_VELOCITY", "Velocidad máxima", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='AorticValve_B']/LabeledMeas[@Name='D_AV_SimpleTrace_s']/RawMeas/ComponentMeas[@Name='MCR_PEAK_SYSTOLE_VELOCITY']"));
                items.Add(new item("D_AV_SimpleTrace_s_MPG_Mean", "Gradiente Medio", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='AorticValve_B']/LabeledMeas[@Name='D_AV_SimpleTrace_s']/RawMeas/ComponentMeas[@ReportLabel='D_AV_SimpleTrace_s_MPG_Mean']"));
                items.Add(new item("D_AV_SimpleTrace_s_PPG_Mean", "Gradiente Máximo", "/Report/App[@Name='Echo']/Group[@Name='2D_mode']/Group[@Name='AorticValve_B']/LabeledMeas[@Name='D_AV_SimpleTrace_s']/RawMeas/ComponentMeas[@ReportLabel='D_AV_SimpleTrace_s_PPG_Mean']"));

                return items;
            }
        }

        public static Dictionary<string, string> Units = new Dictionary<string, string> {
                {"UNITS_KILOGRAMS", "kg"},
                {"UNITS_CENTIMETERS", "cm"},
                {"UNITS_METERS_SQUARED", "m^2"},
                {"UNITS_MILLIMETERS", "mm"},
                {"UNITS_PERCENT", "%"},
                {"UNITS_GRAMS", "g"},
                {"MCR_GRAMS_PER_METER_SQUARED", "g/m^2"},
                {"UNITS_CENTIMETERS_PER_SECOND", "cm/s"},
                {"MCR_MILLIMETERS_HG", "mmHg"},
                {"MCR_DIMENSIONLESS", ""},
                {"Y", " años"}
            };
    }
    public static class StructuredReport
    {
        public static string Read(string fileIn, string startString)
        {
            string content;
            string data;
            int start;

            using (StreamReader _fileIn = new StreamReader(fileIn))
            {
                content = _fileIn.ReadToEnd();
                start = content.IndexOf(startString); // "<Report>"
                data = content.Substring(start);
                content = null;
            }
            return data;
        }

        public static List<Meassurements.item> ExtractDCMData(string dcmFilePath, string sSRBegin)
        {
            string content = Read(dcmFilePath, sSRBegin);
            List<Meassurements.item> meassurementsList = FillList(content);

            return meassurementsList;
        }

        public static List<Meassurements.item> FillList(string content)
        {
            List<Meassurements.item> MeassurementList = Meassurements.GetList.Toshiba();

            XmlDocument sr = new XmlDocument();
            sr.LoadXml(content);

            foreach (Meassurements.item item in MeassurementList)
            {

                try { item.value = sr.SelectSingleNode(item.xPath).Attributes["Value"].Value; }
                catch { item.value = "[ERROR]"; }
                try { item.units = sr.SelectSingleNode(item.xPath).Attributes["Units"].Value; }
                catch { }
            }

            return MeassurementList;
        }
    }
}
