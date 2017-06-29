using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBTool
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow2 : Window
    {
        protected FieldSettings objSettings = new FieldSettings();
        public FieldSettings getSettings()
        {
            this.objSettings.Status = "未处理";
            return this.objSettings;
        }
        public void setSettings(FieldSettings objSettings)
        {
            this.objSettings = objSettings;
        }

        protected bool isCanceled = false;
        private List<DataType> dataTypeList = new List<DataType>();
        private List<InputType> inputTypeList = new List<InputType>();
        private List<InputType> inputTypeList4Search = new List<InputType>();
        private List<ConditionType> conditionTypeList4Search = new List<ConditionType>();
        public SettingsWindow2()
        {
            InitializeComponent();
            dataTypeList.Add(new DataType { TypeName = "固定长度字符串", TypeValue = "char" });
            dataTypeList.Add(new DataType { TypeName = "可变长度字符串", TypeValue = "varchar" });
            dataTypeList.Add(new DataType { TypeName = "JSON", TypeValue = "json" });
            dataTypeList.Add(new DataType { TypeName = "数组", TypeValue = "array" });
            dataTypeList.Add(new DataType { TypeName = "文本", TypeValue = "text" });

            dataTypeList.Add(new DataType { TypeName = "时间", TypeValue = "datetime" });
            dataTypeList.Add(new DataType { TypeName = "是否选择", TypeValue = "boolean" });
            dataTypeList.Add(new DataType { TypeName = "数字", TypeValue = "integer" });
            dataTypeList.Add(new DataType { TypeName = "文件", TypeValue = "file" });


            // 表单画面输入框展示
            inputTypeList.Add(new InputType { TypeName = "单行输入框", TypeValue = "text" });
            inputTypeList.Add(new InputType { TypeName = "多行输入框", TypeValue = "textarea" });

            inputTypeList.Add(new InputType { TypeName = "整数输入框", TypeValue = "number" });
            inputTypeList.Add(new InputType { TypeName = "货币输入框", TypeValue = "currency" });
            inputTypeList.Add(new InputType { TypeName = "浮点数输入框", TypeValue = "decimal" });

            inputTypeList.Add(new InputType { TypeName = "文件输入框", TypeValue = "file" });
            inputTypeList.Add(new InputType { TypeName = "ueditor富文本编辑器", TypeValue = "ueditor" }); 
            inputTypeList.Add(new InputType { TypeName = "ckeditor富文本编辑器", TypeValue = "ckeditor" });
            inputTypeList.Add(new InputType { TypeName = "单选框", TypeValue = "radio" });
            inputTypeList.Add(new InputType { TypeName = "本地数据选择框", TypeValue = "select" });
            inputTypeList.Add(new InputType { TypeName = "远程数据选择框", TypeValue = "select2" });            
            inputTypeList.Add(new InputType { TypeName = "日期输入框", TypeValue = "datetimepicker" });
            inputTypeList.Add(new InputType { TypeName = "图片输入框", TypeValue = "image" });
            inputTypeList.Add(new InputType { TypeName = "隐藏输入框", TypeValue = "hidden" });
            inputTypeList.Add(new InputType { TypeName = "自定义", TypeValue = "partial" });

            // 检索画面输入框展示
            inputTypeList4Search.Add(new InputType { TypeName = "单行输入框", TypeValue = "text" });
            inputTypeList4Search.Add(new InputType { TypeName = "整数输入框", TypeValue = "number" });
            inputTypeList4Search.Add(new InputType { TypeName = "数据选择框", TypeValue = "select" });
            inputTypeList4Search.Add(new InputType { TypeName = "日期输入框", TypeValue = "datetimepicker" });

            // 检索画面输入框条件展示
            conditionTypeList4Search.Add(new ConditionType { TypeName = "未指定", TypeValue = "" });
            conditionTypeList4Search.Add(new ConditionType { TypeName = "区间指定", TypeValue = "period" });

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.isCanceled = false;
            this.Close();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.isCanceled)
            {
                this.DialogResult = false;
                return;
            }
            this.DialogResult = false;
            e.Cancel = true;

            if (string.IsNullOrEmpty(this.txtName.Text.Trim()))
            {
                MessageBox.Show("请输入字段名称");
                return;

            }

            if (string.IsNullOrEmpty(this.txtComment.Text.Trim()))
            {
                MessageBox.Show("请输入字段说明");
                return;

            }
            e.Cancel = false;
            this.DialogResult = true;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dataType = from c1 in dataTypeList
                          where c1.TypeValue == this.objSettings.DataDbSettings.SelectDataType.TypeValue
                          select c1;
            this.objSettings.DataDbSettings.SelectDataType = dataType.FirstOrDefault();

            var inputType = from c2 in inputTypeList
                           where c2.TypeValue == this.objSettings.DataFormSettings.SelectInputType.TypeValue
                           select c2;
            this.objSettings.DataFormSettings.SelectInputType = inputType.FirstOrDefault();

            var inputType4Search = from c3 in inputTypeList4Search
                            where c3.TypeValue == this.objSettings.DataSearchSettings.SelectInputType.TypeValue
                            select c3;
            this.objSettings.DataSearchSettings.SelectInputType = inputType4Search.FirstOrDefault();

            var conditionType4Search = from c4 in conditionTypeList4Search
                                   where c4.TypeValue == this.objSettings.DataSearchSettings.SelectConditionType.TypeValue
                                   select c4;
            this.objSettings.DataSearchSettings.SelectConditionType = conditionType4Search.FirstOrDefault();


            this.myGrid.DataContext = this.objSettings;
            this.cbDataType.ItemsSource = dataTypeList;
            this.cbInputType.ItemsSource = inputTypeList;
            this.cbSearchInputType.ItemsSource = inputTypeList4Search;
            this.cbSearchConditionType.ItemsSource = conditionTypeList4Search;

            this.isCanceled = false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.isCanceled = true;
            this.Close();
        }
    }
}
