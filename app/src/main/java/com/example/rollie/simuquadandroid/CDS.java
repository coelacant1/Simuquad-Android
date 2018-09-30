package com.example.rollie.simuquadandroid;

//Critically Damped Spring
//Steven Rowland 9/30/2018
public class CDS {
    private final double SC;//Spring Constant
    private final double DT;
    private double velocity = 0.0;
    private double position = 0.0;

    public CDS(double SC){
        this.SC = SC;
        this.DT = 1.0;
    }

    public CDS(double SC, double DT){
        this.SC = SC;
        this.DT = DT;
    }

    public double Calculate(double setPoint){
        double cT;//current to target
        double sF;//spring force
        double dF;//damping force
        double f;//cumulative force

        cT = setPoint - this.position;
        sF = cT * this.SC;
        dF = this.velocity * -2.0 * Math.sqrt(this.SC);
        f = sF + dF;

        this.velocity += f * this.DT;
        this.position += this.velocity * this.DT;

        return this.position;
    }

    public double Calculate(double setPoint, double dT){
        double cT;//current to target
        double sF;//spring force
        double dF;//damping force
        double f;//cumulative force

        cT = setPoint - this.position;
        sF = cT * this.SC;
        dF = this.velocity * -2.0 * Math.sqrt(this.SC);
        f = sF + dF;

        this.velocity += f * dT;
        this.position += this.velocity * dT;

        return this.position;
    }
}
