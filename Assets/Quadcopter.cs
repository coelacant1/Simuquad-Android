using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Quadcopter
    {
        private readonly VectorPID rotationControl = new VectorPID(0.01, 0.005, 0.009, 1.0 / 60.0);
        private readonly VectorPID velocityControl = new VectorPID(1.0,  0.0,   0.15,   1.0 / 60.0);
        private readonly double AirDensity = 1.225;
        private readonly double DragCoefficient = 0.2;
        private readonly double Area = 0.1;
        private readonly double ArmLength = 100;//mm
        private readonly double ArmAngle = 60;//degrees
        private readonly double Mass = 0.5;//Kilograms
        private readonly double MaxThrust = 10.0;//Thrust-to-weight ratio
        private readonly bool horizon;
        private readonly double torque;
        private readonly BetterVector gravity = new BetterVector(0.0, -9.81, 0.0);

        private double DT = 1.0 / 60.0;

        private Outputs motorOutputs             = new Outputs(0, 0, 0, 0);
        private BetterVector acceleration        = new BetterVector(0, 0, 0);
        private BetterVector oldAcceleration     = new BetterVector(0, 0, 0);
        private BetterVector velocity            = new BetterVector(0, 0, 0);
        private BetterVector position            = new BetterVector(0, 0, 0);
        private BetterVector angularAcceleration = new BetterVector(0, 0, 0);
        private BetterVector angularVelocity     = new BetterVector(0, 0, 0);
        private BetterQuaternion angularPosition = new BetterQuaternion(1, 0, 0, 0);

        private readonly CDS motorBDelay = new CDS(500.0, 1.0 / 60.0);
        private readonly CDS motorCDelay = new CDS(500.0, 1.0 / 60.0);
        private readonly CDS motorDDelay = new CDS(500.0, 1.0 / 60.0);
        private readonly CDS motorEDelay = new CDS(500.0, 1.0 / 60.0);

        public Quadcopter(bool horizon)
        {
            this.horizon = horizon;

            torque = ArmLength * Math.Sin((180.0 - ArmAngle) * Math.PI / 180.0);
        }

        public void EstimateState(Controls controls, double DT)
        {
            this.DT = DT;

            controls.Thrust *= MaxThrust * Mass / 4.0;// Maximum thrust output per motor

            if (horizon)
            {
                motorOutputs = CalculateMotorOuputsHorizon(controls);
            }
            else
            {
                motorOutputs = CalculateMotorOutputsAcrobatics(controls);
            }

            //Debug.Log(motorOutputs.B);
            
            angularPosition = EstimateRotation();
            position = EstimatePosition();
        }

        private BetterQuaternion EstimateRotation()
        {
            BetterVector dragForce = EstimateDrag(angularVelocity);

            angularAcceleration = new BetterVector(
                ((motorOutputs.D + motorOutputs.E) - (motorOutputs.B + motorOutputs.C)) * torque * (Math.PI / 180.0),
                ((motorOutputs.B + motorOutputs.E) - (motorOutputs.C + motorOutputs.D)) * torque * (Math.PI / 180.0),
                ((motorOutputs.C + motorOutputs.E) - (motorOutputs.B + motorOutputs.D)) * torque * (Math.PI / 180.0)
            );

            //Debug.Log(angularAcceleration);
            
            angularVelocity += angularAcceleration * DT - dragForce * DT;

            //Debug.Log(angularVelocity);

            BetterQuaternion angularRate = new BetterQuaternion(
                0.0,
                angularVelocity.X * 0.5 * DT,
                angularVelocity.Y * 0.5 * DT,
                angularVelocity.Z * 0.5 * DT
            ) * angularPosition;
            
            angularPosition = (angularPosition + angularRate).UnitQuaternion();

            return angularPosition;
        }

        private BetterVector EstimatePosition()
        {
            double thrustSum = motorOutputs.B + motorOutputs.C + motorOutputs.D + motorOutputs.E;
            BetterVector dragForce = EstimateDrag(velocity);

            //T * 9.81 -> Mass thrust to mass in Newtons
            acceleration = (angularPosition.RotateVector(new BetterVector(0.0, thrustSum * 9.81, 0.0)) - dragForce + gravity) / Mass;

            //Debug.Log(thrustSum);

            //Verlet method
            position += DT * (velocity + DT * oldAcceleration / 2.0);
            velocity += DT * oldAcceleration;
            velocity += DT * (acceleration - oldAcceleration) / 2.0;

            velocity *= 0.999;

            //Debug.Log(velocity);

            oldAcceleration = acceleration;

            return position;
        }

        public void ApplyCollision()
        {
            acceleration = new BetterVector(0, 0, 0);
            velocity = new BetterVector(-velocity.X, -velocity.Y, -velocity.Z);
        }

        private Outputs CalculateMotorOuputsHorizon(Controls controls)
        {
            double x, y, z;

            //normalizes x and z values and calculates the y component, magnitude = 1
            x = -controls.Roll / 90.0;
            z = -controls.Pitch  / 90.0;
            
            if (Math.Pow(x, 2.0) + Math.Pow(z, 2.0) >= 1)
                y = 0;
            else
                y = Math.Sqrt(1.0 - Math.Pow(x, 2.0) - Math.Pow(z, 2.0));//1^2 = a^2 + b^2 + c^2 => a = (b^2 + c^2 + 1)^0.5

            //calculates the target rotation, the yaw is actual yaw, the vector defines the target upvector of the quadcopter
            BetterQuaternion target = BetterQuaternion.DirectionAngleToQuaternion(new DirectionAngle(0, new BetterVector(x, y, z)));
            BetterQuaternion targetYaw = BetterQuaternion.EulerToQuaternion(new BetterEuler(new BetterVector(0, controls.Yaw, 0), EulerConstants.EulerOrderYZXS));

            target = targetYaw * target;
            
            BetterVector change = (2.0 * (target - angularPosition) * angularPosition.Conjugate() / DT).GetBiVector();

            //control rates
            BetterVector cr = rotationControl.Calculate(
                new BetterVector(0.0, 0.0, 0.0),
                change,
                DT
            );

            return new Outputs(
                motorBDelay.Calculate(controls.Thrust + (-cr.X + cr.Y - cr.Z), DT),
                motorCDelay.Calculate(controls.Thrust + (-cr.X - cr.Y + cr.Z), DT),
                motorDDelay.Calculate(controls.Thrust + ( cr.X - cr.Y - cr.Z), DT),
                motorEDelay.Calculate(controls.Thrust + ( cr.X + cr.Y + cr.Z), DT)
            );
        }

        private Outputs CalculateMotorOutputsAcrobatics(Controls controls)
        {
            BetterVector rotatedControls = angularPosition.RotateVector(new BetterVector(controls.Pitch, controls.Yaw, controls.Roll));
            BetterVector output = velocityControl.Calculate(rotatedControls, new BetterVector(angularVelocity.X, angularVelocity.Y, angularVelocity.Z), DT);
            
            return new Outputs(
                motorBDelay.Calculate(controls.Thrust + (-output.X + output.Y - output.Z), DT),
                motorCDelay.Calculate(controls.Thrust + (-output.X - output.Y + output.Z), DT),
                motorDDelay.Calculate(controls.Thrust + ( output.X - output.Y - output.Z), DT),
                motorEDelay.Calculate(controls.Thrust + ( output.X + output.Y + output.Z), DT)
            );
        }

        private BetterVector EstimateDrag(BetterVector velocity)
        {
            return new BetterVector(
                0.5 * AirDensity * Math.Pow(velocity.X, 2.0) * DragCoefficient * Area * Math.Sign(velocity.X),
                0.5 * AirDensity * Math.Pow(velocity.Y, 2.0) * DragCoefficient * Area * Math.Sign(velocity.Y),
                0.5 * AirDensity * Math.Pow(velocity.Z, 2.0) * DragCoefficient * Area * Math.Sign(velocity.Z)
            );
        }

        public BetterQuaternion GetQuaternion()
        {
            return angularPosition;
        }

        public Quaternion GetUnityQuaternion()
        {
            return new Quaternion(
                (float)angularPosition.X,
                (float)angularPosition.Y,
                (float)angularPosition.Z,
                (float)angularPosition.W
            );
        }

        public BetterVector GetPosition()
        {
            return position;
        }

        public Vector3 GetUnityPosition()
        {
            return new Vector3(
                (float)position.X,
                (float)position.Y,
                (float)position.Z
            );
        }

        public Outputs GetMotorOuputs()
        {
            return motorOutputs;
        }

        public void Reset()
        {
            acceleration        = new BetterVector(0, 0, 0);
            velocity            = new BetterVector(0, 0, 0);
            position            = new BetterVector(0, 0, 0);
            angularAcceleration = new BetterVector(0, 0, 0);
            angularVelocity     = new BetterVector(0, 0, 0);
            angularPosition     = new BetterQuaternion(1, 0, 0, 0);
        }
    }
}
