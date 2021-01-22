using DG.Tweening;
public class GameUtil
{

    public static void WaitThenDo(float seconds, TweenCallback callback)
    {
        float t = 0;
        DOTween.To(() => t, (x) => t = x, seconds, seconds).OnComplete(callback);
    }

    public static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (layer & 1 << layer) == (1 << layer);
    }
}