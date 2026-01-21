using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfiguration", menuName = "ShroomSniff/Camera Configuration")]
public class CameraConfiguration : ScriptableObject
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    public float initialZoom = 5f;

    [Header("Movement Settings")]
    public float panSpeed = 0.5f;
    public float dragSpeed = 20f;

    [Header("Bounds (optional)")]
    public bool useBounds;
    public Vector2 halfExtents = new(50f, 50f);
}
