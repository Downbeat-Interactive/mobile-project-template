using DG.Tweening;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DefaultScene {
    Preload, MainMenu, Game, Win, Lose
}


public class Loader : MonoBehaviour
{
    public static string MAIN_MENU_SCENE = "MainMenu";
    public static string GAME_SCENE = "Game";
    public static string WIN_SCENE = "Win";
    public static string LOSE_SCENE = "Lose";
    public static string PRELOAD_SCENE = "Preload";

    private void Init() {
        PRELOAD_SCENE       = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(0)).Split('.')[0];
        MAIN_MENU_SCENE     = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(1)).Split('.')[0];
        WIN_SCENE           = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(2)).Split('.')[0];
        LOSE_SCENE          = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(3)).Split('.')[0];
        GAME_SCENE          = Path.GetFileName(SceneUtility.GetScenePathByBuildIndex(4)).Split('.')[0];
    }

    static string GetNameFromDefault(DefaultScene scene) {
        switch (scene){
            case DefaultScene.Preload:
                return PRELOAD_SCENE;
            case DefaultScene.MainMenu:
                return MAIN_MENU_SCENE;
            case DefaultScene.Game:
                return GAME_SCENE;
            case DefaultScene.Win:
                return WIN_SCENE;
            case DefaultScene.Lose:
                return LOSE_SCENE;
            default:
                return "";
        }
    }

    public static void FadeToScene(string levelName, bool switchActive = false) {
        ScreenFadeController.Instance.FadeOutTween(.35f).OnComplete(() => {
            if (switchActive){
                SceneManager.LoadScene(levelName, LoadSceneMode.Single);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
            }
            else
                SceneManager.LoadScene(levelName);
        });
    }

    public static void FadeToScene(DefaultScene defaultScene, bool switchActive = false) {
        string name = GetNameFromDefault(defaultScene);
        if (name.Length > 0){
            FadeToScene(name, switchActive);
        }
        else{
            Debug.LogWarningFormat("Trying to load scene {0} failed. Scene does not exist.", name);
        }
    }

    public static void ImmediateAdditive(string levelName) {
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }

    public static void ImmediateAdditive(DefaultScene defaultScene) {
        string name = GetNameFromDefault(defaultScene);
        if (name.Length > 0){
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }
        else {
            Debug.LogWarningFormat("Trying to load scene {0} failed. Scene does not exist.", name);
        }
    }

    public static void LoadSceneBackground(string levelName, Image fader = null, Action doneCallback = null) {
        var op = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (fader) {
            op.completed += (AsyncOperation operation) => {
                fader.DOFade(0.1f, 1.0f);
                doneCallback?.Invoke();
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
            };
        }

    }

    public static void LoadSceneBackground(DefaultScene defaultScene, Image fader = null, Action doneCallback = null){
        string name = GetNameFromDefault(defaultScene);
        if (name.Length > 0){
            LoadSceneBackground(name, fader, doneCallback);
        } else{
            Debug.LogWarningFormat("Trying to load scene {0} failed. Scene does not exist.", name);
        }
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (scene.name.Equals(PRELOAD_SCENE)){
            // If this is the first load, load the main menu
            Init();
            SceneManager.LoadScene(MAIN_MENU_SCENE);
        } else {
            ScreenFadeController.Instance.FadeInTween();
            GameManager.isPlaying = false;
        }

        if (scene.name.Equals(MAIN_MENU_SCENE))
            GameUI.HideInGameUI();
    }

    public static void UnloadScene(string name) {
        SceneManager.UnloadSceneAsync(name);
    }

    public static void UnloadScene(DefaultScene defaultScene) {
        string name = GetNameFromDefault(defaultScene);
        if (name.Length > 0) {
            SceneManager.UnloadSceneAsync(name);
        } else{
            Debug.LogWarningFormat("Trying to unload scene {0} failed. Scene does not exist.", name);
        }

    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

}
