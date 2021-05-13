using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Helper
{
    [MenuItem("Tools/添加脚本引用")]
    public static void MissingRefrence()
    {
        AddPrefabComponent<GameManager>("Assets/Resources/Prefabs/GameManager.prefab");
        AddPrefabComponent<UI_Start>("Assets/Resources/Prefabs/UI/UI_Start.prefab");
        AddPrefabComponent<Obstacle>("Assets/Resources/Prefabs/Obstacle.prefab");
        AddPrefabComponent<ObstacleGenerator>("Assets/Resources/Prefabs/SceneRoot.prefab");
    }

    static void AddPrefabComponent<T>(string prefabPath)
       where T : Component
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var go = Object.Instantiate(prefab);
        go.AddComponent<T>();
        PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.Default);
        Object.DestroyImmediate(go);
    }
}
