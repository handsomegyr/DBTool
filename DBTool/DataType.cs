using System.ComponentModel;

namespace DBTool
{
    public class DataType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string typeName;
        public string TypeName
        {
            get { return typeName; }
            set
            {
                typeName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TypeName"));
            }
        }

        private string typeValue;
        public string TypeValue
        {
            get { return typeValue; }
            set
            {
                typeValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TypeValue"));
            }
        }
        

    }
}
