using cfgConfig.Core;
using cfgConfig.Core.Attributes;
using cfgConfig.Core.Files;
using System;

namespace cfgConfig.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManager.UseConsole();
            var config = ConfigurableManager.Make("test", "mySettings")
                .Configure(settings => settings.Encrypt())
                .Build();

            config.Implementations.Implement<MySettings>();

            var st = ConfigurationManager.GetManager("mySettings").GetConfig<MySettings>();

            st.test2 = new Test
            {
                MyName = "Waslope"
            };

            Console.ReadLine();

            ConfigurationManager.Terminate();
        }
    }

    [Config]
    public class MySettings
    {
        public string test { get; set; }

        public Test test2 { get; set; }
    }

    public class Test
    {
        public string MyName { get; set; } = "hello";
    }
}
