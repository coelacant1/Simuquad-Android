using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Quadcopter
    {
        private readonly VectorPID rotationControl = new VectorPID(0.01, 0, 0.0045);
        private readonly double AirDensity = 1.225;
        private readonly double DragCoefficient = 1.0;
        private readonly double Area = 0.01;
        private readonly double ArmLength = 100;//mm
        private readonly double ArmAngle = 60;//degrees
        private readonly double Mass = 0.2;//Kg
        private readonly bool horizon;
        private readonly double torque;
        private readonly double ATS = 10.0;//Acro Thrust Scalar
        private readonly double AAS = 10.0;//Acro Angle Scalar
        private readonly double HTS = 2.0;//Horizon Thrust Scalar
        private readonly double HAS = 10.0;//Horizon Angle Scalar
        //private readonly Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
        private readonly BetterVector gravity = new BetterVector(0.0, -9.81, 0.0);

        private double DT;

        private Outputs motorOutputs             = new Outputs(0, 0, 0, 0);
        private BetterVector acceleration        = new BetterVector(0, 0, 0);
        private BetterVector velocity            = new BetterVector(0, 0, 0);
        private BetterVector position            = new BetterVector(0, 0, 0);
        private BetterVector angularAcceleration = new BetterVector(0, 0, 0);
        private BetterVector angularVelocity     = new BetterVector(0, 0, 0);
        private BetterQuaternion angularPosition = new BetterQuaternion(1, 0, 0, 0);

        public Quadcopter(bool horizon)
        {
            this.horizon = horizon;

            torque = ArmLength * Math.Sin((180.0 - ArmAngle) * Math.PI / 180.0);
        }

        public void EstimateState(Controls controls, double DT)
        {
            this.DT = DT;

            if (horizon)
            {
                motorOutputs = CalculateMotorOuputsHorizon(controls);
            }
            else
            {
                motorOutputs = CalculateMotorOutputsAcrobatics(controls);
            }

            //Debug.Log(motorOutputs);
            
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

            acceleration = angularPosition.RotateVector(new BetterVector(0.0, thrustSum, 0.0));

            velocity += acceleration * (DT / Mass) - dragForce * DT + gravity * DT;
            position += velocity * (DT / Mass);

            return position;
        }

        public void ApplyCollision()
        {
            acceleration = new BetterVector(0, 0, 0);
            velocity = new BetterVector(-velocity.X, -velocity.Y, -velocity.Z);
        }

        private Outputs CalculateMotorOuputsHorizon(Controls controls)
        {
            BetterQuaternion target = BetterQuaternion.EulerToQuaternion(new BetterEuler(new BetterVector(controls.Pitch, controls.Yaw, controls.Roll), EulerConstants.EulerOrderYXZS));
            
            BetterVector change = (2.0 * (target - angularPosition) * angularPosition.Conjugate() / DT).GetBiVector();

            //control rates
            BetterVector cr = rotationControl.Calculate(
                new BetterVector(0.0, 0.0, 0.0),
                change,
                DT
            );

            return new Outputs(
                controls.Thrust * HTS + (-cr.X + cr.Y - cr.Z) * HAS,
                controls.Thrust * HTS + (-cr.X - cr.Y + cr.Z) * HAS,
                controls.Thrust * HTS + ( cr.X - cr.Y - cr.Z) * HAS,
                controls.Thrust * HTS + ( cr.X + cr.Y + cr.Z) * HAS
            );
        }

        private Outputs CalculateMotorOutputsAcrobatics(Controls controls)
        {
            return new Outputs(
                controls.Thrust * ATS + (-controls.Pitch + controls.Yaw - controls.Roll) * AAS,
                controls.Thrust * ATS + (-controls.Pitch - controls.Yaw + controls.Roll) * AAS,
                controls.Thrust * ATS + ( controls.Pitch - controls.Yaw - controls.Roll) * AAS,
                controls.Thrust * ATS + ( controls.Pitch + controls.Yaw + controls.Roll) * AAS
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

    }
}
