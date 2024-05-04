using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static bool IsAfterFivePM { get; private set; } = false;

    public delegate void GameEventHandler();
    public static event GameEventHandler OnAfterFivePM;

    public static void SetAfterFivePM()
    {
        IsAfterFivePM = true;
        OnAfterFivePM?.Invoke();
    }
    public static void ResetGameTime()
    {
        IsAfterFivePM = false;
    }
}
