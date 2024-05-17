using System.Windows.Input;

namespace WiiUInjector.ViewModels
{
    public interface ICommonKeyViewModel
    {
        string CommonKey { get; }
        ICommand OpenDialogCommand { get; }
        ICommand SetKeyCommand { get; }
    }
}