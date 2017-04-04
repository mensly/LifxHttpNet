﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp.Helpers
{
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }

    public class LifxColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //Assume we can convert to anything for now.
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is null)
            {
                var token = JToken.Load(reader);
                double? hue = null;
                double? saturation = null;
                double? brightness = null;
                int? kelvin = null;

                if (!token["hue"].IsNullOrEmpty())
                    hue = (double)token["hue"];
                if (!token["saturation"].IsNullOrEmpty())
                    saturation = (double)token["saturation"];
                if (!token["brightness"].IsNullOrEmpty())
                    brightness = (double)token["brightness"];
                if (!token["kelvin"].IsNullOrEmpty())
                    kelvin = (int)token["kelvin"];
                return new LifxColor.HSBK(hue, saturation, brightness, kelvin);
            }
            else
            {
                return serializer.Deserialize<LifxColor.HSBK>(reader);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //Write to string to get correct color formatting.
            writer.WriteValue(value.ToString());
        }
    }
}
