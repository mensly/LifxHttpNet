using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    /// <summary>
    /// The color of light is best represented in terms of hue, saturation, 
    /// kelvin, and brightness components. However, other means of expressing
    /// colors are available
    /// </summary>
    public abstract class LifxColor
    {
        /// <summary>
        /// Color temperature should be at least 2500K
        /// </summary>
        public const int TemperatureMin = 2500;
        /// <summary>
        /// Color temperature should be at most 9000K
        /// </summary>
        public const int TemperatureMax = 9000;
        /// <summary>
        /// A normal white color temperature, this corresponds to the DefaultWhite color.
        /// </summary>
        public const int TemperatureDefault = 3500;

        public static readonly LifxColor DefaultWhite = new Named("white");
        public static readonly LifxColor Red = new Named("red");
        public static readonly LifxColor Orange = new Named("orange");
        public static readonly LifxColor Yellow = new Named("yellow");
        public static readonly LifxColor Cyan = new Named("cyan");
        public static readonly LifxColor Green = new Named("green");
        public static readonly LifxColor Blue = new Named("blue");
        public static readonly LifxColor Purple = new Named("purple");
        public static readonly LifxColor Pink = new Named("pink");

        /// <summary>
        /// A color defined by a particular English name
        /// </summary>
        public sealed class Named : LifxColor
        {
            private readonly string colorName;
            internal Named(string colorName) { this.colorName = colorName; }

            public override string ToString()
            {
                return colorName;
            }
        }

        /// <summary>
        /// A color in its natural Lifx representation
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class HSBK : LifxColor
        {
            [JsonProperty]
            private float? hue;
            [JsonProperty]
            private float? saturation;
            [JsonProperty]
            private float? brightness;
            [JsonProperty]
            private int? kelvin;

            public float Hue { get { return hue ?? float.NaN; } }
            public float Saturation { get { return saturation ?? float.NaN; } }
            public float Brightness { get { return brightness ?? float.NaN; } }
            public int Kelvin { get { return kelvin ?? TemperatureDefault; } }
            internal HSBK() { }
            public HSBK(float? hue = null, float? saturation = null, float? brightness = null, int? kelvin = null)
            {
                if (hue == null && saturation == null && brightness == null && kelvin == null)
                {
                    throw new ArgumentException("HSBKColor requires at least one non-null component");
                }
                this.hue = hue;
                this.saturation = saturation;
                this.brightness = brightness;
                this.kelvin = kelvin;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (hue != null)
                {
                    sb.AppendFormat("hue:{0} ", Math.Min(Math.Max(0, hue.Value), 360));
                }
                if (saturation != null)
                {
                    sb.AppendFormat("saturation:{0} ", Math.Min(Math.Max(0, saturation.Value), 1));
                }
                if (brightness != null)
                {
                    sb.AppendFormat("brightness:{0} ", Math.Min(Math.Max(0, brightness.Value), 1));
                }
                if (kelvin != null)
                {
                    sb.AppendFormat("kelvin:{0} ", Math.Min(Math.Max(TemperatureMin, kelvin.Value), TemperatureMax));
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }

        /// <summary>
        /// A color in its natural Lifx represenation
        /// </summary>
        public sealed class HSB : HSBK
        {
            public HSB(float hue, float saturation = 1f, float brightness = 1f) : base(hue, saturation, brightness) { }
        }

        /// <summary>
        /// A configurable white Light Color
        /// </summary>
        public sealed class White : HSBK
        {
            public White(float brightness = 1f, int kelvin = TemperatureDefault) : base(null, null, brightness, kelvin) { }
        }

        /// <summary>
        /// Automatically converted to HSBK by Lifx cloud.
        /// </summary>
        public sealed class RGB : LifxColor
        {
            public byte R { get; private set; }
            public byte G { get; private set; }
            public byte B { get; private set; }

            public RGB(int r, int g, int b)
            {
                R = (byte)Math.Min(Math.Max(0, r), byte.MaxValue);
                G = (byte)Math.Min(Math.Max(0, g), byte.MaxValue);
                B = (byte)Math.Min(Math.Max(0, b), byte.MaxValue);
            }

            /// <summary>
            /// Unpack a color from an integer
            /// </summary>
            /// <param name="packedRGB">RGB packed integer eg 0xff0000 is bright deep red</param>
            public RGB(int packedRGB)
            {
                R = (byte)(packedRGB >> 16);
                G = (byte)(packedRGB >> 8);
                B = (byte)packedRGB;
            }

            public override string ToString()
            {
                return string.Format("#{0:x}{0:x}{0:x}", R, G, B);
            }
        }
    }
}
