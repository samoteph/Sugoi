using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone
{
    public class Game
    {
        private Action emptyAction = () => { };
        private Action<int> fadeDrawAction;

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
            Pages.Add(typeof(MultiPlayPage), new MultiPlayPage(this));
            Pages.Add(typeof(InputNamePage), new InputNamePage(this));
            Pages.Add(typeof(HallOfFamePage), new HallOfFamePage(this));

            fadeDrawAction = (frameExecuted) =>
            {
                this.Machine.Screen.Clear(Argb32.Black);
            };

            // Lancement du jeu
            //this.Navigate(typeof(HomePage));
            this.Navigate(typeof(HallOfFamePage));
        }

        public IPage Navigate(Type typePage)
        {
            var page = Pages[typePage];

            page.Initialize();

            this.Machine.InitializeCallback = null;
            this.Machine.UpdatingCallback = page.Updating;
            this.Machine.UpdatedCallback = page.Updated;
            this.Machine.DrawCallback = page.Draw;

            return page;
        }

        public IPage NavigateWithFade(Type typePage)
        {
            var page = Pages[typePage];

            this.Machine.UpdatingCallback = emptyAction;
            this.Machine.UpdatedCallback = emptyAction;
            this.Machine.DrawCallback = fadeDrawAction;

            this.Machine.WaitForFrame(30, () =>
            {
                page.Initialize();

                this.Machine.InitializeCallback = null;
                this.Machine.UpdatingCallback = page.Updating;
                this.Machine.UpdatedCallback = page.Updated;
                this.Machine.DrawCallback = page.Draw;
            });

            return page;
        }

        private string currentMusicKey;

        /// <summary>
        /// Jouer la music en loop et laisser la music en cours s'il c'est deja la même qui est jouée 
        /// </summary>
        /// <param name="musicLoop"></param>

        public void PlayLoop(string musicKey)
        {
            if(musicKey != currentMusicKey)
            {
                this.Machine.Audio.Stop(currentMusicKey);

                currentMusicKey = musicKey;

                this.Machine.Audio.PlayLoop(musicKey);
            }
        }

    }

    public enum BatteryRamAddress
    {
        HiScore = 0, // 4 octets
        Name = 4, // 6 * 2 octets
        End = 20,
    }
}
