using UnityEngine;
using System.Collections;

public static class Utils {

    public static float ValueProgress (float start, float end, float current) {
        return (current - start) / (end - start);
    }

   
    public static float EaseValue (float start, float end, float current, Easer ease) {

        return ease(ValueProgress(start, end, current)) * (end - start) + start;
    }



}
