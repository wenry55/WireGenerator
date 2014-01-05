using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NVelocity;
using Commons;
using System.IO;
using Velocity = NVelocity.App.Velocity;
using VelocityEngine = NVelocity.App.VelocityEngine;
using VelocityContext = NVelocity.VelocityContext;
using Template = NVelocity.Template;
using ParseErrorException = NVelocity.Exception.ParseErrorException;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
using System.Collections;
using System.Reflection;
using Commons.Collections;

using System.Data.SqlClient;
using System.Data.SqlServerCe;

using System.Xml.Linq;
using System.Xml;

using WireGenerator.classes;

namespace WireGenerator
{
    public class TestCls
    {
        private string _aa;
        public string aa
        {
            get
            {
                return _aa;
            }
            set
            {
                _aa = value;
            }
        }
        public string bb;
        public string cc;

        
        
    }

        
    public class Item
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }


    public partial class WireBuilder : Form
    {
        public WireBuilder()
        {
            InitializeComponent();
        }

        private void WireBuilder_Load(object sender, EventArgs e)
        {
            StreamWriter sw = null;
            TestCls txt = new TestCls() { aa = "11", bb = "22", cc = "33" };
            List<string> xstrx = new List<string>();
            xstrx.Add("first");
            xstrx.Add("second");

            try
            {
                VelocityEngine vengine = new VelocityEngine();
                vengine.Init();

                VelocityContext context = new VelocityContext();
                context.Put("name", "bk"); 
                context.Put("testcls", txt);
                context.Put("strlst", xstrx);
                context.Put("xxx", 111);

                sw = new StreamWriter(@"c:\tmp\test4.txt");
                string path = @"..\..\vm\test.vm";
                StreamReader sr = new StreamReader(path);
                string templateString = sr.ReadToEnd();
                sr.Close();
                vengine.Evaluate(context, sw, null, templateString);
                sw.Close();
            }
            catch (System.Exception ex)
            {

                sw.Close();
                MessageBox.Show(ex.Message);
            }

            /*
            SqlCeConnection conn = new SqlCeConnection();
            conn.ConnectionString = "Data Source=.\\data\\TestDb.sdf;Persist Security Info=False;";
            try
            {

                conn.Open();
                SqlCeCommand cmd = new SqlCeCommand("select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Schedule'", conn);
                // SqlCeCommand cmd = new SqlCeCommand("select * from Schedule", conn);
                SqlCeDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string yy = reader.GetString(3);    // column name
                    Console.WriteLine(yy);
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
             */

            try
            {
                VelocityEngine vengine = new VelocityEngine();
                vengine.Init();

                List<DbTable> tableList = new List<DbTable>();

                XDocument xdoc = XDocument.Load(@".\data\entities.xml");
                var tables = xdoc.Root.Elements("table");
                foreach (XElement aTable in tables)
                {
                    DbTable tbl = new DbTable();
                    tbl.name = aTable.Attribute("name").Value;
                    XElement cols = aTable.Element("columns");

                    foreach (XElement aCol in cols.Elements("column"))
                    {
                        Console.WriteLine(aCol.Attribute("name").Value);
                        DbColumn dbCol = new DbColumn();
                        dbCol.columnName = aCol.Attribute("name").Value;
                        dbCol.columnType = aCol.Attribute("type").Value;

                        if (dbCol.columnType == "Choice")
                        {
                            dbCol.choices = new List<string>();
                            XElement choices = aCol.Element("choices");
                            foreach (XElement aChoice in choices.Elements("choice")) {
                                dbCol.choices.Add(aChoice.Attribute("name").Value);
                            }
                        }

                        tbl.columns.Add(dbCol);
                    }

                    // do merge for each table
                    VelocityContext context = new VelocityContext();
                    context.Put("table", tbl);

                    sw = new StreamWriter(@"c:\tmp\" + tbl.name.Trim() + ".txt");
                    string path = @"..\..\vm\test2.vm";

                    StreamReader sr = new StreamReader(path);
                    string templateString = sr.ReadToEnd();
                    sr.Close();

                    vengine.Evaluate(context, sw, null, templateString);
                    sw.Close();

                }
            }
            catch (System.Exception ex)
            {
                sw.Close();
                MessageBox.Show(ex.Message);
            }
        }

    }
}
