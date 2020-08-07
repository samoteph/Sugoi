using StarterGame.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class MyGame : Game
    {
        protected override void StartOverride()
        {
            this.Navigation.AddPage<HomePage>();
            this.Navigation.AddPage<PlayPage>();
            
            this.Navigation.Navigate<HomePage>("15");
        }
    }
}
