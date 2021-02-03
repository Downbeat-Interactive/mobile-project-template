using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;

    private void Awake()
    {
        if (Instance) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }
    }
    public static void NewLevel(int currentLevel) { }
}
