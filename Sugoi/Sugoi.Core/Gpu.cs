using System;

namespace Sugoi.Core
{
    public class Gpu
    {
        private VideoMemory videoMemory;
        private Screen screen;

        public VideoMemory VideoMemory
        {
            get
            {
                return this.videoMemory;
            }
        }

        public Screen Screen
        {
            get
            {
                return this.screen;
            }
        }

        public Gpu()
        {
            this.videoMemory = new VideoMemory();
        }

        public void Start(int videoMemorySize, int screenWidth, int screenHeight)
        {
            this.videoMemory.Start(videoMemorySize, screenWidth, screenHeight);
            this.screen = new Screen();

            this.screen.Start(screenWidth, screenHeight);
        }

        public void Stop()
        {
        }

        /// <summary>
        /// Callback du Update
        /// </summary>

        public Action UpdateCallback
        {
            get;
            set;
        }

        public bool IsRendering
        {
            get;
            private set;
        }

        public Argb32[] Render()
        {
            if (IsRendering == true)
            {
                return null;
            }

            IsRendering = true;

            if (UpdateCallback != null)
            {
                UpdateCallback();
            }

            IsRendering = false;

            return screen.Pixels;
        }
    }
}