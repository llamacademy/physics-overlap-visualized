using UnityEngine;

public class OverlapDemo : MonoBehaviour
{
    public OverlapMode Mode;
    public float Radius = 5;
    public Vector3 HalfExtents;
    public float CapsuleLength = 5;
    public Vector3 CapsuleOrientation = new Vector3(1, 0, 0);
    public float CapsuleRadius = 0.5f;
    public LayerMask LayerMask;

    private Transform PlacementCube;
    private Renderer Renderer;
    private Color[] GreenColors;
    private Color[] RedColors;
    private Mesh Mesh;

    public enum OverlapMode
    {
        Box,
        Capsule,
        Sphere
    }

    private void Awake()
    {
        PlacementCube = transform.Find("Cube");
        Renderer = PlacementCube.GetComponent<MeshRenderer>();
        Mesh = Renderer.GetComponent<MeshFilter>().mesh;
        Color[] meshColors = Mesh.colors;

        GreenColors = new Color[meshColors.Length];
        RedColors = new Color[meshColors.Length];
        for (int i = 0; i < meshColors.Length; i++)
        {
            GreenColors[i] = Color.green;
            RedColors[i] = Color.red;
        }
    }
}