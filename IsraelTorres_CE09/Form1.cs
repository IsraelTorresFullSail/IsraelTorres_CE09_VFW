using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Xml;

/*
 * Name: Israel Torres
 * Class: Visual Frameworks - Online (MDV1830-O)
 * Term: C201905 01
 * Code Exercise: Database Connectivity
 * Number: CE09
 */

namespace IsraelTorres_CE09
{
    public partial class Form1 : Form
    {
        // connection string variable
        MySqlConnection conn = new MySqlConnection();

        DataTable theData = new DataTable();

        // variable to for number of records
        int numberOfRecords = 0;

        // variable for the current record
        int record = 0;

        // variable for the current row
        int row = 0;

        public Form1()
        {
            InitializeComponent();

            string connetionString = BuildConnectionString("exampleDatabase", "dbsAdmin", "password");

            Connect(connetionString);

            RetrieveData();
        }

        private bool RetrieveData()
        {
            // create the SQL statement
            string sql = "SELECT DVD_Title, Price, Year, Genre FROM dvd LIMIT 25";

            // Create the DataAdapter
            MySqlDataAdapter adr = new MySqlDataAdapter(sql, conn);

            adr.SelectCommand.CommandType = CommandType.Text;

            adr.Fill(theData);

            // get a count of the number of rows within the DataTable
            numberOfRecords = theData.Select().Length;

            // put the first record's data into the form
            textBoxTitle.Text = theData.Rows[0]["DVD_Title"].ToString();
            textBoxGenre.Text = theData.Rows[0]["Genre"].ToString();
            numericUpDownYear.Value = Convert.ToDecimal(theData.Rows[0]["Year"]);
            numericUpDownPrice.Value = Convert.ToDecimal(theData.Rows[0]["Price"]);

            recordStatus.Text = "Record 1 of " + numberOfRecords;

            return true;
        }

        private void Connect(string myConnectionString)
        {
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                //MessageBox.Show("Connected!");
            }
            catch (MySqlException e)
            {

                // message strig variable
                string msg = "";

                // check what exception was received
                switch (e.Number)
                {
                    case 0:
                        msg = e.ToString();
                        break;
                    case 1042:
                        msg = "Can't resolve host address.\n" + myConnectionString;
                        break;
                    case 1045:
                        msg = "invalid username/password";
                        break;
                    default:
                        // generic message if the others don't cover it
                        msg = e.ToString() + "\n" + myConnectionString;
                        break;
                }
                MessageBox.Show(msg);
            }
        }

        private string BuildConnectionString(string database, string uid, string pwd)
        {
            string serverIP = "";

            try
            {
                // open the text file using stream reader
                using (StreamReader sr = new StreamReader("C:\\VFW\\connect.txt"))
                {
                    // reader the server IP data
                    serverIP = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return "server=" + serverIP + ";uid=" + uid + ";pwd=" + pwd + ";database=" + database + " ;port=8889";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            row = 0;

            textBoxTitle.Text = theData.Rows[row]["DVD_Title"].ToString();
            textBoxGenre.Text = theData.Rows[row]["Genre"].ToString();
            numericUpDownYear.Value = Convert.ToDecimal(theData.Rows[row]["Year"]);
            numericUpDownPrice.Value = Convert.ToDecimal(theData.Rows[row]["Price"]);

            record = row + 1;

            recordStatus.Text = "Record " + record + " of " + numberOfRecords;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            row = 24;

            textBoxTitle.Text = theData.Rows[row]["DVD_Title"].ToString();
            textBoxGenre.Text = theData.Rows[row]["Genre"].ToString();
            numericUpDownYear.Value = Convert.ToDecimal(theData.Rows[row]["Year"]);
            numericUpDownPrice.Value = Convert.ToDecimal(theData.Rows[row]["Price"]);

            record = row + 1;

            recordStatus.Text = "Record " + record + " of " + numberOfRecords;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (row > 0)
            {
                row--;

                textBoxTitle.Text = theData.Rows[row]["DVD_Title"].ToString();
                textBoxGenre.Text = theData.Rows[row]["Genre"].ToString();
                numericUpDownYear.Value = Convert.ToDecimal(theData.Rows[row]["Year"]);
                numericUpDownPrice.Value = Convert.ToDecimal(theData.Rows[row]["Price"]);

                record = row + 1;

                recordStatus.Text = "Record " + record + " of " + numberOfRecords;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (row + 1 < theData.Select().Length)
            {
                row++;

                textBoxTitle.Text = theData.Rows[row]["DVD_Title"].ToString();
                textBoxGenre.Text = theData.Rows[row]["Genre"].ToString();
                numericUpDownYear.Value = Convert.ToDecimal(theData.Rows[row]["Year"]);
                numericUpDownPrice.Value = Convert.ToDecimal(theData.Rows[row]["Price"]);

                record = row + 1;

                recordStatus.Text = "Record " + record + " of " + numberOfRecords;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = "xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // instantiate an XmlWriterSettings object
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                // set the indent to true
                settings.Indent = true;

                // create the XmlWriter
                using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, settings))
                {
                    if (theData.Rows.Count > 0)
                    {
                        // write the unique indentifier
                        writer.WriteStartElement("dvd_info");

                        for (int i = 0; i < theData.Select().Length; i++)
                        {
                            // write the unique indentifier
                            writer.WriteStartElement("dvd");

                            writer.WriteElementString("DVD_Title", theData.Rows[i]["DVD_Title"].ToString());
                            writer.WriteElementString("Genre", theData.Rows[i]["Genre"].ToString());
                            writer.WriteElementString("Year", theData.Rows[i]["Year"].ToString());
                            writer.WriteElementString("Price", theData.Rows[i]["Price"].ToString());

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else
                    {
                        MessageBox.Show("There is no data to save.");
                    }

                }
            }
        }
    }
}
