using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

[InitializeOnLoad]
public static class SceneAutoLoader
{
    // 静态构造：注册监听
    static SceneAutoLoader()
    {
        // 清空旧监听，防止重复
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    // 🔥 这里修复了参数类型，不会再报多重选择
    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        // 延迟一帧，确保场景完全加载
        EditorApplication.delayCall += () =>
        {
            // 只有单独打开场景或替换场景时才设置为可选择状态
            // 如果是叠加模式打开的场景，不自动设置为可选择
            if (mode == OpenSceneMode.Single || mode == OpenSceneMode.AdditiveWithoutLoading)
            {
                if (scene.isLoaded)
                {
                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    foreach (GameObject rootObj in rootObjects)
                    {
                        SceneVisibilityManager.instance.EnablePicking(rootObj, true);
                    }
                }
                
                TryAutoLoadAdditiveScene();
            }
        };
    }
    
    

    private static void TryAutoLoadAdditiveScene()
    {
        // 查找场景中的加载脚本
        var loader = Object.FindObjectOfType<SceneAutoAdditiveLoader>();
        if (loader == null) return;

        string sceneName = loader.additiveSceneName;
        if (string.IsNullOrEmpty(sceneName)) return;

        // 防止重复加载
        Scene loadedScene = EditorSceneManager.GetSceneByName(sceneName);
        if (loadedScene.isLoaded) return;

        // 自动查找场景路径
        string[] guids = AssetDatabase.FindAssets(sceneName + " t:Scene");
        if (guids.Length == 0)
        {
            Debug.LogError("自动叠加失败：找不到场景 " + sceneName);
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Scene additiveScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        
        // 延迟设置，确保场景完全加载后再禁用picking
        EditorApplication.delayCall += () =>
        {
            // 设置叠加场景为不可选择状态（手指图标变灰）
            if (additiveScene.isLoaded)
            {
                GameObject[] rootObjects = additiveScene.GetRootGameObjects();
                foreach (GameObject rootObj in rootObjects)
                {
                    SceneVisibilityManager.instance.DisablePicking(rootObj, true);
                }
                Debug.Log("<color=green>✅ 自动叠加场景完成：</color>" + sceneName + " <color=yellow>(已禁用picking)</color>");
            }
        };
    }
}

// 自定义Inspector面板
[CustomEditor(typeof(SceneAutoAdditiveLoaderEditor))]
public class SceneAutoAdditiveLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("重新打开场景时会自动叠加", MessageType.Info);
    }
}