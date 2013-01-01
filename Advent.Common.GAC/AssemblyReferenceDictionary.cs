
using System.Collections.Generic;

namespace Advent.Common.GAC
{
    public class AssemblyReferenceDictionary
    {
        public IEnumerable<AssemblyReference> this[string assemblyName]
        {
            get
            {
                return (IEnumerable<AssemblyReference>)new AssemblyReferenceCollection(assemblyName);
            }
        }

        internal AssemblyReferenceDictionary()
        {
        }
    }
}
