using Advent.MediaCenter.StartMenu.OEM;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal class EntryPointCapabilityPresenter
    {
        public EntryPointPresenter EntryPoint { get; private set; }

        public EntryPointCapabilities Capability { get; private set; }

        public bool IsRequired
        {
            get
            {
                return (this.EntryPoint.Model.CapabilitiesRequired & this.Capability) == this.Capability;
            }
            set
            {
                if (this.IsRequired == value)
                    return;
                if (value)
                    this.EntryPoint.Model.CapabilitiesRequired |= this.Capability;
                else
                    this.EntryPoint.Model.CapabilitiesRequired &= ~this.Capability;
            }
        }

        internal EntryPointCapabilityPresenter(EntryPointPresenter entryPoint, EntryPointCapabilities capability)
        {
            this.EntryPoint = entryPoint;
            this.Capability = capability;
        }
    }
}
