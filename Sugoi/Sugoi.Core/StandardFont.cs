using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public class StandardFont : Font
    {
        /// <summary>
        /// Déclaration des charactères constituant la fonte standard
        /// </summary>

        public StandardFont()
        {
            this.UnknownTileNumber = 32;
            this.AddCharacters(" !\"#$%&'()*+,-./");
            this.AddCharacters(CharactersGroups.Numeric);
            this.AddCharacters(":;<=>?@");
            this.AddCharacters(CharactersGroups.AlphaUpperAndLower);
            this.AddCharacters("[]^_'");
        }
    }
}