using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone
{
    public class Game
    {
        private Action emptyAction = () => { };
        private Action<int> fadeDrawAction;

        public Leaderboard Leaderboard
        {
            get;
            private set;
        }

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

            this.Leaderboard = new Leaderboard(machine);

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
            this.Navigate(typeof(HomePage));
            //this.Navigate(typeof(HallOfFamePage));
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

        internal async Task SaveNameAndHisScoreAsync(char[] name, int score, Action<bool> saveCompleted = null)
        {
            this.Machine.BatteryRam.WriteCharArray((int)BatteryRamAddress.Name, name);

            if (score > 0)
            {
                // name avec padleft sur 6 caractères
                var nameString = new string(name).Replace("-", "");

                bool isSaved = await this.Leaderboard.SaveScoreAsync(nameString, score);
                // on enregistre si la sauvegarde a bien eu lieu sinon on reesera plus tard
                this.Machine.BatteryRam.WriteBool((int)BatteryRamAddress.IsHiScoreAndNameSaved, isSaved);

                await this.Machine.BatteryRam.FlashAsync();

                saveCompleted?.Invoke(isSaved);
            }
            else
            {
                await this.Machine.BatteryRam.FlashAsync();
            }
        }

        char[] names = new char[6];

        internal Task SaveNameAndScoreIfNeededAsync(Action<bool> saveCompleted = null)
        {
            if(Machine.BatteryRam.ReadBool((int)BatteryRamAddress.IsHiScoreAndNameSaved) == false)
            {
                this.Machine.BatteryRam.ReadCharArray((int)BatteryRamAddress.Name, names);
                var score = this.Machine.BatteryRam.ReadInt((int)BatteryRamAddress.HiScore);

                return this.SaveNameAndHisScoreAsync(names, score, saveCompleted);
            }

            return Task.CompletedTask;
        }
    }

    public enum BatteryRamAddress
    {
        HiScore = 0, // 4 octets
        Name = 4, // 6 * 2 octets
        IsHiScoreAndNameSaved = 20, // 1 octet
        End = 21,
    }
}
