using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sugoi.Core
{
    /// <summary>
    /// Specific gamepad emulation for touch and mouse. Here the purpose is to emulate a simple tap and send a Button  
    /// </summary>

    public class TapGamepad : Gamepad
    {
        private TouchPointPool touchPoints;

        internal TapGamepad(TouchPointPool touchPoints)
        {
            this.touchPoints = touchPoints;

            // Par defaut Tap est désactivé
            this.IsEnabled = false;
        }

        public GamepadKeys KeyToPress
        {
            get;
            set;
        } = GamepadKeys.ButtonA;

        private DateTime dateTimeStartTouch;

        public void StartTouch()
        {
            if(this.IsEnabled == false)
            {
                return;
            }

            if(touchPoints.HaveTouchPoint == false)
            {
                dateTimeStartTouch = DateTime.Now;
            }
        }

        public void StopTouch()
        {
            if(this.IsEnabled == false)
            {
                return;
            }

            var now = DateTime.Now;
            var ms = (now - dateTimeStartTouch).TotalMilliseconds;

            if (ms < 800)
            {
                if (touchPoints.HaveTouchPoint == false)
                {
                    Debug.WriteLine("tapGamepad ms=" + ms);
                    // C'est un tap
                    this.Press(KeyToPress);
                    // ne marche pas 
                    this.machine.Delay(2, () =>
                    {
                        this.Release(KeyToPress);
                    });
                }
            }
        }
    }
}
