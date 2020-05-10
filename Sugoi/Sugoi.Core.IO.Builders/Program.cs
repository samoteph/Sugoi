using System;

namespace Sugoi.Core.IO.Builders
{
    class Program
    {
        static CartridgeBuilder builder = new CartridgeBuilder();

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("***********************************************");
                Console.WriteLine("* Cartridge Builder for Sugoi Virtual Console *");
                Console.WriteLine("***********************************************");
                Console.WriteLine();

                if (args.Length == 0)
                {
                    Console.WriteLine("You must provide a folder parameter of assets and manifest to build the cartridge!");
                    return;
                }

                builder.Build(args[0]);

                Console.WriteLine("\\o/ Cartridge built!");
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR :" + ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
