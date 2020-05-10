using Sugoi.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    public class Machine
    {
        private VideoMemory videoMemory;
        private Screen screen;

        public VideoMemory VideoMemory
        {
            get
            {
                return this.videoMemory;
            }
        }

        public Screen Screen
        {
            get
            {
                return this.screen;
            }
        }

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

        private Memory Memory
        {
            get;
            set;
        }

        public Machine()
        {
            this.Gamepad = new Gamepad();
            this.videoMemory = new VideoMemory();
        }

        public bool IsStarted
        {
            get;
            private set;
        }

        public void Start(Cartridge cartridge)
        {
            if (this.IsStarted == true)
            {
                return;
            }

            this.IsStarted = true;

            this.Cartridge = cartridge;

            if(this.Cartridge.IsLoaded == false)
            {
                throw new Exception("The cartridge is not loaded! Please load the cartridge before launching the machine!");
            }

            this.screen = new Screen();

            this.screen.Start(320, 180);

            this.videoMemory.Start(
                cartridge.Header.VideoMemorySize,
                this);

            this.Gamepad.Start();

            // Permmet de démarrer d'autres services si la classe est dérivée
            InternalStart();

            // démarrage du code contenu dans la cartouche

            var executableCartridge = cartridge as ExecutableCartridge;

            if (executableCartridge != null)
            {
                executableCartridge.Start(this);
            }
        }

        protected virtual void InternalStart()
        {

        }

        public void Initialize()
        {
            // appel du script ici
            this.InitializeCallback?.Invoke();
        }

        /// <summary>
        /// Appel du UpdateCallBack + Update du script
        /// </summary>

        private void Update()
        {
            // appel du script ici
            this.UpdateCallback?.Invoke();
        }

        public bool IsDrawing
        {
            get;
            private set;
        }

        //Stopwatch watcherFrame = new Stopwatch();

        public Argb32[] Draw(bool isRunningSlow)
        {
            if (IsDrawing == true)
            {
                return null;
            }

            IsDrawing = true;

            //watcherFrame.Stop();

            //int frameExecuted = watcherFrame.ElapsedMilliseconds < 30 ? 1 : 2;

            //watcherFrame.Restart();

            var updateExecutedCount = isRunningSlow ? 2 : 1;

            for (int i = 0; i < updateExecutedCount; i++)
            {
                Update();
            }

            DrawCallback?.Invoke(updateExecutedCount);

            IsDrawing = false;

            return screen.Pixels;
        }

        public Action InitializeCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Ici on ecrira le code c# pour le déplacement des objets
        /// </summary>

        public Action UpdateCallback
        {
            get;
            set;
        }

        /// <summary>
        /// ici on ecrira le code C# pour l'affichage
        /// </summary>

        public Action<int> DrawCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Stop the machine
        /// </summary>

        public void Stop()
        {
            if (this.IsStarted == false)
            {
                return;
            }

            this.IsStarted = false;

            this.Gamepad.Stop();
            this.Screen.Stop();
            this.VideoMemory.Stop();

            this.InternalStop();
        }

        protected virtual void InternalStop()
        {
        }

        /// <summary>
        /// Transformation du format de l'ecran ARGB en BGRA byte[]
        /// </summary>
        /// <param name="screenRgba32"></param>
        /// <param name="screenByte"></param>

        public void CopyToBgraByteArray(Argb32[] screenRgba32, byte[] screenByte)
        {
            int address = 0;

            if (screenRgba32 == null || screenByte == null)
            {
                return;
            }

            for (int i = 0; i < screenRgba32.Length; i++)
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
