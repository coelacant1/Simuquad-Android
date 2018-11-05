using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class RotationMatrix
    {
        public BetterVector XAxis { get; set; }
        public BetterVector YAxis { get; set; }
        public BetterVector ZAxis { get; set; }

        private BetterVector InitialVector;
        private bool didRotate = false;

        public RotationMatrix(BetterVector axes)
        {
            XAxis = new BetterVector(axes.X, axes.X, axes.X);
            YAxis = new BetterVector(axes.Y, axes.Y, axes.Y);
            ZAxis = new BetterVector(axes.Z, axes.Z, axes.Z);

            InitialVector = axes;
        }

        public RotationMatrix(BetterVector X, BetterVector Y, BetterVector Z)
        {
            XAxis = new BetterVector(X);
            YAxis = new BetterVector(Y);
            ZAxis = new BetterVector(Z);
        }

        public static RotationMatrix QuaternionToMatrixRotation(BetterQuaternion quaternion)
        {
            BetterVector X = new BetterVector(1, 0, 0);
            BetterVector Y = new BetterVector(0, 1, 0);
            BetterVector Z = new BetterVector(0, 0, 1);

            return new RotationMatrix(new BetterVector(1, 1, 1))
            {
                XAxis = quaternion.RotateVector(X),
                YAxis = quaternion.RotateVector(Y),
                ZAxis = quaternion.RotateVector(Z)
            };
        }

        public static BetterVector RotateVector(BetterVector Rotate, BetterVector Coordinates)
        {
            //calculate rotation matrix
            RotationMatrix matrix = new RotationMatrix(Coordinates);

            matrix.Rotate(Rotate);

            return matrix.ConvertCoordinateToVector();
        }

        public BetterVector ConvertCoordinateToVector()
        {
            if (didRotate)
            {
                return new BetterVector((XAxis.X + YAxis.X + ZAxis.X), (XAxis.Y + YAxis.Y + ZAxis.Y), (XAxis.Z + YAxis.Z + ZAxis.Z));
            }
            else
            {
                return InitialVector;
            }
        }

        /// <summary>
        /// Run between individual rotations to prevent gimbal lock
        /// </summary>
        public void ReadjustMatrix()
        {
            double X = (XAxis.X + YAxis.X + ZAxis.X);
            double Y = (XAxis.Y + YAxis.Y + ZAxis.Y);
            double Z = (XAxis.Z + YAxis.Z + ZAxis.Z);

            XAxis = new BetterVector(X, X, X);
            YAxis = new BetterVector(Y, Y, Y);
            ZAxis = new BetterVector(Z, Z, Z);
        }

        /// <summary>
        /// Rotate about arbitrary axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="theta"></param>
        public RotationMatrix(BetterVector axis, double theta)
        {
            BetterVector u = axis.Normalize();

            double sin = Math.Sin(theta);
            double cos = Math.Cos(theta);

            double uxy = u.X * u.Y * (1 - cos);
            double uyz = u.Y * u.Z * (1 - cos);
            double uxz = u.X * u.Z * (1 - cos);
            double ux2 = u.X * u.X * (1 - cos);
            double uy2 = u.Y * u.Y * (1 - cos);
            double uz2 = u.Z * u.Z * (1 - cos);

            double uxsin = u.X * sin;
            double uysin = u.Y * sin;
            double uzsin = u.Z * sin;

            XAxis = new BetterVector(cos + ux2, uxy + uzsin, uxz - uysin);
            YAxis = new BetterVector(uxy - uzsin, cos + uy2, uyz + uxsin);
            ZAxis = new BetterVector(uxz + uysin, uyz - uxsin, cos + uz2);
        }

        /// <summary>
        /// Rotation with the right-hand rule
        /// </summary>
        /// <param name="alpha">Pitch</param>
        /// <param name="beta">Heading</param>
        /// <param name="gamma">Bank</param>
        public RotationMatrix Rotate(BetterVector rotation)
        {
            if (rotation.X != 0)
            {
                RotateX(rotation.X);
                didRotate = true;

                if (rotation.Y != 0 || rotation.Z != 0)
                {
                    ReadjustMatrix();
                }
            }

            if (rotation.Y != 0)
            {
                RotateY(rotation.Y);
                didRotate = true;

                if (rotation.Z != 0)
                {
                    ReadjustMatrix();
                }
            }

            if (rotation.Z != 0)
            {
                RotateZ(rotation.Z);
                didRotate = true;
            }

            return new RotationMatrix(XAxis, YAxis, ZAxis);
        }

        /// <summary>
        /// Rotates around the X axis, pitch
        /// </summary>
        /// <param name="theta"></param>
        public void RotateX(double theta)
        {
            double cos = Math.Cos(MathExtension.DegreesToRadians(theta));
            double sin = Math.Sin(MathExtension.DegreesToRadians(theta));

            XAxis = new BetterVector(1, 0, 0).Add(XAxis);
            YAxis = new BetterVector(0, cos, -sin).Add(YAxis);
            ZAxis = new BetterVector(0, sin, cos).Add(ZAxis);
        }

        /// <summary>
        /// Rotates around the Y axis, heading
        /// </summary>
        /// <param name="theta"></param>
        public void RotateY(double theta)
        {
            double cos = Math.Cos(MathExtension.DegreesToRadians(theta));
            double sin = Math.Sin(MathExtension.DegreesToRadians(theta));

            XAxis = new BetterVector(cos, 0, sin).Add(XAxis);
            YAxis = new BetterVector(0, 1, 0).Add(YAxis);
            ZAxis = new BetterVector(-sin, 0, cos).Add(ZAxis);
        }

        /// <summary>
        /// Rotates around the Z axis, bank
        /// </summary>
        /// <param name="theta"></param>
        public void RotateZ(double theta)
        {
            double cos = Math.Cos(MathExtension.DegreesToRadians(theta));
            double sin = Math.Sin(MathExtension.DegreesToRadians(theta));

            XAxis = new BetterVector(cos, -sin, 0).Add(XAxis);
            YAxis = new BetterVector(sin, cos, 0).Add(YAxis);
            ZAxis = new BetterVector(0, 0, 1).Add(ZAxis);
        }

        public void Multiply(double d)
        {
            XAxis = XAxis.Multiply(d);
            YAxis = YAxis.Multiply(d);
            ZAxis = ZAxis.Multiply(d);
        }

        private void Multiply(RotationMatrix m)
        {
            XAxis = XAxis.Multiply(m.XAxis);
            YAxis = YAxis.Multiply(m.YAxis);
            ZAxis = ZAxis.Multiply(m.ZAxis);
        }

        public void RotateRelative(RotationMatrix m)
        {
            Multiply(m);
        }

        public void Normalize()
        {
            BetterVector vz = BetterVector.CrossProduct(XAxis, YAxis);
            BetterVector vy = BetterVector.CrossProduct(vz, XAxis);

            XAxis = XAxis.Normalize();
            YAxis = vy.Normalize();
            ZAxis = vz.Normalize();
        }

        public void Transpose()//opposite rotation matrix
        {
            XAxis = new BetterVector(XAxis.X, YAxis.X, ZAxis.X);
            YAxis = new BetterVector(XAxis.Y, YAxis.Y, ZAxis.Y);
            ZAxis = new BetterVector(XAxis.Z, YAxis.Z, ZAxis.Z);
        }

        public double Determinant()
        {
            return XAxis.X * (YAxis.Y * ZAxis.Z - ZAxis.Y * YAxis.Z) -
                   YAxis.X * (ZAxis.Z * XAxis.Y - ZAxis.Y * XAxis.Z) +
                   ZAxis.X * (XAxis.Y * YAxis.Z - YAxis.Y * XAxis.Z);
        }

        public void Inverse()
        {
            XAxis = BetterVector.CrossProduct(YAxis, ZAxis);
            YAxis = BetterVector.CrossProduct(ZAxis, XAxis);
            ZAxis = BetterVector.CrossProduct(XAxis, YAxis);

            Transpose();
            Multiply(1 / Determinant());
        }


        public bool IsEqual(RotationMatrix m)
        {
            return m == this || XAxis.IsEqual(m.XAxis) && YAxis.IsEqual(m.YAxis) && ZAxis.IsEqual(m.ZAxis);
        }

        public override string ToString()
        {
            return String.Format("[[{0}, {1}, {2}]\n[{3}, {4}, {5}]\n[{6}, {7}, {8}]]",
                Math.Round(XAxis.X, 3), Math.Round(YAxis.X, 3), Math.Round(ZAxis.X, 3),
                Math.Round(XAxis.Y, 3), Math.Round(YAxis.Y, 3), Math.Round(ZAxis.Y, 3),
                Math.Round(XAxis.Z, 3), Math.Round(YAxis.Z, 3), Math.Round(ZAxis.Z, 3));
        }

        public static void TestRotationMatrix()
        {
            BetterVector point = new BetterVector(110, -50, 60);
            RotationMatrix rotation = new RotationMatrix(point);

            rotation.Rotate(new BetterVector(60, 60, 30));

            Console.WriteLine(rotation.ToString());

            Console.WriteLine(rotation.ConvertCoordinateToVector().ToString());
        }
    }
}
