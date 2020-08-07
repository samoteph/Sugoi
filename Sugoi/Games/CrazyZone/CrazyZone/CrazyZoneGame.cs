using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone
{
    public class CrazyZoneGame : Game
    {
        public Leaderboard Leaderboard
        {
            get;
            private set;
        }

        protected override void StartOverride()
        {
            this.Leaderboard = new Leaderboard(this.Machine);

            // ajout des pages
            this.Navigation.AddPage<HomePage>();
            this.Navigation.AddPage<PlayPage>();
            this.Navigation.AddPage<MultiPlayPage>();
            this.Navigation.AddPage<InputNamePage>();
            this.Navigation.AddPage<HallOfFamePage>();

            // Lancement du jeu
            this.Navigation.Navigate<HomePage>();
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
