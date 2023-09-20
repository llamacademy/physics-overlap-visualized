using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OverlapDemo))]
public class OverlapDemoEditor : Editor
{
    private GUILayoutOption[] DefaultOptions = new GUILayoutOption[0];

    private Collider[] Colliders;

    private void OnDisable()
    {
        SetColliderColors(Colliders, Color.red);
    }

    public override void OnInspectorGUI()
    {
        OverlapDemo.OverlapMode mode = (OverlapDemo.OverlapMode)serializedObject.FindProperty("Mode").enumValueIndex;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"), new GUIContent("Mode"), DefaultOptions);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("LayerMask"), new GUIContent("Layer Mask"),
            DefaultOptions);

        if (mode == OverlapDemo.OverlapMode.Capsule)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CapsuleRadius"), new GUIContent("Radius"),
                DefaultOptions);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CapsuleLength"), new GUIContent("Length"),
                DefaultOptions);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CapsuleOrientation"),
                new GUIContent("Orientation"),
                DefaultOptions);
        }
        else if (mode == OverlapDemo.OverlapMode.Box)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HalfExtents"), new GUIContent("Half Extents"),
                DefaultOptions);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Radius"),
                DefaultOptions);
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    private void OnSceneGUI()
    {
        if (target == null)
        {
            return;
        }

        OverlapDemo demo = target as OverlapDemo;

        switch (demo.Mode)
        {
            case OverlapDemo.OverlapMode.Box:
                DrawBoxSceneControls(demo);
                break;
            case OverlapDemo.OverlapMode.Capsule:
                DrawCapsuleSceneControls(demo);
                break;
            case OverlapDemo.OverlapMode.Sphere:
                DrawSphereSceneControls(demo);
                break;
        }
    }

    private void DrawBoxSceneControls(OverlapDemo demo)
    {
        Handles.color = new Color(0, 1, 0, 0.25f);
        
        Matrix4x4 angleMatrix = Matrix4x4.TRS(demo.transform.position, demo.transform.rotation, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.DrawWireCube( Vector3.zero, demo.HalfExtents * 2);    
        }

        SetColliderColors(Colliders, Color.red);

        Colliders = Physics.OverlapBox(demo.transform.position, demo.HalfExtents, demo.transform.rotation,
            demo.LayerMask);

        SetColliderColors(Colliders, Color.green);
    }

    private void DrawCapsuleSceneControls(OverlapDemo demo)
    {
        SetColliderColors(Colliders, Color.red);

        Handles.color = new Color(0, 1, 0, 0.25f);

        Vector3 position = demo.transform.position;
        Vector3 direction = demo.CapsuleOrientation;
        Quaternion rotation = Quaternion.LookRotation(demo.CapsuleOrientation);

        float height = demo.CapsuleLength;
        float radius = demo.CapsuleRadius;
        Vector3 point1 = position - (direction * (height / 2));
        Vector3 point2 = position + (direction * (height / 2));
        Vector3 center2 = new Vector3(0f,0,height);
        
        Matrix4x4 angleMatrix = Matrix4x4.TRS(point1, rotation, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * height, -180f, radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * height, -180f, radius);
            Handles.DrawWireDisc(center2, Vector3.forward, radius);
            Handles.DrawWireArc(center2, Vector3.up, Vector3.right * height, -180f, radius);
            Handles.DrawWireArc(center2, Vector3.left, Vector3.up * height, -180f, radius);

            Handles.DrawLine(new Vector3(radius, 0, 0f), new Vector3(radius, 0, height));
            Handles.DrawLine(new Vector3(-radius, 0, 0f), new Vector3(-radius, 0, height));
            Handles.DrawLine(new Vector3(0, radius, 0f), new Vector3(0, radius, height));
            Handles.DrawLine(new Vector3(0, -radius, 0f), new Vector3(0, -radius, height));
        }

        Colliders = Physics.OverlapCapsule(point1, point2, radius, demo.LayerMask);

        SetColliderColors(Colliders, Color.green);
    }
    private static void DrawLine(float arg1,float arg2,float forward)
    {
        Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
    }

    private void DrawSphereSceneControls(OverlapDemo demo)
    {
        SetColliderColors(Colliders, Color.red);

        Handles.color = new Color(0, 1, 0, 0.25f);
        Handles.SphereHandleCap(
            GUIUtility.GetControlID(FocusType.Passive),
            demo.transform.position,
            Quaternion.identity,
            demo.Radius * 2,
            EventType.Repaint
        );

        Colliders = Physics.OverlapSphere(demo.transform.position, demo.Radius, demo.LayerMask);

        SetColliderColors(Colliders, Color.green);
    }

    private void SetColliderColors(Collider[] colliders, Color color)
    {
        if (colliders == null)
        {
            return;
        }

        Color[] colors = null;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out MeshFilter filter))
            {
                if (colors == null)
                {
                    colors = new Color[filter.mesh.colors.Length];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = color;
                    }
                }

                filter.mesh.SetColors(colors);
            }
        }
    }
}