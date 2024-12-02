using PopulationModels.UI.ViewModels.ModelParameter;

namespace PopulationModels.UI.OdeModels;


public sealed class BazykinParameterA(double current = 1, double min = 0, double max = 10) 
    : RangeModelParameter("A", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_A;
}

public sealed class BazykinParameterB(double current = 1, double min = 0, double max = 10) 
    : RangeModelParameter("B", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_B;
}

public sealed class BazykinParameterC(double current = 1, double min = 0, double max = 10) 
    : RangeModelParameter("C", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_C;
}

public sealed class BazykinParameterD(double current = 1, double min = 0, double max = 10) 
    : RangeModelParameter("D", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_D;
}

public sealed class BazykinParameterE(double current = 1, double min = 0, double max = 5) 
    : RangeModelParameter("E", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_E;
}

public sealed class BazykinParameterM(double current = 1, double min = 0, double max = 5) 
    : RangeModelParameter("M", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_E;
}

public sealed class BazykinParameterP(double current = 1, double min = 0, double max = 5) 
    : RangeModelParameter("p", current, min, max)
{
    public override string Description => Resources.Localization.Bazykin_P;
}

public sealed class BazykinParameterX0(double current = 30, double max = 500) 
    : RangeModelParameter("X0", current, 0, max)
{
    public override string Description => Resources.Localization.InitialPrey;
}

public sealed class BazykinParameterY0(double current = 10,  double max = 500) 
    : RangeModelParameter("Y0", current, 0, max)
{
    public override string Description => Resources.Localization.InitialPredator;
}