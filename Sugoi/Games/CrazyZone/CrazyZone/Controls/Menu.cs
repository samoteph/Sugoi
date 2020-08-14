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

        MenuItem[] items;
        Animator cursorAnimator;
        int centerX;
        int verticalInterval;
        int topCursorMargin;

        public Menu()
        {

        }

        int characterLength;
        int labelWidth;

        public void Initialize(Machine machine, Animator cursorAnimator, string[] entries, int verticalInterval = 12, int topCursorMargin = 0)
        {
            this.machine = machine;
            this.screen = this.machine.Screen;
            this.gamepad = this.machine.GamepadGlobal;

            this.verticalInterval = verticalInterval;
            this.topCursorMargin = topCursorMargin;

            this.cursorAnimator = cursorAnimator;

            // Calcul de la taille max des chaine de caracteres
            characterLength = 0;

            items = new MenuItem[entries.Length];

            for(int i=0; i<entries.Length; i++)
            {
                var entry = entries[i];

                var item = new MenuItem(entry, machine);

                this.items[i] = item;

                if (entry.Length > characterLength)
                {
                    characterLength = entry.Length;
                }
            }

            // Taille de la chaine de caractères en pixel (pour les ZoneTouch)
            labelWidth = characterLength * screen.Font.FontSheet.TileWidth;

            this.centerX = (screen.BoundsClipped.Width - labelWidth ) / 2;

            this.InitializeTouchZones();
        }

        public int MenuPosition
        {
            get
            {
                return menuPosition;
            }

            set
            {
                if (items != null)
                {
                    if (value < 0)
                    {
                        menuPosition = items.Length - 1;
                    }
                    else
                    {
                        menuPosition = value % items.Length;
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
            // Test de la Zone de Touch de items du menu
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];

                    item.TouchZone.Update();

                    if (item.TouchZone.IsTaping)
                    {
                        MenuPosition = i;
                        this.CursorMoveCallback?.Invoke(MenuPosition);
                        this.MenuSelectedCallback?.Invoke(this.MenuPosition);
                        break;
                    }
                }
            }

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

                gamepad.WaitForRelease(30);
            }
            else if (verticalController == GamepadKeys.Up)
            {
                MenuPosition--;

                this.CursorMoveCallback?.Invoke(MenuPosition);

                gamepad.WaitForRelease(30);
            }

            if (gamepad.IsPressed(GamepadKeys.ButtonA))
            {
                gamepad.WaitForRelease(() =>
                {
                    this.MenuSelectedCallback?.Invoke(this.MenuPosition);
                });
            }
        }

        public int X
        {
            get;
            set;
        } = int.MinValue;

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                if(value != y)
                {
                    y = value;

                    this.InitializeTouchZones();
                }
            }
        }

        private void InitializeTouchZones()
        {
            if (items != null)
            {
                int x = this.GetCenteredMenuX();

                int itemWidth = cursorAnimator.Width + 8 + labelWidth;

                for (int i = 0; i < items.Length; i++)
                {
                    items[i].TouchZone.Start(new Rectangle(x, Y + i * verticalInterval, itemWidth, verticalInterval));
                }
            }
        }

        private int y;

        private int GetCenteredMenuX()
        {
            // placement manuel
            int x = X;

            // placement par défaut
            if (x == int.MinValue)
            {
                x = centerX - cursorAnimator.Width - 8;
            }

            return x;
        }

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="frameExecuted"></param>
        /// <param name="y"></param>

        public void Draw(int frameExecuted)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                screen.DrawText(item.Label, centerX, Y + i * verticalInterval);
            }

            int x = this.GetCenteredMenuX();

            int centerCursorY = (screen.Font.FontSheet.TileHeight - cursorAnimator.Height) / 2;

            cursorAnimator.Draw(screen, x, Y + topCursorMargin + centerCursorY + menuPosition * verticalInterval, true, false);
        }
    }

    /// <summary>
    /// Item d'un menu
    /// </summary>

    internal class MenuItem
    {
        public TouchZone TouchZone
        {
            get;
            private set;
        }

        public MenuItem(string label, Machine machine)
        {
            this.Label = label;
            this.TouchZone = new TouchZone(machine.TouchPoints);
        }

        public void Start(Rectangle touchZone)
        {
            this.TouchZone.Start(touchZone);
        }

        public string Label
        {
            get;
            private set;
        }
    }
}
