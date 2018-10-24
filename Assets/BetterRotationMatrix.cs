using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //Created by Steven Rowland
    //Provides better functionality than Unity's built in rotation matrix class
    class BetterRotationMatrix
    {
        private double[,] hierarchicalMatrixValues = new double[4, 4];

        public BetterRotationMatrix()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    hierarchicalMatrixValues[i, k] = 0;
                }
            }
        }

        public BetterRotationMatrix(double[,] hMatrix)
        {
            hierarchicalMatrixValues = hMatrix;
        }

        public double this[double index1, double index2]
        {
            get
            {
                return hierarchicalMatrixValues[(int)index1, (int)index2];
            }
            set
            {
                hierarchicalMatrixValues[(int)index1, (int)index2] = value;
            }
        }

        public static BetterRotationMatrix EulerToHMatrix(BetterEuler eulerAngles)
        {
            BetterRotationMatrix hM = new BetterRotationMatrix();
            double sx, sy, sz, cx, cy, cz, cc, cs, sc, ss;
            BetterVector p = eulerAngles.Order.Permutation;

            eulerAngles.Angles.X = MathExtension.DegreesToRadians(eulerAngles.Angles.X);
            eulerAngles.Angles.Y = MathExtension.DegreesToRadians(eulerAngles.Angles.Y);
            eulerAngles.Angles.Z = MathExtension.DegreesToRadians(eulerAngles.Angles.Z);

            if (eulerAngles.Order.FrameTaken == EulerOrder.AxisFrame.Rotating)
            {
                double t = eulerAngles.Angles.X;
                eulerAngles.Angles.X = eulerAngles.Angles.Z;
                eulerAngles.Angles.Z = t;
            }

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                eulerAngles.Angles.X = -eulerAngles.Angles.X;
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
                eulerAngles.Angles.Z = -eulerAngles.Angles.Z;
            }

            sx = Math.Sin(eulerAngles.Angles.X);
            sy = Math.Sin(eulerAngles.Angles.Y);
            sz = Math.Sin(eulerAngles.Angles.Z);
            cx = Math.Cos(eulerAngles.Angles.X);
            cy = Math.Cos(eulerAngles.Angles.Y);
            cz = Math.Cos(eulerAngles.Angles.Z);

            cc = cx * cz;
            cs = cx * sz;
            sc = sx * cz;
            ss = sx * sz;

            if (eulerAngles.Order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                hM[p.X, p.X] = cy; hM[p.X, p.Y] = sy * sx; hM[p.X, p.Z] = sy * cx; hM[0, 3] = 0;
                hM[p.Y, p.X] = sy * sz; hM[p.Y, p.Y] = -cy * ss + cc; hM[p.Y, p.Z] = -cy * cs - sc; hM[1, 3] = 0;
                hM[p.Z, p.X] = -sy * cz; hM[p.Z, p.Y] = cy * sc + cs; hM[p.Z, p.Z] = cy * cc - ss; hM[2, 3] = 0;
                hM[3, 0] = 0; hM[3, 1] = 0; hM[3, 2] = 0; hM[3, 3] = 1;
            }
            else
            {
                hM[p.X, p.X] = cy * cz; hM[p.X, p.Y] = sy * sc - cs; hM[p.X, p.Z] = sy * cc + ss; hM[0, 3] = 0;
                hM[p.Y, p.X] = cy * sz; hM[p.Y, p.Y] = sy * ss + cc; hM[p.Y, p.Z] = sy * cs - sc; hM[1, 3] = 0;
                hM[p.Z, p.X] = -sy; hM[p.Z, p.Y] = cy * sx; hM[p.Z, p.Z] = cy * cx; hM[2, 3] = 0;
                hM[3, 0] = 0; hM[3, 1] = 0; hM[3, 2] = 0; hM[3, 3] = 1;
            }

            return hM;
        }
    }
}
