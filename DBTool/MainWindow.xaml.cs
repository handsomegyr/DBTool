using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;

namespace DBTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TableSettings tableSettings = new TableSettings();
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var objSettingsWindow = new SettingsWindow2();
            objSettingsWindow.setSettings(new FieldSettings());
            if (objSettingsWindow.ShowDialog().Value)
            {
                tableSettings.FieldSettingsList.Add(objSettingsWindow.getSettings());
                refreshBind();                
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var command = sender as Button;
            var fieldname = command.Tag as string;
            var idx = tableSettings.FieldSettingsList.FindIndex(i => i.FieldName == fieldname);
            
            var objSettingsWindow = new SettingsWindow2();
            objSettingsWindow.setSettings(tableSettings.FieldSettingsList[idx]);
            if (objSettingsWindow.ShowDialog().Value)
            {
                tableSettings.FieldSettingsList[idx] = objSettingsWindow.getSettings();
                refreshBind();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var command = sender as Button;
            var fieldname = command.Tag as string;
            var idx = tableSettings.FieldSettingsList.FindIndex(i => i.FieldName == fieldname);
            tableSettings.FieldSettingsList.RemoveAt(idx);
            refreshBind();
        }

        private void btnload_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Windows.Forms.Application.StartupPath;//初始目录，不赋值也可以
            openFileDialog1.Filter = "xml文件(*.xml) | *.xml";//文件类型

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBlock2.Text = "你选择的文件为:" + openFileDialog1.FileName;

                XmlSerializer xs = new XmlSerializer(typeof(TableSettings));
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                var tableSettings = xs.Deserialize(sr) as TableSettings;
                sr.Close();
                if (tableSettings == null)
                {                    
                    MessageBox.Show("加载失败");
                }
                tableSettings.FieldSettingsList.ForEach(f => f.Status = "未处理");
                this.tableSettings = tableSettings;
                refreshBind();
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            this.textBlock2.Text = "";
            this.textBlock3.Text = "";
            button1.IsEnabled = true;
            cmdCancel.IsEnabled = false;
            this.txtTableName.DataContext = this.tableSettings;
            this.txtTableComment.DataContext = this.tableSettings;
            this.txtFolderName.DataContext = this.tableSettings;
            this.txtClassName.DataContext = this.tableSettings;
            listBox1.ItemsSource = tableSettings.FieldSettingsList;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isOk = Worker.merge(tableSettings, backgroundWorker);

            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // Return the result.
            e.Result = isOk;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("已取消.");
            }
            else if (e.Error != null)
            {
                // An error was thrown by the DoWork event handler.
                MessageBox.Show(e.Error.Message, "处理失败");
            }
            else
            {
                MessageBox.Show("处理结束");
            }
            button1.IsEnabled = true;
            cmdCancel.IsEnabled = false;
            progressBar1.Value = 0;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var selectNum = tableSettings.FieldSettingsList.Count(c => c.IsSelected == true);
            if (selectNum <= 0)
            {
                MessageBox.Show("请至少指定一个字段");
                return;
            }

            if (string.IsNullOrEmpty(tableSettings.TableName))
            {
                MessageBox.Show("请指定表名字");
                return;
            }

            if (string.IsNullOrEmpty(tableSettings.TableComment))
            {
                MessageBox.Show("请指定表描述");
                return;
            }

            if (string.IsNullOrEmpty(tableSettings.FolderName))
            {
                MessageBox.Show("请指定目录名字");
                return;

            }

            if (string.IsNullOrEmpty(tableSettings.ClassName))
            {
                MessageBox.Show("请指定类名字");
                return;
            }
            // Disable the button
            button1.IsEnabled = false;
            cmdCancel.IsEnabled = true;
            this.progressBar1.Maximum = tableSettings.FieldSettingsList.Count;
            // Start the process on another thread.
            backgroundWorker.RunWorkerAsync();
        }

        private void refreshBind()
        {
            this.textBlock3.Text = "表字段数量为:" + tableSettings.FieldSettingsList.Count;
            this.txtTableName.DataContext = this.tableSettings;
            this.txtTableComment.DataContext = this.tableSettings;
            this.txtFolderName.DataContext = this.tableSettings;
            this.txtClassName.DataContext = this.tableSettings;
            listBox1.ItemsSource = null;
            listBox1.ItemsSource = tableSettings.FieldSettingsList;
        }

       
    }
}
