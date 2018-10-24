using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //Created by Steven Rowland
    //Provides better functionality than Unity's built in Euler class
    class EulerOrder
    {
        public readonly Axis InitialAxis;
        public readonly Parity AxisPermutation;
        public readonly AxisRepitition InitialAxisRepitition;
        public readonly AxisFrame FrameTaken;
        public readonly BetterVector Permutation;

        public EulerOrder(Axis axis, Parity parity, AxisRepitition axisRepitition, AxisFrame axisFrame, BetterVector permutation)
        {
            InitialAxis = axis;
            AxisPermutation = parity;
            InitialAxisRepitition = axisRepitition;
            FrameTaken = axisFrame;
            Permutation = permutation;
        }

        public enum Axis
        {
            X,
            Y,
            Z
        };

        public enum Parity
        {
            Even,
            Odd
        };

        public enum AxisRepitition
        {
            Yes,
            No
        };

        public enum AxisFrame
        {
            Static,
            Rotating
        };
    }
}
