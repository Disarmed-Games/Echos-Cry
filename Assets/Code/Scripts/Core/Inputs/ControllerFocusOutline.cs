using UnityEngine;
using UnityEngine.UI;

// A hollow frame keeps labels, toggle state, and slider values fully visible.
public class ControllerFocusOutline : MaskableGraphic
{
    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
        maskable = false;
    }

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect bounds = rectTransform.rect;
        AddRing(mesh, bounds, 7f, new Color32(10, 14, 22, 245));
        bounds.xMin += 2f;
        bounds.xMax -= 2f;
        bounds.yMin += 2f;
        bounds.yMax -= 2f;
        AddRing(mesh, bounds, 3f, new Color32(255, 220, 65, 255));
    }

    private static void AddRing(VertexHelper mesh, Rect bounds, float thickness, Color32 tint)
    {
        thickness = Mathf.Min(thickness, Mathf.Min(bounds.width, bounds.height) * 0.5f);
        AddQuad(mesh, bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMin + thickness, tint);
        AddQuad(mesh, bounds.xMin, bounds.yMax - thickness, bounds.xMax, bounds.yMax, tint);
        AddQuad(mesh, bounds.xMin, bounds.yMin + thickness, bounds.xMin + thickness, bounds.yMax - thickness, tint);
        AddQuad(mesh, bounds.xMax - thickness, bounds.yMin + thickness, bounds.xMax, bounds.yMax - thickness, tint);
    }

    private static void AddQuad(VertexHelper mesh, float left, float bottom, float right, float top, Color32 tint)
    {
        int first = mesh.currentVertCount;
        mesh.AddVert(new Vector3(left, bottom, 0f), tint, Vector2.zero);
        mesh.AddVert(new Vector3(left, top, 0f), tint, Vector2.zero);
        mesh.AddVert(new Vector3(right, top, 0f), tint, Vector2.zero);
        mesh.AddVert(new Vector3(right, bottom, 0f), tint, Vector2.zero);
        mesh.AddTriangle(first, first + 1, first + 2);
        mesh.AddTriangle(first + 2, first + 3, first);
    }
}
