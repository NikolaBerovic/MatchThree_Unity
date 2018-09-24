using UnityEngine;

public static class Mathematics {

    ///<summary> 
    ///<para>Interpolates value by specific interpolation type</para>
    ///<para>Returns interpolated value</para>
    ///</summary>
    public static float Interpolate(float value, InterpType interpType)
    {
        float t = value;

        switch (interpType)
        {
            case InterpType.Linear:
                break;
            case InterpType.EaseOut:
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                break;
            case InterpType.EaseIn:
                t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                break;
            case InterpType.SmoothStep:
                t = t * t * (3 - 2 * t);
                break;
            case InterpType.SmootherStep:
                t = t * t * t * (t * (t * 6 - 15) + 10);
                break;
        }
        return t;
    }
}
