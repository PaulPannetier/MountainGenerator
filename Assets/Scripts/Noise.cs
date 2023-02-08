using UnityEngine;

public static class Noise
{
    /*
    public static NoiseMap GenerateNoiseMap(in int mapWidth, in int mapHeight, in int seed, Vector2 scale, in Vector2 offset)
    {
        //vérification des valeurs pris en paramètre
        if (scale.x < 0f)
            scale.x *= -1f;
        if (scale.y < 0f)
            scale.y *= -1f;
        if (scale.sqrMagnitude <= Mathf.Epsilon)
            scale = new Vector2(20f, 20f);

        float[,] noiseMap = new float[mapWidth, mapHeight];// la carte finale
        Random.SetRandomSeed(seed);//On définie la graine d'aléatoire

        //on décale le tout pour que le point (0,0) soit au milieu de la carte
        //et non pas en haut à gauche.
        float halfWidth = mapWidth * 0.5f + offset.x;
        float halfHeight = mapHeight * 0.5f + offset.y;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //On recalcule les coord x et y pour y appliquer le scale
                float sampleX = (x - halfWidth) / scale.x;
                float sampleY = (y - halfHeight) / scale.y;
                noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
            }
        }
        return new NoiseMap(noiseMap);
    }

    */

    public static NoiseMap GenerateNoiseMap(in int mapWidth, in int mapHeight, in int seed, Vector2 scale, in int octave, in float persistance, in float lacunarity, in Vector2 offset)
    {
        if (scale.x < 0f)
            scale.x *= -1f;
        if (scale.y < 0f)
            scale.y *= -1f;
        if (scale.sqrMagnitude == 0f)
            scale = new Vector2(20f, 20f);
        
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random.SetRandomSeed(seed);
        Vector2[] octaveOffset = new Vector2[octave];
        float amplitude = 1f;
        float frequency = 1f;
        for (int i = 0; i < octave; i++)
        {
            //on met un offset différent pour chaque octaves
            float offsetX = Random.Rand(-100000f, +100000f) + offset.x;
            float offsetY = Random.Rand(-100000f, +100000f) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth * 0.5f;
        float halfHeight = mapHeight * 0.5f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1f;
                frequency = 1f;
                float noiseHeight = 0f;
                for (int i = 0; i < octave; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffset[i].x) / scale.x * frequency;
                    float sampleY = (y - halfHeight + octaveOffset[i].y) / scale.y * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                noiseMap[x, y] = noiseHeight;
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);
                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
            }
        }
        //on normalise pour avoir des nombre entre 0 et 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return new NoiseMap(noiseMap);
    }

    //Méthode des cercle pas encore au point
    public static NoiseMap GenerateNoiseMapV2(in int mapWidth, in int mapHeight, in int seed, Vector2 scale, in float circleDensity, in float minRadius, in float maxRadius, AnimationCurve strength, in Vector2 offset)
    {
        if (scale.x < 0f)
            scale.x *= -1f;
        if (scale.y < 0f)
            scale.y *= -1f;
        if (scale.sqrMagnitude == 0f)
            scale = new Vector2(20f, 20f);
        Random.SetRandomSeed(seed);
        float[,] noiseMap = new float[mapWidth, mapHeight];

        int nbCircles = Mathf.Max(1, (int)(mapWidth * mapHeight * circleDensity));
        nbCircles = 5;
        //float scaleF = (scale.x * scale.y) * 0.5f;

        for (int i = 0; i < nbCircles; i++)
        {
            //on prend un centre au pif
            Vector2 center = new Vector2(Random.Rand(0, mapWidth - 1), Random.Rand(0, mapHeight - 1));
            float radius = Random.Rand(minRadius, maxRadius);
            
            int begX = Mathf.Max(0, (int)(center.x - (radius / 2f)) + 1);
            int endX = Mathf.Min(mapWidth - 1, (int)(center.x + (radius / 2f)) + 1);
            int begY = Mathf.Max(0, (int)(center.y - (radius / 2f)) + 1);
            int endY = Mathf.Min(mapHeight - 1, (int)(center.y + (radius / 2f)) + 1);
            
            /*
            int begX = Mathf.Max(0, (int)(center.x - (radius)) + 1);
            int endX = Mathf.Min(mapWidth - 1, (int)(center.x + (radius)) + 1);
            int begY = Mathf.Max(0, (int)(center.y - (radius)) + 1);
            int endY = Mathf.Min(mapHeight - 1, (int)(center.y + (radius)) + 1);
            */

            for (int y = begY; y < endY; y++)
            {
                for (int x = begX; x < endX; x++)
                {
                    float dist = center.Distance(x, y);
                    if(dist <= radius)
                    {
                        noiseMap[x, y] += strength.Evaluate(Mathf.InverseLerp(1f, 0f, Mathf.Clamp(dist / radius, 0f, 1f)));
                    }
                }
            }
        }
        float globalMax = float.MinValue;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                globalMax = Mathf.Max(globalMax, noiseMap[x, y]);
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = ((noiseMap[x, y] / globalMax) * 2f) - 1f;
            }
        }
        return new NoiseMap(noiseMap);
    }

    public static NoiseMap GenerateNoiseMapCombine(in int mapWidth, in int mapHeight, in int seed, Vector2 scale, in int octave, in float persistance, in float lacunarity,
        in Vector2 offset, in float circleDensity, in float minRadius, in float maxRadius, AnimationCurve strength)
    {
        return GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octave, persistance, lacunarity, offset) + GenerateNoiseMapV2(mapWidth, mapHeight, seed, scale, circleDensity, minRadius, maxRadius, strength, offset);
    }
}

public class NoiseMap
{
    public float[,] map;

    public NoiseMap(in float[,] noiseMap)
    {
        this.map = noiseMap;
    }

    public static NoiseMap operator + (NoiseMap n, NoiseMap m)
    {
        if((n.map.GetLength(0) != m.map.GetLength(0)) || (n.map.GetLength(1) != m.map.GetLength(1)))
            return null;

        float[,] res = new float[n.map.GetLength(0), n.map.GetLength(1)];
        float max = float.MinValue;
        for (int y = 0; y < n.map.GetLength(1); y++)
        {
            for (int x = 0; x < n.map.GetLength(0); x++)
            {
                res[x, y] = n.map[x, y] + m.map[x, y];
                max = Mathf.Max(max, res[x, y]);
            }
        }
        float OneOverMax = 1f / max;
        for (int y = 0; y < n.map.GetLength(1); y++)
        {
            for (int x = 0; x < n.map.GetLength(0); x++)
            {
                res[x, y] *= OneOverMax;
            }
        }
        return new NoiseMap(res);
    }
}
