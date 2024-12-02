namespace PopulationModels.UI.ViewModels.ModelParameter;

public sealed class EnumModelParameter<T> : SelectableModelParameter<T>
    where T : struct, Enum
{
    public EnumModelParameter(string name, int selectedIndex = 0) :
        base(name, selectedIndex, Enum.GetValues<T>())
    {
    }
    
    public EnumModelParameter(string name, T defaultValue) :
        base(name, Array.IndexOf(Enum.GetValues<T>(), defaultValue), Enum.GetValues<T>())
    {
    }
}