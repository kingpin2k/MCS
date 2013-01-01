
using Microsoft.Win32;

namespace Advent.Common.Serialization
{
    public interface IRegistryKeySerialisable
    {
        bool BeforeSerialise(RegistrySerialiser rs, RegistryKey key);

        void AfterSerialise(RegistrySerialiser rs, RegistryKey key);

        bool BeforeDeserialise(RegistrySerialiser rs, RegistryKey key);

        void AfterDeserialise(RegistrySerialiser rs, RegistryKey key);
    }
}
