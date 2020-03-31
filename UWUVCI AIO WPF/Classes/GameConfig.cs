﻿using GameBaseClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWUVCI_AIO_WPF.Classes;

namespace UWUVCI_AIO_WPF
{
    [Serializable]
    class GameConfig
    {
        public GameConsoles Console { get; set; }
        public GameBases BaseRom { get; set; }
        public string CBasePath { get; set; }
        public string GameName { get; set; }
        public PNGTGA TGAIco { get; set; }
        public PNGTGA TGADrc { get; set; }
        public PNGTGA TGATv { get; set; }
        public PNGTGA TGALog { get; set; }
        public N64Conf N64Stuff { get; set; }

    }
}