using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesh : MonoBehaviour
{
    private Chunk chunk;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangles;
    private int vertexIndex = 0;
    private List<Vector2> uvs;

    private Chunk[] neighbouringChunks = new Chunk[4];

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void init(Chunk chunk)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        this.chunk = chunk;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider= GetComponent<MeshCollider>();
        mesh = new Mesh();

        generateMesh();

        meshCollider.sharedMesh = mesh;
    }

    public void fini()
    {
        GameObject.Destroy(gameObject);
    }

    private void generateMesh()
    {
        //fitlering out neighbouring chunks; saved to array in order up, right, down, left
        List<Chunk> loadedChunkList = WorldLoader.active.loadedChunksList;

        for (int i = 0; i < loadedChunkList.Count; i++)
        {
            if (loadedChunkList[i].position.x == chunk.position.x)
            {
                if (Mathf.Abs(loadedChunkList[i].position.y - chunk.position.y) == 1)
                {
                    if (loadedChunkList[i].position.y > chunk.position.y) neighbouringChunks[0] = loadedChunkList[i];
                    else neighbouringChunks[2] = loadedChunkList[i]; 
                }
            }
            else if (loadedChunkList[i].position.y == chunk.position.y)
            {
                if(Mathf.Abs(loadedChunkList[i].position.x - chunk.position.x) == 1)
                {
                    if (loadedChunkList[i].position.x > chunk.position.x) neighbouringChunks[1] = loadedChunkList[i];
                    else neighbouringChunks[3] = loadedChunkList[i];
                }
            }
        }

        //iterating over chunk tiles to create mesh
        for (int x = 0; x < ChunkInfo.size; x++)
        {
            for (int y = 0; y < ChunkInfo.size; y++)
            {
                TileMeshType meshType = chunk.tiles[x, y].meshType;

                if(!(chunk.tiles[x, y] is Tile_Empty)) meshType = calcMeshType(meshType, x, y);

                for (int i = 0; i < meshType.vertices.Length; i++)
                {
                    vertices.Add(meshType.vertices[i] + new Vector3(x, y, 0));
                }

                for (int i = 0; i < meshType.triangles.Length; i++)
                {
                    triangles.Add(vertexIndex + meshType.triangles[i]);
                }

                vertexIndex += meshType.vertices.Length;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();


        for (int i = 0; i < vertices.Count; i++)
        {
            switch(i % 4)  
            {
                case 0:
                    uvs.Add(new Vector2(0, 0));
                    break;
                case 1:
                    uvs.Add(new Vector2(1, 0));
                    break;
                case 2:
                    uvs.Add(new Vector2(0, 1));
                    break;
                case 3:
                    uvs.Add(new Vector2(1, 1));
                    break;
            }
        }

        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        

        //for (int i = 0;i < vertices.Count / 4; i++)
        //{

        //}

        meshFilter.mesh = mesh;
    }

    private TileMeshType calcMeshType(TileMeshType meshType, int x, int y)
    {
        bool upEmpty = false;
        bool downEmpty = false;

        bool rightEmpty = false;
        bool leftEmpty = false;


        //check upEmpty
        if (y + 1 < ChunkInfo.size && chunk.tiles[x, y + 1] is Tile_Empty ||
           y + 1 == ChunkInfo.size && neighbouringChunks[0] != null && neighbouringChunks[0].tiles[x, 0] is Tile_Empty) upEmpty = true;

        //check downEmpty
        if (y > 0 && chunk.tiles[x, y - 1] is Tile_Empty ||
           y == 0 && neighbouringChunks[2] != null && neighbouringChunks[2].tiles[x, ChunkInfo.size - 1] is Tile_Empty) downEmpty = true;

        //check rightEmpty
        if (x + 1 < ChunkInfo.size && chunk.tiles[x + 1, y] is Tile_Empty ||
            x + 1 == ChunkInfo.size && neighbouringChunks[1] != null && neighbouringChunks[1].tiles[0, y] is Tile_Empty) rightEmpty = true;

        //check leftEmpty
        if (x > 0 && chunk.tiles[x - 1, y] is Tile_Empty ||
            x == 0 && neighbouringChunks[3] != null && neighbouringChunks[3].tiles[ChunkInfo.size - 1, y] is Tile_Empty) leftEmpty = true;



        int truthCount = MySystem.truths(upEmpty, downEmpty, leftEmpty, rightEmpty);


        //checks for square meshes, handles them and returns early since no other operations are needed
        if(truthCount == 1)
        {
            if (upEmpty) meshType.addWallFace(Orientation.up);
            else if (rightEmpty) meshType.addWallFace(Orientation.right);
            else if (downEmpty) meshType.addWallFace(Orientation.down);
            else if (leftEmpty) meshType.addWallFace(Orientation.left);

            return meshType;
        }
        else if (truthCount == 4)
        {
            meshType.addWallFace(Orientation.up);
            meshType.addWallFace(Orientation.right);
            meshType.addWallFace(Orientation.down);
            meshType.addWallFace(Orientation.left);

            return meshType;
        }

        //checks what polygon variant is being used (half quad or quater quad)
        int polygonVariant = 0;

        if (truthCount > 2)
        {
            meshType = TileMeshType.spike(); //quater quad polygon (spike)
            polygonVariant = 2;
        }
        else if ((upEmpty || downEmpty) && (rightEmpty || leftEmpty)) //from this point on truthCount must be 2
        {
            meshType = TileMeshType.slope(); //half quad polygon
            polygonVariant = 1;
        }
        else if(upEmpty && downEmpty) //square tile with no block above & below; we can return after this since no further operations required
        {
            meshType.addWallFace(Orientation.up);
            meshType.addWallFace(Orientation.down);

            return meshType;
        }
        else if(leftEmpty && rightEmpty) //square tile with no block left & right; we can return after this since no further operations required
        {
            meshType.addWallFace(Orientation.left);
            meshType.addWallFace(Orientation.right);

            return meshType;
        }


        //sets rotation of the polygon meshes
        if (polygonVariant == 1) //half quad polygon
        {
            if (upEmpty && leftEmpty) meshType.setOrientation(Orientation.left);
            else if (upEmpty && rightEmpty) meshType.setOrientation(Orientation.up);
            else if (downEmpty && rightEmpty) meshType.setOrientation(Orientation.right);
            else if (downEmpty && leftEmpty) meshType.setOrientation(Orientation.down);
        }
        else if(polygonVariant == 2) //quater quad polygon (spike)
        {
            if (!downEmpty) meshType.setOrientation(Orientation.up);
            else if (!leftEmpty) meshType.setOrientation(Orientation.right);
            else if (!upEmpty) meshType.setOrientation(Orientation.down);
            else if (!rightEmpty) meshType.setOrientation(Orientation.left);
        }

        return meshType;
    }
}
