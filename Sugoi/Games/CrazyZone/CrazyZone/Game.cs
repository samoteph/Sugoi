using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone
{
    public class Game
    {
        public Machine Machine
        {
            get;
            private set;
        }

        public Dictionary<Type, IPage> Pages
        {
            get;
            private set;
        }

        public void Start(Machine machine)
        {
            this.Machine = machine;

            this.Pages = new Dictionary<Type, IPage>(10);

            // ajout des pages
            Pages.Add(typeof(HomePage), new HomePage(this));
            Pages.Add(typeof(PlayPage), new PlayPage(this));
        }

        public void Navigate(Type typePage)
        {
            var page = Pages[typePage];

            page.Initialize();

            this.Machine.InitializeCallback = null;
            this.Machine.UpdateCallback = page.Update;
            this.Machine.DrawCallback = page.Draw;
        }
    }

    public enum PageTypes
    {
        Home,
    }
}
