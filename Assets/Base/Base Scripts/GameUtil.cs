using DG.Tweening;
using UnityEngine;
public class GameUtil
{

    public static void WaitThenDo(float seconds, TweenCallback callback)
    {
        float t = 0;
        DOTween.To(() => t, (x) => t = x, seconds, seconds).OnComplete(callback);
    }

    public static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & 1 << layer) == (1 << layer);
    }

    public static void GetObjectBoundsOnZAxis(GameObject prefab, out float min, out float max)
    {
        min = float.PositiveInfinity;
        max = float.NegativeInfinity;
        foreach (var child in prefab.GetComponentsInChildren<Renderer>())
        {
            if (child.bounds.min.z < min)
                min = (float)child.bounds.min.z;
            if (child.bounds.max.z > max)
                max = (float)child.bounds.max.z;
        }
    }
}