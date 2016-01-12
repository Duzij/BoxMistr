using ADOX;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoxMistr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.localDatabaseDataSet.EnforceConstraints = false;
        }
        public static List<string> tmp;
        public string CSVPath { get; set; }
        public string pathPPL { get; set; }
        public string pathUlozenka { get; set; }
        public string finalLine { get; set; }
        public string finalLineExpedovani { get; set; }
        private static readonly string PathToDatabase = @"C:\LocalDatabase.accdb";
        public OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathToDatabase + "; Persist Security Info=False;");
        StreamWriter writer;
        StreamWriter writerExpedovani;
        private int ExportedBoxesCounter;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.mainTableAdapter.Fill(this.localDatabaseDataSet.Main);
                DubbleBuffer();

                MesseagePainter();
            }
            catch
            {
                CreateNewDatabase();
                DubbleBuffer();
                MesseagePainter();
            }
        }
        private void DubbleBuffer()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();
        }
        private void MesseagePainter()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[15].Value.ToString() != "")
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
            }
        }
        static void CreateNewDatabase()
        {
            ADOX.Catalog cat = new ADOX.Catalog();
            string tmpStr;
            string filename = @"C:\LocalDatabase.accdb";
            tmpStr = "Provider=Microsoft.Jet.OLEDB.4.0;";
            tmpStr += "Data Source=" + filename + ";Jet OLEDB:Engine Type=5";
            cat.Create(tmpStr);

            Table table = new Table();
            table.Name = "Main";
            table.Columns.Append("NObjednavky", ADOX.DataTypeEnum.adInteger);
            table.Columns.Append("STAV", ADOX.DataTypeEnum.adBoolean);
            table.Columns.Append("Cena", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Dobirka", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Firma", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Jmeno", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Adresa", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("PSC", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Mesto", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Vaha", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Sluzby", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("V_symbol", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Telefon", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Email", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Doprava", ADOX.DataTypeEnum.adVarWChar);
            table.Columns.Append("Vzkaz", ADOX.DataTypeEnum.adLongVarWChar);

            cat.Tables.Append(table);


            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(table);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.Tables);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.ActiveConnection);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat);

            cat = null;

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //this.mainTableAdapter.Update(this.localDatabaseDataSet.Main);
            button1.BackColor = Color.Red;
        }
        private void SaveDatabase(object sender, EventArgs e)
        {
            this.mainTableAdapter.Update(this.localDatabaseDataSet.Main);
            button1.BackColor = Color.Gray;
        }
        static string[] RemoveLastInArray(string[] array, int index)
        {
            tmp = new List<string>(array);
            tmp.RemoveAt(index);
            array = tmp.ToArray();
            return array;
        }
        static string[] RemoveArray(int LastIndex, string[] array)
        {
            if (array.Length > LastIndex)
            {
                for (int i = LastIndex + 1; i < array.Length; i++)
                {
                    array[LastIndex] += ("," + array[i]);
                }
                tmp = new List<string>(array);

                for (int i = LastIndex + 1; i < array.Length; i++)
                {
                    tmp.RemoveAt(LastIndex + 1);
                }
                array = tmp.ToArray();
            }
            return array;

        }
        private void Import(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Csv files (*.csv) | *.csv;";
            if (ofd.ShowDialog() == DialogResult.OK)
                CSVPath = ofd.FileName;
            ofd.Dispose();

            try
            {
                StreamReader readerCSV = new StreamReader(File.OpenRead(CSVPath), Encoding.GetEncoding(1250));
                while (!readerCSV.EndOfStream)
                {
                    this.progressBar1.Increment(1);
                    string[] hodnoty = readerCSV.ReadLine().Split(';');
                    if (hodnoty.Length > 15)
                    {
                        if (hodnoty[15] == "P")
                           hodnoty = RemoveLastInArray(hodnoty, 15);

                        if (hodnoty.Length > 15)
                        hodnoty = RemoveArray(14, hodnoty);
                    }
                    objednavky obj = new objednavky();

                    if (hodnoty[13].Contains("Uloženka") || hodnoty[13].Contains("PPL"))
                    {
                        obj.CObjednavky = hodnoty[0];
                        obj.Cena = hodnoty[1];
                        obj.Dobirka = hodnoty[2];
                        obj.Firma = hodnoty[3];
                        obj.Jmeno = hodnoty[4];
                        obj.Adresa = hodnoty[5];
                        obj.PSC = hodnoty[6];
                        obj.Mesto = hodnoty[7];
                        obj.Vaha = hodnoty[8];
                        obj.Sluzby = hodnoty[9];
                        obj.VSymbol = hodnoty[10];
                        obj.Telefon = obj.minus420(hodnoty[11]);
                        obj.Emal = hodnoty[12];
                        obj.Doprava = hodnoty[13];
                        obj.Vzkaz = hodnoty[14];
                        obj.Stav = false;

                        if (LookForDublicates(obj.CObjednavky) == true)
                            continue;

                        mainTableAdapter.Insert(Convert.ToInt32(obj.CObjednavky), obj.Stav, obj.Cena, obj.Dobirka, obj.Firma, obj.Jmeno,
                            obj.Adresa, obj.PSC, obj.Mesto, obj.Vaha, obj.Sluzby, obj.VSymbol, obj.Telefon, obj.Emal, obj.Doprava, obj.Vzkaz);

                    }
                }


                this.mainTableAdapter.Fill(this.localDatabaseDataSet.Main);
                DubbleBuffer();
                MesseagePainter();
                this.progressBar1.Increment(100);
                MessageBox.Show("Data loaded");
                this.progressBar1.Value = 0;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public bool LookForDublicates(string cislo)
        {
            connection.Open();
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Main", connection);
            OleDbDataReader reader = cmd.ExecuteReader();
            List<string> list = new List<string>();

            while (reader.Read())
                list.Add(reader[0].ToString());

            if (list.Contains(cislo))
            {
                connection.Close();
                return true;
            }
            connection.Close();
            return false;


        }
        private void ButtonPPL(object sender, EventArgs e)
        {
            ExportedBoxesCounter = 0;
            StringBuilder builder = new StringBuilder();
            StringBuilder builderExpedovani = new StringBuilder();
            string DefaultFileName = DateTime.Today.ToString("d.M.yy") + " PPL.csv";
            string DefaultFileExpedovaniName = DateTime.Today.ToString("d.M.yy") + " UlozenkaExpedovani.csv";
            SaveFileDialog sfd = SaveFile(DefaultFileName);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pathPPL = sfd.FileName;
                pathPPL.Replace("/", "");
                sfd.Dispose();
                string pathPPLExpedovani = pathPPL.Substring(0, pathPPL.Length - 4) + "Expedovani.csv";
                writer = new StreamWriter(pathPPL);
                writerExpedovani = new StreamWriter(pathPPLExpedovani);
                objednavky obj = new objednavky();
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[1].Value) == true && dataGridView1.Rows[i].Cells[2].Value.ToString().Contains("PPL") == true)
                    {
                        ExportedBoxesCounter++;
                        string NObjednavky = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        bool Stav = Convert.ToBoolean(dataGridView1.Rows[i].Cells[1].Value);
                        string Cena = dataGridView1.Rows[i].Cells[3].Value.ToString();
                        string Dobirka = dataGridView1.Rows[i].Cells[4].Value.ToString();
                        string Firma = dataGridView1.Rows[i].Cells[5].Value.ToString();
                        string Jmeno = dataGridView1.Rows[i].Cells[6].Value.ToString();
                        string Adresa = dataGridView1.Rows[i].Cells[7].Value.ToString();
                        string PSC = dataGridView1.Rows[i].Cells[8].Value.ToString();
                        string Mesto = dataGridView1.Rows[i].Cells[9].Value.ToString();
                        string Vaha = dataGridView1.Rows[i].Cells[10].Value.ToString();
                        string Sluzby = dataGridView1.Rows[i].Cells[11].Value.ToString();
                        string VSymbol = dataGridView1.Rows[i].Cells[12].Value.ToString();
                        string Telefon = dataGridView1.Rows[i].Cells[13].Value.ToString();
                        string Email = dataGridView1.Rows[i].Cells[14].Value.ToString();
                        string Doprava = dataGridView1.Rows[i].Cells[2].Value.ToString();
                        string Vzkaz = dataGridView1.Rows[i].Cells[15].Value.ToString();
                        finalLine = builder.Append(NObjednavky + ';' + Cena + ';' + Dobirka + ';' + Firma + ';' + Jmeno + ';' + Adresa + ';' + PSC + ';' + Mesto + ';' + Vaha + ';' + Sluzby + ';' + VSymbol + ';' + Telefon +
                            ';' + Email + ';' + Doprava + ';' + Vzkaz + ";\n").ToString();
                        mainTableAdapter.DeleteQuery(Convert.ToInt32(NObjednavky));

                        finalLineExpedovani = builderExpedovani.Append(NObjednavky + ";\n").ToString();
                    }
                }
                MessageBox.Show(ExportedBoxesCounter + " packages exported");
                if (String.IsNullOrWhiteSpace(finalLine))
                {
                    MessageBox.Show("No checked packages found");
                }
                else
                {
                    finalLine = finalLine.Substring(0, finalLine.Length - 2); /*uziznem posledni \n*/
                   writerExpedovani.WriteLine(finalLineExpedovani);
                    writer.WriteLine(finalLine);
                    writerExpedovani.Close();
                    writer.Close();
                }
                this.mainTableAdapter.Fill(this.localDatabaseDataSet.Main);
            }
        }
        private SaveFileDialog SaveFile(string DefaultFileName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + @"\\BoxMistrFiles");
            sfd.RestoreDirectory = false;
            sfd.FileName = DefaultFileName;
            sfd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + @"\\BoxMistrFiles\\";

            sfd.DefaultExt = "csv";
            sfd.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            return sfd;
        }
        private void ButtonUlozenka(object sender, EventArgs e)
        {
            ExportedBoxesCounter = 0;
            StringBuilder builder = new StringBuilder();
            StringBuilder builderExpedovani = new StringBuilder();
            string DefaultFileName = DateTime.Today.ToString("d.M.yy") + " Ulozenka.csv";
            string DefaultFileExpedovaniName = DateTime.Today.ToString("d.M.yy") + " UlozenkaExpedovani.csv";
            SaveFileDialog sfd = SaveFile(DefaultFileName);
            sfd.Dispose();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pathUlozenka = sfd.FileName;
                pathUlozenka.Replace("/", "");
                string pathUlozenkaExpedovani = pathUlozenka.Substring(0, pathUlozenka.Length - 4) + "Expedovani.csv";
                writer = new StreamWriter(pathUlozenka);
                writerExpedovani = new StreamWriter(pathUlozenkaExpedovani);
                objednavky obj = new objednavky();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[1].Value) == true && dataGridView1.Rows[i].Cells[2].Value.ToString().Contains("Ulož") == true)
                    {
                        ExportedBoxesCounter++;
                        string NObjednavky = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        string Cena = dataGridView1.Rows[i].Cells[3].Value.ToString();
                        string Dobirka = dataGridView1.Rows[i].Cells[4].Value.ToString();
                        string Firma = dataGridView1.Rows[i].Cells[5].Value.ToString();
                        string Jmeno = obj.NameSurname(dataGridView1.Rows[i].Cells[6].Value.ToString())[0];
                        string Prijmeni = obj.NameSurname(dataGridView1.Rows[i].Cells[6].Value.ToString())[1];
                        string Telefon = dataGridView1.Rows[i].Cells[13].Value.ToString();
                        string Email = dataGridView1.Rows[i].Cells[14].Value.ToString();
                        string Doprava = obj.UlozenkaCode(dataGridView1.Rows[i].Cells[2].Value.ToString());

                        finalLine = builder.Append(NObjednavky + ';' + Cena + ';' + Dobirka + ';' + Jmeno + ';' + Prijmeni + ';' + Telefon + ';' + Email + ';' + Doprava + ";\n").ToString();
                        mainTableAdapter.DeleteQuery(Convert.ToInt32(NObjednavky));

                        finalLineExpedovani = builderExpedovani.Append(NObjednavky + ";\n").ToString();
                    }
                }
                MessageBox.Show(ExportedBoxesCounter + " packages exported");
                if (String.IsNullOrWhiteSpace(finalLine))
                {
                    MessageBox.Show("No checked packages found");
                }
                else
                {
                    finalLine = finalLine.Substring(0, finalLine.Length - 2); /*uziznem posledni \n*/
                    writerExpedovani.WriteLine(finalLineExpedovani);
                    writer.WriteLine(finalLine);
                    writerExpedovani.Close();
                    writer.Close();
                }
                this.mainTableAdapter.Fill(this.localDatabaseDataSet.Main);
            }
        }
        private void ShowToBottomTextBox(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentCell.Value.ToString();
        }
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.mainTableAdapter.Update(this.localDatabaseDataSet.Main);
            button1.BackColor = Color.Red;
        }
        private void dataGridView1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            MesseagePainter();
        }
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            MesseagePainter();
        }
    }
}
