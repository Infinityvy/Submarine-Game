using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk
{
    public Vector2Int position;

    public Tile[,] tiles = new Tile[ChunkInfo.size, ChunkInfo.size];

    public ChunkMesh chunkMesh;

    public bool initiated = false;

    public Chunk(Vector2Int pos)
    {
        position = pos;
    }

    /// <summary>
    /// Initiates the chunk mesh for this chunk.
    /// </summary>
    public void init()
    {
        Transform chunkMeshPrefab = Resources.Load<Transform>("ChunkMesh");

        chunkMesh = Transform.Instantiate(chunkMeshPrefab, new Vector3(position.x, position.y, 0) * ChunkInfo.size, Quaternion.identity, WorldLoader.active.transform).GetComponent<ChunkMesh>();
        chunkMesh.init(this);

        updateName();
        initiated = true;
    }

    /// <summary>
    /// Removes the chunk mesh of this chunk.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void fini()
    {
        if (chunkMesh == null) throw new System.Exception("Attempting to destroy ChunkMesh of Chunk that doesn't have a ChunkMesh.");
        chunkMesh.fini();
        chunkMesh = null;

        initiated = false;
    }

    /// <summary>
    /// Updates the chunk mesh objects name to display the current position.
    /// </summary>
    public void updateName()
    {
        chunkMesh.name = "ChunkMesh: " + position.x + ", " + position.y;
    }
}
