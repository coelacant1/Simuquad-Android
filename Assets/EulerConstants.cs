using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //Created by Steven Rowland
    //Provides better functionality than Unity's built in Euler class
    class EulerConstants
    {
        //Static frame of reference, inertial reference frame
        public readonly static EulerOrder EulerOrderXYZS = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(0, 1, 2));
        public readonly static EulerOrder EulerOrderXYXS = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(0, 1, 2));
        public readonly static EulerOrder EulerOrderXZYS = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(0, 2, 1));
        public readonly static EulerOrder EulerOrderXZXS = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(0, 2, 1));
        public readonly static EulerOrder EulerOrderYZXS = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(1, 2, 0));
        public readonly static EulerOrder EulerOrderYZYS = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(1, 2, 0));
        public readonly static EulerOrder EulerOrderYXZS = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(1, 0, 2));
        public readonly static EulerOrder EulerOrderYXYS = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(1, 0, 2));
        public readonly static EulerOrder EulerOrderZXYS = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(2, 0, 1));
        public readonly static EulerOrder EulerOrderZXZS = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(2, 0, 1));
        public readonly static EulerOrder EulerOrderZYXS = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Static, new BetterVector(2, 1, 0));
        public readonly static EulerOrder EulerOrderZYZS = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Static, new BetterVector(2, 1, 0));

        //Rotating frame of reference, non-inertial reference frame
        public readonly static EulerOrder EulerOrderZYXR = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(0, 1, 2));
        public readonly static EulerOrder EulerOrderXYXR = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(0, 1, 2));
        public readonly static EulerOrder EulerOrderYZXR = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(0, 2, 1));
        public readonly static EulerOrder EulerOrderXZXR = new EulerOrder(EulerOrder.Axis.X, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(0, 2, 1));
        public readonly static EulerOrder EulerOrderXZYR = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(1, 2, 0));
        public readonly static EulerOrder EulerOrderYZYR = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(1, 2, 0));
        public readonly static EulerOrder EulerOrderZXYR = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(1, 0, 2));
        public readonly static EulerOrder EulerOrderYXYR = new EulerOrder(EulerOrder.Axis.Y, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(1, 0, 2));
        public readonly static EulerOrder EulerOrderYXZR = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(2, 0, 1));
        public readonly static EulerOrder EulerOrderZXZR = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Even, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(2, 0, 1));
        public readonly static EulerOrder EulerOrderXYZR = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.No, EulerOrder.AxisFrame.Rotating, new BetterVector(2, 1, 0));
        public readonly static EulerOrder EulerOrderZYZR = new EulerOrder(EulerOrder.Axis.Z, EulerOrder.Parity.Odd, EulerOrder.AxisRepitition.Yes, EulerOrder.AxisFrame.Rotating, new BetterVector(2, 1, 0));
    }
}
