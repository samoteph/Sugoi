using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sugoi.Console.Controls
{
    public sealed partial class SugoiControl : UserControl
    {
        Machine machine = new Machine();
        bool isLoaded = false;
        private byte[] screenArray;

        public SugoiControl()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;           
        }

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(this.isLoaded == true)
            {
                return;
            }

            this.isLoaded = true;

            this.SlateView.DrawStart += OnSlateViewDraw;
        }

        private void OnSlateViewDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            // appel du UpdateCallback
            var screenArgb32Array = this.machine.Render();
            this.machine.Transform(screenArgb32Array, screenArray);

            var screen = this.machine.Gpu.Screen;
            this.SlateView.SetPixels(screenArray, screen.Width, screen.Height );
        }

        public void Start(Cartridge cartridge)
        {
            if (this.machine.IsStarted == false)
            {
                this.machine.Start(cartridge);

                this.KeyDown += OnKeyDown;
                this.KeyUp += OnKeyUp;

                // la machine appelera le FrameUpdate à chaque Render
                this.machine.UpdateCallback = () =>
                {
                    this.FrameUpdated?.Invoke();
                };
            }
        }

        /// <summary>
        /// Mise à jour d'une frame
        /// </summary>
        public event SugoiFrameUpdatedHandler FrameUpdated;

        public void Stop()
        {
            this.machine.Stop();
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Up:
                    this.machine.Gamepad.Press(GamepadKeys.Up);
                    break;
                case Windows.System.VirtualKey.Down:
                    this.machine.Gamepad.Press(GamepadKeys.Down);
                    break;
                case Windows.System.VirtualKey.Right:
                    this.machine.Gamepad.Press(GamepadKeys.Right);
                    break;
                case Windows.System.VirtualKey.Left:
                    this.machine.Gamepad.Press(GamepadKeys.Left);
                    break;
                case Windows.System.VirtualKey.GamepadA:
                case Windows.System.VirtualKey.W:
                    this.machine.Gamepad.Press(GamepadKeys.ButtonA);
                    break;
                case Windows.System.VirtualKey.GamepadB:
                case Windows.System.VirtualKey.X:
                    this.machine.Gamepad.Press(GamepadKeys.ButtonB);
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
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
    }

    public delegate void SugoiFrameUpdatedHandler();
}
