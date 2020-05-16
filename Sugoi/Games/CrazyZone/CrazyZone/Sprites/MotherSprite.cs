﻿using CrazyZone.Pages;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;

namespace CrazyZone.Sprites
{
    public class MotherSprite : Sprite
    {
        public const int HEALTH = 100;

        private Machine machine;
        private PlayPage page;
        private SurfaceTileSheet tiles;
        
        private Map[] flyMaps;
        private Map[] openMaps;
        private Map tiredMap;

        private int openIndex;
        private int frameOpen;
        private int frameBaby;

        private int flyIndex;

        private int health;

        private int tileNose = -1;

        private bool isTired = false;

        public MotherSprite Create(Machine machine, PlayPage page, int x, int y)
        {
            this.machine = machine;
            this.page = page;

            tiles = AssetStore.Tiles;
            flyMaps = AssetStore.MotherFlyMaps;
            openMaps = AssetStore.MotherOpenMaps;
            tiredMap = AssetStore.MotherTired;

            this.ScrollWidth = page.ScrollWidth;

            X = x;
            Y = y;

            openIndex = 0;
            Initialize();
            
            return this;
        }

        public int Direction
        {
            get;
            set;
        }

        public bool IsFiring
        {
            get;
            private set;
        }

        public bool IsOpening
        {
            get;
            private set;
        }

        public override bool CanCollide
        {
            get
            {
                return this.isTired == false;
            }
        }

        public override void Collide(ISprite sprite)
        {
            if (this.isTired == false)
            {
                health--;

                if (health <= 0)
                {
                    this.isTired = true;

                    this.page.Kabooms.GetSprite()
                        .Create(this.machine, this.page)
                        .Explode(this.X, this.Y+ 8, true);

                    this.page.Kabooms.GetSprite()
                        .Create(this.machine, this.page)
                        .Explode(this.X + 24, this.Y + 8, true);

                    this.page.Kabooms.GetSprite()
                        .Create(this.machine, this.page)
                        .Explode(this.X + 16, this.Y + 16, true);

                }
            }
        }

        public override void Initialize()
        {
            this.IsAlive = true;

            this.isTired = false;

            frameOpen = 0;
            frameBaby = 0;

            this.Width = openMaps[0].Width + (2 * 8); // on ajoute les ailes qui dépassent
            this.Height = openMaps[0].Height;

            this.health = 20;

            this.InitializeCollision(8 + 3, 8, 16 + 3, 3);
        }

        public override void Updated()
        {
            if(this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;
            var frame = this.machine.Frame;

            if (this.isTired == false)
            {
                // avant de faire un bébé on attend des centaines de frames
                // on passe ensuite en ouverture IsOpening
                if(frameBaby > 100)
                {
                    this.IsOpening = true;
                }
                else
                {
                    frameBaby++;
                }

                // l'ouverture est activé on peut lancé le bébé

                if (this.IsOpening)
                {
                    frameOpen++;

                    if (frameOpen > 10)
                    {
                        // Fermeture 
                        openIndex = 0;
                        frameOpen = 0;
                        frameBaby = 0;
                        this.IsOpening = false;                    
                    }
                    else
                    {
                        if(frameOpen == 1)
                        {
                            // lancement du bébé
                            page.Babies.GetSprite().Create(machine, page, X + (this.Width / 2) - 16, Y + (Height / 2) - 8);
                        }

                        openIndex = 1;
                    }
                }

                // Ailes
                var frameFly = frame % 30;

                if (frameFly < 10)
                {
                    flyIndex = 0;
                }
                else if (frameFly < 20)
                {
                    flyIndex = 1;
                }
                else
                {
                    flyIndex = 2;
                }

                // santé

                if (health < (HEALTH * 0.33))
                {
                    tileNose = 151;
                }
                else if (health < (HEALTH * 0.66))
                {
                    tileNose = 176;
                }
                else
                {
                    tileNose = -1;
                }
            }

            base.Updated();
        }

        public override void Draw(int frameExecuted)
        {
            if (this.IsAlive == false)
            {
                return;
            }

            var screen = this.machine.Screen;

            if (this.isTired == false)
            {
                screen.DrawSpriteMap(openMaps[openIndex], XScrolled, YScrolled, false, false);

                if (flyIndex != 2)
                {
                    screen.DrawSpriteMap(flyMaps[flyIndex], XScrolled - 11, YScrolled - 9);
                    screen.DrawSpriteMap(flyMaps[flyIndex], XScrolled + 27, YScrolled - 9, true, false);
                }
                else
                {
                    screen.DrawSpriteMap(flyMaps[2], XScrolled - 11, YScrolled - 1);
                    screen.DrawSpriteMap(flyMaps[2], XScrolled + 27, YScrolled - 1, true, false);
                }

                if (tileNose > -1)
                {
                    screen.DrawTile(tiles, tileNose, XScrolled + 12, YScrolled + 7);
                }
            }
            else
            {
                screen.DrawSpriteMap(tiredMap, XScrolled - 8, YScrolled, false, false);
            }
        }
    }
}
