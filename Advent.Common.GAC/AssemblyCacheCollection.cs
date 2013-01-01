
using System.Collections;
using System.Collections.Generic;

namespace Advent.Common.GAC
{
    internal class AssemblyCacheCollection : IEnumerable<string>, IEnumerable
    {
        private readonly string assemblyFilter;

        public AssemblyCacheCollection()
        {
            this.assemblyFilter = (string)null;
        }

        public AssemblyCacheCollection(string assemblyFilter)
        {
            this.assemblyFilter = assemblyFilter;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return (IEnumerator<string>)new AssemblyEnumerator(this.assemblyFilter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
