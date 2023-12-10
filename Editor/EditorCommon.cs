using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RG.Events
{
    public class EditorCommon : Editor
    {
        [MenuItem("Radioactive Goat/Event System/Create Event System")]
        public static void CreateEventSystem()
        {
            if (EditorSceneManager.GetSceneByBuildIndex(0) != EditorSceneManager.GetActiveScene())
            {
                EditorUtility.DisplayDialog("Incorrect Scene",
                    "The current scene is not a initializer scene or the first scene" +
                    "in your game. Event System is a DDOL object and must be initialized " +
                    "in a scene which loads first i.e., whose build index is 0.", " OK");
                return;
            }

            if (FindObjectOfType<EventSystem>())
            {
                EditorUtility.DisplayDialog("Already exists", "An Event System already exists in this scene." +
                    "Please remove it before trying again.", "OK");
                return;
            }
            var evsys = new GameObject("RG Event System").AddComponent<EventSystem>();

            Undo.RegisterCreatedObjectUndo(evsys, "Create " + evsys.name);
            EditorSceneManager.SaveOpenScenes();
        }
    }
}