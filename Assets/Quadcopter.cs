using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Quadcopter
    {
        private readonly VectorPID rotationControl = new VectorPID(0.1, 0, 0.06);
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

        private double DT;

        private MotorOutputs.Outputs motorOutputs = new MotorOutputs.Outputs(0, 0, 0, 0);
        private Vector3 acceleration        = new Vector3(0, 0, 0);
        private Vector3 velocity            = new Vector3(0, 0, 0);
        private Vector3 position            = new Vector3(0, 0, 0);
        private Vector3 angularAcceleration = new Vector3(0, 0, 0);
        private Vector3 angularVelocity     = new Vector3(0, 0, 0);
        private Quaternion angularPosition  = new Quaternion(0, 0, 0, 1);

        public Quadcopter(bool horizon)
        {
            this.horizon = horizon;

            torque = ArmLength * Math.Sin((180.0 - ArmAngle) * Math.PI / 180.0);
        }

        public void EstimateState(Controls controls, double DT)
        {
            this.DT = DT;

            if (horizon)
                motorOutputs = CalculateMotorOuputsHorizon(controls);
            else
                motorOutputs = CalculateMotorOutputsAcrobatics(controls);


            angularPosition = EstimateRotation();
            position = EstimatePosition();
        }

        private Quaternion EstimateRotation()
        {
            Vector3 dragForce = EstimateDrag(angularVelocity);

            angularAcceleration.x = (float)(((motorOutputs.D + motorOutputs.E) - (motorOutputs.B + motorOutputs.C)) * torque * (Math.PI / 180.0));
            angularAcceleration.y = (float)(((motorOutputs.B + motorOutputs.E) - (motorOutputs.C + motorOutputs.D)) * torque * (Math.PI / 180.0));
            angularAcceleration.z = (float)(((motorOutputs.C + motorOutputs.E) - (motorOutputs.B + motorOutputs.D)) * torque * (Math.PI / 180.0));
            
            angularVelocity.x = (float)(angularVelocity.x + angularAcceleration.x * DT - dragForce.x * DT);
            angularVelocity.y = (float)(angularVelocity.y + angularAcceleration.y * DT - dragForce.y * DT);
            angularVelocity.z = (float)(angularVelocity.z + angularAcceleration.z * DT - dragForce.z * DT);

            Quaternion angularRate = new Quaternion(
                angularVelocity.x * 0.5f * (float)DT,
                angularVelocity.y * 0.5f * (float)DT,
                angularVelocity.z * 0.5f * (float)DT,
                0.0f
            ) * angularPosition;

            angularPosition = new Quaternion(
                angularPosition.x + angularRate.x,
                angularPosition.y + angularRate.y,
                angularPosition.z + angularRate.z,
                angularPosition.w + angularRate.w
            );

            angularPosition.Normalize();

            return angularPosition;
        }

        private Vector3 EstimatePosition()
        {
            float thrustSum = (float)(motorOutputs.B + motorOutputs.C + motorOutputs.D + motorOutputs.E);
            Vector3 dragForce = EstimateDrag(velocity);

            acceleration = QuaternionMath.RotateVector3(angularPosition, new Vector3(0.0f, thrustSum, 0.0f));
            velocity = new Vector3(
                (float)(velocity.x + acceleration.x * (DT / Mass) - dragForce.x * DT),
                (float)(velocity.y + acceleration.y * (DT / Mass) - dragForce.y * DT),
                (float)(velocity.z + acceleration.z * (DT / Mass) - dragForce.z * DT)
            );

            position = new Vector3(
                (float)(position.x + velocity.x * (DT / Mass)),
                (float)(position.y + velocity.y * (DT / Mass)),
                (float)(position.z + velocity.z * (DT / Mass))
            );

            return position;
        }

        public void ApplyCollision()
        {
            acceleration = new Vector3(0, 0, 0);
            velocity = new Vector3(-velocity.x, -velocity.y, -velocity.z);
        }

        private MotorOutputs.Outputs CalculateMotorOuputsHorizon(Controls controls)
        {
            Quaternion target = Quaternion.Euler((float)controls.Pitch, (float)controls.Yaw, (float)controls.Roll);
            Quaternion change = new Quaternion(
                2.0f * (target.x - angularPosition.x),
                2.0f * (target.y - angularPosition.y),
                2.0f * (target.z - angularPosition.z),
                2.0f * (target.w - angularPosition.w)
            );
            Quaternion combine = change * target;

            //control rates
            Vector3 cr = rotationControl.Calculate(
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3((float)(combine.x / DT), (float)(combine.y / DT), (float)(combine.z / DT)),
                (float)DT
            );

            return new MotorOutputs.Outputs(
                controls.Thrust * HTS + (-cr.x + cr.y - cr.z) * HAS,
                controls.Thrust * HTS + (-cr.x - cr.y + cr.z) * HAS,
                controls.Thrust * HTS + ( cr.x - cr.y - cr.z) * HAS,
                controls.Thrust * HTS + ( cr.x + cr.y + cr.z) * HAS
            );
        }

        private MotorOutputs.Outputs CalculateMotorOutputsAcrobatics(Controls controls)
        {
            return new MotorOutputs.Outputs(
                controls.Thrust * ATS + (-controls.Pitch + controls.Yaw - controls.Roll) * AAS,
                controls.Thrust * ATS + (-controls.Pitch - controls.Yaw + controls.Roll) * AAS,
                controls.Thrust * ATS + ( controls.Pitch - controls.Yaw - controls.Roll) * AAS,
                controls.Thrust * ATS + ( controls.Pitch + controls.Yaw + controls.Roll) * AAS
            );
        }

        private Vector3 EstimateDrag(Vector3 velocity)
        {
            //Calculates drag force for each dimension
            return new Vector3(
                (float)(0.5 * AirDensity * Math.Pow(velocity.x, 2.0) * DragCoefficient * Area * Math.Sign(velocity.x)),
                (float)(0.5 * AirDensity * Math.Pow(velocity.y, 2.0) * DragCoefficient * Area * Math.Sign(velocity.y)),
                (float)(0.5 * AirDensity * Math.Pow(velocity.z, 2.0) * DragCoefficient * Area * Math.Sign(velocity.z))
            );
        }

        public Quaternion GetQuaternion()
        {
            return angularPosition;
        }

        public Vector3 GetPosition()
        {
            return position;
        }
    }
}
