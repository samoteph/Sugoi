using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    public class Animator
    {
        public AnimationFrame[] AnimationFrames
        {
            get;
            private set;
        }

        /// <summary>
        /// Frame globale
        /// </summary>

        public int FramePlayed
        {
            get;
            private set;
        }

        /// <summary>
        /// Nombre de Frame à jouer avant de s'arreter
        /// </summary>

        public int FramePlayedMaximum
        {
            get;
            private set;
        }

        public Action<object> FramePlayedCompleted
        {
            get;
            private set;
        }

        public object FramePlayedCompletedParameter
        {
            get;
            private set;
        }

        public AnimatorStates State
        {
            get;
            private set;
        }

        /// <summary>
        /// Frame relative à l'AnimationFrame en cours
        /// </summary>

        public int CurrentFrame
        {
            get;
            private set;
        }

        public AnimationFrame CurrentAnimationFrame
        {
            get;
            private set;
        }

        /// <summary>
        /// Index de la frame courante
        /// </summary>

        public int CurrentAnimationFrameIndex
        {
            get
            {
                return currentAnimationFrameIndex;
            }
        }

        private int currentAnimationFrameIndex;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="animationFrames"></param>

        public void Initialize(params AnimationFrame[] animationFrames)
        {
            if(animationFrames == null || animationFrames.Length == 0)
            {
                throw new Exception("Pleas provide some animationFrames to Animator!");
            }

            this.AnimationFrames = animationFrames;

            this.Speed = 1;

            this.Stop();
        }

        /// <summary>
        /// Stop et demarre l'animation
        /// </summary>

        public void Start()
        {
            this.Stop();
            this.Play();
        }

        public void Start(int framePlayedMaximum, Action<object> framePlayedCompleted = null, object parameter = null)
        {
            this.Stop();
            this.Play(framePlayedMaximum, framePlayedCompleted, parameter);
        }

        public void Play()
        {
            this.Play(-1);
        }

        public void Play(int framePlayedMaximum, Action<object> framePlayedCompleted = null, object parameter = null)
        {
            State = AnimatorStates.Play;

            this.FramePlayedMaximum = framePlayedMaximum;
            this.FramePlayedCompleted = framePlayedCompleted;
            this.FramePlayedCompletedParameter = parameter;
        }

        public void Stop()
        {
            State = AnimatorStates.Stop;
            this.FramePlayed = 0;
            this.CurrentFrame = 0;
            this.CurrentAnimationFrame = AnimationFrames[0];
            this.currentAnimationFrameIndex = 0;
        }

        public void Pause()
        {
            State = AnimatorStates.Pause;
        }

        public int Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = Math.Abs(value);
            }
        }

        private int speed = 1;

        public int Width
        {
            get
            {
                return this.AnimationFrames[CurrentAnimationFrameIndex].Width;
            }
        }

        public int Height
        {
            get
            {
                return this.AnimationFrames[CurrentAnimationFrameIndex].Height;
            }
        }

        public AnimationTypes AnimationType
        {
            get;
            set;
        }

        /// <summary>
        /// Affichage de la prochaine frame
        /// </summary>

        public void NextFrame(int frameIndex = -1)
        {
            if (AnimationType == AnimationTypes.Manual)
            {
                if (frameIndex < 0)
                {
                    int nextFrame = (currentAnimationFrameIndex + 1) % AnimationFrames.Length;
                    this.currentAnimationFrameIndex = nextFrame;
                }
                else
                {
                    int nextFrame = frameIndex % AnimationFrames.Length;
                    this.currentAnimationFrameIndex = nextFrame;
                }

                this.CurrentAnimationFrame = AnimationFrames[currentAnimationFrameIndex];
            }
        }

        /// <summary>
        /// Update peut être executé dans Updating ou Updated
        /// </summary>

        public void Update()
        {
            if (AnimationType == AnimationTypes.Auto)
            {
                if (State == AnimatorStates.Play)
                {
                    int nextFrame = CurrentFrame + speed;

                    if (nextFrame >= CurrentAnimationFrame.FrameCount)
                    {
                        CurrentFrame = nextFrame % CurrentAnimationFrame.FrameCount;

                        // frame suivante
                        currentAnimationFrameIndex = (currentAnimationFrameIndex + 1) % AnimationFrames.Length;
                        CurrentAnimationFrame = AnimationFrames[currentAnimationFrameIndex];
                        // frame global
                        FramePlayed++;
                    }
                    else
                    {
                        CurrentFrame += speed;
                    }

                    if (FramePlayedMaximum > 0)
                    {
                        if (FramePlayed >= FramePlayedMaximum)
                        {
                            this.Stop();
                            this.FramePlayedCompleted?.Invoke(this.FramePlayedCompletedParameter);
                            return;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Affichage sans flip
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Draw(SurfaceSprite screen, int x, int y)
        {
            if (State != AnimatorStates.Stop)
            {
                CurrentAnimationFrame.Draw(screen, x, y);
            }
        }

        /// <summary>
        /// Affichage avec flip
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>

        public void Draw(SurfaceSprite screen, int x, int y, bool isHorizontalFlipped, bool isVerticalFlipped)
        {
            if (State != AnimatorStates.Stop)
            {
                CurrentAnimationFrame.Draw(screen, x, y, isHorizontalFlipped, isVerticalFlipped);
            }
        }

        public void SetFrame(int frameIndex)
        {
            if (AnimationType == AnimationTypes.Manual)
            {
                int nextFrame = frameIndex % AnimationFrames.Length;
                this.currentAnimationFrameIndex = nextFrame;
                this.CurrentAnimationFrame = AnimationFrames[currentAnimationFrameIndex];
            }
        }
    }

    public enum AnimatorStates
    {
        Play,
        Pause,
        Stop
    }

    public enum AnimationTypes
    {
        // par frame
        Auto,
        // call NextFrame
        Manual
    }
}
