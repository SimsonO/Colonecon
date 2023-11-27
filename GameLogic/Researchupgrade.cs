using System;
using System.Collections.Generic;

public class ResearchUpgrade
{   
    public String Name;
    public String Description;
    public bool Active {get; private set;}
    public Dictionary<ResourceType, int> UpgradeCost { get; set; }

    public void SetActive()
    {
        Active = true;
    }
    public void Deactivate()
    {
        Active = false;
    }
}