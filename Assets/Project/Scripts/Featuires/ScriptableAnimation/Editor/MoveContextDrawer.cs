#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static AnimationLibrary;

[InitializeOnLoad]
public static class MoveContextSceneViewDrawer
{
    static MoveContextSceneViewDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        foreach (GameObject go in selectedObjects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                SerializedObject so = new SerializedObject(component);
                SerializedProperty prop = so.FindProperty("moveContext"); // замените на имя вашего поля

                if (prop != null && prop.propertyType == SerializedPropertyType.ManagedReference)
                {
                    SerializedProperty objProp = prop.FindPropertyRelative("Object");
                    SerializedProperty startProp = prop.FindPropertyRelative("Start");
                    SerializedProperty endProp = prop.FindPropertyRelative("End");
                    SerializedProperty spaceProp = prop.FindPropertyRelative("Space");

                    if (objProp == null || startProp == null || endProp == null || spaceProp == null) continue;

                    Transform obj = objProp.objectReferenceValue as Transform;
                    if (obj == null) continue;

                    Vector3 start = startProp.vector3Value;
                    Vector3 end = endProp.vector3Value;
                    AnimationSpace space = (AnimationSpace)spaceProp.enumValueIndex;

                    if (space == AnimationSpace.Local)
                    {
                        start = obj.TransformPoint(start);
                        end = obj.TransformPoint(end);
                    }

                    Handles.color = Color.yellow;
                    Handles.DrawLine(start, end);
                }
                so.Dispose();
            }
        }
    }
}
#endif
