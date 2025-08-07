using UnityEditor;
using UnityEngine;

public class RandomPlacementTool : EditorWindow
{
    GameObject prefabToPlace;
    GameObject parentObject;

    Vector2 scaleRange = new Vector2(1f, 1f);
    Vector3 rotationMin = Vector3.zero;
    Vector3 rotationMax = Vector3.zero;

    bool toolEnabled = false;

    [MenuItem("Tools/Simple Object Placement Tool")]
    public static void ShowWindow()
    {
        GetWindow<RandomPlacementTool>("Simple Object Placement Tool");
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI()
    {
        GUILayout.Label("Simple Object Placement Tool", EditorStyles.boldLabel);

        prefabToPlace = (GameObject)EditorGUILayout.ObjectField("Prefab to Place", prefabToPlace, typeof(GameObject), false);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        GUILayout.Space(10);
        GUILayout.Label("Random Scale (Uniform)");
        scaleRange = EditorGUILayout.Vector2Field("Min/Max Scale", scaleRange);

        GUILayout.Space(10);
        GUILayout.Label("Random Rotation");
        rotationMin = EditorGUILayout.Vector3Field("Min Rotation (Euler)", rotationMin);
        rotationMax = EditorGUILayout.Vector3Field("Max Rotation (Euler)", rotationMax);

        GUILayout.Space(10);
        toolEnabled = GUILayout.Toggle(toolEnabled, toolEnabled ? "Tool Enabled (Click to Disable)" : "Tool Disabled (Click to Enable)", "Button");

        EditorGUILayout.HelpBox("When enabled, click in the Scene view to place the prefab with random rotation and uniform scale.", MessageType.Info);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!toolEnabled || prefabToPlace == null || Event.current.type != EventType.MouseDown || Event.current.button != 0)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
            if (newObj != null)
            {
                Undo.RegisterCreatedObjectUndo(newObj, "Place Randomized Object");

                newObj.transform.position = hit.point;

                float randomScale = Random.Range(scaleRange.x, scaleRange.y);
                newObj.transform.localScale = Vector3.one * randomScale;

                Vector3 randomRot = new Vector3(
                    Random.Range(rotationMin.x, rotationMax.x),
                    Random.Range(rotationMin.y, rotationMax.y),
                    Random.Range(rotationMin.z, rotationMax.z)
                );
                newObj.transform.rotation = Quaternion.Euler(randomRot);

                if (parentObject != null)
                {
                    newObj.transform.SetParent(parentObject.transform);
                }

                Event.current.Use(); // consume event
            }
        }
    }
}
