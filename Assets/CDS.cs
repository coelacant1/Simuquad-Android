using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class CDS
    {
        private readonly double SC;//Spring Constant
        private readonly double DT;
        private double velocity = 0.0;
        private double position = 0.0;

        public CDS(double SC)
        {
            this.SC = SC;
            DT = 1.0;
        }

        public CDS(double SC, double DT)
        {
            this.SC = SC;
            this.DT = DT;
        }

        public double Calculate(double setPoint)
        {
            return Calculate(setPoint, DT);
        }

        public double Calculate(double setPoint, double dT)
        {
            double cT = setPoint - position;
            double sF = cT * SC;
            double dF = velocity * -2.0 * Math.Sqrt(SC);
            double f = sF + dF;

            velocity += f * dT;
            position += velocity * dT;

            return position;
        }
        
        public double GetPosition()
        {
            return position;
        }
    }
}
