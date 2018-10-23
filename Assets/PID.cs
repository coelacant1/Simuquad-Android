using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class PID
    {
        private readonly double KP;//proportional gain
        private readonly double KI;//integral gain
        private readonly double KD;//derivative gain
        private readonly double DT;//time derivative
        private double integral = 0.0;
        private double previousError = 0.0;

        public PID(double KP, double KI, double KD)
        {
            this.KP = KP;
            this.KI = KI;
            this.KD = KD;
            DT = 1.0 / 30.0;//30 fps
        }

        public PID(double KP, double KI, double KD, double DT)
        {
            this.KP = KP;
            this.KI = KI;
            this.KD = KD;
            this.DT = DT;
        }


        public double Calculate(double setPoint, double processVariable)
        {
            double p, i, d, error, errorOffset;

            error = setPoint - processVariable;
            integral += error * DT;
            errorOffset = (error - previousError) / DT;

            p = KP * error;
            i = KI * integral;
            d = KD * errorOffset;

            previousError = error;

            return p + i + d;
        }

        public double Calculate(double setPoint, double processVariable, double dT)
        {
            double p, i, d, error, errorOffset;

            error = setPoint - processVariable;
            integral += error * dT;
            errorOffset = (error - previousError) / dT;

            p = KP * error;
            i = KI * integral;
            d = KD * errorOffset;

            previousError = error;

            return p + i + d;
        }
    }
}
