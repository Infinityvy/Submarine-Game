using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public Transform chunkMeshPrefab;

    public float noiseScale = 10;

    public float trenchWidth = 100;
    public float trenchChangeFac = 0.05f;

    public float maxOffset = 20;
    public int seed = 0;
    public float gap = 10;

    public Vector2 offset = Vector2.zero;


    //private Vector2 noiseDefaultOffset = new Vector2(WorldLoader.renderDistance * ChunkInfo.size, WorldLoader.renderDistance * ChunkInfo.size);

    private void Awake()
    {
        //offset = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)) * 0.01f;
    }

    public Chunk generateChunk(Vector2Int chunkPos)
    {
        Chunk chunk = new Chunk(chunkPos);

        Vector2Int localPos = Vector2Int.zero;

        for (localPos.x = 0; localPos.x < ChunkInfo.size; localPos.x++)
        {
            for (localPos.y = 0; localPos.y < ChunkInfo.size; localPos.y++)
            {


                if (isTile(chunkPos * ChunkInfo.size + localPos))
                {
                    chunk.tiles[localPos.x, localPos.y] = new Tile_Rock();
                }
                else chunk.tiles[localPos.x, localPos.y] = new Tile_Empty();

            }
        }

        return chunk;
    }

    private bool isTile(Vector2Int tilePos)
    {

        Vector2 tilePos01 = ((Vector2)tilePos) * noiseScale / WorldLoader.generationDistance / ChunkInfo.size + offset;



        //float noiseValueOffset0 = Mathf.PerlinNoise(tilePos01.x + offset.x, tilePos01.y + offset.y);
        //float noiseValueOffset1 = Mathf.PerlinNoise(tilePos01.x + offset.x + 100, tilePos01.y + offset.y + 100);

        double noiseValueOffset0 = PerlinNoise.perlin(tilePos01.x + offset.x, tilePos01.y + offset.y, 0);
        double noiseValueOffset1 = PerlinNoise.perlin(tilePos01.x + offset.x + 100, tilePos01.y + offset.y + 100, 0);


        float trenchValueL = DonkNoise.donk(tilePos.y + offset.y, seed, gap, maxOffset) - trenchWidth - (int)Mathf.Pow((tilePos.y + 500) * trenchChangeFac, 2);
        float trenchValueR = DonkNoise.donk(tilePos.y + offset.y, seed + 1, gap, maxOffset) + trenchWidth + (int)Mathf.Pow((tilePos.y + 500) * trenchChangeFac, 2);

        return (trenchValueL > tilePos.x || trenchValueR < tilePos.x) &&
               (noiseValueOffset0 > 0.5f || 1 - noiseValueOffset1 > 0.5f);
    }

    public void testNoise()
    {
        for (int i = 0; i < 20; i++)
        {
            //Debug.Log("For i = " + i + " noise = " + DonkNoise.donk(i, seed, gap, maxOffset));
            
        }

        Debug.Log(".1: " + Mathf.PerlinNoise(0.1f, 0.1f));
        Debug.Log(".2: " + Mathf.PerlinNoise(0.2f, 0.2f));
        Debug.Log(".3: " + Mathf.PerlinNoise(0.3f, 0.3f));
        Debug.Log(".4: " + Mathf.PerlinNoise(0.4f, 0.4f));
    }
}
