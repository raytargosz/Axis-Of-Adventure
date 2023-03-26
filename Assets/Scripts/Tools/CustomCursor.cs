using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Custom Cursor Settings")]
    [Tooltip("The custom cursor texture")]
    [SerializeField] private Texture2D cursorTexture;

    [Tooltip("The cursor hotspot")]
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private void Start()
    {
        ApplyCustomCursor();
    }

    private void ApplyCustomCursor()
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("CustomCursor: cursorTexture not assigned!");
        }
    }
}