using UnityEditor;
using UnityEngine;
using System.IO;

public static class TerrainSplatExporter
{
    [MenuItem("ArtTools/Level/Terrain Export All Splat Maps (所有地形)")]
    public static void ExportAllTerrainSplatMaps()
    {
        Terrain[] allTerrains = Terrain.activeTerrains;

        if (allTerrains == null || allTerrains.Length == 0)
        {
            Debug.LogWarning("场景中未找到任何地形！");
            return;
        }

        string saveFolder = EditorUtility.SaveFolderPanel("选择保存文件夹", "", "");
        if (string.IsNullOrEmpty(saveFolder)) return;

        for (int i = 0; i < allTerrains.Length; i++)
        {
            Terrain terrain = allTerrains[i];
            TerrainData terrainData = terrain.terrainData;

            Texture2D splatMap = terrainData.alphamapTextures[0];

            string fileName = $"Terrain_{i}.png";
            string fullPath = Path.Combine(saveFolder, fileName);

            byte[] pngData = splatMap.EncodeToPNG();
            File.WriteAllBytes(fullPath, pngData);

            Debug.Log($"导出成功：{fullPath}");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("导出完成", 
            $"成功导出 {allTerrains.Length} 个地形的 Splat Map！", "确定");
    }
}