using UnityEngine;

public class NoisePixelTerrainGeneration : MonoBehaviour
{
    //The texture we're using as the map
    public Texture2D picture;
    //The maximum height of this surface
    public float maxHeight = 10f;
    //The minimum height that any given block may have
    public float minHeight = 1f;

    //How many instance of the map can fit in the surface
    public Vector2 tiling = Vector2.one;
    //Offset, in pixels, for the noise map
    public Vector2 offset = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrintPicture();
    }

    //Takes the image and sets the height of each pixel according to the value of it
    private void PrintPicture()
    {
        //For each column
        for (int x = 0; x < picture.width; x++)
        {
            //For each row
            for (int z = 0; z < picture.height; z++)
            {
                //We calculate what the position of the pixel is using the settings for tiling and offset
                Vector2Int pixelPos = new Vector2Int(
                    Mathf.RoundToInt(offset.x + x * tiling.x) % picture.width, 
                    Mathf.RoundToInt(offset.y + z * tiling.y) % picture.height
                    );

                //Get the pixel
                Color currentPixel = picture.GetPixel(pixelPos.x, pixelPos.y);

                //We create a 1x1 cube in the scene
                Transform voxel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;

                //Get the value between A and B based on the value of this pixel
                float height = Mathf.Lerp(minHeight, maxHeight, currentPixel.grayscale);
                //Debug.Log(height);

                //We calculate the position by taking the position of this object as the bottom-left corner of the image.
                //We assume the cubes are 1x1 (they are), if we were to use cubes of smaller or bigger dimensions we would need to multiply the offset.
                Vector3 position = this.transform.position + new Vector3(x, height, z);

                //Move the voxel to the calculated position
                voxel.position = position;
                //Parent the voxel to this object
                voxel.SetParent(this.transform);

                //Set the color of the voxel to the same color as the pixel
                voxel.GetComponent<MeshRenderer>().material.color = currentPixel;

                //Scale the pixel so it isn't floating in the air
                ScaleUp(
                    new Vector3(voxel.position.x, this.transform.position.y, voxel.position.z),
                    voxel.position,
                    voxel);
            }
        }
    }

    //
    private void ScaleUp(Vector3 bottom, Vector3 top, Transform obj)
    {
        //We calculate what the final scale should be by getting the distance between both points
        float finalScale = (top - bottom).magnitude;
        //The pivot position should be the midpoint between the bottom and the top position
        Vector3 pivotPos = new Vector3(
            (top.x + bottom.x) / 2,
            (top.y + bottom.y) / 2,
            (top.z + bottom.z) / 2
            );

        //The object is moved to its pivot position
        obj.position = pivotPos;
        //The scale is set to match the final scale we calculated
        obj.localScale = new Vector3(obj.localScale.x, finalScale, obj.localScale.y);
    }
}
