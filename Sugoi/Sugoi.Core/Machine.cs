﻿using Sugoi.Core.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core
{
    public class Machine
    {
        private VideoMemory videoMemory;
        private Screen screen;

        public BatteryRam BatteryRam
        {
            get;
            private set;
        }

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
            this.BatteryRam = new BatteryRam();
        }

        public bool IsStarted
        {
            get;
            private set;
        }

        public async Task StartAsync(Cartridge cartridge)
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

            this.Gamepad.Start(this);

            // demarrage de la Ram avec battery
            await this.BatteryRam.StartAsync(this);

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

        public bool ExecuteWaitManually
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Appel du UpdateCallBack + Update du script
        /// </summary>

        private void Update()
        {
            // Toujours executé car avant le Wait
            this.UpdatingCallback?.Invoke();

            // ici une methode wait s'execute prioritairement à l'update
            if (this.Wait() == false)
            {
                // appel du script ici
                this.UpdatedCallback?.Invoke();
            }
        }

        public int Frame
        {
            get;
            set;
        }

        private int endWaitFrame;

        public void WaitForFrame(int frameToWait, Action completed = null)
        {
            endWaitFrame = this.Frame + frameToWait;

            Debug.WriteLine("Init WaitForFrame " + endWaitFrame);

            this.PrepareWaiting(() =>
           {
               return this.Frame < endWaitFrame;
           },
           completed
           );
        }

        public Random Random
        {
            get;
            private set;
        } = new Random();

        public int GetRandomInteger(int max)
        {
            return Random.Next(max);
        }

        public int GetRandomInteger(int min, int max)
        {
            return Random.Next(min, max);
        }

        public double GetRandomDouble(double max)
        {
            return Random.NextDouble() * max;
        }

        public double GetRandomDouble(double min, double max)
        {
            return min + (Random.NextDouble() * (max - min)) ;
        }

        public Argb32[] Draw(bool isRunningSlow)
        {
            var updateExecutedCount = isRunningSlow ? 2 : 1;

            for (int i = 0; i < updateExecutedCount; i++)
            {
                Update();
            }

            DrawCallback?.Invoke(updateExecutedCount);

            this.Frame += updateExecutedCount;

            return screen.Pixels;
        }

        public Action InitializeCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Ici on ecrira le code c# pour le déplacement des objets avant le wait (toujours executé meme en case de wait)
        /// </summary>

        public Action UpdatingCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Ici on ecrira le code c# pour le déplacement des objets après le wait (pas executé en cas de wait)
        /// </summary>

        public Action UpdatedCallback
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

        public Func<byte[], Task<bool>> WriteBatteryRamCallback
        {
            get;
            set;
        }

        public Func<Task<byte[]>> ReadBatteryRamCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Callback interne d'attente (de frame, de touche, ...)
        /// </summary>

        private Func<bool> MustWaitCallBack
        {
            get;
            set;
        }

        public void PrepareWaiting(Func<bool> waitConditionFunction, Action completed)
        {
            this.MustWaitCallBack = () =>
            {
                if (waitConditionFunction != null)
                {
                    if (waitConditionFunction() == true)
                    {
                        return true;
                    }
                }

                completed?.Invoke();

                return false;
            };
        }

        /// <summary>
        /// Return true s'il doit attendre que le MustWaitCallBack soit terminé
        /// </summary>
        /// <returns></returns>

        public bool Wait()
        {
            var currentCallBack = this.MustWaitCallBack;

            if (currentCallBack != null)
            {
                if( currentCallBack() == false )
                {
                    // après l'execution on lance un completed qui peut effectuer une PrepareWaiting et xhanger le MustWaitCallBack
                    // On doit alors continuer à attendre
                    if (currentCallBack == this.MustWaitCallBack)
                    {
                        // C'est terminé
                        MustWaitCallBack = null;
                        return false;
                    }
                }

                // on continue d'attendre
                return true;
                
            }

            return false;
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

        /// <summary>
        /// Lecture de la Ram avec battery
        /// </summary>
        /// <returns></returns>

        internal Task<byte[]> ReadBatteryRamAsync()
        {
            return this.ReadBatteryRamCallback?.Invoke();
        }

        /// <summary>
        /// Ecriture de la Ram avec battery
        /// </summary>
        /// <param name="batteryRam"></param>

        internal Task<bool> WriteBatteryRamAsync(byte[] batteryRam)
        {
            return this.WriteBatteryRamCallback?.Invoke(batteryRam);
        }

        /// <summary>
        /// Taille de la Ram avec Batterie 
        /// </summary>

        public int BatteryRamSize
        {
            get
            {
                return BatteryRam.BATTERY_RAM_SIZE;
            }
        }
    }
}
