using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrassBender : MonoBehaviour
{
    public Material bendingMaterial;
    public float radius = 1.5f;

    private void OnTriggerStay(Collider other)
    {
        // Only update when camera sees this area
        if (!Camera.current || !bendingMaterial) return;

        // Project into grass bending RT space
        Vector3 center = transform.position;
        Vector4 worldPos = new Vector4(center.x, center.y, center.z, radius);
        bendingMaterial.SetVector("_BendPosition", worldPos);

        Graphics.DrawMeshNow(CreateQuad(), Matrix4x4.TRS(center + Vector3.up * 0.01f, Quaternion.identity, Vector3.one * radius));
    }

    Mesh CreateQuad()
    {
        Mesh m = new Mesh();
        m.vertices = new Vector3[] {
            new Vector3(-1, 0, -1), new Vector3(1, 0, -1),
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1)
        };
        m.uv = new Vector2[] {
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(1, 1), new Vector2(0, 1)
        };
        m.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        return m;
    }
}
