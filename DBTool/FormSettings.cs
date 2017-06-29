using System.ComponentModel;

namespace DBTool
{
    public class FormSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private bool isRequire = true;
        public bool IsRequire
        {
            get { return isRequire; }
            set
            {
                isRequire = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsRequire"));
            }
        }

        private bool isShow = true;
        public bool IsShow
        {
            get { return isShow; }
            set
            {
                isShow = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsShow"));
            }
        }

        private InputType selectInputType = new InputType { TypeName = "单行输入框", TypeValue = "text" };
        public InputType SelectInputType
        {
            get { return selectInputType; }
            set
            {
                selectInputType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectInputType"));
            }
        }


    }
}
