using System.ComponentModel;

namespace DBTool
{
    public class DbSettings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private bool isNotNull=true;
        public bool IsNotNull
        {
            get { return isNotNull; }
            set
            {
                isNotNull = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsNotNull"));
            }
        }

        
        private DataType selectDataType = new DataType {TypeName="字符串",TypeValue="varchar" };
        public DataType SelectDataType
        {
            get { return selectDataType; }
            set
            {
                selectDataType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectDataType"));
            }
        }

        private uint dataLength = 0;
        public uint DataLength
        {
            get { return dataLength; }
            set
            {
                dataLength = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DataLength"));
            }

        }
        private string defaultValue = "";
        public string DefaultValue
        {
            get { return defaultValue; }
            set
            {
                defaultValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DefaultValue"));
            }
        }

    }
}
