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
        /// <summary>
        /// Sets saturation to 0
        /// </summary>
        public static readonly LifxColor DefaultWhite = new Named("white");
        /// <summary>
        /// Sets hue to 0
        /// </summary>
        public static readonly LifxColor Red = new Named("red");
        /// <summary>
        /// Sets hue to 34
        /// </summary>
        public static readonly LifxColor Orange = new Named("orange");
        /// <summary>
        /// Sets hue to 60
        /// </summary>
        public static readonly LifxColor Yellow = new Named("yellow");
        /// <summary>
        /// Sets hue to 180
        /// </summary>
        public static readonly LifxColor Cyan = new Named("cyan");
        /// <summary>
        /// Sets hue to 120
        /// </summary>
        public static readonly LifxColor Green = new Named("green");
        /// <summary>
        /// Sets hue to 250
        /// </summary>
        public static readonly LifxColor Blue = new Named("blue");
        /// <summary>
        /// Sets hue to 280
        /// </summary>
        public static readonly LifxColor Purple = new Named("purple");
        /// <summary>
        /// Sets hue to 325
        /// </summary>
        public static readonly LifxColor Pink = new Named("pink");
        public static readonly IEnumerable<LifxColor> NamedColors = new List<LifxColor>()
        {
            DefaultWhite, Red, Orange, Yellow, Cyan, Green, Blue, Purple, Pink
        };

        public override bool Equals(object obj)
        {
            LifxColor color = obj as LifxColor;
            return color != null && color.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

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
                if (kelvin != null && (saturation ?? 0) < 0.001)
                {
                    sb.AppendFormat("kelvin:{0} ", Math.Min(Math.Max(TemperatureMin, kelvin.Value), TemperatureMax));
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }

            internal HSBK WithBrightness(float brightness)
            {
                return new HSBK(this.hue, this.saturation, brightness, this.kelvin);
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
                R = (byte)Math.Min(Math.Max(r, 0), byte.MaxValue);
                G = (byte)Math.Min(Math.Max(g, 0), byte.MaxValue);
                B = (byte)Math.Min(Math.Max(b, 0), byte.MaxValue);
            }

            /// <summary>
            /// Unpack a color from an integer
            /// </summary>
            /// <param name="packedRGB">RGB packed integer eg 0xff0000 is bright deep red</param>
            public RGB(int packedRGB)
            {
                R = (byte)((packedRGB >> 16) & 0xff);
                G = (byte)((packedRGB >> 8) & 0xff);
                B = (byte)(packedRGB & 0xff);
            }

            public override string ToString()
            {
                return string.Format("#{0:x2}{1:x2}{2:x2}", R, G, B);
            }
        }
    }
}
