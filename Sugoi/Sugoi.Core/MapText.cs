using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class MapText : Map
    {
        private char[] integerString = new char[10];

        public Font Font
        {
            get;
            private set;
        }

        public void Create(string name, int mapWidth, int mapHeight, Font font, MapTileDescriptor defaultTile)
        {
            this.Create(name, mapWidth, mapHeight, font.FontSheet, defaultTile);
            this.Font = font;
        }

        /// <summary>
        /// Fixe le texte
        /// </summary>
        /// <param name="xMap"></param>
        /// <param name="yMap"></param>
        /// <param name="text"></param>

        public void SetText(int xMap, int yMap, string text, TextPositions textPosition = TextPositions.LeftToRight)
        {
            var index = xMap + yMap * MapWidth;
            var length = text.Length;

            if (Tiles.Length < text.Length)
            {
                length = Tiles.Length;
            }

            if (textPosition == TextPositions.LeftToRight)
            {
                for (int i = 0; i < length; i++)
                {
                    var character = text[i];
                    var number = this.Font.GetTileNumber(character);
                    this.Tiles[i + index] = new MapTileDescriptor(number);
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    var character = text[text.Length - 1 - i];
                    var number = this.Font.GetTileNumber(character);
                    this.Tiles[index - i] = new MapTileDescriptor(number);
                }
            }
        }

        public void SetText(int xMap, int yMap, char[] text, TextPositions textPosition = TextPositions.LeftToRight)
        {
            var index = xMap + yMap * MapWidth;
            var length = text.Length;

            if (Tiles.Length < text.Length)
            {
                length = Tiles.Length;
            }

            if (textPosition == TextPositions.LeftToRight)
            {
                for (int i = 0; i < length; i++)
                {
                    var character = text[i];
                    var number = this.Font.GetTileNumber(character);
                    this.Tiles[i + index] = new MapTileDescriptor(number);
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    var character = text[text.Length - 1 - i];
                    var number = this.Font.GetTileNumber(character);
                    this.Tiles[index - i] = new MapTileDescriptor(number);
                }
            }
        }

        /// <summary>
        /// Affiche un integer
        /// </summary>
        /// <param name="xMap"></param>
        /// <param name="yMap"></param>
        /// <param name="integer"></param>

        public void SetText(int xMap, int yMap, int integer, TextPositions textPosition = TextPositions.LeftToRight)
        {
            int index = 0;
            var font = this.Font;

            int sign = Math.Sign(integer);
            integer = Math.Abs(integer);

            while (true)
            {
                var mask = ((integer / 10) * 10);

                int digit = integer - mask;

                integerString[index] = (char)('0' + digit);

                index++;

                integer = integer / 10;

                if (integer == 0)
                {
                    break;
                }
            }

            if (sign == -1)
            {
                integerString[index] = '-';
                index++;
            }

            int x = 0;

            if (textPosition == TextPositions.LeftToRight)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    var tileNumber = font.GetTileNumber(integerString[i]);
                    this.SetTile(xMap + x, yMap, new MapTileDescriptor(tileNumber));
                    x++;
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    var tileNumber = font.GetTileNumber(integerString[i]);
                    this.SetTile(xMap + x, yMap, new MapTileDescriptor(tileNumber));
                    x--;
                }
            }
        }

        public enum TextPositions
        {
            LeftToRight,
            RightToLeft,
        }
    }
}
