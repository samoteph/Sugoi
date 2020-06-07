using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Pages
{
    public class MultiPlayPage : IPage
    {
        Machine machine;
        Screen screen;

        Rectangle rectPlayer1;
        Rectangle rectPlayer2;

        PlayPage player1Page;
        PlayPage player2Page;

        /// <summary>
        /// Stockage du Gamepad de la PlayPage qui appuie en premier sur Start
        /// </summary>

        public Gamepad FirstGamepad
        {
            get;
            private set;
        }

        public MultiPlayPage(Game game)
        {
            machine = game.Machine;
            screen = machine.Screen;

            rectPlayer1 = new Rectangle(0, 0, screen.Width / 2, screen.Height);
            rectPlayer2= new Rectangle(rectPlayer1.Width, rectPlayer1.Y, rectPlayer1.Width, rectPlayer1.Height);

            this.State = MultiStates.Play;

            player1Page = new PlayPage(game);
            player2Page = new PlayPage(game);
        }

        public MultiStates State
        {
            get;
            private set;
        }

        public void ReadyToPlay()
        {
            this.State = MultiStates.P1andP2Ready;

            this.machine.WaitForFrame(30, () =>
            {
                this.player1Page.State = PlayStates.Play;
                this.player2Page.State = PlayStates.Play;
                this.player1Page.ScrollX = 0;
                this.player2Page.ScrollX = 0;
                this.State = MultiStates.Play;

                this.machine.Audio.PlayLoop("playSound");

            });
        }

        public void Draw(int frameExecuted)
        {
            if(State == MultiStates.P1andP2Ready)
            {
                screen.ResetClip();
                screen.Clear(Argb32.Black);
                return;
            }

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer1);
            }
            else
            {
                screen.ResetClip();
            }

            player1Page.Draw(frameExecuted);

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer2);
                player2Page.Draw(frameExecuted);

                screen.SetClip(screen.Bounds);
                screen.DrawRectangle(rectPlayer1.Width - 2, 0, 4, rectPlayer1.Height, Argb32.Black);
            }
        }

        public void Initialize()
        {
            this.State = MultiStates.WaitForP1andP2;
            this.FirstGamepad = null;

            screen.SetClip(rectPlayer1);
            player1Page.Initialize(this, Players.Player1);

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer2);
                player2Page.Initialize(this, Players.Player2);
            }
        }

        public void Updated()
        {
            if(State == MultiStates.P1andP2Ready)
            {
                return;
            }

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer1);
            }
            else
            {
                screen.ResetClip();
            }

            player1Page.Updated();

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer2);
                player2Page.Updated();
            }
        }

        public void Updating()
        {
            if(State == MultiStates.P1andP2Ready)
            {
                return;
            }

            if (State!= MultiStates.Quit)
            {
                screen.SetClip(rectPlayer1);
            }
            else
            {
                screen.ResetClip();
            }

            player1Page.Updating();

            if (State != MultiStates.Quit)
            {
                screen.SetClip(rectPlayer2);
                player2Page.Updating();
            }
        }

        /// <summary>
        /// Une page est gameover
        /// </summary>

        internal void GameOver()
        {
            // Tout le monde est gameover, on peut partir sereinement
            if(this.player1Page.State == PlayStates.GameOver && this.player2Page.State == PlayStates.GameOver)
            {
                this.State = MultiStates.GameOver;
            }
        }

        internal void Quit()
        {
            if (this.State != MultiStates.Quit)
            {
                this.State = MultiStates.Quit;

                // passe tout le monde à l'etat Quit
                this.player1Page.Quit();
                this.player2Page.Quit();
            }
        }

        public int GetOpponentScore(Players player)
        {
            if(player == Players.Player1)
            {
                return player2Page.Score;
            }

            return player1Page.Score;
        }

        public enum MultiStates
        {
            WaitForP1andP2,
            P1andP2Ready, // fade après que les deux jouer
            Play,
            GameOver,
            Quit
        }

        public void SelectFirstGamepad(Gamepad g)
        {
            this.FirstGamepad = g;
            this.player2Page.MultiPlayState = MultiPlayStates.WaitStart;
        }

        public void UnselectFirstGamepad()
        {
            this.FirstGamepad = null;
            this.player2Page.MultiPlayState = MultiPlayStates.WaitOtherPlayer;
        }
    }
}
