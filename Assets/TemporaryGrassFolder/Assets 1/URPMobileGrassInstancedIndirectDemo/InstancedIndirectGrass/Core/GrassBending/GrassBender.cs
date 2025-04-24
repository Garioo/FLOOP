using UnityEngine;

[ExecuteAlways]
public class GrassBender : MonoBehaviour
{
    public Material bendingMaterial;
    public float radius = 1.5f;
    public float heightThreshold = 2f; // only bend grass if Y is below this

    static Mesh _quad;

    private void OnEnable()
    {
        CreateQuadIfNeeded();
    }

    void Update()
    {
        if (bendingMaterial == null) return;

        Vector3 pos = transform.position;

        // Only bend if within Y height range (near ground)
        if (pos.y > heightThreshold) return;

        // Set bend position (X, ignored, Z, radius)
        bendingMaterial.SetVector("_BendPosition", new Vector4(pos.x, 0, pos.z, radius));

        // Draw bend quad into the RT
        Graphics.DrawMeshNow(_quad, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * radius));
    }

    void CreateQuadIfNeeded()
    {
        if (_quad != null) return;

        _quad = new Mesh();
        _quad.vertices = new Vector3[]
        {
            new Vector3(-1, 0, -1), new Vector3(1, 0, -1),
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1)
        };
        _quad.uv = new Vector2[]
        {
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(1, 1), new Vector2(0, 1)
        };
        _quad.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
    }
}

