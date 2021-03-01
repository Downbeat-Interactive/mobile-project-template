using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    static int score = 0;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // SetScore(0, false);
        GameUtil.WaitThenDo(.02f, () => { SetScore(0, false); });
    }

    public static void AddScore(int v)
    {
        SetScore(score + v);
    }

    private static void SetScore(int v, bool animate = true)
    {
        score = v;
        GameUI.UpdateScore(score, animate);
    }
    public static int GetScore()
    {
        return score;
    }

    public static void NewLevel(int currentLevel)
    {
        GameUI.ChangeLevelText(GameManager.GetCurrentLevelForDisplay());
        UnityAction callback = () => {
            float t = 0;
            DOTween.To(() => t, (x) => { t = x; }, 1, 1.0f).OnComplete(() =>
            {
                GameManager.StartGame();
            });
        };
        Loader.FadeToScene("level-" + currentLevel.ToString(), true, callback);


    }

}
