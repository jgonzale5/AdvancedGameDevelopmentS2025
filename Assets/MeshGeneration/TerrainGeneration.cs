using UnityEngine;
using UnityEngine.Rendering;

//We add this to make sure this script cannot be added to a game object without a meshfilter
[RequireComponent(typeof(MeshFilter))]
public class TerrainGeneration : MonoBehaviour
{
    /// <summary>
    /// The number of columns of faces
    /// </summary>
    public int xFaceCount = 1;
    /// <summary>
    /// The number of rows of faces
    /// </summary>
    public int zFaceCount = 1;
    //The maximum height of the terrain
    public float terrainHeight = 2f;
    //
    public float perlinMultiplier = 0.3f;
    //
    public Vector2 perlinOffset = Vector2.zero;

    /// <summary>
    /// The mesh filter that will display this mesh
    /// </summary>
    private MeshFilter meshFilter;
    /// <summary>
    /// The mesh of the terrain
    /// </summary>
    private Mesh mesh;

    /// <summary>
    /// The array of vertices that will make up this surface
    /// </summary>
    private Vector3[] vertices;
    /// <summary>
    /// The array of vertex indices that tells the mesh what are the triangle faces making it up
    /// </summary>
    private int[] triangles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //We get the mesh filter from this object
        meshFilter = GetComponent<MeshFilter>();

        //
        CreateShape();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    private void CreateBasicShape()
    {
        //For testing, we're gonna start by making four points that will make up a quad
        vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1)
        };

        //For testing, we'll start by defining this hard-coded array of integers, each representing an index in the vertices array.
        //This indicates the points that will be used to draw the triangles
        triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3
        };

        //We create the mesh we'll be using
        mesh = new Mesh();
        //Assign the vertices
        mesh.vertices = vertices;
        //Assign the triangles that it will use
        mesh.triangles = triangles;

        //
        mesh.RecalculateNormals();


        //Set it to be the mesh dispalyed by the mesh filter
        meshFilter.mesh = mesh;

    }

    //
    private void CreateShape()
    {
        //For every face we need two vertices. Since most faces share vertices, this means we need 1 more pair of vertices in each direction.
        int vertexCount = (xFaceCount + 1) * (zFaceCount + 1);

        //We create the array of vertices with the length we defined
        vertices = new Vector3[vertexCount];

        int ind = 0;

        //We create the vertices in a grid pattern
        for (int z = 0; z <= zFaceCount; z++)
        {
            for (int x = 0; x <= xFaceCount; x++)
            {
                //We set the y value of each vertex based on the perlin noise, using the height, offet, and multiplier to control the way in which it behaves
                float y = Mathf.PerlinNoise(
                    perlinOffset.x + x * perlinMultiplier,
                    perlinOffset.y + z * perlinMultiplier)
                    * terrainHeight;

                //We create a new vertex based on the value of x and z
                vertices[ind] = new Vector3(x,y,z);
                //We increase ind to move to the next index
                ind++;
            }
        }

        //For each square, there are two triangles. Since each triangle requires three vertices, each face increases the number of items in triangles by 6.
        triangles = new int[xFaceCount * zFaceCount * 6];

        //We use this to keep track of where in the grid we are as we populate it with faces
        int vert = 0;
        int tris = 0;

        //For each row of faces in z
        for (int z = 0; z < zFaceCount; z++)
        {
            //For each face in x
            for (int x = 0; x < xFaceCount; x++)
            {
                //This is defining two faces that make up a square
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xFaceCount + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xFaceCount + 1;
                triangles[tris + 5] = vert + xFaceCount + 2;

                //We increase verts at the end, since we're shifting to the next group of four vertices
                vert++;
                //We shift the tris by 6 since we are moving to the next group of six vertices that make up two triangles
                tris += 6;
            }
            vert++;
        }

        //We create the mesh we'll be using
        mesh = new Mesh();
        //Assign the vertices
        mesh.vertices = vertices;
        //Assign the triangles that it will use
        mesh.triangles = triangles;

        //
        mesh.RecalculateNormals();


        //Set it to be the mesh dispalyed by the mesh filter
        meshFilter.mesh = mesh;

    }

    /// <summary>
    /// Will draw the vertices on the scene so we can test them
    /// </summary>
    private void OnDrawGizmos()
    {
        //If the vertices array isn't empty, end the function
        if (vertices == null)
            return;

        //For each vertex, draw a sphere at that location
        foreach (Vector3 v in vertices)
        {
            Gizmos.DrawSphere(v, 0.1f);
        }
    }
}
