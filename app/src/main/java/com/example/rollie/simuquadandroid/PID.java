package com.example.rollie.simuquadandroid;

//Proportional Integral Derivative feedback controller
//Steven Rowland 9/30/2018
public class PID {
    private final double KP;//proportional gain
    private final double KI;//integral gain
    private final double KD;//derivative gain
    private final double DT;//time derivative
    private double integral = 0.0;
    private double previousError = 0.0;

    public PID(double KP, double KI, double KD){
        this.KP = KP;
        this.KI = KI;
        this.KD = KD;
        this.DT = 1.0;
    }

    public PID(double KP, double KI, double KD, double DT){
        this.KP = KP;
        this.KI = KI;
        this.KD = KD;
        this.DT = DT;
    }


    public double Calculate(double setPoint, double processVariable){
        double p, i, d, error, errorOffset;

        error = setPoint - processVariable;
        integral += error * this.DT;
        errorOffset = (error - previousError) / this.DT;

        p = KP * error;
        i = KI * integral;
        d = KD * errorOffset;

        previousError = error;

        return p + i + d;
    }

    public double Calculate(double setPoint, double processVariable, double dT){
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
