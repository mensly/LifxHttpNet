using LifxHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttpSample
{
    class Program
    {
        private const string TOKEN = "REDACTED - Generate from https://cloud.lifx.com/settings";

        static void Main(string[] args)
        {
            RunDemos(new LifxClient(args.Length > 0 ? args[0] : TOKEN)).Wait();
            Console.ReadKey();
        }

        private static async Task RunDemos(LifxClient client)
        {
            // await DemoListing(client);
            await DemoModify(client);
        }
        
        private static async Task DemoListing(LifxClient client)
        {
            Console.WriteLine("Lights:");
            foreach (var light in await client.ListLights())
            {
                Console.WriteLine("{0} - {1}", light, light.Color);
            }
            Console.WriteLine();

            Console.WriteLine("Groups:");
            foreach (var group in await client.ListGroups())
            {
                Console.WriteLine("{0} - {1} lights", group, group.Count());
            }
            Console.WriteLine();

            Console.WriteLine("Locations:");
            foreach (var group in await client.ListLocations())
            {
                Console.WriteLine("{0} - {1} lights", group, group.Count());
            }
            Console.WriteLine();
        }

        private static async Task DemoModify(LifxClient client)
        {
            // Not working yet
            Light light = (await client.ListLights(new Selector.GroupLabel("Living Room"))).FirstOrDefault();
            if (light == null)
            {
                // Find a connected light
                foreach (var l in await client.ListLights())
                {
                    if (l.IsConnected)
                    {
                        light = l;
                        break;
                    }
                }
                if (!light.IsConnected)
                {
                    Console.WriteLine("No connected lights");
                    return;
                }
            }
            Console.WriteLine("Using light: {0}", light);
            const int delay = 1000;
            Console.WriteLine("Turning light off");
            await client.SetPower(light, false);
            await Task.Delay(delay);
            Console.WriteLine("Toggling light on");
            await client.TogglePower(light);
            await Task.Delay(delay);
            Console.WriteLine("Turning light soft red");
            await client.SetColor(light, new LifxColor.HSB(0, 0.2f, 0.5f));
            await Task.Delay(delay);
            Console.WriteLine("Turning light white");
            await client.SetColor(light, LifxColor.DefaultWhite);
            await Task.Delay(delay);
            Console.WriteLine("Turning light hot white");
            await client.SetColor(light, new LifxColor.White(0.8f, LifxColor.TemperatureMax));
            await Task.Delay(delay);
        }
    }
}
