using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using SamuelBlanchard.Audio;
using Sugoi.Core;
using Sugoi.Core.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sugoi.Console.Controls
{
    public sealed partial class SugoiControl : UserControl, ISugoiConsole
    {
        Machine machine = new Machine();
        private byte[] screenArray;
        private AudioPlayer<string> audioPlayer = new AudioPlayer<string>();
        private SystemNavigationManager navigationManager;
        private XBoxGamepadManager xboxGamepadManager = new XBoxGamepadManager();

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

        //Stopwatch watch = new Stopwatch();

        //private void StartWatch()
        //{
        //    watch.Restart();
        //}

        //private void StopWatch(string description)
        //{
        //    watch.Stop();

        //    Debug.WriteLine("{0}={1}ms", description, watch.ElapsedMilliseconds);
        //}


        public async Task StartAsync(Cartridge cartridge)
        {
            if (this.machine.IsStarted == false)
            {
                navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.BackRequested += BackRequested;

                cartridge.ExportFileAsyncCallback = (name, stream, count) =>
                {
                    //stream.BaseStream.Seek(stream.BaseStream.Position + count, SeekOrigin.Begin);
                    //return Task.FromResult<bool>(true);

                    return this.WriteFileAsync(name, stream, count);
                };

                //StartWatch();
                await cartridge.LoadAsync();
                //StopWatch("LoadAsync");

                // callback de Ram avec battery (appelé dans le Start de la machine)
                this.machine.ReadBatteryRamAsyncCallback = () =>
                {
                    return this.ReadBatteryRamAsync();
                };

                this.machine.WriteBatteryRamAsyncCallback = (memory) =>
                {
                    return this.WriteBatteryRamAsync(memory);
                };

                //Execution asynchrone quand ce n'est pas possible normalement via un await
                this.machine.ExecuteAsyncCallBack = (delegateAsync) =>
                {
                    return this.SlateView.RunOnGameLoopThreadAsync(delegateAsync);
                };

                //StartWatch();
                // Gestion du son
                await audioPlayer.InitializeAsync();
                //StopWatch("Audio InitializeAsync");

                this.machine.PreloadSoundAsyncCallBack = (name, channelCount) =>
                {
                    return this.PreloadSoundAsync(name, channelCount);
                };

                this.machine.PlaySoundCallBack = (name, volume, isLoop) =>
                {
                    if (isLoop == true)
                    {
                        audioPlayer.PlayLoop(name, volume);
                    }
                    else
                    {
                        audioPlayer.PlaySound(name, volume);
                    }
                };

                this.machine.StopSoundCallBack = (name) =>
                {
                    audioPlayer.Stop(name);
                };

                this.machine.PauseSoundCallBack = (name) =>
                {
                    // TODO : y a pas de pause pour le moment dans le audioplayer
                };

                // L'affichage est ready
                // il semblerait que le demrrage trop to de DrawStart / Update fasse planter la XBOX

                //this.machine.DrawCallback = (frameExecuted) =>
                //{
                //    this.machine.Screen.Clear(Argb32.White);
                //};

                //this.SlateView.DrawStart += OnSlateViewDraw;
                //this.SlateView.Update += OnSlateViewUpdate;

                // Lancement de la console
                //StartWatch();
                await this.machine.StartAsync(cartridge);
                //StopWatch("machine.StartAsync");

                this.cartridge = this.machine.Cartridge;
                this.screen = this.machine.Screen;
                this.videoMemory = this.machine.VideoMemory;

                this.xboxGamepadManager.Start(this.machine);

                this.keyboardGamepad = this.machine.Gamepads.GetFreeGamepad();

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
                //StartWatch();
                this.machine.Initialize();
                //StopWatch("Machine initialize");

                // On appelle Update de la machine pour lancer le callback
                var cartridgeUpdateCallback = this.machine.UpdatedCallback;

                this.machine.UpdatedCallback = () =>
                {
                    cartridgeUpdateCallback?.Invoke();
                    this.FrameUpdated?.Invoke();
                };

                // la machine appelera le DrawCallback à chaque Render
                var cartridgeDrawCallback = this.machine.DrawCallback;
                this.machine.DrawCallback = (frameExecuted) =>
                {
                    cartridgeDrawCallback?.Invoke(frameExecuted);
                    this.FrameDrawn?.Invoke(frameExecuted);
                };

                this.SlateView.DrawStart += OnSlateViewDraw;
                this.SlateView.Update += OnSlateViewUpdate;

                this.GotFocus += OnSugoiGotFocus;
                this.LostFocus += OnSugoiLostFocus;
            }
        }

        // La manette virtuelle keyboard
        Sugoi.Core.Gamepad keyboardGamepad = new Core.Gamepad();

        /// <summary>
        /// Touche retour en arrière
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            // pas le droit de sortir !
            e.Handled = true;
        }

        /// <summary>
        /// Prechargement des fichiers son
        /// </summary>
        /// <param name="name"></param>
        /// <param name="channelCount"></param>
        /// <returns></returns>

        private async Task PreloadSoundAsync(string name, int channelCount)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            var folder = (StorageFolder)await storageFolder.TryGetItemAsync("Files");
            var file = await folder.GetFileAsync(name);

            await audioPlayer.AddSoundAsync(name, file, channelCount);
        }

        /// <summary>
        /// Ecriture d'un fichier en provenance de la cartouche (son par exemple)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="streamReader"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        private async Task<bool> WriteFileAsync(string name, BinaryReader streamReader, int count)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                StorageFolder folder = (StorageFolder)await storageFolder.TryGetItemAsync("Files");
                
                if (folder == null)
                {
                    folder = await storageFolder.CreateFolderAsync("Files");
                }

                var storageFile = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

                byte[] data = streamReader.ReadBytes(count);

                using (var fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var streamWriter = new BinaryWriter(fileStream.AsStream()))
                    {
                        streamWriter.BaseStream.Position = 0;

                        streamWriter.Write(data);
                        streamWriter.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Sauvegarde de la batterie
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>

        private async Task<bool> WriteBatteryRamAsync(byte[] memory)
        {
            try
            {
                StorageFolder storageFolder =ApplicationData.Current.LocalFolder;

                var storageFile = await storageFolder.CreateFileAsync("BatteryRam.bin", CreationCollisionOption.OpenIfExists);

                using (var fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var stream = new BinaryWriter(fileStream.AsStream()))
                    {
                        stream.BaseStream.Position = 0;

                        stream.Write(memory);

                        stream.Close();

                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private async Task<byte[]> ReadBatteryRamAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                StorageFile file = (StorageFile) await storageFolder.TryGetItemAsync("BatteryRam.bin");

                if (file != null)
                {
                    using (var fileStream = await file.OpenReadAsync())
                    {
                        using (var stream = new BinaryReader(fileStream.AsStream()))
                        {
                            var memory = stream.ReadBytes(machine.BatteryRamSize);

                            stream.Close();

                            return memory;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
            }

            return null;
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

            // nettoyage au cas ou une touche n'aurait pas été relevée après le départ du controle
            //this.gamepad1.Release();
            //this.gamepad2.Release();

            System.Diagnostics.Debug.WriteLine("SUGOI GotFocus");
        }

        public void Stop()
        {
            if (IsStarted == true)
            {
                // Touche en arrière
                if (navigationManager != null)
                {
                    navigationManager.BackRequested -= BackRequested;
                }

                this.SlateView.DrawStart -= OnSlateViewDraw;
                this.SlateView.Update -= OnSlateViewUpdate;

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

            //if (e.VirtualKey != VirtualKey.GamepadRightShoulder)
            //{
            //    Debug.WriteLine(e.VirtualKey + " " + e.KeyStatus.ScanCode);
            //}

            //var virtualKey = e.VirtualKey;

            //switch(e.KeyStatus.ScanCode)
            //{
            //    case 44: // W en français, Z en anglais, ..
            //        virtualKey = VirtualKey.W;
            //        break;
            //    case 45: // X en français, ? en anglais, ..
            //        virtualKey = VirtualKey.X;
            //        break;
            //}

            switch (e.VirtualKey)
            {
                case VirtualKey.Up:
                    keyboardGamepad.Press(GamepadKeys.Up);
                    break;
                case VirtualKey.Down:
                    keyboardGamepad.Press(GamepadKeys.Down);
                    break;
                case VirtualKey.Right:
                    keyboardGamepad.Press(GamepadKeys.Right);
                    break;
                case VirtualKey.Left:
                    keyboardGamepad.Press(GamepadKeys.Left);
                    break;
                case VirtualKey.Z:
                case VirtualKey.W:
                case VirtualKey.Y:
                    keyboardGamepad.Press(GamepadKeys.ButtonA);
                    break;
                case VirtualKey.X:
                    keyboardGamepad.Press(GamepadKeys.ButtonB);
                    break;
                case VirtualKey.Space:
                    keyboardGamepad.Press(GamepadKeys.ButtonStart);
                    break;
                // pleine écran
                case VirtualKey.F11:

                    try
                    {
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
                    }
                    catch
                    {
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

            //var virtualKey = e.VirtualKey;

            //switch (e.KeyStatus.ScanCode)
            //{
            //    case 44: // W en français, Z en anglais, ..
            //        virtualKey = VirtualKey.W;
            //        break;
            //    case 45: // X en français, ? en anglais, ..
            //        virtualKey = VirtualKey.X;
            //        break;
            //}

            switch (e.VirtualKey)
            {
                case VirtualKey.Up:
                    keyboardGamepad.Release(GamepadKeys.Up);
                    break;
                case VirtualKey.Down:
                    keyboardGamepad.Release(GamepadKeys.Down);
                    break;
                case VirtualKey.Right:
                    keyboardGamepad.Release(GamepadKeys.Right);
                    break;
                case VirtualKey.Left:
                    keyboardGamepad.Release(GamepadKeys.Left);
                    break;
                    // pour prendre en compte les clavier Qwerty/Azerty/Qwertz
                case VirtualKey.Z:
                case VirtualKey.W:
                case VirtualKey.Y:
                    keyboardGamepad.Release(GamepadKeys.ButtonA);
                    break;
                case VirtualKey.X:
                    keyboardGamepad.Release(GamepadKeys.ButtonB);
                    break;
                case VirtualKey.Space:
                    keyboardGamepad.Release(GamepadKeys.ButtonStart);
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
        
        private void OnSlateViewUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (machine.IsStarted)
            {
                var xboxGamepads = xboxGamepadManager.GetGamepads();

                // fusion entre les Gamepads XBOX et le keyboard

                int value1 = 0;
                int value2 = 0;

                if (xboxGamepads[0] != null)
                {
                    value1 = xboxGamepads[0].GetValue();
                }

                if (xboxGamepads[1] != null)
                {
                    value2 = xboxGamepads[1].GetValue();
                }

                value1 = value1 | value2;

                value2 = keyboardGamepad.GetValue();

                value1 = value1 | value2;

                machine.GamepadGlobal.SetValue(value1);
            }
        }

        /// <summary>
        /// Drw du SlateView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        private void OnSlateViewDraw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            // appel du UpdateCallback
            var screenArgb32Array = this.machine.RenderOneFrame(args.Timing.IsRunningSlowly);
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

        /// <summary>
        /// Y a t'il un keyboard
        /// </summary>
        /// <returns></returns>

        private bool HavePhysicalKeyboard()
        {
            KeyboardCapabilities capabilities = new KeyboardCapabilities();
            return capabilities.KeyboardPresent == 1;
        }
    }
}
