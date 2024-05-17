namespace UWUVCI_AIO_WPF
{
    public interface IEnvironmentService
    {
        bool Debug { get; }
        bool AllowSpaceBypass { get; }
        bool SkipInstanceCheck { get; }
    }

}
