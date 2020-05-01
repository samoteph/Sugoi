﻿using Microsoft.Graphics.Canvas.UI.Xaml;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sugoi.Console.Controls
{
    public sealed partial class SugoiControl : UserControl, ISugoiConsole
    {
        Machine machine = new Machine();
        private byte[] screenArray;

        public SugoiControl()
        {
            this.InitializeComponent();
        }

        public void Start(Cartridge cartridge)
        {
            if (this.machine.IsStarted == false)
            {
                this.machine.Start(cartridge);

                this.screen = this.machine.Screen;
                this.videoMemory = this.machine.VideoMemory;
                this.gamepad = this.machine.Gamepad;

                this.screenArray = new byte[4 * screen.Size];

                Window.Current.CoreWindow.KeyDown += OnKeyDown;
                Window.Current.CoreWindow.KeyUp += OnKeyUp;

                // On appelle Update de la machine pour lancer le callback
                this.machine.UpdateCallback = () =>
                {
                    this.FrameUpdated?.Invoke();
                };

                // la machine appelera le FrameDrawn à chaque Render
                this.machine.DrawCallback = () =>
                {
                    this.FrameDrawn?.Invoke();
                };

                this.SlateView.DrawStart += OnSlateViewDraw;
                this.SlateView.Update += OnSlateViewUpdate;
            }
        }

        private bool isAltKeyPressed;

        public void Stop()
        {
            if (IsStarted == true)
            {
                this.SlateView.DrawStart -= OnSlateViewDraw;
                this.SlateView.Update -= OnSlateViewUpdate;

                this.screen = null;
                this.machine.Stop();
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.machine.IsStarted;
            }
        }

        /// <summary>
        /// Screen
        /// </summary>

        public SurfaceSprite Screen
        {
            get
            {
                return screen;
            }
        }

        private SurfaceSprite screen;

        public Gamepad Gamepad
        {
            get
            {
                return gamepad;
            }
        }

        private Gamepad gamepad;

        /// <summary>
        /// VideoMemory
        /// </summary>

        public VideoMemory VideoMemory
        {
            get
            {
                return videoMemory;
            }
        }

        private VideoMemory videoMemory;

        /// <summary>
        /// Mise à jour d'une frame
        /// </summary>
        public event SugoiFrameUpdatedHandler FrameUpdated;
        
        /// <summary>
        /// Affichage d 'une frame
        /// </summary> 
        public event SugoiFrameDrawnHandler FrameDrawn;

        /// <summary>
        /// Touche pressée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnKeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Up:
                    this.machine.Gamepad.Press(GamepadKeys.Up);
                    break;
                case VirtualKey.Down:
                    this.machine.Gamepad.Press(GamepadKeys.Down);
                    break;
                case VirtualKey.Right:
                    this.machine.Gamepad.Press(GamepadKeys.Right);
                    break;
                case VirtualKey.Left:
                    this.machine.Gamepad.Press(GamepadKeys.Left);
                    break;
                case VirtualKey.GamepadA:
                case VirtualKey.W:
                    this.machine.Gamepad.Press(GamepadKeys.ButtonA);
                    break;
                case VirtualKey.GamepadB:
                case VirtualKey.X:
                    this.machine.Gamepad.Press(GamepadKeys.ButtonB);
                    break;
                // pleine écran
                case VirtualKey.Enter:

                    // alt (pour alt+entrée) est compliqué à trouver, on verra plus tard...
                    ApplicationView view = ApplicationView.GetForCurrentView();
                        
                    if (view.IsFullScreenMode)
                    {
                        view.ExitFullScreenMode();
                    }
                    else
                    {
                        view.TryEnterFullScreenMode();
                    }

                    break;
            }
        }

        private void OnKeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case Windows.System.VirtualKey.Up:
                    this.machine.Gamepad.Release(GamepadKeys.Up);
                    break;
                case Windows.System.VirtualKey.Down:
                    this.machine.Gamepad.Release(GamepadKeys.Down);
                    break;
                case Windows.System.VirtualKey.Right:
                    this.machine.Gamepad.Release(GamepadKeys.Right);
                    break;
                case Windows.System.VirtualKey.Left:
                    this.machine.Gamepad.Release(GamepadKeys.Left);
                    break;
                case Windows.System.VirtualKey.GamepadA:
                case Windows.System.VirtualKey.W:
                    this.machine.Gamepad.Release(GamepadKeys.ButtonA);
                    break;
                case Windows.System.VirtualKey.GamepadB:
                case Windows.System.VirtualKey.X:
                    this.machine.Gamepad.Release(GamepadKeys.ButtonB);
                    break;
            }
        }

        /// <summary>
        /// Update du SlateView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSlateViewUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            this.machine.Update();
        }

        /// <summary>
        /// Drw du SlateView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        private void OnSlateViewDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            // appel du UpdateCallback
            var screenArgb32Array = this.machine.Draw();
            this.machine.CopyToBgraByteArray(screenArgb32Array, screenArray);

            var screen = this.machine.Screen;
            this.SlateView.SetPixels(screenArray, screen.Width, screen.Height);
        }

        /// <summary>
        /// Execution d'un script (en plus du code de la cartouche)
        /// </summary>
        /// <param name="script"></param>
        public void ExecuteScript(string script)
        {
            throw new NotImplementedException();
        }
    }
}
