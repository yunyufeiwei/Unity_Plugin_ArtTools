using UnityEngine;

/// <summary>
/// 自动叠加场景脚本（可挂载到 GameObject）
/// </summary>
public class SceneAutoAdditiveLoader : MonoBehaviour
{
    [Header("自动叠加的场景名（不用写 .scene）")]
    public string additiveSceneName;
}