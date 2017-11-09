using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : UIFunction
{
    [SerializeField]
    Text m_lapCounter;
    int laps = 0;

    public override void CallFunction(int _index, UiScroller _ref)
    {
        
    }

    public void NextLap()
    {
        laps++;
        m_lapCounter.text = laps + "/3";
    }

    void Update()
    {
 
    }
}