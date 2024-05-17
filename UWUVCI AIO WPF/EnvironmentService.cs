using System;

namespace UWUVCI_AIO_WPF
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly string _skipInstanceCheckArg;
        private readonly string _allowSpaceBypassArg;
        private readonly string _debugArg;
        private readonly int _everett;
        private readonly int _oldAge = 1000;

        public EnvironmentService(string skipInstanceCheckArg, string allowSpaceBypassArg, string debugArg) 
        {
            _skipInstanceCheckArg = skipInstanceCheckArg;   
            _allowSpaceBypassArg = allowSpaceBypassArg;
            _debugArg = debugArg;

            var args = Environment.GetCommandLineArgs();
            foreach(var arg in args)
            {
                if (arg.ToLower() == _skipInstanceCheckArg.Trim('-').ToLower()) SkipInstanceCheck = true;
                if (arg.ToLower() == _debugArg.Trim('-').ToLower()) Debug = true;
                if (arg.ToLower() == _allowSpaceBypassArg.Trim('-').ToLower()) AllowSpaceBypass = true;    
            }
        }


        public bool Debug { get; private set; }
        public bool AllowSpaceBypass { get; private set; }
        public bool SkipInstanceCheck { get; private set; }
    }

}
