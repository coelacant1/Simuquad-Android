using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public struct Controls
    {
        public double Yaw;
        public double Pitch;
        public double Roll;
        public double Thrust;

        public Controls(double Yaw, double Pitch, double Roll, double Thrust)
        {
            this.Yaw = Yaw;
            this.Pitch = Pitch;
            this.Roll = Roll;
            this.Thrust = Thrust;
        }
    }
}
