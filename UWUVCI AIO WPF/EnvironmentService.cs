using System;

namespace UWUVCI_AIO_WPF
{
    public class EnvironmentService : IEnvironmentService
    {
        public bool Debug { get; private set; }
        public bool AllowSpaceBypass { get; private set; }
        public bool SkipInstanceCheck { get; private set; }

        public EnvironmentService() 
        {
            var args = Environment.GetCommandLineArgs();
            foreach(var arg in args)
            {
                if (arg == "--skip") SkipInstanceCheck = true;
                if (arg == "--debug") Debug = true;
                if (arg == "--spacebypass") AllowSpaceBypass = true;    
            }
        }
    }

}
