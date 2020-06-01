using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Controls
{
    public class Menu
    {
        Machine machine;
        Screen screen;
        Gamepad gamepad;

        string[] entries;
        Animator cursorAnimator;
        int centerX;
        int verticalInterval;
        int topCursorMargin;

        public Menu()
        {

        }

        public void Initialize(Machine machine, Animator cursorAnimator, string[] entries, int verticalInterval = 12, int topCursorMargin = 0)
        {
            this.machine = machine;
            this.screen = this.machine.Screen;
            this.gamepad = this.machine.GamepadGlobal;

            this.verticalInterval = verticalInterval;
            this.topCursorMargin = topCursorMargin;

            this.cursorAnimator = cursorAnimator;

            this.entries = entries;

            int maxLength = 0;

            foreach(var entry in entries)
            {
                if(entry.Length > maxLength)
                {
                    maxLength = entry.Length;
                }
            }

            this.centerX = (screen.BoundsClipped.Width - maxLength * screen.Font.FontSheet.TileWidth) / 2;
        }

        public int MenuPosition
        {
            get
            {
                return menuPosition;
            }

            set
            {
                if (entries != null)
                {
                    if (value < 0)
                    {
                        menuPosition = -(value % entries.Length);
                    }
                    else
                    {
                        menuPosition = value % entries.Length;
                    }
                }
                else
                {
                    throw new Exception("Please define some entries before setting MenuPosition");
                }
            }
        }

        private int menuPosition;

        /// <summary>
        /// en arrière ! 
        /// </summary>

        public Action BackCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Un menu est selectionnée
        /// </summary>

        public Action<int> MenuSelectedCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Le cursor est bougé
        /// </summary>

        public Action<int> CursorMoveCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Mise à jour
        /// </summary>

        public void Update()
        {
            // retour en arrière        
            if (gamepad.IsPressed(GamepadKeys.ButtonB))
            {
                gamepad.WaitForRelease(() =>
                {
                    BackCallback?.Invoke();
                });
            }

            var verticalController = gamepad.VerticalController;

            if (verticalController == GamepadKeys.Down)
            {
                MenuPosition++;

                this.CursorMoveCallback?.Invoke(MenuPosition);

                gamepad.WaitForRelease();
            }
            else if (verticalController == GamepadKeys.Up)
            {
                MenuPosition--;

                this.CursorMoveCallback?.Invoke(MenuPosition);

                gamepad.WaitForRelease();
            }

            if (gamepad.IsPressed(GamepadKeys.ButtonA))
            {
                gamepad.WaitForRelease(() =>
                {
                    this.MenuSelectedCallback?.Invoke(this.MenuPosition);
                });
            }
        }

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="frameExecuted"></param>
        /// <param name="y"></param>

        public void Draw(int frameExecuted, int y)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                screen.DrawText(entry, centerX, y + i * verticalInterval);
            }

            int centerCursorY = (screen.Font.FontSheet.TileHeight - cursorAnimator.Height) / 2;

            cursorAnimator.Draw(screen, centerX - cursorAnimator.Width - 8, y + topCursorMargin + centerCursorY + menuPosition * verticalInterval, true, false);
        }
    }
}
