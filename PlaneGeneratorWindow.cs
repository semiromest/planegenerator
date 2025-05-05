using UnityEditor;
using UnityEngine;

public class PlaneGeneratorWindow : EditorWindow
{
    private int widthSegments = 10;
    private int heightSegments = 10;
    private float planeWidth = 5f;
    private float planeHeight = 5f;

    [MenuItem("Tools/Plane Generator")]
    public static void ShowWindow()
    {
        GetWindow<PlaneGeneratorWindow>("Plane Generator");
    }

    private void OnGUI()
    {
        widthSegments = EditorGUILayout.IntField("Width Segments", widthSegments);
        heightSegments = EditorGUILayout.IntField("Height Segments", heightSegments);
        planeWidth = EditorGUILayout.FloatField("Plane Width", planeWidth);
        planeHeight = EditorGUILayout.FloatField("Plane Height", planeHeight);

        if (GUILayout.Button("Generate Plane"))
        {
            CreatePlane();
        }
    }

    private void CreatePlane()
    {
        GameObject planeObject = new GameObject("Generated Plane");
        MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int numVertices = (widthSegments + 1) * (heightSegments + 1);
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];

        float xOffset = planeWidth / 2f;
        float zOffset = planeHeight / 2f;

        for (int z = 0, i = 0; z <= heightSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++, i++)
            {
                float normalizedX = x / (float)widthSegments;
                float normalizedZ = z / (float)heightSegments;

                float xPos = normalizedX * planeWidth - xOffset;
                float zPos = normalizedZ * planeHeight - zOffset;

                vertices[i] = new Vector3(xPos, 0f, zPos);
                uvs[i] = new Vector2(normalizedX, normalizedZ);
            }
        }

        int numTriangles = widthSegments * heightSegments * 6;
        int[] triangles = new int[numTriangles];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < heightSegments; z++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + widthSegments + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + widthSegments + 1;
                triangles[tris + 5] = vert + widthSegments + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // URP kullanýyorsan aþaðýdaki shader kullanýlabilir
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("HDRP/Lit");

        if (shader != null)
        {
            meshRenderer.material = new Material(shader);
        }
        else
        {
            Debug.LogWarning("URP Shader bulunamadý. Varsayýlan Standard shader kullanýlacak.");
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
    }
}
