using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

public class ActionEvaluator
{
    private Random _rnd;
    public ActionEvaluator()
    {
        _rnd = new Random();
    }
    public void Evaluate(INPCAction action)
    {
        switch (action)
        {
            case NPCBuildingAction buildAction:
                EvaluateBuildAction(buildAction);
                break;
            case INPCTradingAction tradeAction:
                EvaluateTradeAction(tradeAction);
                break;
            default:
                throw new InvalidOperationException("Unknown action type");
        }
    }

    public void Evaluate(List<INPCAction> actions)
    {
        foreach(INPCAction action in actions)
        {
            Evaluate(action);
        }
    }

    private void EvaluateBuildAction(NPCBuildingAction action)
    {
        int value = 0;
        action.Value = value;
    }


    private void EvaluateTradeAction(INPCTradingAction action)
    {
        action.Value = 10 + _rnd.Next(-10,100);
    }
}
