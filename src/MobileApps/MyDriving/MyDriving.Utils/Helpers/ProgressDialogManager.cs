using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDriving.Utils.Helpers
{
    public static class ProgressDialogManager
    {
        private static Acr.UserDialogs.IProgressDialog currentProgressDialog;

        public static void LoadProgressDialog(string title)
        {
            currentProgressDialog = Acr.UserDialogs.UserDialogs.Instance.Loading(title, maskType: Acr.UserDialogs.MaskType.Clear);
        }

        public static void HideProgressDialog()
        {
            currentProgressDialog?.Hide();
        }

        public static void ShowProgressDialog()
        {
            currentProgressDialog?.Show();
        }

        public static void DisposeProgressDialog()
        {
            currentProgressDialog?.Dispose();
            currentProgressDialog = null;
        }
    }
}
