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
        private const int DELAY = 2000;

        static void Main(string[] args)
        {
            try
            {
                RunDemos(new LifxClient(args.Length > 0 ? args[0] : TOKEN)).Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
            Console.ReadKey();
        }

        private static async Task RunDemos(LifxClient client)
        {
            await DemoListing(client);
            await DemoModifyLight(client);
            await DemoModifyCollections(client);
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

        private static async Task DemoModifyLight(LifxClient client)
        {
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
            Console.WriteLine("Turning light off");
            await light.SetPower(false);
            await Task.Delay(DELAY);
            Console.WriteLine("Toggling light on");
            await light.TogglePower();
            await Task.Delay(DELAY);
            Console.WriteLine("Turning light soft red");
            await light.SetColor(new LifxColor.HSB(0, 0.2f, 0.5f));
            await Task.Delay(DELAY);
            Console.WriteLine("Turning light blue-green");
            await light.SetColor(new LifxColor.RGB(0x00c89c));
            await Task.Delay(DELAY);
            Console.WriteLine("Turning light white");
            await light.SetColor(LifxColor.DefaultWhite);
            await Task.Delay(DELAY);
            Console.WriteLine("Turning light hot white");
            await light.SetColor(new LifxColor.White(0.8f, LifxColor.TemperatureMax));
            await Task.Delay(DELAY);

            Console.WriteLine("Named colors:");
            foreach (var c in LifxColor.NamedColors)
            {
                Console.Write("{0}: ", c);
                await light.SetColor(c);
                await light.Refresh();
                Console.WriteLine("{0}", light.Color);
                await Task.Delay(DELAY);
            }
        }

        private static async Task DemoModifyCollections(LifxClient client)
        {
            Group group = (await client.ListGroups()).FirstOrDefault();
            if (group == null)
            {
                Console.WriteLine("No groups");
            }
            else
            {
                Console.WriteLine("Using group: {0}", group);
                Console.WriteLine("Toggling group");
                await group.TogglePower();
                Console.WriteLine("Turning group green");
                await group.SetColor(LifxColor.Green);
            }
            Location location = (await client.ListLocations()).FirstOrDefault();
            if (location == null)
            {
                Console.WriteLine("No locations");
            }
            else
            {
                Console.WriteLine("Using location: {0}", location);
                Console.WriteLine("Turning off location");
                await location.SetPower(false);
                Console.WriteLine("Turning location pink");
                await location.SetColor(LifxColor.Pink);
            }
        }
    }
}
