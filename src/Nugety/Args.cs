using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Nugety
{
    public class ModuleIntanceEventArgs : EventArgs
    {
        public ModuleIntanceEventArgs(ModuleInfo module, object value)
        {
            this.Module = module;
            this.Value = value;
        }

        public ModuleInfo Module { get; set; }

        public object Value { get; set; }
    }

    public class CancelModuleEventArgs : CancelEventArgs
    {
        public CancelModuleEventArgs(ModuleInfo module)
        {
            this.Module = module;
        }

        public ModuleInfo Module { get; set; }
    }
}
