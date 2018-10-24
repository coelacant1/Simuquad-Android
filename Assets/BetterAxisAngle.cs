using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class BetterAxisAngle
    {
        public double Rotation { get; set; }//
        public BetterVector Axis { get; set; }//

        public BetterAxisAngle(double Rotation, double X, double Y, double Z)
        {
            this.Rotation = Rotation;
            Axis = new BetterVector(X, Y, Z);

        }

        public BetterAxisAngle(double Rotation, BetterVector vector)
        {
            this.Rotation = Rotation;

            Axis = new BetterVector(vector);
        }

        public static BetterAxisAngle QuaternionToStandardAxisAngle(BetterQuaternion quaternion)
        {
            BetterAxisAngle axisAngle = new BetterAxisAngle(0, 0, 1, 0);

            quaternion = (Math.Abs(quaternion.W) > 1.0) ? quaternion.UnitQuaternion() : quaternion;

            axisAngle.Rotation = MathExtension.RadiansToDegrees(2.0 * Math.Acos(quaternion.W));

            double quaternionCheck = Math.Sqrt(1.0 - Math.Pow(quaternion.W, 2.0));//Prevents rotation jumps, and division by zero

            if (quaternionCheck >= 0.001)//Prevents division by zero
            {
                //Normalizes axis
                axisAngle.Axis = new BetterVector(0, 0, 0)
                {
                    X = quaternion.X / quaternionCheck,
                    Y = quaternion.Y / quaternionCheck,
                    Z = quaternion.Z / quaternionCheck,
                };
            }
            else
            {
                //If X is close to zero the axis doesn't matter
                axisAngle.Axis = new BetterVector(0, 0, 0)
                {
                    X = 0.0,
                    Y = 1.0,
                    Z = 0.0
                };
            }

            return axisAngle;
        }

        /// <summary>
        /// Rotates vector by axis-angle
        /// </summary>
        /// <returns></returns>
        public BetterVector RotateVector(BetterVector v)
        {
            //r′ = cos(θ)r + ((1− cos (θ))(r • n)n + sin(θ) (n × r)
            BetterQuaternion q = BetterQuaternion.AxisAngleToQuaternion(this);

            return q.RotateVector(v);
        }

        private double RotateAxis(double angle, double axis)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation).PadLeft(8);
            string x = String.Format("{0:0.000}", Axis.X).PadLeft(8);
            string y = String.Format("{0:0.000}", Axis.Y).PadLeft(8);
            string z = String.Format("{0:0.000}", Axis.Z).PadLeft(8);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
