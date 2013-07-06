using System.ComponentModel;
using TimeTracker.Annotations;

namespace TimeTracker.Model
{
    public class Tag : INotifyPropertyChanged
    {
        public enum TagType
        {
            BillingCode,
            UserDefined
        }

        public Tag()
        {
            _name = "New Tag";
        }
        private string _name;
        private TagType _type;
        private string _primaryBillingCode;
        private string _secondaryBillingCode;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public TagType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public string PrimaryBillingCode
        {
            get { return _primaryBillingCode; }
            set
            {
                if (value == _primaryBillingCode) return;
                _primaryBillingCode = value;
                OnPropertyChanged("PrimaryBillingCode");
            }
        }

        public string SecondaryBillingCode
        {
            get { return _secondaryBillingCode; }
            set
            {
                if (value == _secondaryBillingCode) return;
                _secondaryBillingCode = value;
                OnPropertyChanged("SecondaryBillingCode");
            }
        }

        public override string ToString()
        {
            return _name;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}