using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RDLParser
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Prep for RDLParser
            Dictionary<string, string> paramvalues = new Dictionary<string, string>();
            string filename = @"C:\Users\usari\source\repos\RDLParser\Report Project1\Report1.rdl";
            paramvalues.Add("id", "2");
            paramvalues.Add("testParm2", "zorro");

            // Set the processing mode for the ReportViewer to Local  
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = filename;

            string sqlcon = "Data Source=(local);Initial Catalog=usarian;Integrated Security=SSPI";
            ParseRDL rp = new ParseRDL(filename, paramvalues, ref localReport, sqlcon);

            // Refresh the report  
            reportViewer1.RefreshReport();
        }
    }

    //public class ParseRDL
    //{
    //    XmlNodeList DataSetName;
    //    Dictionary<string, XmlNodeList> SQLParameters = new Dictionary<string, XmlNodeList>();
    //    Dictionary<string, string> SQLQueryText = new Dictionary<string, string>();
    //    XmlNodeList ReportParameters;
    //    Dictionary<string, string> paramvalues;

    //    public ParseRDL(string RDLFileName, Dictionary<string, string> ParameterValues, ref LocalReport localReport)
    //    {
    //        ReadXML(RDLFileName);
    //        paramvalues = ParameterValues;

    //        // Create report parameters  
    //        foreach (XmlNode node in ReportParameters)
    //        {
    //            ReportParameter rptparam = new ReportParameter(name: node.Attributes["Name"].Value);
    //            rptparam.Values.Add(paramvalues[node.Attributes["Name"].Value]);

    //            // Set the report parameters for the report  
    //            localReport.SetParameters(new ReportParameter[] { rptparam });
    //        }

    //        // loop datasets
    //        foreach (XmlNode dsname in DataSetName)
    //        {
    //            string dsnamestr = dsname.Attributes["Name"].Value;

    //            DataSet ds = new DataSet(dsnamestr);

    //            GetSalesOrderDetailData(ref ds, dsnamestr);

    //            // Create report data source 
    //            ReportDataSource rds = new ReportDataSource();
    //            rds.Name = dsnamestr;
    //            rds.Value = ds.Tables[dsnamestr];

    //            localReport.DataSources.Add(rds);
    //        }
    //    }

    //    private void GetSalesOrderDetailData(ref DataSet ds, string dsname)
    //    {
    //        string sqltxt;
    //        SQLQueryText.TryGetValue(dsname, out sqltxt);

    //        XmlNodeList splist;
    //        SQLParameters.TryGetValue(dsname, out splist);

    //        using (SqlConnection connection = new SqlConnection("Data Source=(local);Initial Catalog=usarian;Integrated Security=SSPI"))
    //        {
    //            SqlCommand command = new SqlCommand(sqltxt, connection);
    //            foreach (XmlNode sqlparam in splist)
    //            {
    //                string pname = sqlparam.Value.Remove(0, 1);
    //                command.Parameters.Add(new SqlParameter(pname, paramvalues[pname]));
    //            }
    //            SqlDataAdapter da = new SqlDataAdapter(command);
    //            da.Fill(ds, dsname);
    //        }
    //    }

    //    private void ReadXML(string filepath)
    //    {
    //        var xmlDoc2 = new XmlDocument();
    //        xmlDoc2.Load(filepath);
    //        var nsmgr = new XmlNamespaceManager(xmlDoc2.NameTable);
    //        nsmgr.AddNamespace("x", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
    //        nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

    //        //DataSet Names
    //        DataSetName = xmlDoc2.SelectNodes("/x:Report/x:DataSets/*", nsmgr); 

    //        //SQLParameterName 
    //        foreach (XmlNode xmlnode in DataSetName)
    //        {
    //            foreach (XmlAttribute dsn in xmlnode.Attributes)
    //            {
    //                string x = dsn.Value;

    //                //SQLParameterName
    //                XmlNodeList sp = xmlDoc2.SelectNodes("//x:DataSet[@Name = '" + x + "']/x:Query/x:QueryParameters/x:QueryParameter/@Name", nsmgr);
    //                SQLParameters.Add(x, sp);

    //                //SQLQueryText
    //                SQLQueryText.Add(x, xmlDoc2.SelectNodes("//x:DataSet[@Name='" + x + "']/x:Query/x:CommandText/text()", nsmgr)[0].Value);
    //            }
    //        }

    //        //ReportParameters
    //        ReportParameters = xmlDoc2.SelectNodes("/x:Report/x:ReportParameters/*", nsmgr);

    //    }
    //}

}

