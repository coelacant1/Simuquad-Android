using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //Created by Steven Rowland
    //Provides better functionality than Unity's built in Quaternion class
    class BetterQuaternion
    {
        public double W { get; set; }// Real
        public double X { get; set; }// Imaginary I
        public double Y { get; set; }// Imaginary J
        public double Z { get; set; }// Imaginary K

        /// <summary>
        /// Creates quaternion object.
        /// </summary>
        /// <param name="W">Real part of quaternion.</param>
        /// <param name="X">Imaginary I of quaternion.</param>
        /// <param name="Y">Imaginary J of quaternion.</param>
        /// <param name="Z">Imaginary K of quaternion.</param>
        public BetterQuaternion(double W, double X, double Y, double Z)
        {
            this.W = W;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public BetterQuaternion(BetterQuaternion quaternion)
        {
            W = quaternion.W;
            X = quaternion.X;
            Y = quaternion.Y;
            Z = quaternion.Z;
        }

        /// <summary>
        /// Intializes quaternion with vector parameters for imaginary part.
        /// Creates quaternion bivector.
        /// </summary>
        /// <param name="vector">Imaginary values of quaternion.</param>
        public BetterQuaternion(BetterVector vector)
        {
            W = 0;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Rotates a vector coordinate in space given a quaternion value.
        /// </summary>
        /// <param name="coordinate">Coordinate vector that is rotated.</param>
        /// <returns>Returns new vector position coordinate.</returns>
        public BetterVector RotateVector(BetterVector coordinate)
        {
            BetterQuaternion current = new BetterQuaternion(this);
            BetterQuaternion qv = new BetterQuaternion(0, coordinate.X, coordinate.Y, coordinate.Z);
            BetterQuaternion qr = current * qv * current.MultiplicativeInverse();

            BetterVector rotatedVector = new BetterVector(0, 0, 0)
            {
                X = qr.X,
                Y = qr.Y,
                Z = qr.Z
            };

            return rotatedVector;
        }

        /// <summary>
        /// Rotates a vector coordinate in space by the inverse of a given quaternion value.
        /// </summary>
        /// <param name="coordinate">Coordinate vector that is unrotated.</param>
        /// <returns>Returns new vector position coordinate.</returns>
        public BetterVector UnrotateVector(BetterVector coordinate)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return current.Conjugate().RotateVector(coordinate);
        }

        /// <summary>
        /// Converts Euler angles to a unit quaternion.
        /// </summary>
        /// <param name="eulerAngles">Input Euler angles.</param>
        /// <returns>Returns new Quaternion object.</returns>
        public static BetterQuaternion EulerToQuaternion(BetterEuler eulerAngles)
        {
            BetterQuaternion q = new BetterQuaternion(1, 0, 0, 0);
            double sx, sy, sz, cx, cy, cz, cc, cs, sc, ss;

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
                eulerAngles.Angles.Y = -eulerAngles.Angles.Y;
            }

            sx = Math.Sin(eulerAngles.Angles.X * 0.5);
            sy = Math.Sin(eulerAngles.Angles.Y * 0.5);
            sz = Math.Sin(eulerAngles.Angles.Z * 0.5);

            cx = Math.Cos(eulerAngles.Angles.X * 0.5);
            cy = Math.Cos(eulerAngles.Angles.Y * 0.5);
            cz = Math.Cos(eulerAngles.Angles.Z * 0.5);

            cc = cx * cz;
            cs = cx * sz;
            sc = sx * cz;
            ss = sx * sz;

            if (eulerAngles.Order.InitialAxisRepitition == EulerOrder.AxisRepitition.Yes)
            {
                q.X = cy * (cs + sc);
                q.Y = sy * (cc + ss);
                q.Z = sy * (cs - sc);
                q.W = cy * (cc - ss);
            }
            else
            {
                q.X = cy * sc - sy * cs;
                q.Y = cy * ss + sy * cc;
                q.Z = cy * cs - sy * sc;
                q.W = cy * cc + sy * ss;
            }

            q.Permutate(eulerAngles.Order.Permutation);

            if (eulerAngles.Order.AxisPermutation == EulerOrder.Parity.Odd)
            {
                q.Y = -q.Y;
            }

            return q;
        }

        /// <summary>
        /// Converts axis angle rotation representation to quaternion.
        /// </summary>
        /// <param name="axisAngle">Axis-Angle rotation representation.</param>
        /// <returns>Converted quaternion value.</returns>
        public static BetterQuaternion AxisAngleToQuaternion(BetterAxisAngle axisAngle)
        {
            double rotation = MathExtension.DegreesToRadians(axisAngle.Rotation);
            double scale = Math.Sin(rotation / 2);

            return new BetterQuaternion(1, 0, 0, 0)
            {
                W = Math.Cos(rotation / 2),
                X = axisAngle.Axis.X * scale,
                Y = axisAngle.Axis.Y * scale,
                Z = axisAngle.Axis.Z * scale
            };
        }
        
        /// <summary>
        /// Converts from rotation matrix to quaternion value
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
        /// Avoid division by zero - We need to be sure that S is never zero even with possible floating point errors or de-orthogonalised matrix input.
        /// Avoid square root of a negative number. - We need to be sure that the tr value chosen is never negative even with possible floating point errors or de-orthogonalised matrix input.
        /// Accuracy of dividing by (and square rooting) a very small number.
        /// Resilient to a de-orthogonalised matrix
        /// </summary>
        /// <param name="X">X Axis</param>
        /// <param name="Y">Y Axis</param>
        /// <param name="Z">Z Axis</param>
        /// <returns>Returns a new quaternion</returns>
        public static BetterQuaternion RotationMatrixToQuaternion(BetterRotationMatrix rM)
        {
            BetterQuaternion q = new BetterQuaternion(1, 0, 0, 0);

            BetterVector X = new BetterVector(rM[0, 0], rM[0, 1], rM[0, 2]);
            BetterVector Y = new BetterVector(rM[1, 0], rM[1, 1], rM[1, 2]);
            BetterVector Z = new BetterVector(rM[2, 0], rM[2, 1], rM[2, 2]);

            double matrixTrace = X.X + Y.Y + Z.Z;
            double square;

            if (matrixTrace > 0)//standard procedure
            {
                square = Math.Sqrt(1.0 + matrixTrace) * 2;//4 * qw

                q.W = 0.25 * square;
                q.X = (Z.Y - Y.Z) / square;
                q.Y = (X.Z - Z.X) / square;
                q.Z = (Y.X - X.Y) / square;
            }
            else if ((X.X > Y.Y) && (X.X > Z.Z))
            {
                square = Math.Sqrt(1.0 + X.X - Y.Y - Z.Z) * 2;//4 * qx

                q.W = (Z.Y - Y.Z) / square;
                q.X = 0.25 * square;
                q.Y = (X.Y + Y.X) / square;
                q.Z = (X.Z + Z.X) / square;
            }
            else if (Y.Y > Z.Z)
            {
                square = Math.Sqrt(1.0 + Y.Y - X.X - Z.Z) * 2;//4 * qy

                q.W = (X.Z - Z.X) / square;
                q.X = (X.Y + Y.X) / square;
                q.Y = 0.25 * square;
                q.Z = (Y.Z + Z.Y) / square;
            }
            else
            {
                square = Math.Sqrt(1.0 + Z.Z - X.X - Y.Y) * 2;//4 * qz

                q.W = (Y.X - X.Y) / square;
                q.X = (X.Z + Z.X) / square;
                q.Y = (Y.Z + Z.Y) / square;
                q.Z = 0.25 * square;
            }

            return q.UnitQuaternion().Conjugate();
        }

        /// <summary>
        /// Creates quaternion of shortest rotation from two separate vectors.
        /// </summary>
        /// <param name="initial">Initial direction vector.</param>
        /// <param name="target">Target direction vector.</param>
        /// <returns>Returns quaternion rotation between two vectors.</returns>
        public static BetterQuaternion QuaternionFromTwoVectors(BetterVector initial, BetterVector target)
        {
            BetterQuaternion q = new BetterQuaternion(1, 0, 0, 0);
            BetterVector tempV = new BetterVector(0, 0, 0);
            BetterVector xAxis = new BetterVector(1, 0, 0);
            BetterVector yAxis = new BetterVector(0, 1, 0);

            double dot = BetterVector.DotProduct(initial, target);

            if (dot < -0.999999)
            {
                tempV = BetterVector.CrossProduct(xAxis, initial);

                if (tempV.GetLength() < 0.000001)
                {
                    tempV = BetterVector.CrossProduct(yAxis, initial);
                }

                tempV = tempV.Normalize();

                q = AxisAngleToQuaternion(new BetterAxisAngle(Math.PI, tempV));
            }
            else if (dot > 0.999999)
            {
                q.W = 1;
                q.X = 0;
                q.Y = 0;
                q.Z = 0;
            }
            else
            {
                tempV = BetterVector.CrossProduct(initial, target);

                q.W = 1 + dot;
                q.X = tempV.X;
                q.Y = tempV.Y;
                q.Z = tempV.Z;

                q = q.UnitQuaternion();
            }

            return q;
        }


        /// <summary>
        /// Adds two quaternions together.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is added to.</param>
        /// <param name="quaternionTwo">Quaternion that adds to.</param>
        /// <returns>Returns the combined quaternions.</returns>
        public BetterQuaternion Add(BetterQuaternion quaternion)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W + quaternion.W,
                X = current.X + quaternion.X,
                Y = current.Y + quaternion.Y,
                Z = current.Z + quaternion.Z
            };
        }

        public static BetterQuaternion operator +(BetterQuaternion q1, BetterQuaternion q2)
        {
            return q1.Add(q2);
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is subtracted from.</param>
        /// <param name="quaternionTwo">Quaternion that subtracts from.</param>
        /// <returns>Returns the subtracted quaternion.</returns>
        public BetterQuaternion Subtract(BetterQuaternion quaternion)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W - quaternion.W,
                X = current.X - quaternion.X,
                Y = current.Y - quaternion.Y,
                Z = current.Z - quaternion.Z
            };
        }

        public static BetterQuaternion operator -(BetterQuaternion q1, BetterQuaternion q2)
        {
            return q1.Subtract(q2);
        }

        /// <summary>
        /// Multiplies a scalar by a quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales the quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public BetterQuaternion Multiply(double scale)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W * scale,
                X = current.X * scale,
                Y = current.Y * scale,
                Z = current.Z * scale
            };
        }

        /// <summary>
        /// Multiplies two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is multiplied to.</param>
        /// <param name="quaternionTwo">Quathernion that multiplies to.</param>
        /// <returns>Returns the multiplied quaternions.</returns>
        public BetterQuaternion Multiply(BetterQuaternion quaternion)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W * quaternion.W - current.X * quaternion.X - current.Y * quaternion.Y - current.Z * quaternion.Z,
                X = current.W * quaternion.X + current.X * quaternion.W + current.Y * quaternion.Z - current.Z * quaternion.Y,
                Y = current.W * quaternion.Y - current.X * quaternion.Z + current.Y * quaternion.W + current.Z * quaternion.X,
                Z = current.W * quaternion.Z + current.X * quaternion.Y - current.Y * quaternion.X + current.Z * quaternion.W
            };
        }

        public static BetterQuaternion operator *(double s, BetterQuaternion q1)
        {
            return q1.Multiply(s);
        }

        public static BetterQuaternion operator *(BetterQuaternion q1, double s)
        {
            return q1.Multiply(s);
        }

        public static BetterQuaternion operator *(BetterQuaternion q1, BetterQuaternion q2)
        {
            return q1.Multiply(q2);
        }

        /// <summary>
        /// Divides a quaternion by a scalar.
        /// </summary>
        /// <param name="quaternion">Quaternion that is scaled.</param>
        /// <param name="scale">Scalar that scales quaternion.</param>
        /// <returns>Returns the scaled quaternion.</returns>
        public BetterQuaternion Divide(double scale)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W / scale,
                X = current.X / scale,
                Y = current.Y / scale,
                Z = current.Z / scale
            };
        }

        /// <summary>
        /// Divides two quaternions by each other.
        /// </summary>
        /// <param name="quaternionOne">Quaternion that is divided.</param>
        /// <param name="quaternionTwo">Quaternion that divides by.</param>
        /// <returns>Returns the divided quaternion.</returns>
        public BetterQuaternion Divide(BetterQuaternion quaternion)
        {
            double scale = quaternion.W * quaternion.W + quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z;
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = (current.W * quaternion.W + current.X * quaternion.X + current.Y * quaternion.Y + current.Z * quaternion.Z) / scale,
                X = (-current.W * quaternion.X + current.X * quaternion.W + current.Y * quaternion.Z - current.Z * quaternion.Y) / scale,
                Y = (-current.W * quaternion.Y - current.X * quaternion.Z + current.Y * quaternion.W + current.Z * quaternion.X) / scale,
                Z = (-current.W * quaternion.Z + current.X * quaternion.Y - current.Y * quaternion.X + current.Z * quaternion.W) / scale
            };
        }

        public static BetterQuaternion operator /(double s, BetterQuaternion q1)
        {
            return q1.Divide(s);
        }

        public static BetterQuaternion operator /(BetterQuaternion q1, double s)
        {
            return q1.Divide(s);
        }

        public static BetterQuaternion operator /(BetterQuaternion q1, BetterQuaternion q2)
        {
            return q1.Divide(q2);
        }

        /// <summary>
        /// Returns the absolute value of each individual quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is converted to a magnitude.</param>
        /// <returns>Returns the absolute value of the input quaternion.</returns>
        public BetterQuaternion Absolute()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = Math.Abs(current.W),
                X = Math.Abs(current.X),
                Y = Math.Abs(current.Y),
                Z = Math.Abs(current.Z)
            };
        }

        /// <summary>
        /// Returns the inverse of the input quaternion. (Not the conjugate.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is inverted.</param>
        /// <returns>Returns the inverse of the quaternion.</returns>
        public BetterQuaternion AdditiveInverse()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = -current.W,
                X = -current.X,
                Y = -current.Y,
                Z = -current.Z
            };
        }

        /// <summary>
        /// Returns the conjugate of the input quaternion. (Not the inverse.)
        /// </summary>
        /// <param name="quaternion">Quaternion that is conjugated.</param>
        /// <returns>Returns the conjugate of the input quaternion.</returns>
        public BetterQuaternion Conjugate()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = current.W,
                X = -current.X,
                Y = -current.Y,
                Z = -current.Z
            };
        }

        /// <summary>
        /// Returns the scalar power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Scalar that is used as the exponent.</param>
        /// <returns>Returns the scalar power of the input quaternion.</returns>
        public BetterQuaternion Power(double exponent)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = Math.Pow(current.W, exponent),
                X = Math.Pow(current.X, exponent),
                Y = Math.Pow(current.Y, exponent),
                Z = Math.Pow(current.Z, exponent)
            };
        }

        /// <summary>
        /// Returns the quaternion power of the input quaternion.
        /// </summary>
        /// <param name="quaternion">Quaternion that is exponentiated.</param>
        /// <param name="exponent">Quaternion that is used as the exponent.</param>
        /// <returns>Returns the quaternion power of the input quaternion.</returns>
        public BetterQuaternion Power(BetterQuaternion exponent)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return new BetterQuaternion(0, 0, 0, 0)
            {
                W = Math.Pow(current.W, exponent.W),
                X = Math.Pow(current.X, exponent.X),
                Y = Math.Pow(current.Y, exponent.Y),
                Z = Math.Pow(current.Z, exponent.Z)
            };
        }

        /// <summary>
        /// Restricts the quaternion to a single hemisphere of rotation, disables constant rotation around any real or imaginary axis.
        /// This will cause jumping in calculations, similar to that of a tangent function.
        /// </summary>
        /// <returns>Returns the unit quaternion of the current quaternion values.</returns>
        public BetterQuaternion UnitQuaternion()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            double n = Math.Sqrt(Math.Pow(current.W, 2) + Math.Pow(current.X, 2) + Math.Pow(current.Y, 2) + Math.Pow(current.Z, 2));

            current.W /= n;
            current.X /= n;
            current.Y /= n;
            current.Z /= n;

            return current;
        }

        /// <summary>
        /// Calculates the norm of the quaternion.
        /// </summary>
        /// <returns>Returns the norm of the quaternion.</returns>
        public double Normal()
        {
            BetterQuaternion q = new BetterQuaternion(this);

            return Math.Pow(q.W, 2.0) + Math.Pow(q.X, 2.0) + Math.Pow(q.Y, 2.0) + Math.Pow(q.Z, 2.0);
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="q">Input quaternion</param>
        /// <returns>Returns the dot product of the current quaternion and the input quaternion.</returns>
        public double DotProduct(BetterQuaternion q)
        {
            return (W * q.W) + (X * q.X) + (Y * q.Y) + (Z * q.Z);
        }

        /// <summary>
        /// Calculates the magnitude of the quaternion.
        /// </summary>
        /// <returns>Returns the magnitude of the quaternion.</returns>
        public double Magnitude()
        {
            return Math.Sqrt(Normal());
        }

        public double Determinant()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the multiplicative inverse of the quaternion.
        /// </summary>
        /// <returns>Returns the multiplicative inverse of the quaternion.</returns>
        public BetterQuaternion MultiplicativeInverse()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return current.Conjugate().Multiply(1.0 / current.Normal());
        }

        /// <summary>
        /// Slerp operation or Spherical Linear Interpolation, used to interpolate between quaternions.
        /// Constant speed motion along a unit-radius orthodrome(great circle) arc, given the ends and
        /// an interpolation parameter between 0 and 1.
        /// </summary>
        /// <param name="q2">Second quaternion for interpolation.</param>
        /// <param name="t">Ratio between input quaternions.</param>
        /// <returns>Interpolated quaternion.</returns>
        public BetterQuaternion SphericalLinearInterpolation(BetterQuaternion q2, double t)
        {
            BetterQuaternion q1 = new BetterQuaternion(this);

            q2 = new BetterQuaternion(q2);

            q1 = q1.UnitQuaternion();
            q2 = q2.UnitQuaternion();

            double dot = q1.DotProduct(q2);//Cosine between the two quaternions

            if (dot < 0.0)//Shortest path correction
            {
                q1 = q1.AdditiveInverse();
                dot = -dot;
            }

            if (dot > 0.9995)//Linearly interpolates if results are close
            {
                BetterQuaternion result = (q1 + t * (q1 - q2)).UnitQuaternion();
                return result;
            }
            else
            {
                dot = MathExtension.Constrain(dot, -1, 1);

                double theta0 = Math.Acos(dot);
                double theta = theta0 * t;

                BetterQuaternion q3 = (q2 - q1 * dot).UnitQuaternion();//UQ for orthonomal 

                return q1 * Math.Cos(theta) + q3 * Math.Sin(theta);
            }
        }

        /// <summary>
        /// Determines if any individual value of the quaternion is not a number.
        /// </summary>
        /// <param name="quaternion">Quaternion that isNaN checked.</param>
        /// <returns>Returns true if all any of the values are not a number.</returns>
        public bool IsNaN()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return double.IsNaN(current.W) || double.IsNaN(current.X) || double.IsNaN(current.Y) || double.IsNaN(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are finite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are finite.</returns>
        public bool IsFinite()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return !double.IsInfinity(current.W) && !double.IsInfinity(current.X) && !double.IsInfinity(current.Y) && !double.IsInfinity(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are infinite.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are infinite.</returns>
        public bool IsInfinite()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return double.IsInfinity(current.W) && double.IsInfinity(current.X) && double.IsInfinity(current.Y) && double.IsInfinity(current.Z);
        }

        /// <summary>
        /// Determines if all of the quaternions values are nonzero.
        /// </summary>
        /// <param name="quaternion">Quaternion that is checked.</param>
        /// <returns>Returns true if all values are nonzero.</returns>
        public bool IsNonZero()
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return current.W != 0 && current.X != 0 && current.Y != 0 && current.Z != 0;
        }

        /// <summary>
        /// Determines if the two input quaternions are equal.
        /// </summary>
        /// <param name="quaternionA">Quaternion that is checked.</param>
        /// <param name="quaternionB">Quaternion that is checked.</param>
        /// <returns>Returns true if both quaternions are equal.</returns>
        public bool IsEqual(BetterQuaternion quaternion)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            return !current.IsNaN() && !quaternion.IsNaN() &&
                    current.W == quaternion.W &&
                    current.X == quaternion.X &&
                    current.Y == quaternion.Y &&
                    current.Z == quaternion.Z;
        }

        public BetterQuaternion Permutate(BetterVector permutation)
        {
            BetterQuaternion current = new BetterQuaternion(this);

            double[] perm = new double[3];

            perm[(int)permutation.X] = current.X;
            perm[(int)permutation.Y] = current.Y;
            perm[(int)permutation.Z] = current.Z;

            current.X = perm[0];
            current.Y = perm[1];
            current.Z = perm[2];

            return current;
        }

        public BetterVector GetBiVector()
        {
            return new BetterVector(X, Y, Z);
        }

        public override string ToString()
        {
            string w = String.Format("{0:0.000}", W).PadLeft(7);
            string x = String.Format("{0:0.000}", X).PadLeft(7);
            string y = String.Format("{0:0.000}", Y).PadLeft(7);
            string z = String.Format("{0:0.000}", Z).PadLeft(7);

            return "[" + w + " " + x + " " + y + " " + z + "]";
        }

    }
}
