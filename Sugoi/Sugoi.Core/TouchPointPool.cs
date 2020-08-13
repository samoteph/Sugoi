using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sugoi.Core
{
    public class TouchPointPool
    {
        private Dictionary<uint, TouchPoint> touchPoints;
        private Machine machine;

        private object locker = new object();

        public TouchPointPool(int size)
        {
            this.touchPoints = new Dictionary<uint, TouchPoint>(size);
        }

        /// <summary>
        /// Démarre le pool
        /// </summary>
        /// <param name="machine"></param>

        internal void Start(Machine machine)
        {
            this.machine = machine;

            lock (locker)
            {
                this.touchPoints.Clear();
            }

            TapGamepad = new TapGamepad(this);

            machine.Gamepads.AddGamepad( TapGamepad );            
            TapGamepad.Start(machine);
        }

        /// <summary>
        /// Gamepad 
        /// </summary>

        public TapGamepad TapGamepad
        {
            get;
            private set;
        }

        /// <summary>
        /// Ajouter un TouchPoint automatiquement
        /// </summary>

        public void Add(uint touchPointId, int touchPointX, int touchPointY)
        {
            lock (locker)
            {
                this.TapGamepad.StartTouch();

                if (touchPoints.ContainsKey(touchPointId) == false)
                {
                    touchPoints.Add(touchPointId, new TouchPoint(touchPointId, touchPointX, touchPointY));
                }
                else
                {
                    touchPoints[touchPointId] = new TouchPoint(touchPointId, touchPointX, touchPointY);
                }
            }
        }

        public bool Remove(uint touchPointId)
        {
            lock (locker)
            {
                if(touchPoints.ContainsKey(touchPointId))
                {
                    touchPoints.Remove(touchPointId);

                    this.TapGamepad.StopTouch();

                    return true;
                }
            }

            return false;
        }

        public bool HaveTouchPoint
        {
            get
            {
                lock (locker)
                {
                    return this.touchPoints.Count > 0;
                }
            }
        }

        public bool Get(uint touchPointId, out TouchPoint touchPoint)
        {
            lock (locker)
            {
                return touchPoints.TryGetValue(touchPointId, out touchPoint);
            }
        }

        public bool Set(uint touchPointId, int touchPointX, int touchPointY)
        {
            lock (locker)
            {
                if (touchPoints.ContainsKey(touchPointId) == true)
                {
                    touchPoints[touchPointId] = new TouchPoint(touchPointId, touchPointX, touchPointY);
                    return true;
                }

                return false;
            }
        }

        public void ForEach(Func<TouchPoint, ForeachReturn> enumatorAction)
        {
            lock (locker)
            {
                foreach (var touchPoint in touchPoints.Values)
                {
                    if( enumatorAction(touchPoint) == ForeachReturn.Break)
                    {
                        break;
                    }
                }
            }
        }

        public bool IsHitTestRectangle(TouchPoint touchPoint, Rectangle rectangle)
        {
            return rectangle.Contains(new Point(touchPoint.X, touchPoint.Y));
        }

        internal void Stop()
        {
            machine.Gamepads.RemoveGamepad(TapGamepad);
            TapGamepad.Stop();
        }

        public bool ShowTouchPoints
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="screen"></param>

        internal void Draw(Screen screen)
        {
            if(ShowTouchPoints == false)
            {
                return;
            }

            if (this.HaveTouchPoint)
            {
                this.ForEach((tp) =>
                {
                    var opacity = screen.Opacity;

                    screen.Opacity = 0.5;
                    screen.DrawRectangle(tp.X - 10, tp.Y - 10, 20, 20, Argb32.Black, true);

                    screen.Opacity = opacity;

                    return ForeachReturn.Continue;
                });
            }
        }
    }
}
