public interface INPCAction
{
    double Value { get; set; }
    Faction Faction{ get; }
    void ExecuteAction();
}
