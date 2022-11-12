using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler 
{
    private static GameHandler instance = null;
    private static readonly object padlock = new object();

    public GameHandler()
    {
    }

    public static GameHandler Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GameHandler();
                }
                return instance;
            }
        }
    }

    
}

