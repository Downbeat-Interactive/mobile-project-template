
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LoadingSymbol))]
public class LoadingSymbolEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test Loading On") && ScreenFadeController.Instance!=null)
            ScreenFadeController.FadeOut();

        if (GUILayout.Button("Test Loading Off") && ScreenFadeController.Instance != null)
            ScreenFadeController.FadeIn();
    }
}