using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Microsoft.SharePoint.Client;
using Microsoft.Win32;
using File = System.IO.File;

namespace Oxbow.InfopathDeploymentUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _appLocation;
        private readonly string _cabArcLocation;
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
        private readonly SaveFileDialog _saveFileDialog = new SaveFileDialog();
        private bool _isDebugMode;

        public MainWindow()
        {
            InitializeComponent();
            _openFileDialog.FileOk += _openFileDialog_FileOk;
            _saveFileDialog.FileOk += _saveFileDialog_FileOk;
            _appLocation = Environment.CurrentDirectory;
            _cabArcLocation = Environment.CurrentDirectory + "\\cabarc.exe";
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            var clientContext = new ClientContext(txtSiteUrl.Text);
            Web web = clientContext.Web;
            clientContext.Load(web);
            cmbContentType.Items.Clear();
            clientContext.Load(web.AvailableContentTypes);
            clientContext.ExecuteQuery();
            foreach (ContentType spContentType in web.AvailableContentTypes)
            {
                list.Add(spContentType.Name);
            }
            clientContext.ExecuteQuery();
            list.OrderBy(x => x).ToList().ForEach(x => cmbContentType.Items.Add(x));
        }

        private void RefreshCompare()
        {
            for (int j = 0; j < lstInfopathFields.Items.Count; j++)
            {
                var formItem = (ComboBoxItem) lstInfopathFields.Items[j];
                formItem.Background = new SolidColorBrush(Colors.White);
            }
            for (int i = 0; i < lstServerFields.Items.Count; i++)
            {
                var serverItem = (ComboBoxItem) lstServerFields.Items[i];
                serverItem.Background = new SolidColorBrush(Colors.White);
                string serverColumnName = serverItem.Content.ToString().Split(new[] {','})[0];
                string serverColumnId = serverItem.Content.ToString().Split(new[] {','})[1];
                bool found = false, matches = false;
                for (int j = 0; j < lstInfopathFields.Items.Count; j++)
                {
                    var formItem = (ComboBoxItem) lstInfopathFields.Items[j];
                    string formColumnName = formItem.Content.ToString().Split(new[] {','})[0];
                    string formColumnId = formItem.Content.ToString().Split(new[] {','})[1];
                    if (formColumnName == serverColumnName)
                    {
                        found = true;
                        matches = formColumnId == serverColumnId;
                        formItem.Background = matches ? new SolidColorBrush(Colors.LawnGreen) : new SolidColorBrush(Colors.Pink);
                        formItem.Tag = "x";
                        break;
                    }
                }
                if (found)
                    serverItem.Background = matches ? new SolidColorBrush(Colors.LawnGreen) : new SolidColorBrush(Colors.Pink);
            }
        }

        private void cmbContentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstServerFields.Items.Clear();
            if (string.IsNullOrEmpty((cmbContentType.SelectedValue ?? "").ToString()))
                return;
            var fields = new List<string>();
            var clientContext = new ClientContext(txtSiteUrl.Text);
            Web web = clientContext.Web;
            clientContext.Load(web);
            clientContext.Load(web.AvailableContentTypes);
            clientContext.ExecuteQuery();
            ContentType contentType = null;
            foreach (ContentType ct in web.AvailableContentTypes)
            {
                if (ct.Name.ToLower().Trim() == cmbContentType.SelectedValue.ToString().ToLower().Trim())
                    contentType = ct;
            }
            if (contentType == null)
                throw new Exception("Unable to get the content type.");
            clientContext.Load(contentType.Fields);
            clientContext.ExecuteQuery();
            foreach (Field field in contentType.Fields)
            {
                fields.Add(string.Format("{0},{{{1}}}", field.Title, field.Id.ToString().ToLower()));
            }
            fields.OrderBy(x => x).ToList().ForEach(x => lstServerFields.Items.Add(new ComboBoxItem {Content = x}));
            RefreshCompare();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            _openFileDialog.Filter = "(InfoPath Form Template)|*.xsn";
            _openFileDialog.ShowDialog();
        }

        private void _openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            txtInfoPathFileLocation.Text = _openFileDialog.FileName;
            string fileName = new FileInfo(_openFileDialog.FileName).Name;
            Environment.CurrentDirectory = _appLocation;
            try
            {
                ExecuteCommandSync("rmdir /s /q tmp");
            }
            catch
            {
            }
            ExecuteCommandSync("mkdir tmp");
            ExecuteCommandSync("copy " + _openFileDialog.FileName + " " + _appLocation + "\\tmp");
            Environment.CurrentDirectory = _appLocation + "\\tmp\\";
            ExecuteCommandSync(_cabArcLocation + " x " + _appLocation + "\\tmp\\" + fileName);
            ExecuteCommandSync("del " + _appLocation + "\\tmp\\" + fileName);
            LoadManifestFile();
        }

        public void LoadManifestFile()
        {
            var fields = new List<string>();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_appLocation + "\\tmp\\manifest.xsf");
            var nsManager = new XmlNamespaceManager(xmlDocument.NameTable);
            nsManager.AddNamespace("xsf", "http://schemas.microsoft.com/office/infopath/2003/solutionDefinition");
            nsManager.AddNamespace("xsf2", "http://schemas.microsoft.com/office/infopath/2006/solutionDefinition/extensions");
            XmlNodeList promotedFields = xmlDocument.SelectNodes("//xsf:field", nsManager);
            foreach (XmlNode promotedField in promotedFields)
            {
                string infopathColumnName = promotedField.Attributes["columnName"].Value;
                XmlNode fieldExtension = xmlDocument.SelectSingleNode(string.Format("//xsf2:fieldExtension[@columnName='{0}']", infopathColumnName), nsManager);
                if (fieldExtension != null)
                {
                    fields.Add(promotedField.Attributes["name"].Value + "," + fieldExtension.Attributes["columnId"].Value.ToLower());
                }
            }
            lstInfopathFields.Items.Clear();
            fields.OrderBy(x => x).ToList().ForEach(x => lstInfopathFields.Items.Add(new ComboBoxItem {Content = x}));
            RefreshCompare();
        }

        public void ExecuteCommandSync(string command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run, and "/c " as the parameters.
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                //procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                if (_isDebugMode)
                    MessageBox.Show(string.Format("Command: {0} \nResult:{1}", command, result));
            }
            catch (Exception exp)
            {
                MessageBox.Show(string.Format("Command: {0} ERROR: [{1}]", command, exp));
            }
        }

        private void btnSyncForm_Click(object sender, RoutedEventArgs e)
        {
            string manifest = File.ReadAllText(_appLocation + "\\tmp\\manifest.xsf");
            foreach (ComboBoxItem formItem in lstInfopathFields.Items)
            {
                if (formItem.Tag == "x")
                {
                    string formColumnName = formItem.Content.ToString().Split(new[] {','})[0];
                    string formColumnId = formItem.Content.ToString().Split(new[] {','})[1];
                    ComboBoxItem serverItem = null;
                    try
                    {
                        foreach (ComboBoxItem i in lstServerFields.Items)
                        {
                            if (i.Content.ToString().StartsWith(formColumnName))
                            {
                                serverItem = i;
                                break;
                            }
                        }
                        if (serverItem == null)
                            throw new Exception("not found.");
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(string.Format("An error occurred when trying to update [{0}] in the form to the new guid: [{1}]", formColumnName, exp));
                    }
                    string serverColumnId = serverItem.Content.ToString().Split(new[] {','})[1];
                    manifest = manifest.Replace("<xsf2:fieldExtension columnId=\"" + formColumnId + "\"", "<xsf2:fieldExtension columnId=\"" + serverColumnId + "\"");
                }
            }
            File.WriteAllText(_appLocation + "\\tmp\\manifest.xsf", manifest);
            LoadManifestFile();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_openFileDialog.FileName))
                _saveFileDialog.FileName = _openFileDialog.FileName;
            _saveFileDialog.Filter = "(InfoPath Form Template)|*.xsn";
            _saveFileDialog.ShowDialog();
        }

        private void _saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = new FileInfo(_saveFileDialog.FileName).Name;
            Environment.CurrentDirectory = _appLocation + "\\tmp\\";
            ExecuteCommandSync(_cabArcLocation + " n " + _appLocation + "\\tmp\\" + fileName + " *");
            ExecuteCommandSync("copy " + _appLocation + "\\tmp\\" + fileName + " " + _saveFileDialog.FileName);
            Environment.CurrentDirectory = _appLocation;
            try
            {
                ExecuteCommandSync("rmdir /s /q tmp");
            }
            catch
            {
            }
        }
    }
}