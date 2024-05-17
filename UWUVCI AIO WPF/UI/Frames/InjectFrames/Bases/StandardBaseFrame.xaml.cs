using System.Windows;
using System.Windows.Controls;
using WiiUInjector.ViewModels;

namespace UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Bases
{
    internal partial class StandardBaseFrame : Page
    {
        readonly BaseViewModel _gameBaseViewModel;

        public StandardBaseFrame(BaseViewModel gameBase)
        {
            InitializeComponent();
            this._gameBaseViewModel = gameBase;
            this.DataContext = _gameBaseViewModel;
        }

        public StandardBaseFrame()
        {
            InitializeComponent();
        }

        private void BtnDwnln_Copy_Click(object sender, RoutedEventArgs e)
        {
            //var ek = new CommonKeyDialogWindow("Title Key: " + _gameBaseViewModel.Name + " " + _gameBaseViewModel.Region);
            //ek.Owner = Application.Current.MainWindow;
            //ek.ShowDialog();
            //var key = ek.EnteredKey;

            //mvm.EnterKey(false);
        }
    }
}
