using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class Delay
    {
        private readonly int steps;
        private List<double> previousValue = new List<double>();

        public Delay(int steps)
        {
            this.steps = steps;
        }
        
        public double Calculate(double setPoint)
        {
            previousValue.Add(setPoint);

            if (previousValue.Count < steps)
            {
                return 0.0;
            }
            else
            {
                double first = previousValue.ElementAt(0);

                previousValue.RemoveAt(0);

                return first;
            }
        }
    }
}
