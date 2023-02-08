using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="lstTrees"></param>
    /// <param name="heightRange">x : min de hauteurs de forets, y : max de hauteur des forets</param>
    /// <param name="nbForest">x : le nb min de foret, y : le nb max de foret</param>
    /// <param name="forestRadius">x : le rayon min des forets, y : le rayon max des forets</param>
    /// <param name="treesPerForest">x : le nb min d'arbres par foret, y : le  nb max d'arbres par foret</param>
    public void GenerateForest(Map map, MapDisplay mapDisplay, List<GameObject> lstTrees, in Vector2 heightRange, in Vector2 nbForest, in Vector2 forestRadius, in Vector2 treesPerForest)
    {
        int w = map.chunks.GetLength(0);
        int h = map.chunks.GetLength(1);
        float offsetX = -((w - 1) / 2f) * MapGenerator.mapChunkSize - (MapGenerator.mapChunkSize / 2f) * 0f;
        float offsetY = ((h - 1) / 2f) * MapGenerator.mapChunkSize - (MapGenerator.mapChunkSize / 2f) * 0f;

        int nbForestRand = Random.Rand((int)nbForest.x, (int)nbForest.y);
        for (int _ = 0; _ < nbForestRand; _++)
        {
            bool pointAvailable = false;
            MapChunk mapChunk = null;
            int xChunck = 0, yChunck = 0;
            Vector2 point = Vector2.zero;//position du center de la foret dans le repère du chunk

            do
            {
                xChunck = Random.Rand(0, w - 1);
                yChunck = Random.Rand(0, h - 1);

                mapChunk = map.chunks[xChunck, yChunck];
                point.x = Random.RandExclude(0, MapGenerator.mapChunkSize);
                point.y = Random.RandExclude(0, MapGenerator.mapChunkSize);
                pointAvailable = mapChunk.noiseMap.map[(int)point.x, (int)point.y] >= heightRange.x && mapChunk.noiseMap.map[(int)point.x, (int)point.y] <= heightRange.y;

            } while (!pointAvailable);
            //Debug.Log(point);

            float noiseMapValue = mapChunk.noiseMap.map[(int)point.x, (int)point.y];

            //Mon point est valide
            int nbTrees = Random.Rand((int)treesPerForest.x, (int)treesPerForest.y);
            float radius = Random.Rand(forestRadius.x, forestRadius.y);
            GameObject chunk = mapDisplay.chunks[xChunck, yChunck];
            for (int __ = 0; __ < nbTrees; __++)
            {
                GameObject tree = lstTrees[Random.RandExclude(0, lstTrees.Count)];
                Vector3 treePos = chunk.transform.position - new Vector3(MapGenerator.mapChunkSize / 2f, 0f, MapGenerator.mapChunkSize / 2f);
                Vector2 randVec = Random.RandomVector2(0f, radius);
                treePos += new Vector3(point.x + randVec.x, 0f, point.y + randVec.y);
                treePos.y = MapGenerator.instance.GetHeight(mapChunk, point + randVec);
                
                Instantiate(tree, treePos, Quaternion.identity, chunk.transform);
            }
        }
    }
}
