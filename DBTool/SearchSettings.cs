using System.ComponentModel;

namespace DBTool
{
    public class SearchSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        
        private bool isShow = false;
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

        private ConditionType selectConditionType = new ConditionType { TypeName = "未指定", TypeValue = "" };
        public ConditionType SelectConditionType
        {
            get { return selectConditionType; }
            set
            {
                selectConditionType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectConditionType"));
            }
        }

    }
}
