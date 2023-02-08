using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    //la texture du plan pour afficher la noiseMap
    public Renderer textureRenderer;

    public GameObject defaultChunk;
    public Transform meshParent;

    public Material material;
    private List<Material> lstMat = new List<Material>();
    private List<Mesh> lstMesh = new List<Mesh>();

    //La grande Map
    public GameObject[,] chunks;

    //dessiné la noiseMap ou la colorMap sur le plane
    public void DrawTexture(Texture2D texture)
    {
        //valide mais material n'est utilisé qu'au runtime
        //textureRenderer.material.mainTexture = texture;

        RemoveOldGameObjects();
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
    }

    public void DrawMapTexture(List<Texture2D> textures, in int mapWidth, in int mapHeight)//col et ligne
    {
        //Pour eviter les milliards de planes dans la hirarchie
        RemoveOldGameObjects();

        lstMat.Clear();
        float offsetX =  ((mapWidth - 1) / 2f) * MapGenerator.mapChunkSize;
        float offsetY = ((mapHeight - 1) / 2f) * MapGenerator.mapChunkSize;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = new Vector3(-x * MapGenerator.mapChunkSize + offsetX, 0f, -y * MapGenerator.mapChunkSize + offsetY);
                plane.transform.localScale = new Vector3(MapGenerator.mapChunkSize / 10f, 1f, MapGenerator.mapChunkSize / 10f);
                lstMat.Add(new Material(material));
                lstMat[y * mapWidth + x].mainTexture = textures[y * mapWidth + x];
                plane.GetComponent<Renderer>().sharedMaterial = lstMat[y * mapWidth + x];
                plane.transform.parent = transform;
            }
        }
    }

    public void DrawMeshes(MapChunk[,] mapChunk)
    {
        //on use sharedmesh et sharedMaterial car mesh et material sont dispo que au runtime.
        //meshFilter.sharedMesh = meshData.CreatMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
        //Pour eviter les milliards de mesh dans la hirarchie
        RemoveOldGameObjects();

        int w = mapChunk.GetLength(0);
        int h = mapChunk.GetLength(1);
        chunks = new GameObject[w, h];
        lstMesh.Clear();
        lstMat.Clear();
        float offsetX = -((w - 1) / 2f) * MapGenerator.mapChunkSize;
        float offsetY = ((h - 1) / 2f) * MapGenerator.mapChunkSize;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                GameObject chunk = Instantiate(defaultChunk, meshParent);
                chunks[x, y] = chunk;
                chunk.transform.tag = "Environement";
                chunk.transform.position = new Vector3(x * (MapGenerator.mapChunkSize - 1f) + offsetX, 0f, -y * (MapGenerator.mapChunkSize - 1f) + offsetY);
                MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = chunk.GetComponent<MeshRenderer>();
                lstMesh.Add(mapChunk[x, y].meshData.CreatMesh());
                meshFilter.sharedMesh = lstMesh[y * w + x];
                lstMat.Add(new Material(material));
                lstMat[y * w + x].mainTexture = mapChunk[x, y].texture;
                meshRenderer.sharedMaterial = lstMat[y * w + x];
                chunk.AddComponent<MeshCollider>();
            }
        }
    }

    private void RemoveOldGameObjects()
    {
        List<GameObject> goToRemove = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            goToRemove.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < meshParent.transform.childCount; i++)
        {
            goToRemove.Add(meshParent.transform.GetChild(i).gameObject);
        }
        for (int i = goToRemove.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(goToRemove[i]);
        }
        goToRemove.Clear();
    }
}
