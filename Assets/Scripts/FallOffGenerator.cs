using System;
using UnityEngine;

[Serializable]
public class FallOffGenerator
{
    public enum FallOfMode
    {
        circleInclusive,
        circleExclusive,
        rectangle,
    }

    public FallOfMode fallOfMode;
    public AnimationCurve fallOffCurve;
    public float[,] fallOffMap;

    public void GenerateFallOffMap(NoiseMap noiseMap)
    {
        int w = noiseMap.map.GetLength(0);
        int h = noiseMap.map.GetLength(1);
        fallOffMap = new float[w, h];
        Vector2 center = new Vector2(w * 0.5f, h * 0.5f);
        switch (fallOfMode)
        {
            case FallOfMode.circleInclusive:
                float maxDist = Mathf.Min(w * 0.5f, h * 0.5f);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        float distNormalized = Vector2.Distance(center, new Vector2(x, y)) / maxDist;
                        float f = Mathf.InverseLerp(1f, 0f, Mathf.Clamp(distNormalized, 0f, 1f));
                        fallOffMap[x, y] = fallOffCurve.Evaluate(f);
                    }
                }
                break;
            case FallOfMode.rectangle:
                float wO2 = w * 0.5f;
                float hO2 = h * 0.5f;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        float distx = Mathf.Abs(x - center.x);
                        float disty = Mathf.Abs(y - center.y);
                        float distNormalized = Mathf.Max(distx / wO2, disty / hO2);
                        float f = Mathf.InverseLerp(1f, 0f, Mathf.Clamp(distNormalized, 0f, 1f));
                        fallOffMap[x, y] = fallOffCurve.Evaluate(f);
                    }
                }
                break;

            case FallOfMode.circleExclusive:
                maxDist = Vector2.Distance(Vector2.zero, center);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        float distNormalized = Vector2.Distance(center, new Vector2(x, y)) / maxDist;
                        float f = Mathf.InverseLerp(1f, 0f, Mathf.Clamp(distNormalized, 0f, 1f));
                        fallOffMap[x, y] = fallOffCurve.Evaluate(f);
                    }
                }
                break;
        }
    }

    public void ApplyFallOffMap(NoiseMap noiseMap)
    {
        for (int y = 0; y < noiseMap.map.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.map.GetLength(0); x++)
            {
                noiseMap.map[x, y] *= fallOffMap[x, y];
            }
        }
    }
}
