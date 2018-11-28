using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class VectorCDS
    {
        private readonly CDS X;
        private readonly CDS Y;
        private readonly CDS Z;

        public VectorCDS(double SC)
        {
            X = new CDS(SC);
            Y = new CDS(SC);
            Z = new CDS(SC);
        }

        public VectorCDS(BetterVector SC)
        {
            X = new CDS(SC.X);
            Y = new CDS(SC.Y);
            Z = new CDS(SC.Z);
        }

        public VectorCDS(double SC, double DT)
        {
            X = new CDS(SC, DT);
            Y = new CDS(SC, DT);
            Z = new CDS(SC, DT);
        }

        public VectorCDS(BetterVector SC, BetterVector DT)
        {
            X = new CDS(SC.X, DT.X);
            Y = new CDS(SC.Y, DT.Y);
            Z = new CDS(SC.Z, DT.Z);
        }

        public BetterVector Calculate(BetterVector setPoint)
        {
            return new BetterVector(
                X.Calculate(setPoint.X),
                Y.Calculate(setPoint.Y),
                Z.Calculate(setPoint.Z)
            );
        }

        public BetterVector Calculate(BetterVector setPoint, double DT)
        {
            return new BetterVector(
                X.Calculate(setPoint.X, DT),
                Y.Calculate(setPoint.Y, DT),
                Z.Calculate(setPoint.Z, DT)
            );
        }
    }
}
