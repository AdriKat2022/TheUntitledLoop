using UnityEngine;
using UnityEngine.UI;

public class CursorSettings : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool dontDestroyOnLoad = false;
    [SerializeField] private bool applyOnStart = true;
    [SerializeField] private CursorLockMode lockMode;
    [SerializeField] private bool visible;
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Sprite cursorOverlay;

    private Canvas canvas;
    private Image cursorOverlayInstance;

    private void Start()
    {
        if (applyOnStart)
        {
            ApplySettings();
        }

        InitializeCursorOverlay();
    }

    private void Update()
    {
        // Place the overlay image directly on the cursor position
        if (cursorOverlayInstance != null)
        {
            cursorOverlayInstance.rectTransform.position = Input.mousePosition;
        }
    }

    public void ToogleCursorVisibility(bool visibility)
    {
        Cursor.visible = visibility;
    }

    public void ToogleCursorOverlay(bool visibility)
    {
        cursorOverlayInstance.gameObject.SetActive(visibility);
    }

    public void ToogleCustomCursor(bool visibility)
    {
        ToogleCursorOverlay(visibility);
        ToogleCursorVisibility(!visibility);
    }

    public void ApplySettings()
    {
        Cursor.lockState = lockMode;
        Cursor.visible = visible;
        if (cursorSprite != null)
        {
            Cursor.SetCursor(cursorSprite.texture, Vector2.zero, CursorMode.Auto);
        }
    }

    private void InitializeCursorOverlay()
    {
        // CanvasCreation if null

        if (canvas == null)
        {
            canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if (cursorOverlayInstance == null)
        {
            cursorOverlayInstance = new GameObject("CursorOverlay", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            cursorOverlayInstance.transform.SetParent(canvas.transform);
            cursorOverlayInstance.rectTransform.sizeDelta = new Vector2(cursorOverlay.texture.width, cursorOverlay.texture.height);
            cursorOverlayInstance.sprite = cursorOverlay;
            cursorOverlayInstance.raycastTarget = false;
        }
    }
}
