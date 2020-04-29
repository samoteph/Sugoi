using Sugoi.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class Machine
    {
        public Cartridge Cartridge
        {
            get;
            private set;
        }

        public Gamepad Gamepad
        {
            get;
            private set;
        }

        public Gpu Gpu
        {
            get;
            private set;
        }

        private Memory Memory
        {
            get;
            set;
        }

        public Machine()
        {
            this.Cartridge = new Cartridge();
            this.Gpu = new Gpu();
            this.Gamepad = new Gamepad();
        }

        public bool IsStarted
        {
            get;
            private set;
        }   

        public void Start()
        {
            if(this.IsStarted == true)
            {
                return;
            }

            this.IsStarted = true;

            this.Cartridge.Start();
            // ici lecture de la cartridge
            this.Gpu.Start(this.Cartridge.Header.VideoMemorySize, 240, 136);

            this.Gamepad.Start();
        }

        public Action UpdateCallback
        {
            get
            {
                return this.Gpu.UpdateCallback;
            }

            set
            {
                this.Gpu.UpdateCallback = value;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <returns></returns>

        public Argb32[] Render()
        {
            return this.Gpu.Render();
        }

        /// <summary>
        /// Stop the machine
        /// </summary>

        public void Stop()
        {
            if(this.IsStarted == false)
            {
                return;
            }

            this.IsStarted = false;

            this.Gpu.Stop();
        }

        /// <summary>
        /// Transformation du format de l'ecran RGBA en BGRA byte[]
        /// </summary>
        /// <param name="screenRgba32"></param>
        /// <param name="screenByte"></param>

        public void Transform(Argb32[] screenRgba32, byte[] screenByte)
        {
            int address = 0;

            if(screenRgba32 == null || screenByte == null)
            {
                return;
            }

            for(int i=0; i < screenRgba32.Length; i++)
            {
                var argb = screenRgba32[i];

                screenByte[address + 0] = argb.B;
                screenByte[address + 1] = argb.G;
                screenByte[address + 2] = argb.R;
                screenByte[address + 3] = argb.A;

                address += 4;
            }
        }
    }
}
