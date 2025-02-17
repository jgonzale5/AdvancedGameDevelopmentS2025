using UnityEngine;

public class NoiseTerrainGeneration : MonoBehaviour
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
    public Texture2D noiseMap;
    //
    public Vector2 tiling = Vector2.one;
    //
    public Vector2 offset = Vector2.zero;

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
    //
    private Color[] colors;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //We get the mesh filter from this object
        meshFilter = GetComponent<MeshFilter>();

        //
        CreateShape();
    }

    //
    private void CreateShape()
    {
        //For every face we need two vertices. Since most faces share vertices, this means we need 1 more pair of vertices in each direction.
        int vertexCount = (xFaceCount + 1) * (zFaceCount + 1);

        //We create the array of vertices with the length we defined
        vertices = new Vector3[vertexCount];
        //
        colors = new Color[vertexCount];

        int ind = 0;

        //We create the vertices in a grid pattern
        for (int z = 0; z <= zFaceCount; z++)
        {
            for (int x = 0; x <= xFaceCount; x++)
            {
                //
                float y = GetHeightFromPixel(this.noiseMap, x, z);

                //We create a new vertex based on the value of x and z
                vertices[ind] = new Vector3(x, y, z);

                //
                colors[ind] = GetColorFromPixel(this.noiseMap, x, z);

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

    /// <summary>
    /// Returns what the height should be at the specified pixel
    /// </summary>
    /// <param name="noiseMap"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private float GetHeightFromPixel(Texture2D noiseMap, int x, int y)
    {
        //We calculate what the position of the pixel is using the settings for tiling and offset
        Vector2Int pixelPos = new Vector2Int(
            Mathf.RoundToInt(offset.x + x * tiling.x) % noiseMap.width,
            Mathf.RoundToInt(offset.y + y * tiling.y) % noiseMap.height
            );

        //Get the pixel
        Color currentPixel = noiseMap.GetPixel(pixelPos.x, pixelPos.y);

        //Get the value between A and B based on the value of this pixel
        float height = Mathf.Lerp(0, terrainHeight, currentPixel.grayscale);

        //
        return height;
    }

    //
    private Color GetColorFromPixel(Texture2D noiseMap, int x, int y)
    {

        //We calculate what the position of the pixel is using the settings for tiling and offset
        Vector2Int pixelPos = new Vector2Int(
            Mathf.RoundToInt(offset.x + x * tiling.x) % noiseMap.width,
            Mathf.RoundToInt(offset.y + y * tiling.y) % noiseMap.height
            );

        return noiseMap.GetPixel(pixelPos.x, pixelPos.y);
    }
}
