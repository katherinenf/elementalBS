using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    private static GameConfig instance;

    public static GameConfig Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new GameConfig();
            }
            return instance;
        }
    }

    public bool useAI = true;
}
