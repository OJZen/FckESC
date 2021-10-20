using FckESC.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckESC.ViewModel
{
    class LoginResultViewModel : INotifyPropertyChanged
    {
        public static LoginResultViewModel viewModel = null;

        public event PropertyChangedEventHandler PropertyChanged;

        private string info;

        public string LoginInfo
        {
            get { return info; }
            set
            {
                info = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LoginInfo"));
                }
            }
        }

        public static LoginResultViewModel GetInstance()
        {
            if (viewModel == null)
            {
                viewModel = new LoginResultViewModel();
            }
            return viewModel;
        }

    }
}
