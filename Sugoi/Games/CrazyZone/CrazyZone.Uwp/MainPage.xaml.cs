using CrazyZone;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CrazyZone.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.SugoiControl.Loaded += OnSugoiLoaded;

            Window.Current.CoreWindow.Activated += CoreWindow_Activated;
        }

        private void CoreWindow_Activated(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs args)
        {
            this.SugoiControl.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Console prête à fonctionner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void OnSugoiLoaded(object sender, RoutedEventArgs e)
        {
            await this.SugoiControl.StartAsync(new CrazyZoneCartridge());
        }
    }
}
