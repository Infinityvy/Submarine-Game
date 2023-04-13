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

    public static int generationDistance = 5;
    public static int halfGenerationDistance = generationDistance / 2;

    public WorldGenerator worldGenerator;

    public List<Chunk> loadedChunksList;

    private Vector2Int centerChunkPos;
    private bool[,] neighbourChunksExistanceLookup;

    void Start()
    {

        active = this;
        centerChunkPos = Vector2Int.zero;

        
        loadedChunksList = new List<Chunk>(generationDistance * generationDistance);

        generateChunks();

        neighbourChunksExistanceLookup = new bool[generationDistance, generationDistance];

        for (int x = 0;x < generationDistance; x++) 
        {
            for (int y = 0; y < generationDistance; y++)
            {
                neighbourChunksExistanceLookup[x, y] = true;
            }
        }

        drawChunks();
    }

    void Update()
    {
        chunkLoading();
    }

    

    private void chunkLoading()
    {
        //determine if a chunkloading operation needs to happen based on the players position
        if(Mathf.Abs(playerTransform.position.x - (centerChunkPos.x + 0.5f) * ChunkInfo.size) > ChunkInfo.size * 0.5f ||
           Mathf.Abs(playerTransform.position.y - (centerChunkPos.y + 0.5f) * ChunkInfo.size) > ChunkInfo.size * 0.5f)
        {
            //calculates the new chunk position of the player
            centerChunkPos = new Vector2Int(Mathf.FloorToInt(playerTransform.position.x / ChunkInfo.size),
                                            Mathf.FloorToInt(playerTransform.position.y / ChunkInfo.size));

            //unloads chunks that are too far away
            for (int i = loadedChunksList.Count - 1; i >= 0; i--)
            {
                if (Mathf.Abs(loadedChunksList[i].position.x - centerChunkPos.x) > halfGenerationDistance ||
                    Mathf.Abs(loadedChunksList[i].position.y - centerChunkPos.y) > halfGenerationDistance)
                {
                    if (loadedChunksList[i].initiated) loadedChunksList[i].fini();
                    loadedChunksList.RemoveAt(i);
                }
            }

            //refresh the neighbourChunksExistanceLookup matrix
            neighbourChunksExistanceLookup = new bool[generationDistance, generationDistance];

            for (int i = 0; i < loadedChunksList.Count; i++)
            {
                int x = loadedChunksList[i].position.x - centerChunkPos.x + halfGenerationDistance;
                int y = loadedChunksList[i].position.y - centerChunkPos.y + halfGenerationDistance;

                neighbourChunksExistanceLookup[x, y] = true;

                
            }

            //generate missing chunks that should exist
            for (int x = 0; x < generationDistance; x++)
            {
                for (int y = 0; y < generationDistance; y++)
                {
                    if(!neighbourChunksExistanceLookup[x, y])
                    {
                        Chunk newChunk = worldGenerator.generateChunk(new Vector2Int(x - halfGenerationDistance, y - halfGenerationDistance) + centerChunkPos);
                        loadedChunksList.Add(newChunk);
                        neighbourChunksExistanceLookup[x, y] = true;
                    }
                }
            }


            foreach (Chunk ch in loadedChunksList)
            {
                //initiate the chunk mesh if chunk is not already initiated and the chunk is not an edge chunk
                if (!ch.initiated && !(Mathf.Abs(ch.position.x - centerChunkPos.x) == halfGenerationDistance ||
                Mathf.Abs(ch.position.y - centerChunkPos.y) == halfGenerationDistance)) ch.init();
            }
        }
    }

    private void generateChunks()
    {
        Vector2Int chunkPos = Vector2Int.zero;


        for (chunkPos.x = -halfGenerationDistance; chunkPos.x < generationDistance * 0.5f; chunkPos.x++)
        {
            for (chunkPos.y = -halfGenerationDistance; chunkPos.y < generationDistance * 0.5f; chunkPos.y++)
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

        loadedChunksList = new List<Chunk>(generationDistance * generationDistance);

        generateChunks();

        drawChunks();
    }

    public void drawChunks()
    {
        foreach (Chunk ch in loadedChunksList)
        {
            if (!(Mathf.Abs(ch.position.x - centerChunkPos.x) == halfGenerationDistance ||
                Mathf.Abs(ch.position.y - centerChunkPos.y) == halfGenerationDistance)) ch.init();
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            foreach (Chunk ch in loadedChunksList)
            {
                Gizmos.DrawWireCube((Vector3)(Vector2)(ch.position * ChunkInfo.size) + new Vector3(ChunkInfo.size / 2, ChunkInfo.size / 2, 0), 
                                    new Vector3(ChunkInfo.size - 2, ChunkInfo.size - 2, 0));
            }
        }
    }
}
