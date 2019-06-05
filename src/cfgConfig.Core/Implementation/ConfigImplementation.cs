using System;

namespace cfgConfig.Core.Implementation
{
    internal abstract class BaseConfigImplementation
    {
        public string Name { get; set; }

        public object RuntimeInstance { get; set; }

        public Type Type { get; set; }

        public BaseConfigImplementation() { }

        public BaseConfigImplementation(Type type)
        {
            Type = type;
        }
    }

    internal class ConfigImplementation : BaseConfigImplementation
    {
        public ConfigImplementation(Type type) : base(type) { }
    }

    internal class ConfigImplementation<TRuntime> : BaseConfigImplementation
    {
        public new TRuntime RuntimeInstance
        {
            get => (TRuntime)base.RuntimeInstance;
            set => base.RuntimeInstance = value;
        }

        public ConfigImplementation(Type type) : base(type) { }

        public ConfigImplementation() : base() { }
    }
}
