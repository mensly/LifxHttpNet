﻿using LifxHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxHttp.Enums;

namespace LifxHttpSample
{
    class Program
    {
        private const string TOKEN = "REDACTED - Generate from https://cloud.lifx.com/settings";
        private const int DELAY = 2000;
        private const int EFFECT_DELAY = 10000;

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
            await DemoScenes(client);
            await DemoModifyLight(client);
            await DemoModifyCollections(client);
            await DemoEffects(client);
            await DemoValidateColor(client);
        }

        private static async Task DemoListing(LifxClient client)
        {
            Console.WriteLine();
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
        }

        private static async Task DemoScenes(LifxClient client)
        {
            Console.WriteLine();
            Console.WriteLine("Scenes:");
            List<Scene> scenes = await client.ListScenes();
            if (scenes.Count() > 0)
            {
                foreach (var scene in scenes)
                {
                    Console.WriteLine(scene.ToString());
                }
                Console.WriteLine(string.Format("Activating Scene: {0}", scenes.First().Name));
                await client.ActivateScene(scenes.First().UUID);
                await Task.Delay(EFFECT_DELAY);
            }
            else Console.WriteLine("No scenes on account.");
        }

        private static async Task DemoModifyLight(LifxClient client)
        {
            Console.WriteLine();
            Light light = (await client.ListLights(new Selector.GroupLabel("Room 1"))).FirstOrDefault();
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
            Console.WriteLine();
            List<Light> lights = (await client.ListLights(new Selector.LightLabel("LIFX 027d98")));
            Group group = (await client.ListGroups()).FirstOrDefault();
            if (group == null)
            {
                Console.WriteLine("No groups");
            }
            else
            {
                Console.WriteLine("Using group: {0}", group);
                Console.WriteLine("Toggling group, 3 second duration.");
                await group.TogglePower(3);
                await Task.Delay(DELAY + 3000);

                Console.WriteLine("Turning group green");
                await group.SetColor(LifxColor.Green, 0);
                await Task.Delay(DELAY);
            }
            Location location = (await client.ListLocations()).FirstOrDefault();
            await Task.Delay(DELAY);
            if (location == null)
            {
                Console.WriteLine("No locations");
            }
            else
            {
                Console.WriteLine("Using location: {0}", location);

                Console.WriteLine("Turning off location");
                await location.SetPower(PowerState.Off);
                await Task.Delay(DELAY);

                Console.WriteLine("Setting color to white with 5 second duration.");
                await location.SetColor(new LifxColor.White(1), 5);
                await Task.Delay(DELAY + 5000);

                Console.WriteLine("Set light to a blue color, using 90% brightness override. Uses SetState explicitely.");
                await location.SetState(PowerState.On, new LifxColor.HSBK(180, 1, 1, 2000), 0.9, 2);
                await Task.Delay(DELAY);
            }
        }

        private static async Task DemoEffects(LifxClient client)
        {
            Console.WriteLine();
            Light light = (await client.ListLights(new Selector.GroupLabel("Room 1"))).FirstOrDefault();
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

            Console.WriteLine("Pulsing slowly between previously set color and green");
            await light.PulseEffect(LifxColor.Green, 1, 10);
            await Task.Delay(EFFECT_DELAY);

            /* If strobing with a single color (therefore the light alternates between the "color" parameter and "fromColor"
            parameter which should be set to LifxColor.OffState (zero brightness) you must set the "color" parameter to a custom color 
            with a brightness value explicitly set. The default named parameters appear to leave the light in a fixed zero brightness state 
            until the end of the effect. */
            Console.WriteLine("Pulsing quickly in white");
            await light.PulseEffect(new LifxColor.White(1, 3500), 0.1, 50, fromColor: LifxColor.OffState);
            await Task.Delay(EFFECT_DELAY);

            Console.WriteLine("Breathing red alert light.");
            await light.BreatheEffect(new LifxColor.RGB(120, 0, 0), 2.5, 4, LifxColor.OffState, peak: 0.4d);
            await Task.Delay(EFFECT_DELAY);

            Console.WriteLine("\"Party breathe\", will pass through intermediate colors.");
            await light.BreatheEffect(LifxColor.Cyan, 5, 4, LifxColor.Orange, peak: 0.5d);
            await Task.Delay(EFFECT_DELAY);

            List<LightState> stateList = new List<LightState>();
            stateList.Add(new LightState(PowerState.On, LifxColor.Pink, 0.5d));
            stateList.Add(new LightState(PowerState.On, LifxColor.Pink, 1.0d));
            stateList.Add(new LightState(PowerState.On, LifxColor.DefaultWhite, 1.0d));
            stateList.Add(new LightState(PowerState.On, LifxColor.Green));
            stateList.Add(new LightState(PowerState.On, LifxColor.Blue, 1.0d));
            stateList.Add(new LightState(PowerState.On, LifxColor.Blue, 0.5d));
            LightState defaults = new LightState();
            defaults.Duration = 3.0d;
            defaults.Brightness = 1.0d;

            // Cycle forward
            Console.WriteLine("Cycling forward through set of 6 light states.");
            for (int i = 0; i < stateList.Count(); i++)
            {
                await light.Cycle(stateList, defaults);
                await Task.Delay(4000);
            }

            await Task.Delay(1000);

            // Cycle backward
            Console.WriteLine("Cycling backward through set of 6 light states.");
            for (int i = 0; i < stateList.Count(); i++)
            {
                await light.Cycle(stateList, defaults, Direction.Backward);
                await Task.Delay(4000);
            }
        }

        private static async Task DemoValidateColor(LifxClient client)
        {
            string colorName = "Pink";
            Console.WriteLine();
            Console.WriteLine(string.Format("Validating color: {0}", colorName));
            var color = await client.ValidateColor(colorName);
            if (color.Item1) { Console.WriteLine(string.Format("{0} is a valid color name. Values are {1}.", colorName, color.Item2)); }
            else Console.WriteLine(string.Format("{0} is not a valid color name.", colorName));
        }
    }
}
