public interface INPCAction
{
    int Value { get; set; }
    Faction Faction{ get; }
    void ExecuteAction();
}
