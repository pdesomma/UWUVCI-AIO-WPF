using System;
using System.Diagnostics;
using System.Windows.Input;
using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;
using System.Threading.Tasks;

namespace WiiUInjector.ViewModels
{
    public sealed class InjectionViewModel : ViewModel
    {
        private readonly Config _config;
        private readonly BaseRom _def;
        private readonly Metadata _metadata;
        private readonly IInjectionService _service;
        private readonly ICommonKeyViewModel _commonKeyViewModel;        
        private readonly BackgroundViewModel _backgroundTaskViewModel;
        private Injection _injection;
        private string _path;

        /// <summary>
        /// Creates a new instance of the <see cref="InjectionViewModel"/> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="def"></param>
        /// <param name="service"></param>
        /// <param name="commonKeyViewModel"></param>
        /// <param name="exceptionViewModel"></param>
        public InjectionViewModel(Config config, BaseRom def, IInjectionService service, 
            ICommonKeyViewModel commonKeyViewModel,
            BackgroundViewModel backgroundTaskViewModel,
            ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            _config = config;
            _def = def;           
            _service = service;
            _commonKeyViewModel = commonKeyViewModel;
            _backgroundTaskViewModel = backgroundTaskViewModel;

            ConfirmInjectionCommand = new BasicCommand(ExecuteConfirmInjectionCommand, y => true);
            CreateInjectionCommand = new BasicCommand(ExecuteCreateInjectionCommand, (y) => true);
            OpenInjectionPathCommand = new BasicCommand(x => Process.Start("explorer.exe", Path));
        }

        public bool IsLoadiine { get; set; }
        public string Name => _injection?.Name;
        public string Path => _path;
        public string ProdCode => _injection?.ProdCode;
        public ICommand CreateInjectionCommand { get; private set; }
        public ICommand OpenInjectionPathCommand { get; private set; }
        public ICommand ConfirmInjectionCommand { get; private set; }
        public NavigationViewModel NavigationViewModel { get; private set; }
        public bool Working { get; private set; } = false;

        /// <summary>
        /// Executes the <see cref="ConfirmInjectionCommand"/>.
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteConfirmInjectionCommand(object parameter)
        {
            Messenger.Send(new InjectionConfirmedNotification());
        }

        /// <summary>
        /// Creates the injection and packs it up.
        /// </summary>
        /// <returns></returns>
        private async void ExecuteCreateInjectionCommand(object parameter)
        {
            try
            {
                Working = true;
                RaisePropertyChange(nameof(Working));

                Task<Injection> task = null;
                switch (_config)
                {
                    case GbaConfig gbConfig:
                        task = _service.InjectGameBoyAsync(gbConfig, _def, _metadata, true);
                        break;
                    case GcConfig gcConfig:
                        task = _service.InjectGameCubeAsync(gcConfig, _def, _metadata, true);
                        break;
                    case MsxConfig msxConfig:
                        task = _service.InjectMsxAsync(msxConfig, _def, _metadata, true);
                        break;
                    case N64Config n64Config:
                        task = _service.InjectNintendo64Async(n64Config, _def, _metadata, true);
                        break;
                    case NdsConfig ndsConfig:
                        task = _service.InjectNintendoDsAsync(ndsConfig, _def, _metadata, true);
                        break;
                    case NesConfig nesConfig:
                        task = _service.InjectNesAsync(nesConfig, _def, _metadata, true);
                        break;
                    case SnesConfig snesConfig:
                        task = _service.InjectSuperNintendoAsync(snesConfig, _def, _metadata, true);
                        break;
                    case Tg16Config tgConfig:
                        task = _service.InjectTurboGrafx16Async(tgConfig, _def, _metadata, true);
                        break;
                }
                _backgroundTaskViewModel.Enqueue(task, "Creating injection");
                _injection = await task;

                if (_injection != null)
                {
                    if (IsLoadiine)
                    {
                        var p = _service.PackLoadiineAsync(_injection);
                        _backgroundTaskViewModel.Enqueue(p, "Creating Loadiine pack");
                        _path =  await p;
                    }
                    else
                    {
                        var p = _service.PackWupAsync(_injection, _commonKeyViewModel.CommonKey);
                        _backgroundTaskViewModel.Enqueue(p, "Creating Wup pack");
                        _path = await p;
                    }
                }
                RaisePropertyChange(nameof(Name));
                RaisePropertyChange(nameof(Path));
                RaisePropertyChange(nameof(ProdCode));
            }
            catch(Exception fx)
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(fx);
            }
            finally
            {
                Working = false;
                RaisePropertyChange(nameof(Working));
            }
        }
    }
}