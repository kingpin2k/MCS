
using System;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [ComVisible(false)]
    internal class InstallReferenceGuid
    {
        public static readonly Guid UninstallSubkeyGuid = new Guid("8cedc215-ac4b-488b-93c0-a50a49cb2fb8");
        public static readonly Guid FilePathGuid = new Guid("b02f9d65-fb77-4f7a-afa5-b391309f11c9");
        public static readonly Guid OpaqueGuid = new Guid("2ec93463-b0c3-45e1-8364-327e96aea856");
        public static readonly Guid MsiGuid = new Guid("25df0fc1-7f97-4070-add7-4b13bbfd7cb8");
        public static readonly Guid OsInstallGuid = new Guid("d16d444c-56d8-11d5-882d-0080c847b195");

        static InstallReferenceGuid()
        {
        }

        public static bool IsValidGuidScheme(Guid guid)
        {
            if (!guid.Equals(InstallReferenceGuid.UninstallSubkeyGuid) && !guid.Equals(InstallReferenceGuid.FilePathGuid) && !guid.Equals(InstallReferenceGuid.OpaqueGuid))
                return guid.Equals(Guid.Empty);
            else
                return true;
        }
    }
}
