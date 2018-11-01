using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class DirectionAngle
    {
        public double Rotation { get; set; }//
        public BetterVector Direction { get; set; }//

        public DirectionAngle(double Rotation, double X, double Y, double Z)
        {
            this.Rotation = Rotation;
            Direction = new BetterVector(X, Y, Z).Normalize();
        }

        public DirectionAngle(double Rotation, BetterVector direction)
        {
            this.Rotation = Rotation;

            Direction = new BetterVector(direction).Normalize();
        }

        public DirectionAngle(DirectionAngle directionAngle)
        {
            Rotation = directionAngle.Rotation;
            Direction = new BetterVector(directionAngle.Direction).Normalize();
        }

        /// <summary>
        /// This form of axis-angle is a custom type of rotation, the orientation is defined as the up vector of the object pointing at a specific 
        /// point in cartesian space; defining an axis of rotation, in which the object rotates about.
        /// </summary>
        /// <param name="quaternion">Quaternion rotation of the current object.</param>
        /// <returns></returns>
        public static DirectionAngle QuaternionToDirectionAngle(BetterQuaternion quaternion)
        {
            BetterVector up = new BetterVector(0, 1, 0);//up vector
            BetterVector right = new BetterVector(1, 0, 0);
            BetterVector rotatedUp = quaternion.RotateVector(up);//new direction vector
            BetterVector rotatedRight = quaternion.RotateVector(right);
            BetterQuaternion rotationChange = BetterQuaternion.QuaternionFromTwoVectors(up, rotatedUp);

            //rotate forward vector by direction vector rotation
            BetterVector rightXZCompensated = rotationChange.UnrotateVector(rotatedRight);//should only be two points on circle, compare against right

            //define angles that define the forward vector, and the rotated then compensated forward vector
            double rightAngle = MathExtension.RadiansToDegrees(Math.Atan2(right.Z, right.X));//forward as zero
            double rightRotatedAngle = MathExtension.RadiansToDegrees(Math.Atan2(rightXZCompensated.Z, rightXZCompensated.X));//forward as zero

            //angle about the axis defined by the direction of the object
            double angle = rightAngle - rightRotatedAngle;

            //returns the angle rotated about the rotated up vector as an axis
            return new DirectionAngle(angle, rotatedUp);
        }

        public override string ToString()
        {
            string r = String.Format("{0:0.000}", Rotation).PadLeft(8);
            string x = String.Format("{0:0.000}", Direction.X).PadLeft(8);
            string y = String.Format("{0:0.000}", Direction.Y).PadLeft(8);
            string z = String.Format("{0:0.000}", Direction.Z).PadLeft(8);

            return r + ": [" + x + " " + y + " " + z + "]";
        }
    }
}
