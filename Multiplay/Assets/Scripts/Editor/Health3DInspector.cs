using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Health3D))]
public class Health3DInspector : Editor
{
    private int maxNodesX = 5; // Default value, can be adjusted
    private int maxNodesY = 5; // Default value, can be adjusted
    private float spacing = 1; // Default spacing between nodes

    public override void OnInspectorGUI()
    {
        Health3D health3D = (Health3D)target;

        maxNodesX = EditorGUILayout.IntField("Max Nodes X", maxNodesX);
        maxNodesY = EditorGUILayout.IntField("Max Nodes Y", maxNodesY);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        DrawDefaultInspector();

        if (GUILayout.Button("Populate Array"))
        {
            health3D.PopulateArray();
        }

        if(GUILayout.Button("Arrange Health Nodes"))
        {
            GameObject[] nodes = health3D.HealthNodes;

            if (nodes.Length == 0)
            {
                Debug.LogWarning("No health nodes found to arrange.");
                return;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                int x = i % maxNodesX;
                int y = i / maxNodesX;
                if (y >= maxNodesY)
                {
                    Debug.LogWarning("Exceeded maximum number of nodes in the specified grid.");
                    break;
                }
                Vector3 newPosition = new Vector3(x * spacing, 0, y * spacing);
                nodes[i].transform.localPosition = newPosition;
            }

        }
    }

}

