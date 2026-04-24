using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GradientRender : Graphic
{
    [SerializeField] private RectTransform mRectTransform;
    [SerializeField] private Color leftColor = Color.white;
    [SerializeField] private Color rightColor = Color.black;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = mRectTransform.rect;

        //Gradient Background
        int startIdx = vh.currentVertCount;

        UIVertex vertex = UIVertex.simpleVert;

        vertex.position = new Vector3(rect.xMin, rect.yMin);
        vertex.color = leftColor;
        vh.AddVert(vertex);

        vertex.position = new Vector3(rect.xMin, rect.yMax);
        vertex.color = leftColor;
        vh.AddVert(vertex);

        vertex.position = new Vector3(rect.xMax, rect.yMax);
        vertex.color = rightColor;
        vh.AddVert(vertex);

        vertex.position = new Vector3(rect.xMax, rect.yMin);
        vertex.color = rightColor;
        vh.AddVert(vertex);

        vh.AddTriangle(startIdx, startIdx + 1, startIdx + 2);
        vh.AddTriangle(startIdx + 2, startIdx + 3, startIdx);
    }
}