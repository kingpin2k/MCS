
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Advent.Common.Serialization
{
    public class RegistrySerialiser
    {
        private readonly IDictionary context;

        public IDictionary Context
        {
            get
            {
                return this.context;
            }
        }

        public RegistrySerialiser()
        {
            this.context = (IDictionary)new ListDictionary();
        }

        public void Serialise(object o, RegistryKey parentKey)
        {
            this.Serialise(o, parentKey, true);
        }

        public void Serialise(object o, RegistryKey parentKey, bool isParentKey)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (parentKey == null)
                throw new ArgumentNullException("parentKey");
            PropertyInfo keyNameProperty = RegistrySerialiser.GetKeyNameProperty(o.GetType());
            if (keyNameProperty == null)
                throw new ArgumentException("At least one property must have a RegistryKeyName attribute.");
            RegistryKey key;
            if (isParentKey)
            {
                string subkey = keyNameProperty.GetValue(o, (object[])null) as string;
                if (subkey == null)
                    throw new SerializationException("Key name cannot be null.");
                key = parentKey.CreateSubKey(subkey);
            }
            else
                key = parentKey;
            try
            {
                IRegistryKeySerialisable registryKeySerialisable = o as IRegistryKeySerialisable;
                if (registryKeySerialisable != null && !registryKeySerialisable.BeforeSerialise(this, key))
                    return;
                foreach (PropertyInfo propertyInfo in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(RegistryValueAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(string))
                            throw new SerializationException("Cannot serialise an object of type " + propertyInfo.PropertyType.FullName + " to a registry value.");
                        RegistryValueAttribute registryValueAttribute = (RegistryValueAttribute)customAttributes[0];
                        string name = registryValueAttribute.Name ?? propertyInfo.Name;
                        object obj = propertyInfo.GetValue(o, (object[])null);
                        if (obj != null && registryValueAttribute.ValueKind != RegistryValueKind.Unknown && !RegistrySerialiser.GetRegistryType(registryValueAttribute.ValueKind).IsAssignableFrom(obj.GetType()))
                            obj = Convert.ChangeType(obj, RegistrySerialiser.GetRegistryType(registryValueAttribute.ValueKind));
                        object defaultValue;
                        RegistrySerialiser.TryGetDefaultValue((ICustomAttributeProvider)propertyInfo, out defaultValue);
                        if (obj != null && obj != defaultValue)
                            key.SetValue(name, obj);
                        else
                            key.DeleteValue(name, false);
                    }
                }
                if (registryKeySerialisable == null)
                    return;
                registryKeySerialisable.AfterSerialise(this, key);
            }
            finally
            {
                if (isParentKey)
                    key.Close();
            }
        }

        public void Deserialise(Type type, string key, IEnumerable<RegistryHive> hives)
        {
            if (type == null || key == null || hives == null)
                throw new ArgumentNullException();
            object o = (object)null;
            foreach (RegistryHive hive in hives)
            {
                RegistryKey key1 = RegistrySerialiser.GetKey(key, hive);
                if (key1 != null)
                {
                    if (o == null)
                        o = this.Deserialise(type, key1);
                    else
                        this.Deserialise(o, key1);
                }
            }
        }

        public void Deserialise(object o, RegistryKey key)
        {
            this.Deserialise(o, key, true);
        }

        public void Deserialise(object o, RegistryKey key, bool ignorePropertyErrors)
        {
            if (o == null || key == null)
                throw new ArgumentNullException();
            this.Deserialise(o, o.GetType(), key, ignorePropertyErrors);
        }

        public object Deserialise(Type type, RegistryKey key)
        {
            return this.Deserialise(type, key, true);
        }

        public object Deserialise(Type type, RegistryKey key, bool ignorePropertyErrors)
        {
            if (type == null || key == null)
                throw new ArgumentNullException();
            object instance = Activator.CreateInstance(type);
            this.Deserialise(instance, type, key, ignorePropertyErrors);
            return instance;
        }

        private static RegistryKey GetKey(string key, RegistryHive hive)
        {
            RegistryKey registryKey;
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    registryKey = Registry.ClassesRoot;
                    break;
                case RegistryHive.CurrentUser:
                    registryKey = Registry.CurrentUser;
                    break;
                case RegistryHive.LocalMachine:
                    registryKey = Registry.LocalMachine;
                    break;
                case RegistryHive.Users:
                    registryKey = Registry.Users;
                    break;
                case RegistryHive.PerformanceData:
                    registryKey = Registry.PerformanceData;
                    break;
                case RegistryHive.CurrentConfig:
                    registryKey = Registry.CurrentConfig;
                    break;
                //case RegistryHive.DynData:
                //    registryKey = Registry.DynData;
                //    break;
                default:
                    throw new ArgumentException("Unknown RegistryHive.");
            }
            return registryKey.OpenSubKey(key);
        }

        private static bool TryGetDefaultValue(ICustomAttributeProvider property, out object defaultValue)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            if (customAttributes != null && customAttributes.Length > 0)
            {
                DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)customAttributes[0];
                defaultValue = defaultValueAttribute.Value;
                return true;
            }
            else
            {
                defaultValue = (object)null;
                return false;
            }
        }

        private static Type GetRegistryType(RegistryValueKind kind)
        {
            switch (kind)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return typeof(string);
                case RegistryValueKind.Binary:
                    return typeof(byte[]);
                case RegistryValueKind.DWord:
                    return typeof(int);
                case RegistryValueKind.MultiString:
                    return typeof(string[]);
                case RegistryValueKind.QWord:
                    return typeof(long);
                default:
                    throw new NotImplementedException();
            }
        }

        private static PropertyInfo GetKeyNameProperty(Type type)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetCustomAttributes(typeof(RegistryKeyNameAttribute), true).Length > 0)
                {
                    if (propertyInfo.PropertyType != typeof(string))
                        throw new SerializationException("RegistryKeyName attribute can only apply to properties of type string.");
                    else
                        return propertyInfo;
                }
            }
            return (PropertyInfo)null;
        }

        private void Deserialise(object o, Type type, RegistryKey key, bool ignorePropertyErrors)
        {
            ISupportInitialize supportInitialize = o as ISupportInitialize;
            if (supportInitialize != null)
                supportInitialize.BeginInit();
            IRegistryKeySerialisable registryKeySerialisable = o as IRegistryKeySerialisable;
            if (registryKeySerialisable != null && !registryKeySerialisable.BeforeDeserialise(this, key))
                return;
            foreach (PropertyInfo propertyInfo in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(RegistryValueAttribute), true);
                if (customAttributes.Length > 0)
                {
                    string name = ((RegistryValueAttribute)customAttributes[0]).Name ?? propertyInfo.Name;
                    object obj = key.GetValue(name);
                    if (obj != null)
                    {
                        try
                        {
                            if (!propertyInfo.PropertyType.IsAssignableFrom(obj.GetType()))
                                obj = Convert.ChangeType(obj, propertyInfo.PropertyType);
                            propertyInfo.SetValue(o, obj, (object[])null);
                        }
                        catch (Exception ex)
                        {
                            if (!ignorePropertyErrors)
                                throw;
                            else
                                Trace.TraceError("Error converting property {0}.{1}: {2} Registry key is: {3}, Value is: {4}", (object)type.FullName, (object)propertyInfo.Name, (object)ex.Message, (object)key.Name, (object)obj.ToString());
                        }
                    }
                }
                else if (propertyInfo.GetCustomAttributes(typeof(RegistryKeyNameAttribute), true).Length > 0)
                    propertyInfo.SetValue(o, (object)key.Name.Substring(key.Name.LastIndexOf('\\') + 1), (object[])null);
            }
            if (registryKeySerialisable != null)
                registryKeySerialisable.AfterDeserialise(this, key);
            if (supportInitialize == null)
                return;
            supportInitialize.EndInit();
        }
    }
}
