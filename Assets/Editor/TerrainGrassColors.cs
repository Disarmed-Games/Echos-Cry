using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Apply the grass color preset once, including terrains in unloaded scenes.
[InitializeOnLoad]
public static class TerrainGrassColors
{
    private const string AppliedMarker = "Library/TerrainGrassColors-white-v1.txt";

    static TerrainGrassColors()
    {
        EditorApplication.update += ApplyOnce;
    }

    private static void ApplyOnce()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating ||
            EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        EditorApplication.update -= ApplyOnce;
        if (!File.Exists(AppliedMarker))
            Apply();
    }

    [MenuItem("Tools/Terrain/Set All Grass Detail Colors to White")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        var report = new StringBuilder();
        int terrainCount = 0;
        int detailCount = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:TerrainData", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
            if (data == null)
                continue;

            DetailPrototype[] details = data.detailPrototypes;
            bool changed = false;
            foreach (DetailPrototype detail in details)
            {
                if (detail.renderMode != DetailRenderMode.Grass &&
                    detail.renderMode != DetailRenderMode.GrassBillboard)
                    continue;

                changed |= detail.healthyColor != Color.white || detail.dryColor != Color.white;
                detail.healthyColor = Color.white;
                detail.dryColor = Color.white;
                detailCount++;
            }

            if (changed)
            {
                Undo.RecordObject(data, "Set grass detail colors to white");
                data.detailPrototypes = details;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssetIfDirty(data);
            }

            foreach (DetailPrototype detail in data.detailPrototypes)
            {
                if (detail.renderMode != DetailRenderMode.Grass &&
                    detail.renderMode != DetailRenderMode.GrassBillboard)
                    continue;
                if (detail.healthyColor != Color.white || detail.dryColor != Color.white)
                    throw new System.InvalidOperationException($"Grass colors were not saved: {path}");
                report.AppendLine($"{path}: {detail.prototypeTexture?.name ?? detail.prototype?.name}, healthy={detail.healthyColor}, dry={detail.dryColor}");
            }
            terrainCount++;
        }

        report.Insert(0, $"Verified white healthy/dry colors on {detailCount} grass details across {terrainCount} terrain assets.\n");
        File.WriteAllText(AppliedMarker, report.ToString());
        Debug.Log(report.ToString());
    }
}
