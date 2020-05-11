using Sugoi.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CrazyZone.Pages
{
    public class PlayPage : IPage
    {
        private Game game;
        private Machine machine;
        private int frame;

        Map[] maps;
 

        public PlayPage(Game game)
        {
            this.game = game;
            this.machine = game.Machine;
        }

        public void Initialize()
        {
            frame = 0;
            this.maps = AssetStore.ParallaxMaps;
        }

        public void Update()
        {
            var gamepad = this.machine.Gamepad;
        }

        public void Draw(int frameExecuted)
        {
            var screen = this.machine.Screen;
            var font = screen.Font;

            screen.Clear(new Argb32(0xeeeecc));

            frame += frameExecuted;

            int frameScroll = (int)(frame * 0.5);

            screen.DrawScrollMap(maps[0], true, (int)(-frameScroll * 0.25), 0, 0, screen.Height - maps[0].Height - 32, 320, 136);
            screen.DrawScrollMap(maps[1], true, (int)(-frameScroll * 0.50), 0, 0, screen.Height - maps[1].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[3], true, (int)(-frameScroll * 1.00), 0, 0, screen.Height - maps[3].Height - 16, 320, 136);
            screen.DrawScrollMap(maps[2], true, (int)(-frameScroll * 1.25), 0, 0, screen.Height - maps[2].Height, 320, 136);
            screen.DrawScrollMap(maps[4], true, (int)(-frameScroll * 1.50), 0, 0, screen.Height - maps[4].Height, 320, 136);
            screen.DrawScrollMap(maps[5], true, (int)(-frameScroll * 2.00), 0, 0, screen.Height - maps[5].Height, 320, 136);

            screen.DrawText(frameExecuted == 1 ? "1" : "2", 0, 0);
        }
    }
}
