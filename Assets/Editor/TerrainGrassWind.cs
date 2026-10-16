using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Reapply the project-wide wind preset when its values change, including unloaded terrains.
[InitializeOnLoad]
public static class TerrainGrassWind
{
    private const string AppliedMarker = "Library/TerrainGrassWind-v3.txt";
    private const float Speed = 1.25f;
    private const float Amount = 0.5f;
    private const float Strength = 0.2f;
    private static string SettingsSignature =>
        System.FormattableString.Invariant($"speed={Speed}, amount={Amount}, strength={Strength}\n");

    static TerrainGrassWind()
    {
        EditorApplication.update += ApplyOnce;
    }

    private static void ApplyOnce()
    {
        // Wait until asset loading and play mode have finished before saving terrains.
        if (EditorApplication.isCompiling || EditorApplication.isUpdating ||
            EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(AppliedMarker) ||
            !File.ReadAllText(AppliedMarker).StartsWith(SettingsSignature, System.StringComparison.Ordinal))
            Apply();

        EditorApplication.update -= ApplyOnce;
    }

    [MenuItem("Tools/Terrain/Apply Gentle Grass Wind")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        var report = new StringBuilder();
        int count = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:TerrainData", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
            if (data == null)
                continue;

            data.wavingGrassSpeed = Speed;
            data.wavingGrassAmount = Amount;
            data.wavingGrassStrength = Strength;
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssetIfDirty(data);
            report.AppendLine($"{path}: speed={data.wavingGrassSpeed}, amount={data.wavingGrassAmount}, strength={data.wavingGrassStrength}, details={data.detailPrototypes.Length}");
            foreach (DetailPrototype detail in data.detailPrototypes)
                report.AppendLine($"  {detail.prototypeTexture?.name ?? detail.prototype?.name}: mode={detail.renderMode}, instancing={detail.useInstancing}");
            count++;
        }

        report.Insert(0, SettingsSignature + $"Applied gentle grass wind to {count} terrains.\n");
        File.WriteAllText(AppliedMarker, report.ToString());
        Debug.Log(report.ToString());
    }
}
