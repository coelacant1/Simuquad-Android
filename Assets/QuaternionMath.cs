using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    static class QuaternionMath
    {
        public static Vector3 RotateVector3(Quaternion q, Vector3 v)
        {
            Quaternion qv = new Quaternion(v.x, v.y, v.z, 0.0f);
            Quaternion qr = q * qv * MultiplicativeInverse(q);

            return new Vector3(
                qr.x,
                qr.y,
                qr.z
            );
        }

        public static Quaternion MultiplicativeInverse(Quaternion q)
        {
            Quaternion conjugate = Conjugate(q);
            float normal = (float)Normal(q);

            return MultiplyScalar(q, (1.0f / normal));
        }

        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(
                -q.x,
                -q.y,
                -q.z,
                q.w
            );
        }

        public static double Normal(Quaternion q)
        {
            return Math.Pow(q.w, 2.0) + Math.Pow(q.x, 2.0) + Math.Pow(q.y, 2.0) + Math.Pow(q.z, 2.0);
        }

        public static Quaternion MultiplyScalar(Quaternion q, float scalar)
        {
            return new Quaternion(
                q.x * scalar,
                q.y * scalar,
                q.z * scalar,
                q.w * scalar
            );
        }
    }
}
