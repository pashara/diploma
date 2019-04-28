using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : InitializableMonobehaviour
{
    [SerializeField] Level currentLevel;

    public static LevelManager Instance
    {
        get;
        private set;
    }

    public Level CurrentLevel
    {
        get;
        private set;
    }

    public override void Initialize()
    {
        Instance = this;
        CurrentLevel = currentLevel;

    }
}
