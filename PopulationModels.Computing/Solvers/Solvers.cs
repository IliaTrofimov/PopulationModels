using System.ComponentModel;


namespace PopulationModels.Computing.Solvers;

public enum Solvers
{
   [Description("Runge-Kutta 2")]
   Rk2, 
   
   [Description("Runge-Kutta 4")]
   Rk4,
   
   [Description("Runge-Kutta 2 (implicit)")]
   Rk2Implicit
}