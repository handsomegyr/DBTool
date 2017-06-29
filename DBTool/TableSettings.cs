using System.ComponentModel;
using System.Collections.Generic;

namespace DBTool
{
    public class TableSettings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string tableName =  "";
        public string TableName
        {
            get { return tableName; }
            set
            {
                tableName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TableName"));
            }
        }


        private string tableComment = "";
        public string TableComment
        {
            get { return tableComment; }
            set
            {
                tableComment = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TableComment"));
            }
        }

        private string folderName = "";
        public string FolderName
        {
            get { return folderName; }
            set
            {
                folderName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
                OnPropertyChanged(new PropertyChangedEventArgs("FolderName"));
            }
        }

        private string className = "";
        public string ClassName
        {
            get { return className; }
            set
            {
                className = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
                OnPropertyChanged(new PropertyChangedEventArgs("ClassName"));
            }
        }

        private List<FieldSettings> fieldSettingsList = new List<FieldSettings>();
        public List<FieldSettings> FieldSettingsList
        {
            get { return fieldSettingsList; }
            set
            {
                fieldSettingsList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FieldSettingsList"));
            }
        }

    }
}
