using UnityEngine;
using UnityEditor;

public class RemoveNumeration : EditorWindow
{
    [SerializeField] private string newName;
    [MenuItem("Tools/Remove Numeration")]
    
    static void CreateRemoveNumeration()
    {
        EditorWindow.GetWindow<RemoveNumeration>();
    }

    private void OnGUI()
    {
        newName = (string)EditorGUILayout.TextField("New Name", newName);
        if (GUILayout.Button("Rename"))
        {
            var selection = Selection.gameObjects;

            for (var i = 0; i < selection.Length; ++i)
            {
                var selected = selection[i];
                var name = selected.name;
                selected.name = name.Split(" ")[0];
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
