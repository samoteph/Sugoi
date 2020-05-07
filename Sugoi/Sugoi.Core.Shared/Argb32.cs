using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sugoi.Core
{
    public struct Argb32
    {
        private byte r;
        private byte g;
        private byte b;
        private byte a;
        private uint color;

        public static Argb32 Empty  = new Argb32(0);
        public static Argb32 Black  = new Argb32(0x00,0x00,0x00);
        public static Argb32 White  = new Argb32(0xFF,0xFF,0xFF);

        public static Argb32 Red    = new Argb32(0xFF, 0x00, 0x00);
        public static Argb32 Green = new Argb32(0x00, 0xFF, 0x00);
        public static Argb32 Blue = new Argb32(0x00, 0x00, 0xFF);
        public static Argb32 Magenta = new Argb32(0xFF, 0x00, 0xFF);

        public bool IsEmpty
        {
            get
            {
                return this.color == 0; 
            }
        }

        public bool IsTransparent
        {
            get
            {
                return this.A == 0;
            }
        }

        public bool IsOpaque
        {
            get
            {
                return this.A == 0xFF;
            }
        }

        public Argb32(Argb32 color)
        {
            uint c = color.Color;
            this.color = c;
            this.a = (byte)(c >> 24);
            this.r = (byte)(c >> 16);
            this.g = (byte)(c >> 8);
            this.b = (byte)c;
        }

        public Argb32(uint color = 0x000000FF)
        {
            this.color = color;
            this.a = (byte)(color >> 24);
            this.r = (byte)(color >> 16);
            this.g = (byte)(color >> 8);
            this.b = (byte)color;
        }

        public Argb32(byte r, byte g = 0x00, byte b = 0x00, byte a = 0xff)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        public byte R
        {
            get
            {
                return r;
            }

            set
            {
                r = value;
                color = (color & 0xFF00FFFF) & (uint)value << 16;
            }
        }

        public byte G
        {
            get
            {
                return g;
            }

            set
            {
                g = value;
                color = (color & 0xFFFF00FF) & (uint)value << 8;
            }
        }

        public byte B
        {
            get
            {
                return b;
            }

            set
            {
                b = value;
                color = (color & 0xFFFFFF00) + value;
            }
        }

        public byte A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
                color = (color & 0x00FFFFFF) & (uint)value << 24;
            }
        }

        public uint Color
        {
            get
            {
                return color;
            }

            set
            {
                this.a = (byte)(value >> 24);
                this.r = (byte)(value >> 16);
                this.g = (byte)(value >> 8);
                this.b = (byte)value;

                color = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetColor(byte a, byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;

            this.color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        /// <summary>
        /// Set color sans toucher à l'alpha
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }
    }
}
