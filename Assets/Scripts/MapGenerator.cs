using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("They is already an instance of MapGenerator");
            return;
        }
        instance = this;
    }

    public enum PlanDrawMode
    {
        NoiseMap, ColorMap, FallOfMap, Mesh
    }

    public const int mapChunkSize = 241;

    [Header("Generals")]
    public int mapWidth;//nb de mapchunk en hauteur et longueur
    public int mapHeight;
    [Range(0, 6)] public int LOD;// peut prendre les val 2, 4, 6, 8, 10, 12 mais on va le multipllier pas 2
    public bool autoUpdate = true;
    public PlanDrawMode drawMode;

    [Header("Noise")]
    public bool useNoiseMapV2;
    public bool useCombineNoiseMap;
    public Vector2 noiseScale;
    public Vector2 offset;

    [Header("Perlin")]
    public int octave;
    public int seed;
    public float lacunarity;// persistance €[0, 1], lacunarity >= 1
    public float heightMultiplier;
    [Range(0, 1)] public float persistance;

    [Header("Circles Method")]
    public AnimationCurve circlesStrenght;
    public float circleDensity, minRadius, maxRadius;

    [Header("Mesh")]
    public AnimationCurve meshHeightCurve;
    public bool useRegion = false;
    public List<TerrainType> regions;
    public bool constructGradientWithRegions = true;
    public Gradient gradientLow, gradientHigh;

    [Header("Fall Of Map")]
    public bool useFallOffMap = false;
    public FallOffGenerator fallOffGenerator;

    [Header("Forest")]
    public bool useForest;
    public ForestGeneratorType forestGeneratorType;

    public Map map;

    public Map GenerateMap()
    {
        instance = this;
        //la noise map
        int tmpWidth = mapChunkSize + (mapChunkSize - 1) * (mapWidth - 1);
        int tmpHeight = mapChunkSize + (mapChunkSize - 1) * (mapHeight - 1);
        NoiseMap noiseMap = null;
        if (useNoiseMapV2)
        {
            noiseMap = Noise.GenerateNoiseMapV2(tmpWidth, tmpHeight, seed, noiseScale, circleDensity, minRadius, maxRadius, circlesStrenght, offset);
        }
        else if (useCombineNoiseMap)
        {
            noiseMap = Noise.GenerateNoiseMapCombine(tmpWidth, tmpHeight, seed, noiseScale, octave, persistance, lacunarity, offset, circleDensity, minRadius, maxRadius, circlesStrenght);
        }
        else
        {
            noiseMap = Noise.GenerateNoiseMap(tmpWidth, tmpHeight, seed, noiseScale, octave, persistance, lacunarity, offset);
        }

        //On applique la fallOfMap
        if (useFallOffMap)
        {
            fallOffGenerator.GenerateFallOffMap(noiseMap);
            fallOffGenerator.ApplyFallOffMap(noiseMap);
        }
        List<float[,]> noiseChunks = Usefull.CutArrayWithMargin(noiseMap.map, mapChunkSize, mapChunkSize);

        //on crée les textures
        List<Texture2D> textures = new List<Texture2D>();
        regions.Sort(new TerrainTypeComparer());
        foreach (float[,] noiseChunk in noiseChunks)
        {
            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            if(useRegion)
            {
                //On remplit directement la colormap
                for (int y = 0; y < mapChunkSize; y++)
                {
                    for (int x = 0; x < mapChunkSize; x++)
                    {
                        float currentHeight = noiseChunk[x, y];
                        for (int i = 0; i < regions.Count; i++)
                        {
                            if (currentHeight <= regions[i].height)
                            {
                                colorMap[y * mapChunkSize + x] = regions[i].color;
                                break;
                            }
                        }
                    }
                }
            }
            else//On utilise les gradients
            {
                if(constructGradientWithRegions)//On construit les gradients
                {
                    gradientLow = new Gradient();
                    gradientHigh = new Gradient();
                    List<TerrainType> terrainsLow = new List<TerrainType>();
                    List<TerrainType> terrainsHigh = new List<TerrainType>();
                    for (int i = 0; i < regions.Count; i++)
                    {
                        if(regions[i].height <= 0.5f)
                            terrainsLow.Add(regions[i]);
                    }
                    terrainsHigh.Add(terrainsLow[terrainsLow.Count - 1]);
                    for (int i = 0; i < regions.Count; i++)
                    {
                        if (regions[i].height > 0.5f)
                            terrainsHigh.Add(regions[i]);
                    }
                    GradientColorKey[] gck = new GradientColorKey[terrainsLow.Count];
                    GradientAlphaKey[] gak = new GradientAlphaKey[2] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) };
                    for (int i = 0; i < terrainsLow.Count; i++)
                    {
                        gck[i] = new GradientColorKey(terrainsLow[i].color, Mathf.InverseLerp(0f, 0.5f, terrainsLow[i].height));
                    }
                    gradientLow.SetKeys(gck, gak);
                    gck = new GradientColorKey[terrainsHigh.Count];
                    for (int i = 0; i < terrainsHigh.Count; i++)
                    {
                        gck[i] = new GradientColorKey(terrainsHigh[i].color, Mathf.InverseLerp(0.5f, 1f, terrainsHigh[i].height));
                    }
                    gradientHigh.SetKeys(gck, gak);
                }
                //On remplie la colormap
                for (int y = 0; y < mapChunkSize; y++)
                {
                    for (int x = 0; x < mapChunkSize; x++)
                    {
                        float currentHeight = noiseChunk[x, y];
                        if(currentHeight <= 0.5f)
                            colorMap[y * mapChunkSize + x] = gradientLow.Evaluate(Mathf.InverseLerp(0f, 0.5f, currentHeight));
                        else
                            colorMap[y * mapChunkSize + x] = gradientHigh.Evaluate(Mathf.InverseLerp(0.5f, 1f, currentHeight));
                    }
                }
            }

            textures.Add(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        
        //on crée les meshData des différent chunks
        List<MeshData> meshesData = new List<MeshData>();
        foreach (float[,] noiseChunk in noiseChunks)
        {
            meshesData.Add(MeshGenerator.GenerateTerrainMeshData(noiseChunk, heightMultiplier, meshHeightCurve, LOD * 2));
        }

        //On a les noiseMap, les textures et les meshDatas des différents chunks!, donc on les crées
        MapChunk[,] mapChunks = new MapChunk[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = y * mapWidth + x;
                mapChunks[x, y] = new MapChunk(new NoiseMap(noiseChunks[i]), meshesData[i], textures[i], LOD * 2);
            }
        }
        //Il ne reste plus qu'a crée la map!
        map = new Map(mapChunks);

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        //on affiche sur le plan la noisemap ou la color map
        if(drawMode == PlanDrawMode.NoiseMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeight(noiseMap.map));
        }
        else if(drawMode == PlanDrawMode.ColorMap)
        {
            mapDisplay.DrawMapTexture(textures, mapWidth, mapHeight);
        }
        else if(drawMode == PlanDrawMode.FallOfMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeight(fallOffGenerator.fallOffMap));
        }
        else if(drawMode == PlanDrawMode.Mesh)
        {
            mapDisplay.DrawMeshes(mapChunks);
        }
        //Si on ne veux^pas tracer les mesh on arrete
        if (drawMode != PlanDrawMode.Mesh)
            return map;

        //On ajoute les props, on commence par les arbres
        if(useForest)
        {
            ForestGenerator forestGenerator = FindObjectOfType<ForestGenerator>();
            forestGenerator.GenerateForest(map, mapDisplay, forestGeneratorType.TreesPrefab, forestGeneratorType.heightRange, forestGeneratorType.nbForest, forestGeneratorType.forestRadius, forestGeneratorType.treesPerForest);
        }

        return map;
    }

    public MeshData GenerateMeshData(NoiseMap noiseMap, int LOD) => MeshGenerator.GenerateTerrainMeshData(noiseMap.map, heightMultiplier, meshHeightCurve, LOD);

    public float GetHeight(MapChunk chunk, in Vector2 position)
    {
        return meshHeightCurve.Evaluate(chunk.noiseMap.map[(int)position.x, (int)position.y]) * heightMultiplier;
    }
    public float GetHeight(in float noiseMapValue)
    {
        return meshHeightCurve.Evaluate(noiseMapValue) * heightMultiplier;
    }

    //appelez lorsque une variable est changé
    private void OnValidate()
    {
        lacunarity = Mathf.Max(1f, lacunarity);
        octave = Mathf.Max(1, octave);
        minRadius = Mathf.Max(0f, minRadius);
        maxRadius = Mathf.Max(0f, maxRadius);
        useNoiseMapV2 = useCombineNoiseMap ? false : useNoiseMapV2;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0, 1)]
    public float height;
    public Color color;

    public override bool Equals(object obj) => obj.GetType() == typeof(TerrainType) && this == (TerrainType)obj;
    public override int GetHashCode() => base.GetHashCode();
    public static bool operator ==(TerrainType t1, TerrainType t2) => t1.name == t2.name && t1.height == t2.height && t1.color == t2.color;
    public static bool operator !=(TerrainType t1, TerrainType t2) => !(t1 == t2);
}

public class TerrainTypeComparer : IComparer<TerrainType>
{
    public int Compare(TerrainType t1, TerrainType t2)
    {
        if (t1 == default(TerrainType) && t2 == default(TerrainType))
        {
            return 0;
        }
        if (t1 == default(TerrainType))
        {
            return 1;
        }
        if (t2 == default(TerrainType))
        {
            return -1;
        }
        return t1.height >= t2.height ? 1 : -1;
    }
}

[System.Serializable]
public class ForestGeneratorType
{
    public List<GameObject> TreesPrefab;
    public Vector2 heightRange, nbForest, forestRadius, treesPerForest;
}

public class Map
{
    public MapChunk[,] chunks;

    public Map(MapChunk[,] chunks)
    {
        this.chunks = chunks;
    }
}
