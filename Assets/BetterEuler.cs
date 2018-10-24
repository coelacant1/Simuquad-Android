using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //Created by Steven Rowland
    //Provides better functionality than Unity's built in Euler class
    class BetterEuler
    {
        public BetterVector Angles { get; set; }
        public EulerOrder Order { get; set; }

        public BetterEuler(BetterVector angles, EulerOrder order)
        {
            Angles = new BetterVector(angles);
            Order = order;
        }

        public static BetterEuler HMatrixToEuler(BetterRotationMatrix hM, EulerOrder order)
        {
            BetterEuler eulerAngles = new BetterEuler(new BetterVector(0, 0, 0), order);
            BetterVector p = order.Permutation;

            if (order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                double sy = Math.Sqrt(Math.Pow(hM[p.X, p.Y], 2) + Math.Pow(hM[p.X, p.Z], 2));

                if (sy > 32 * double.Epsilon)//16 * float.Epsilon
                {
                    eulerAngles.Angles.X = Math.Atan2(hM[p.X, p.Y], hM[p.X, p.Z]);
                    eulerAngles.Angles.Y = Math.Atan2(sy, hM[p.X, p.X]);
                    eulerAngles.Angles.Z = Math.Atan2(hM[p.Y, p.X], -hM[p.Z, p.X]);
                }
                else
                {
                    eulerAngles.Angles.X = Math.Atan2(-hM[p.Y, p.Z], hM[p.Y, p.Y]);
                    eulerAngles.Angles.Y = Math.Atan2(sy, hM[p.X, p.X]);
                    eulerAngles.Angles.Z = 0;
                }
            }
            else
            {
                double cy = Math.Sqrt(Math.Pow(hM[p.X, p.X], 2) + Math.Pow(hM[p.Y, p.X], 2));

                if (cy > 32 * double.Epsilon)
                {
                    eulerAngles.Angles.X = Math.Atan2(hM[p.Z, p.Y], hM[p.Z, p.Z]);
                    eulerAngles.Angles.Y = Math.Atan2(-hM[p.Z, p.X], cy);
                    eulerAngles.Angles.Z = Math.Atan2(hM[p.Y, p.X], hM[p.X, p.X]);
                }
                else
                {
                    eulerAngles.Angles.X = Math.Atan2(-hM[p.Y, p.Z], hM[p.Y, p.Y]);
                    eulerAngles.Angles.Y = Math.Atan2(-hM[p.Z, p.X], cy);
                    eulerAngles.Angles.Z = 0;
                }
            }

            if (order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                eulerAngles.Angles.X = -eulerAngles.Angles.X;
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
                eulerAngles.Angles.Z = -eulerAngles.Angles.Z;
            }

            if (order.FrameTaken == EulerOrder.AxisFrame.Rotating)
            {
                double temp = eulerAngles.Angles.X;
                eulerAngles.Angles.X = eulerAngles.Angles.Z;
                eulerAngles.Angles.Z = temp;
            }

            eulerAngles.Angles.X = MathExtension.RadiansToDegrees(eulerAngles.Angles.X);
            eulerAngles.Angles.Y = MathExtension.RadiansToDegrees(eulerAngles.Angles.Y);
            eulerAngles.Angles.Z = MathExtension.RadiansToDegrees(eulerAngles.Angles.Z);

            return eulerAngles;
        }

        public static BetterEuler QuaternionToEuler(BetterQuaternion q, EulerOrder order)
        {
            double norm = q.Normal();
            double scale = norm > 0.0 ? 2.0 / norm : 0.0;
            BetterRotationMatrix hM = new BetterRotationMatrix();

            BetterVector s = new BetterVector(q.X * scale, q.Y * scale, q.Z * scale);
            BetterVector w = new BetterVector(q.W * s.X, q.W * s.Y, q.W * s.Z);
            BetterVector x = new BetterVector(q.X * s.X, q.X * s.Y, q.X * s.Z);
            BetterVector y = new BetterVector(0.0, q.Y * s.Y, q.Y * s.Z);
            BetterVector z = new BetterVector(0.0, 0.0, q.Z * s.Z);

            //0X, 1Y, 2Z, 3W
            hM[0, 0] = 1.0 - (y.Y + z.Z); hM[0, 1] = x.Y - w.Z;         hM[0, 2] = x.Z + w.Y;         hM[0, 3] = 0.0;
            hM[1, 0] = x.Y + w.Z;         hM[1, 1] = 1.0 - (x.X + z.Z); hM[1, 2] = y.Z - w.X;         hM[1, 3] = 0.0;
            hM[2, 0] = x.Z - w.Y;         hM[2, 1] = y.Z + w.X;         hM[2, 2] = 1.0 - (x.X + y.Y); hM[2, 3] = 0.0;
            hM[3, 0] = 0.0;               hM[3, 1] = 0.0;               hM[3, 2] = 0.0;               hM[3, 3] = 1.0;

            return HMatrixToEuler(hM, order);
        }

        public override string ToString()
        {
            return Angles.ToString();
        }
    }
}
