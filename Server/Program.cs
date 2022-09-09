using System;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(1 > args.Length)
            {
                Console.WriteLine("Command Line Not Found. Input ConfigFile Path");
                return;
            }

            Config config = new Config();
            if(false == config.LoadConfig(args[0]))
            {
                Console.WriteLine($"Config Load Fail ConfigPath={args[0]}");
                return;
            }

            if(null == config.ConfigInfo)
            {
                Console.WriteLine("ConfigInfo is Null");
                return;
            }

            if(false == Server.Instance.Start(config.ConfigInfo.Port, config.ConfigInfo.IPFSAddress, config.ConfigInfo.UploadRootPath))
            {
                Console.WriteLine($"Server Start Fail Port={config.ConfigInfo.Port}");
                return;
            }

            Console.WriteLine($"Server Start.. Port={config.ConfigInfo.Port}");

            while (true)
            {
                string? input = Console.ReadLine();
                if(true == string.IsNullOrEmpty(input))
                {
                    continue;
                }
            }
        }
    }
}
