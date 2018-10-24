using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public struct Outputs
    {
        public readonly double B;
        public readonly double C;
        public readonly double D;
        public readonly double E;

        public Outputs(double B, double C, double D, double E)
        {
            this.B = B;
            this.C = C;
            this.D = D;
            this.E = E;
        }

        public override string ToString()
        {
            return B + ", " + C + ", " + D + ", " + E;
        }
    }
}
