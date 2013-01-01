
namespace Advent.Common.GAC
{
    public class AssemblyReference
    {
        private InstallReference installReference;
        private string assembly;

        public string AssemblyName
        {
            get
            {
                return this.assembly;
            }
        }

        public string Description
        {
            get
            {
                return this.installReference.Description;
            }
        }

        public string InstalledApplicationID { get; private set; }

        public string OpaqueID { get; private set; }

        public string FilePath { get; private set; }

        internal InstallReference InstallReference
        {
            get
            {
                return this.installReference;
            }
        }

        internal AssemblyReference(string assemblyName, InstallReference installRef)
        {
            if (installRef.GuidScheme == InstallReferenceGuid.OpaqueGuid)
                this.OpaqueID = installRef.Identifier;
            else if (installRef.GuidScheme == InstallReferenceGuid.FilePathGuid)
                this.FilePath = installRef.Identifier;
            else if (installRef.GuidScheme == InstallReferenceGuid.UninstallSubkeyGuid)
                this.InstalledApplicationID = installRef.Identifier;
            this.assembly = assemblyName;
            this.installReference = installRef;
        }
    }
}
