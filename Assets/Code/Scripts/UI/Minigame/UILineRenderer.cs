using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    private List<Vector2> onBeatPoints = new List<Vector2>();
    [SerializeField] private Color beatTargetColor = Color.white;

    [SerializeField] private RectTransform mRectTransform;
    [SerializeField] private float thickness = 10f;
    [SerializeField] private float lineWidth = 10f;
    [SerializeField] private Color onBeatLineColor;
    [SerializeField] private Color secondBeatLineColor;
    [SerializeField] private Color fourthBeatLineColor;
    [Header("Hit Ranges")]
    [SerializeField] private Color badRangeColor = new Color(1f, 0f, 0f, 0.45f);
    [SerializeField] private Color goodRangeColor = new Color(47f / 255f, 235f / 255f, 81f / 255f, 0.45f);
    [SerializeField] private Color excellentRangeColor = new Color(110f / 255f, 44f / 255f, 222f / 255f, 0.45f);
    
    private void Update()
    {
        if (!BeatManager.Instance) return;

        float progress = Mathf.Repeat(0.5f - BeatManager.Instance.BeatProgress, 1f);
        float rectWidth = mRectTransform.rect.width;

        onBeatPoints.Clear();
        onBeatPoints.Add(new Vector2(mRectTransform.rect.xMin + (rectWidth * progress) - (lineWidth / 2), 0));
        onBeatPoints.Add(new Vector2(mRectTransform.rect.xMin + (rectWidth * progress) + (lineWidth / 2), 0));


        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = mRectTransform.rect;

        // Marker speed is one bar width per beat. The scoring percentages are
        // total window widths, so each colored band uses the same fraction of the bar.
        var tempo = Application.isPlaying ? TempoConductor.Instance : null;
        float excellent = tempo != null ? tempo.ExcellentPercent : 0.1f;
        float good = tempo != null ? tempo.GoodPercent : 0.2f;
        float excellentHalf = rect.width * Mathf.Clamp01(excellent) * 0.5f;
        float goodHalf = rect.width * Mathf.Clamp01(Mathf.Max(good, excellent)) * 0.5f;
        float target = rect.center.x;
        // Blend narrowly around the scoring boundaries, keeping each range readable.
        float blend = Mathf.Min(excellentHalf, goodHalf - excellentHalf) * 0.2f;
        float goodLeft = target - goodHalf;
        float excellentLeft = target - excellentHalf;
        float excellentRight = target + excellentHalf;
        float goodRight = target + goodHalf;
        Color fadedBad = badRangeColor;
        fadedBad.a = 0f;
        AddRange(vh, rect, rect.xMin, goodLeft - blend, fadedBad, badRangeColor);
        AddRange(vh, rect, goodLeft - blend, goodLeft + blend, badRangeColor, goodRangeColor);
        AddRange(vh, rect, goodLeft + blend, excellentLeft - blend, goodRangeColor, goodRangeColor);
        AddRange(vh, rect, excellentLeft - blend, excellentLeft + blend, goodRangeColor, excellentRangeColor);
        AddRange(vh, rect, excellentLeft + blend, excellentRight - blend, excellentRangeColor, excellentRangeColor);
        AddRange(vh, rect, excellentRight - blend, excellentRight + blend, excellentRangeColor, goodRangeColor);
        AddRange(vh, rect, excellentRight + blend, goodRight - blend, goodRangeColor, goodRangeColor);
        AddRange(vh, rect, goodRight - blend, goodRight + blend, goodRangeColor, badRangeColor);
        AddRange(vh, rect, goodRight + blend, rect.xMax, badRangeColor, fadedBad);
        UIVertex vertex = UIVertex.simpleVert;
        // Fixed full-height center target; the moving marker crosses it on each beat.
        int targetIndex = vh.currentVertCount;
        float center = rect.center.x;
        float halfWidth = Mathf.Max(1f, lineWidth * 0.15f);
        vertex.color = beatTargetColor;
        vertex.position = new Vector3(center - halfWidth, rect.yMin); vh.AddVert(vertex);
        vertex.position = new Vector3(center - halfWidth, rect.yMax); vh.AddVert(vertex);
        vertex.position = new Vector3(center + halfWidth, rect.yMax); vh.AddVert(vertex);
        vertex.position = new Vector3(center + halfWidth, rect.yMin); vh.AddVert(vertex);
        vh.AddTriangle(targetIndex, targetIndex + 1, targetIndex + 2);
        vh.AddTriangle(targetIndex + 2, targetIndex + 3, targetIndex);

        //On Beat Line Renderer
        for (int i = 0; i < onBeatPoints.Count - 1; i++)
        {
            Vector2 p1 = onBeatPoints[i];
            Vector2 p2 = onBeatPoints[i + 1];

            // Clamp inside rect
            p1.x = Mathf.Clamp(p1.x, rect.xMin, rect.xMax);
            p2.x = Mathf.Clamp(p2.x, rect.xMin, rect.xMax);
            p1.y = Mathf.Clamp(p1.y, rect.yMin, rect.yMax);
            p2.y = Mathf.Clamp(p2.y, rect.yMin, rect.yMax);

            Vector2 direction = (p1 - p2).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x);

            int idx = vh.currentVertCount;

            vertex = UIVertex.simpleVert;
            vertex.color = onBeatLineColor;

            if (Player.Instance != null && BeatManager.Instance)
            {
                if (BeatManager.Instance.BeatInMeasure == 0 && Player.Instance.HeatGauge.CurrentCharge >= 6)
                {
                    vertex.color = secondBeatLineColor;
                }
                else if (BeatManager.Instance.BeatInMeasure == 2 && Player.Instance.HeatGauge.CurrentCharge >= 6)
                {
                    vertex.color = fourthBeatLineColor;
                }
                else
                    vertex.color = onBeatLineColor;
            }

            vertex.position = p1 + normal * (thickness / 2);
            vh.AddVert(vertex);

            vertex.position = p1 - normal * (thickness / 2);
            vh.AddVert(vertex);

            vertex.position = p2 - normal * (thickness / 2);
            vh.AddVert(vertex);

            vertex.position = p2 + normal * (thickness / 2);
            vh.AddVert(vertex);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx + 2, idx + 3, idx);
        }
    }

    private static void AddRange(VertexHelper vh, Rect rect, float left, float right, Color leftColor, Color rightColor)
    {
        if (right <= left) return;
        int index = vh.currentVertCount;
        var vertex = UIVertex.simpleVert;
        vertex.color = leftColor;
        vertex.position = new Vector3(left, rect.yMin); vh.AddVert(vertex);
        vertex.position = new Vector3(left, rect.yMax); vh.AddVert(vertex);
        vertex.color = rightColor;
        vertex.position = new Vector3(right, rect.yMax); vh.AddVert(vertex);
        vertex.position = new Vector3(right, rect.yMin); vh.AddVert(vertex);
        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index + 2, index + 3, index);
    }
}
