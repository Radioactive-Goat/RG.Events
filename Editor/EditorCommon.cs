using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorCommon : Editor
{
    [MenuItem("Radioactive Goat/Event System/Create Event System")]
    public static void CreateEventSystem()
    {
        if (EditorSceneManager.GetSceneByBuildIndex(0) != EditorSceneManager.GetActiveScene())
        {
            EditorUtility.DisplayDialog("Incorrect Scene",
                "The current scene is not a initializer scene or the first scene" +
                "in your game. Developer Console is a DDOL object and must be initialized " +
                "in a scene which loads first i.e., whose build index is 0.", " OK");
            return;
        }

        if (FindObjectOfType<GameObject>())
        {
            EditorUtility.DisplayDialog("Already exists", "A Developer Console object already exists in this scene." +
                "Please remove it before trying again.", "OK");
            return;
        }
        var ui = new GameObject("RGS Event System"); //.AddComponent<EventSystem>();
        
        Undo.RegisterCreatedObjectUndo(ui, "Create " + ui.name);
        EditorSceneManager.SaveOpenScenes();
    }
}
