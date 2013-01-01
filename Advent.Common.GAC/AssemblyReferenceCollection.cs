
using System.Collections;
using System.Collections.Generic;

namespace Advent.Common.GAC
{
    internal class AssemblyReferenceCollection : IEnumerable<AssemblyReference>, IEnumerable
    {
        private readonly string assemblyName;

        public AssemblyReferenceCollection(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        public IEnumerator<AssemblyReference> GetEnumerator()
        {
            return (IEnumerator<AssemblyReference>)new AssemblyReferenceEnumerator(this.assemblyName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
