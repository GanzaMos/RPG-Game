using RPG.Control;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAIStateManager))]
public class EnemyAIStateManagerCI : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyAIStateManager stateManager = (EnemyAIStateManager)target;

        if (GUILayout.Button("Activate search behaviour"))
        {
            // Call the method or perform the action you want
            stateManager.SwitchState(stateManager.EnemyStateSearching);
        }
    }
}