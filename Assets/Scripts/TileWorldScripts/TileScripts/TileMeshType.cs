using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMeshType
{
    private static Vector3 offsetVec = new Vector3(0.5f, 0.5f, 0);


#region Presets

    private static TileMeshType squarePreset = new TileMeshType(new Vector3[] 
    { 
        Vector3.zero, Vector3.up, Vector3.right, new Vector3(1, 1, 0)
    }, 
        new int[] 
        { 
            0, 1, 2, 
            3, 2, 1 
        }, new Vector2[] { });
    
    private static TileMeshType slopePeset = new TileMeshType(new Vector3[] 
    { 
        Vector3.zero, Vector3.up, Vector3.right, //front face
        Vector3.zero, Vector3.up, Vector3.right, new Vector3(1, 0, ChunkInfo.thickness), new Vector3(0, 1, ChunkInfo.thickness) //wall
    }, 
        new int[] 
        { 
            0, 1, 2, //front face
            5, 7, 6, //wall
            4, 7, 5  //wall
        }, new Vector2[] { });

    private static TileMeshType emptyPreset = new TileMeshType(new Vector3[] { }, new int[] { }, new Vector2[] { });
   
    private static TileMeshType spikePeset = new TileMeshType(new Vector3[] 
    { 
        Vector3.zero, new Vector3(0.5f, 0.5f, 0), Vector3.right, //front face
        Vector3.zero, new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, ChunkInfo.thickness), new Vector3(0.5f, 0.5f, ChunkInfo.thickness), //left wall
        new Vector3(0.5f, 0.5f, 0), Vector3.right, new Vector3(0.5f, 0.5f, ChunkInfo.thickness), new Vector3(1, 0, ChunkInfo.thickness)  //right wall
    }, 
        new int[] 
        { 
            0, 1, 2, //front face
            3, 5, 4, //left wall
            5, 6, 4, //left wall
            7, 9, 8, //right wall
            9, 10, 8  //right wall
        }, new Vector2[] { });



    public static TileMeshType square() { return Clone(squarePreset); }
    public static TileMeshType slope() { return Clone(slopePeset); }
    public static TileMeshType empty() { return Clone(emptyPreset); }
    public static TileMeshType spike() { return Clone(spikePeset); }

#endregion

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;


    public TileMeshType(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public void setOrientation(Orientation orientation)
    {
        Quaternion rot = Quaternion.Euler(0, 0, -90 * ((int)orientation));

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rot * (vertices[i] - offsetVec) + offsetVec;
        }
    }

    public void addWallFace(Orientation orientation)
    {
        List<Vector3> vertsList = new List<Vector3>();
        vertsList.AddRange(vertices);
        int vertIndex = vertices.Length;

        List<int> trisList = new List<int>();
        trisList.AddRange(triangles);

        Vector3[] vertsTmp = new Vector3[] { Vector3.up, new Vector3(1, 1, 0), new Vector3(0, 1, ChunkInfo.thickness), new Vector3(1, 1, ChunkInfo.thickness) };
        int[] trisTmp = new int[] 
        { 
            vertIndex + 0, 
            vertIndex + 2, 
            vertIndex + 1,

            vertIndex + 3, 
            vertIndex + 1, 
            vertIndex + 2 
        };

        Quaternion rot = Quaternion.Euler(0, 0, -90 * ((int)orientation));

        for (int i = 0; i < vertsTmp.Length; i++)
        {
            vertsTmp[i] = rot * (vertsTmp[i] - offsetVec) + offsetVec;
        }

        vertsList.AddRange(vertsTmp);
        trisList.AddRange(trisTmp);

        vertices = vertsList.ToArray();
        triangles = trisList.ToArray();
    }

    private static TileMeshType Clone(TileMeshType tmt)
    {
        return new TileMeshType((Vector3[])tmt.vertices.Clone(), (int[])tmt.triangles.Clone(), (Vector2[])tmt.uvs.Clone());
    }
}
