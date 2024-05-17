using System.Windows;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    public partial class CommonKeyDialogWindow : Window
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommonKeyDialogWindow"/>
        /// </summary>
        /// <param name="title"></param>
        public CommonKeyDialogWindow()
        {
            InitializeComponent();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}