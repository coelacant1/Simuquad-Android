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
            double cT;//current to target
            double sF;//spring force
            double dF;//damping force
            double f;//cumulative force

            cT = setPoint - position;
            sF = cT * SC;
            dF = velocity * -2.0 * Math.Sqrt(SC);
            f = sF + dF;

            velocity += f * DT;
            position += velocity * DT;

            return position;
        }

        public double Calculate(double setPoint, double dT)
        {
            double cT;//current to target
            double sF;//spring force
            double dF;//damping force
            double f;//cumulative force

            cT = setPoint - position;
            sF = cT * SC;
            dF = velocity * -2.0 * Math.Sqrt(SC);
            f = sF + dF;

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
