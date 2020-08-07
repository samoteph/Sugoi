using Sugoi.Core;
using Sugoi.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame.Pages
{
    public class HomePage : INavigationPage
    {
        public HomePage()
        {
            this.Machine = GameService.Instance.Game.Machine;
        }

        public Machine Machine
        {
            get;
        }

        public void Draw(int frameExecuted)
        {
            var game = GameService.Instance.Game;
            var screen = this.Machine.Screen;
            
            screen.Clear(Argb32.Red);
            screen.DrawText("HomePage in da place! param=" + game.Navigation.CurrentPageInformation?.Parameter);
        }

        public void Initialize()
        {
        }

        public bool Navigate(NavigationStates state, object parameter)
        {
            System.Diagnostics.Debug.WriteLine("Navigate HomePage=" + state + " " + parameter);
            return true;
        }

        public void Updated()
        {
        }

        public void Updating()
        {
            if (Machine.GamepadGlobal.IsButtonsPressed == true)
            {
                var game = GameService.Instance.Game;
                game.Navigation.NavigateWithFade<PlayPage>("2");
            }
        }
    }
}
