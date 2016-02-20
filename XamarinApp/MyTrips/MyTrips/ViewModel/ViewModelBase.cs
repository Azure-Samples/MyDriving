using MvvmHelpers;
using MyTrips.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {

        public static void Init()
        {
        }

        public Settings Settings
        {
            get { return Helpers.Settings.Current; }
        }

    }

}
