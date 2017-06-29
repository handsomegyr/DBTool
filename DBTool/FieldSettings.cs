using System.ComponentModel;

namespace DBTool
{
    public class FieldSettings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string fieldName =  "";
        public string FieldName
        {
            get { return fieldName; }
            set
            {
                fieldName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FieldName"));
            }
        }


        private string fieldComment = "";
        public string FieldComment
        {
            get { return fieldComment; }
            set
            {
                fieldComment = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FieldComment"));
            }
        }

        private DbSettings dataDbSettings = new DbSettings();
        public DbSettings DataDbSettings
        {
            get { return dataDbSettings; }
            set
            {
                dataDbSettings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataDbSettings"));
            }
        }

        private FormSettings dataFormSettings = new FormSettings();
        public FormSettings DataFormSettings
        {
            get { return dataFormSettings; }
            set
            {
                dataFormSettings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataFormSettings"));
            }
        }


        private ListSettings dataListSettings = new ListSettings();
        public ListSettings DataListSettings
        {
            get { return dataListSettings; }
            set
            {
                dataListSettings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataListSettings"));
            }
        }

        private SearchSettings dataSearchSettings = new SearchSettings();
        public SearchSettings DataSearchSettings
        {
            get { return dataSearchSettings; }
            set
            {
                dataSearchSettings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataSearchSettings"));
            }
        }

        private ExportSettings dataExportSettings = new ExportSettings();
        public ExportSettings DataExportSettings
        {
            get { return dataExportSettings; }
            set
            {
                dataExportSettings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataExportSettings"));
            }
        }


        private bool isSelected = true;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private string status= "未处理";
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Status"));
            }
        }

    }
}
