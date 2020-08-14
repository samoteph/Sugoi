using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    /// <summary>
    /// Zone with the capacity to manage one touch 
    /// </summary>

    public class TouchZone
    {
        private TouchPointPool touchPoints;
        
        public Rectangle Zone
        {
            get;
            private set;
        }

        public TouchZone(TouchPointPool touchPoints)
        {
            this.touchPoints = touchPoints;
        }

        public bool IsStarting
        {
            get;
            private set;
        }

        public void Start(Rectangle zone)
        {
            this.IsStarting = true;
            this.Zone = zone;

            Debug.WriteLine("Start=" + zone);
        }

        public bool IsPressing
        {
            get;
            private set;
        }

        public bool IsTaping
        {
            get;
            private set;
        }

        public bool IsMoving
        {
            get;
            private set;
        }

        public bool IsIn
        {
            get;
            private set;
        }

        public Point Position
        {
            get;
            private set;
        }

        private DateTime startTapDateTime;
        private uint startTouchId;

        public void Update()
        {
            bool isTaping = false;
            bool isMoving = false;
            bool isIn = false;

            if (this.IsPressing == false)
            {
                // première fois ?

                touchPoints.ForEach((tp) =>
                {
                    if (Zone.Contains(tp.X, tp.Y))
                    {
                        Debug.WriteLine("TouchZone Start Pressing");

                        // première fois détecté
                        startTapDateTime = DateTime.Now;
                        startTouchId = tp.Id;

                        this.IsPressing = true;
                        isIn = true;
                        this.Position = new Point(tp.X, tp.Y);

                        // on break;
                        return ForeachReturn.Break;
                    }

                    return ForeachReturn.Continue;
                });
            }
            else
            {
                // recherche si le premier point est toujours là
                if (touchPoints.Get(startTouchId, out var touchPoint))
                {
                    var position = new Point(touchPoint.X, touchPoint.Y);

                    if( position != this.Position)
                    {
                        isMoving = true;
                    }

                    if(Zone.Contains(position))
                    {
                        isIn = true;
                    }
                }
                else
                {
                    // detection du IsTaping
                    // le touchpoint n'est plus là
                    // la dernière frame était déjà pressée mais pas celle là ?
                    if (this.IsPressing == true)
                    {
                        DateTime now = DateTime.Now;

                        var ms = (now - startTapDateTime).TotalMilliseconds;

                        // LE taping est valide sur un temps court
                        if (ms < 800)
                        {
                            // dernière position valide (de la frame d'avant car on a plus de TouchPoint disponible car le touch n'existe plus)
                            var p = this.Position;

                            if (Zone.Contains(p.X, p.Y))
                            {
                                // Il faut qu'il soit encore dans la zone pour que le IsTaping soit valide 
                                isTaping = true;
                            }
                        }
                    }

                    Debug.WriteLine("TouchZone Stop Pressing");

                    this.IsPressing = false;
                }
            }

            this.IsMoving = isMoving;
            this.IsTaping = isTaping;
            this.IsIn = isIn;
        }

        public void Stop()
        {

        }
    }
}
