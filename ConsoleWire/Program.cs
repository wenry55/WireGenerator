using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
using ConsoleWire.Entity;

namespace ConsoleWire
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter sw = null;


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
                            foreach (XElement aChoice in choices.Elements("choice"))
                            {
                                dbCol.choices.Add(aChoice.Attribute("name").Value);
                            }
                        }
                        dbCol.isKey = aCol.Attribute("isKey") != null ? Boolean.Parse(aCol.Attribute("isKey").Value) : false;

                        tbl.columns.Add(dbCol);
                    }

                    // do merge for each table
                    VelocityContext context = new VelocityContext();
                    context.Put("table", tbl);
                    string templateString = string.Empty;
                    StreamReader sr = null;

                    //// client side
                    //sw = new StreamWriter(@"c:\tmp\" + tbl.name.Trim() + ".html");
                    //StreamReader sr = new StreamReader(@"..\..\vm\SigmaClient.vm");
                    //templateString = sr.ReadToEnd();
                    //sr.Close();

                    //vengine.Evaluate(context, sw, null, templateString);
                    //sw.Close();

                    // server side
                    sw = new StreamWriter(@"c:\tmp\" + tbl.name.Trim() + ".cs");
                    sr = new StreamReader(@"..\..\vm\SigmaServer.vm");
                    templateString = sr.ReadToEnd();
                    sr.Close();

                    vengine.Evaluate(context, sw, null, templateString);
                    sw.Close();

                    // server side
                    sw = new StreamWriter(@"c:\tmp\" + tbl.name.Trim() + ".html");
                    sr = new StreamReader(@"..\..\vm\SigmaClient.vm");
                    templateString = sr.ReadToEnd();
                    sr.Close();

                    vengine.Evaluate(context, sw, null, templateString);
                    sw.Close();

                }
            }
            catch (System.Exception ex)
            {
                sw.Close();
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press Enter Key To Return ...");
            Console.ReadLine();
        }

    }

    
}
