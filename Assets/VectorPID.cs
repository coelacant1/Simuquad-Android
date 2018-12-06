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

        public VectorPID(BetterVector KP, BetterVector KI, BetterVector KD)
        {
            X = new PID(KP.X, KI.X, KD.X);
            Y = new PID(KP.Y, KI.Y, KD.Y);
            Z = new PID(KP.Z, KI.Z, KD.Z);
        }

        public VectorPID(double KP, double KI, double KD, double DT)
        {
            X = new PID(KP, KI, KD, DT);
            Y = new PID(KP, KI, KD, DT);
            Z = new PID(KP, KI, KD, DT);
        }

        public VectorPID(BetterVector KP, BetterVector KI, BetterVector KD, BetterVector DT)
        {
            X = new PID(KP.X, KI.X, KD.X, DT.X);
            Y = new PID(KP.Y, KI.Y, KD.Y, DT.Y);
            Z = new PID(KP.Z, KI.Z, KD.Z, DT.Z);
        }

        public BetterVector Calculate(BetterVector setPoint, BetterVector processVariable)
        {
            return new BetterVector(
                X.Calculate(setPoint.X, processVariable.X),
                Y.Calculate(setPoint.Y, processVariable.Y),
                Z.Calculate(setPoint.Z, processVariable.Z)
            );
        }

        public BetterVector Calculate(BetterVector setPoint, BetterVector processVariable, double DT)
        {
            return new BetterVector(
                X.Calculate(setPoint.X, processVariable.X, DT),
                Y.Calculate(setPoint.Y, processVariable.Y, DT),
                Z.Calculate(setPoint.Z, processVariable.Z, DT)
            );
        }

        public BetterVector GetKP()
        {
            return new BetterVector(X.GetKP(), Y.GetKP(), Z.GetKP());
        }

        public BetterVector GetKI()
        {
            return new BetterVector(X.GetKI(), Y.GetKI(), Z.GetKI());
        }

        public BetterVector GetKD()
        {
            return new BetterVector(X.GetKD(), Y.GetKD(), Z.GetKD());
        }
    }
}
