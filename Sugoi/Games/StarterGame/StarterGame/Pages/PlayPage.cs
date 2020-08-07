using Sugoi.Core;
using Sugoi.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame.Pages
{
    public class PlayPage : INavigationPage
    {
        public PlayPage()
        {
            this.Machine = GameService.Instance.Game.Machine;
            ;
        }

        public Machine Machine
        {
            get;
        }

        public void Draw(int frameExecuted)
        {
            var game = GameService.Instance.Game;
            var screen = this.Machine.Screen;
            
            screen.Clear(Argb32.Green);
            screen.DrawText("PlayPage is here! param=" + game.Navigation.CurrentPageInformation?.Parameter);
        }

        public void Initialize()
        {
        }

        public bool Navigate(NavigationStates state, object parameter)
        {
            System.Diagnostics.Debug.WriteLine("Navigate PlayPage=" + state + " " + parameter);
            return true;
        }


        public void Updated()
        {
        }

        public void Updating()
        {
            if(this.Machine.GamepadGlobal.IsButtonsPressed == true)
            {
                var game = GameService.Instance.Game;
                game.Navigation.GoBackWithFade();
            }
        }
    }
}
