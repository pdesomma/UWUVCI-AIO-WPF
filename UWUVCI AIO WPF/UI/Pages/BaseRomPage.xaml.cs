using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WiiUInjector;

namespace UWUVCI_AIO_WPF.UI.Pages
{
    public partial class BaseRomPage : UserControl
    {
        protected List<BaseRom> gameBases = new List<BaseRom>();

        public BaseRomPage() 
        {
            InitializeComponent();
        }

        private void Copy_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (txtTitleId?.Text != null) Clipboard.SetText(txtTitleId?.Text);
        }
    }
}
