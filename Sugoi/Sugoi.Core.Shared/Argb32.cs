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
        public void SetColor(Argb32 color)
        {
            SetColor(color.a, color.r, color.g, color.b);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetColor(uint color)
        {
            this.Color = color;

            this.a = (byte)(color >> 24);
            this.r = (byte)(color >> 16);
            this.g = (byte)(color >> 8);
            this.b = (byte)color;
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

        /// <summary>
        /// Set a color with alph blending
        /// </summary>
        /// <param name="foreGround"></param>
        /// <param name="opacity"></param>
        public void AlphaBlend(Argb32 foreGround, double opacity)
        {
            var colora = this.color;
            var colorb = foreGround.color;

            uint a2 = (colorb & 0xFF000000) >> 24;

            // prendre en compte l'opacité globale
            a2 = (uint)((double)a2 * opacity);

            if (a2 == 0)
            {
                return;
            }

            if (a2 == 255)
            {
                this.A = foreGround.A;
                this.R = foreGround.R;
                this.G = foreGround.G;
                this.B = foreGround.B;
                this.Color = foreGround.Color;
            }

            uint a1 = (colora & 0xFF000000) >> 24;
            uint nalpha = 0x100 - a2;
            uint rb1 = (nalpha * (colora & 0xFF00FF)) >> 8;
            uint rb2 = (a2 * (colorb & 0xFF00FF)) >> 8;
            uint g1 = (nalpha * (colora & 0x00FF00)) >> 8;
            uint g2 = (a2 * (colorb & 0x00FF00)) >> 8;
            
            uint anew = a1 + a2;

            if (anew > 255) 
            { 
                anew = 255; 
            }
            
            this.SetColor( ((rb1 + rb2) & 0xFF00FF) + ((g1 + g2) & 0x00FF00) + (anew << 24) );

            //if (foreGround.A == 0)
            //    return;

            //if (this.A == 0)
            //{
            //    this.SetColor(foreGround);
            //    return;
            //}

            //foreGround.A = (byte)((double)foreGround.A * opacity);

            //if (foreGround.A == 255)
            //{
            //    this.SetColor(foreGround);
            //    return;
            //}

            //int Alpha = ((int)foreGround.A) + 1;
            //int B = Alpha * foreGround.B + (255 - Alpha) * this.B >> 8;
            //int G = Alpha * foreGround.G + (255 - Alpha) * this.G >> 8;
            //int R = Alpha * foreGround.R + (255 - Alpha) * this.R >> 8;
            //int A = foreGround.A;

            //if (this.A == 255)
            //    A = 255;
            //if (A > 255)
            //    A = 255;
            //if (R > 255)
            //    R = 255;
            //if (G > 255)
            //    G = 255;
            //if (B > 255)
            //    B = 255;

            //this.SetColor((byte)Math.Abs(A), (byte)Math.Abs(R), (byte)Math.Abs(G), (byte)Math.Abs(B));
        }
    }
}
