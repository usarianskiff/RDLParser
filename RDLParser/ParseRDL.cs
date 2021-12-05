using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
namespace RDLParser
{
    public class ParseRDL
    {
        XmlNodeList DataSetName;
        XmlNodeList ReportParameters;
        Dictionary<string, XmlNodeList> SQLParameters = new Dictionary<string, XmlNodeList>();
        Dictionary<string, string> SQLQueryText = new Dictionary<string, string>();
        Dictionary<string, string> paramvalues;
        string _SQLConnection;

        public ParseRDL(string RDLFileName, Dictionary<string, string> ReportParameterValues /*Parameter Name, ParameterValue*/, ref LocalReport localReport, string SQLConnection)
        {
            // set globals
            _SQLConnection = SQLConnection;
            paramvalues = ReportParameterValues;

            // grab rdl xml
            ReadXML(RDLFileName);

            // loop thru report parameters node list
            foreach (XmlNode node in ReportParameters)
            {
                // Create report parameters, get name from name attribute of current node, then get value by name
                string paramname = node.Attributes["Name"].Value;
                ReportParameter rptparam = new ReportParameter(name: paramname);
                rptparam.Values.Add(paramvalues[paramname]);

                // Set current report parameter for the report  
                localReport.SetParameters(new ReportParameter[] { rptparam });
            }

            // create the datasets
            foreach (XmlNode dsname in DataSetName)
            {
                string dsnamestr = dsname.Attributes["Name"].Value;

                DataSet ds = new DataSet(dsnamestr);
                FillDataset(ref ds, dsnamestr);

                // Create report data source 
                ReportDataSource rds = new ReportDataSource();
                rds.Name = dsnamestr;
                rds.Value = ds.Tables[dsnamestr];

                localReport.DataSources.Add(rds);
            }
        }

        private void FillDataset(ref DataSet ds, string dsname)
        {
            string sqltxt;
            SQLQueryText.TryGetValue(dsname, out sqltxt);

            XmlNodeList splist;
            SQLParameters.TryGetValue(dsname, out splist);

            using (SqlConnection connection = new SqlConnection(_SQLConnection))
            {
                SqlCommand command = new SqlCommand(sqltxt, connection);
                foreach (XmlNode sqlparam in splist)
                {
                    string pname = sqlparam.Value.Remove(0, 1);
                    command.Parameters.Add(new SqlParameter(pname, paramvalues[pname]));
                }
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds, dsname);
            }
        }

        private void ReadXML(string filepath)
        {
            // load the RDL as an XML document
            var xmlDoc2 = new XmlDocument();
            xmlDoc2.Load(filepath);
            var nsmgr = new XmlNamespaceManager(xmlDoc2.NameTable);

            // xml namespaces
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"); // .NET is stupid about default namespaces. you have to assign them an actual name then use that for every single node.
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            // Parse the components of the XML RDL

            //      DataSet Names
            DataSetName = xmlDoc2.SelectNodes("/x:Report/x:DataSets/*", nsmgr);

            //      SQLParameterName for each dataset in the report
            foreach (XmlNode xmlnode in DataSetName)
            {
                foreach (XmlAttribute dsn in xmlnode.Attributes)
                {
                    string x = dsn.Value;

                    //SQLParameterName
                    XmlNodeList sp = xmlDoc2.SelectNodes("//x:DataSet[@Name = '" + x + "']/x:Query/x:QueryParameters/x:QueryParameter/@Name", nsmgr);
                    SQLParameters.Add(x, sp);

                    //SQLQueryText
                    SQLQueryText.Add(x, xmlDoc2.SelectNodes("//x:DataSet[@Name='" + x + "']/x:Query/x:CommandText/text()", nsmgr)[0].Value);
                }
            }

            //      ReportParameters
            ReportParameters = xmlDoc2.SelectNodes("/x:Report/x:ReportParameters/*", nsmgr); // these are report parameters, not sql parameters

        }
    }
}
