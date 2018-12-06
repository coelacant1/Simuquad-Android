using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    [System.Serializable]
    class SaveData
    {
        public BetterVector HKP;
        public BetterVector HKI;
        public BetterVector HKD;

        public BetterVector AKP;
        public BetterVector AKI;
        public BetterVector AKD;

        public double Mass;
        public double Gravity;
        public double TWRatio;
        public double Drag;
        public double WorldScale;
        public double CameraAngle;
        public double DTMultiplier;
        public double RenderDistance;
        public double AcrobaticsExpo;
        public double AcrobaticsRate;
        public double HorizonExpo;

        public SaveData()
        {

        }
    }
}
