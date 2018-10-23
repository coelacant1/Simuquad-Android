﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class MotorOutputs
    {
        //used for simulating delay in motor outputs
        private readonly CDS cdsB = new CDS(100);
        private readonly CDS cdsC = new CDS(100);
        private readonly CDS cdsD = new CDS(100);
        private readonly CDS cdsE = new CDS(100);

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
        }

        public MotorOutputs(double B, double C, double D, double E)
        {
            SetMotorOutputs(B, C, D, E);
        }

        public void SetMotorOutputs(double B, double C, double D, double E)
        {
            cdsB.Calculate(B);
            cdsC.Calculate(C);
            cdsD.Calculate(D);
            cdsE.Calculate(E);
        }

        public Outputs GetMotorOutputs()
        {
            return new Outputs(
                cdsB.GetPosition(),
                cdsC.GetPosition(),
                cdsD.GetPosition(),
                cdsE.GetPosition()
            );
        }
    }
}
