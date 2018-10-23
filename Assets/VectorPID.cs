using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class VectorPID
    {
        private readonly PID X;
        private readonly PID Y;
        private readonly PID Z;

        public VectorPID(double KP, double KI, double KD)
        {
            X = new PID(KP, KI, KD);
            Y = new PID(KP, KI, KD);
            Z = new PID(KP, KI, KD);
        }

        public VectorPID(Vector3 KP, Vector3 KI, Vector3 KD)
        {
            X = new PID(KP.x, KI.x, KD.x);
            Y = new PID(KP.y, KI.y, KD.y);
            Z = new PID(KP.z, KI.z, KD.z);
        }

        public VectorPID(double KP, double KI, double KD, double DT)
        {
            X = new PID(KP, KI, KD, DT);
            Y = new PID(KP, KI, KD, DT);
            Z = new PID(KP, KI, KD, DT);
        }

        public VectorPID(Vector3 KP, Vector3 KI, Vector3 KD, Vector3 DT)
        {
            X = new PID(KP.x, KI.x, KD.x, DT.x);
            Y = new PID(KP.y, KI.y, KD.y, DT.y);
            Z = new PID(KP.z, KI.z, KD.z, DT.z);
        }

        public Vector3 Calculate(Vector3 setPoint, Vector3 processVariable)
        {
            return new Vector3(
                (float)X.Calculate(setPoint.x, processVariable.x),
                (float)Y.Calculate(setPoint.y, processVariable.y),
                (float)Z.Calculate(setPoint.z, processVariable.z)
            );
        }

        public Vector3 Calculate(Vector3 setPoint, Vector3 processVariable, double DT)
        {
            return new Vector3(
                (float)X.Calculate(setPoint.x, processVariable.x, DT),
                (float)Y.Calculate(setPoint.y, processVariable.y, DT),
                (float)Z.Calculate(setPoint.z, processVariable.z, DT)
            );
        }
    }
}
