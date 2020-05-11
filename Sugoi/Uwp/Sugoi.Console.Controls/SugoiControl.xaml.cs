using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Collections;
using System.Diagnostics;
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
            this.IsTabStop = true;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
        }

        IEnumerator updateEnumerator = null;

        public void Start(Cartridge cartridge)
        {
            if (this.machine.IsStarted == false)
            {
                cartridge.Load();

                this.machine.Start(cartridge);
                this.cartridge = this.machine.Cartridge;
                this.screen = this.machine.Screen;
                this.videoMemory = this.machine.VideoMemory;
                this.gamepad = this.machine.Gamepad;

                this.screenArray = new byte[4 * screen.Size];

                Window.Current.CoreWindow.KeyDown += OnKeyDown;
                Window.Current.CoreWindow.KeyUp += OnKeyUp;

                // elle sera remplie si la cartouche est executable
                var cartridgeInitCallback = this.machine.InitializeCallback;
                
                this.machine.InitializeCallback = () =>
                {
                    cartridgeInitCallback?.Invoke();
                    this.Initialized?.Invoke();
                };

                // initialisation du code de l'application
                this.machine.Initialize();

                // On appelle Update de la machine pour lancer le callback
                var cartridgeUpdateCallback = this.machine.UpdatedCallback;

                this.machine.UpdatedCallback = () =>
                {
                    cartridgeUpdateCallback?.Invoke();
                    this.FrameUpdated?.Invoke();
                };

                // la machine appelera le FrameDrawn à chaque Render
                var cartridgeDrawCallback = this.machine.DrawCallback;
                this.machine.DrawCallback = (frameExecuted) =>
                {
                    cartridgeDrawCallback?.Invoke(frameExecuted);
                    this.FrameDrawn?.Invoke(frameExecuted);
                };

                this.SlateView.DrawStart += OnSlateViewDraw;
                //this.SlateView.Update += OnSlateViewUpdate;

                this.GotFocus += OnSugoiGotFocus;
                this.LostFocus += OnSugoiLostFocus;
            }
        }

        private void OnSugoiLostFocus(object sender, RoutedEventArgs e)
        {
            haveFocus = false;
            System.Diagnostics.Debug.WriteLine("SUGOI LostFocus");
        }

        private void OnSugoiGotFocus(object sender, RoutedEventArgs e)
        {
            haveFocus = true;
            this.Focus(FocusState.Programmatic);
            System.Diagnostics.Debug.WriteLine("SUGOI GotFocus");
        }

        public void Stop()
        {
            if (IsStarted == true)
            {
                this.SlateView.DrawStart -= OnSlateViewDraw;
                //this.SlateView.Update -= OnSlateViewUpdate;

                Window.Current.CoreWindow.KeyDown -= OnKeyDown;
                Window.Current.CoreWindow.KeyUp -= OnKeyUp;

                this.GotFocus -= OnSugoiGotFocus;
                this.LostFocus -= OnSugoiLostFocus;

                this.screen = null;
                this.machine.Stop();
            }
        }

        bool haveFocus = true;

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

        public Cartridge Cartridge
        {
            get
            {
                return cartridge;
            }
        }

        private Cartridge cartridge;

        /// <summary>
        /// Mise à jour d'une frame
        /// </summary>
        public event SugoiInitializedHandler Initialized;

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
        //private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(haveFocus == false)
            {
                return;
            }

            switch (e.VirtualKey)
            {
                case VirtualKey.Up:
                case VirtualKey.GamepadDPadUp:
                    this.machine.Gamepad.Press(GamepadKeys.Up);
                    break;
                case VirtualKey.Down:
                case VirtualKey.GamepadDPadDown:
                    this.machine.Gamepad.Press(GamepadKeys.Down);
                    break;
                case VirtualKey.Right:
                case VirtualKey.GamepadDPadRight:
                    this.machine.Gamepad.Press(GamepadKeys.Right);
                    break;
                case VirtualKey.Left:
                case VirtualKey.GamepadDPadLeft:
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
        //private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if(haveFocus == false)
            {
                return;
            }

            switch (e.VirtualKey)
            {
                case VirtualKey.Up:
                case VirtualKey.GamepadDPadUp:
                    this.machine.Gamepad.Release(GamepadKeys.Up);
                    break;
                case VirtualKey.Down:
                case VirtualKey.GamepadDPadDown:
                    this.machine.Gamepad.Release(GamepadKeys.Down);
                    break;
                case VirtualKey.Right:
                case VirtualKey.GamepadDPadRight:
                    this.machine.Gamepad.Release(GamepadKeys.Right);
                    break;
                case VirtualKey.Left:
                case VirtualKey.GamepadDPadLeft:
                    this.machine.Gamepad.Release(GamepadKeys.Left);
                    break;
                case VirtualKey.GamepadA:
                case VirtualKey.W:
                    this.machine.Gamepad.Release(GamepadKeys.ButtonA);
                    break;
                case VirtualKey.GamepadB:
                case VirtualKey.X:
                    this.machine.Gamepad.Release(GamepadKeys.ButtonB);
                    break;
            }
        }

        /// <summary>
        /// Initilaisation du Slateview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSlateViewInitialized(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            this.machine.Initialize();
        }

        /// <summary>
        /// Update du SlateView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //private void OnSlateViewUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        //{
        //    this.machine.Update();
        //}

        /// <summary>
        /// Drw du SlateView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        private void OnSlateViewDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            // appel du UpdateCallback
            var screenArgb32Array = this.machine.Draw(args.Timing.IsRunningSlowly);
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
