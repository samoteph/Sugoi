using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    /// <summary>
    /// Description d'une font
    /// </summary>

    public class Font
    {
        public Dictionary<char, int> tileNumbers = new Dictionary<char, int>(100);

        public int currentIndex = 0;

        public SurfaceFontSheet FontSheet
        {
            get;
            set;
        }

        public int UnknownTileNumber
        {
            get;
            set;
        } = 0;

        public bool AddCharacter(char character)
        {
            int index = currentIndex;
            currentIndex++;

            if (tileNumbers.ContainsKey(character) == false)
            {
                tileNumbers.Add(character, index);
                return true;
            }

            return false;
        }

        public int CharacterIndex
        {
            get
            {
                return currentIndex;
            }

            set
            {
                currentIndex = value;
            }
        }

        public void AddCharacters(string characters)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                this.AddCharacter(characters[i]);
            }
        }

        public void AddCharacters(params char[] characters)
        {
            for(int i=0; i < characters.Length; i++)
            {
                this.AddCharacter(characters[i]);
            }
        }

        public void AddCharacters(CharactersGroups group)
        {
            switch(group)
            {
                case CharactersGroups.AlphaLower:
                    this.AddCharacters('a', 26);
                    break;
                case CharactersGroups.AlphaUpper:
                    this.AddCharacters('A', 26);
                    break;
                case CharactersGroups.AlphaUpperAndLower:
                    
                    var index = this.currentIndex;
                    
                    this.AddCharacters('A', 26);
                    
                    currentIndex = index;
                    
                    this.AddCharacters('a', 26);
                    break;
                case CharactersGroups.Numeric:
                    this.AddCharacters('0', 10);
                    break;
            }
        }

        public void AddCharacters(char startCharacter, int count)
        {
            int asciiCharacter = (int)startCharacter;

            for (int i = 0; i < count; i++)
            {
                this.AddCharacter((char)(asciiCharacter + i));
            }
        }

        public int GetTileNumber(char character)
        {
            if (this.tileNumbers.ContainsKey(character))
            {
                return this.tileNumbers[character];
            }

            return UnknownTileNumber;
        }
    }

    public enum CharactersGroups
    {
        AlphaUpper,
        AlphaLower,
        AlphaUpperAndLower,
        Numeric
    }
}
