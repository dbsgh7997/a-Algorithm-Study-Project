using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y)
        {
            return x == y;
        }

        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private static Dictionary<float, WaitForSeconds> timeSeconds = new Dictionary<float, WaitForSeconds>(new FloatComparer());
    private static Dictionary<float, WaitForSecondsRealtime> timeSecondsReal = new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!timeSeconds.TryGetValue(seconds, out wfs))
        {
            timeSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));
        }
        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime wfsReal;
        if (!timeSecondsReal.TryGetValue(seconds, out wfsReal))
        {
            timeSecondsReal.Add(seconds, wfsReal = new WaitForSecondsRealtime(seconds));
        }
        return wfsReal;
    }
}
