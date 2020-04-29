using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    public struct MapTileDescriptor
    {
        public static MapTileDescriptor Hidden = new MapTileDescriptor(0) { hidden = true };

        public MapTileDescriptor(MapTileDescriptor tile)
        {
            this.number = tile.number;
            this.isHorizontalFlipped = tile.isHorizontalFlipped;
            this.isVerticalFlipped = tile.isVerticalFlipped;
            this.hidden = tile.hidden;
        }

        public MapTileDescriptor(int number)
        {
            this.number = number;
            this.isHorizontalFlipped = false;
            this.isVerticalFlipped = false;
            this.hidden = false;
        }

        // la tuile ne sera jamais affiché
        public bool hidden;
        public int number;
        public bool isHorizontalFlipped;
        public bool isVerticalFlipped;

        public MapTileDescriptor Hide(bool hidden)
        {
            return new MapTileDescriptor(this)
            {
                hidden = hidden
            };
        }

        public MapTileDescriptor SwitchHide()
        {
            return new MapTileDescriptor(this)
            {
                hidden = !this.hidden
            };
        }

        public MapTileDescriptor Flip(bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            return new MapTileDescriptor()
            {
                number = this.number,
                hidden = this.hidden,
                isHorizontalFlipped = isHorizontalFlipped,
                isVerticalFlipped = isVerticalFlipped,
            };
        }

        public MapTileDescriptor Number(int number)
        {
            return new MapTileDescriptor(this) 
            { 
                number = number 
            };
        }
    }
}
