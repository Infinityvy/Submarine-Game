using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldLoader : MonoBehaviour
{
    public static WorldLoader active;

    public Transform playerTransform;

    public static int renderDistance = 3;
    public static int halfRenderDistance = renderDistance / 2;

    public WorldGenerator worldGenerator;

    public List<Chunk> loadedChunksList;

    private Vector2Int centerChunkPos;
    private bool[,] neighbourChunksExistanceLookup;

    void Start()
    {

        active = this;
        centerChunkPos = Vector2Int.zero;

        
        loadedChunksList = new List<Chunk>(renderDistance * renderDistance);

        generateChunks();

        neighbourChunksExistanceLookup = new bool[renderDistance, renderDistance];

        for (int x = 0;x < renderDistance; x++) 
        {
            for (int y = 0; y < renderDistance; y++)
            {
                neighbourChunksExistanceLookup[x, y] = true;
            }
        }

        worldGenerator.drawChunks();
    }

    void Update()
    {
        chunkLoading();
    }

    

    private void chunkLoading()
    {
        if(Mathf.Abs(playerTransform.position.x - (centerChunkPos.x + 0.5f) * ChunkInfo.size) > ChunkInfo.size * 0.5f ||
           Mathf.Abs(playerTransform.position.y - (centerChunkPos.y + 0.5f) * ChunkInfo.size) > ChunkInfo.size * 0.5f)
        {
            centerChunkPos = new Vector2Int(Mathf.FloorToInt(playerTransform.position.x / ChunkInfo.size),
                                            Mathf.FloorToInt(playerTransform.position.y / ChunkInfo.size));

            for (int i = loadedChunksList.Count - 1; i >= 0; i--)
            {
                if (Mathf.Abs(loadedChunksList[i].position.x - centerChunkPos.x) > halfRenderDistance ||
                    Mathf.Abs(loadedChunksList[i].position.y - centerChunkPos.y) > halfRenderDistance)
                {
                    loadedChunksList[i].fini();
                    loadedChunksList.RemoveAt(i);
                }
            }


            neighbourChunksExistanceLookup = new bool[renderDistance, renderDistance];
            
            for (int i = 0; i < loadedChunksList.Count; i++)
            {
                int x = loadedChunksList[i].position.x - centerChunkPos.x + halfRenderDistance;
                int y = loadedChunksList[i].position.y - centerChunkPos.y + halfRenderDistance;

                neighbourChunksExistanceLookup[x, y] = true;
            }

            for (int x = 0; x < renderDistance; x++)
            {
                for (int y = 0; y < renderDistance; y++)
                {
                    if(!neighbourChunksExistanceLookup[x, y])
                    {
                        Chunk newChunk = worldGenerator.generateChunk(new Vector2Int(x - halfRenderDistance, y - halfRenderDistance) + centerChunkPos);
                        loadedChunksList.Add(newChunk);
                        newChunk.init();
                        neighbourChunksExistanceLookup[x, y] = true;
                    }
                }
            }
            
        }
    }

    private void generateChunks()
    {
        Vector2Int chunkPos = Vector2Int.zero;


        for (chunkPos.x = -halfRenderDistance; chunkPos.x < renderDistance * 0.5f; chunkPos.x++)
        {
            for (chunkPos.y = -halfRenderDistance; chunkPos.y < renderDistance * 0.5f; chunkPos.y++)
            {
                loadedChunksList.Add(worldGenerator.generateChunk(chunkPos));
            }
        }
    }

    public void regenerateChunks()
    {
        foreach (Chunk ch in loadedChunksList)
        {
            ch.fini();
        }

        loadedChunksList = new List<Chunk>(renderDistance * renderDistance);

        generateChunks();

        worldGenerator.drawChunks();
    }

    
}
