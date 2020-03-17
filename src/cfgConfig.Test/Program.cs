using cfgConfig.Core;
using cfgConfig.Core.Configuration;
using cfgConfig.Core.Environment;
using System;

namespace cfgConfig.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationEnvironment environment = ConfigurationManager.NewEnvironment(@"C:\users\tomas\Desktop\cfgConfigTests\env1", "AppSettings")
                .WithConfiguration<KeyValuePairConfiguration>("languages", configure => configure.Encrypt().Build()).Build();

            KeyValuePairConfiguration config = environment.GetKeyValueConfiguration("languages");

            

            Console.ReadLine();
        }
    }
}
