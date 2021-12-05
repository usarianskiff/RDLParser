using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RDLParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            if(o.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtXML.Text = File.ReadAllText(o.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtXPath_TextChanged(object sender, EventArgs e)
        {
            var xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(txtXML.Text);
            var nsmgr = new XmlNamespaceManager(xmlDoc2.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");


            try
            {
                lblError.Visible = false;
                var nodes = xmlDoc2.SelectNodes(txtXPath.Text, nsmgr);
                lblMatch.Text = nodes.Count.ToString();

                string tempTxt = txtXML.Text;
                txtXML.Clear();
                txtXML.Text = tempTxt;

                foreach (XmlNode node in nodes)
                {
                    var attributes = node.Attributes;
                    string word = node.Value;
                    string nodename = node.Name;
                    

                    int startindex = 0;
                    if (word != null)
                    {
                        while (startindex < txtXML.TextLength)
                        {
                            int wordstartIndex = txtXML.Find(word, startindex, RichTextBoxFinds.None);
                            if (wordstartIndex != -1)
                            {
                                txtXML.SelectionStart = wordstartIndex;
                                txtXML.SelectionLength = word.Length;
                                txtXML.SelectionBackColor = Color.Yellow;
                            }
                            else
                                break;
                            startindex += wordstartIndex + word.Length;
                        }
                    }

                    if (nodename != null)
                    {
                        startindex = 0;
                        while (startindex < txtXML.TextLength)
                        {
                            int wordstartIndex = txtXML.Find(nodename, startindex, RichTextBoxFinds.None);
                            if (wordstartIndex != -1)
                            {
                                txtXML.SelectionStart = wordstartIndex;
                                txtXML.SelectionLength = nodename.Length;
                                txtXML.SelectionBackColor = Color.Green;
                            }
                            else
                                break;
                            startindex += wordstartIndex + nodename.Length;
                        }
                    }

                    if (attributes != null)
                    {
                        foreach (var attribute in attributes)
                        {
                            startindex = 0;
                            while (startindex < txtXML.TextLength)
                            {
                                int wordstartIndex = txtXML.Find(attribute.ToString(), startindex, RichTextBoxFinds.None);
                                if (wordstartIndex != -1)
                                {
                                    txtXML.SelectionStart = wordstartIndex;
                                    txtXML.SelectionLength = attribute.ToString().Length;
                                    txtXML.SelectionBackColor = Color.Blue;
                                }
                                else
                                    break;
                                startindex += wordstartIndex + attribute.ToString().Length;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.ToString();
            }
        }
    }
}
