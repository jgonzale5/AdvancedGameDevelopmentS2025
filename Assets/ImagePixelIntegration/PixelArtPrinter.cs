using Unity.Mathematics;
using UnityEngine;

public class PixelArtPrinter : MonoBehaviour
{
    //
    public Texture2D picture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrintPicture();
    }

    //
    private void PrintPicture()
    {
        //For each column
        for (int x = 0; x < picture.width; x++)
        {
            //For each row
            for (int z = 0; z < picture.height; z++)
            {
                //Get the pixel
                Color currentPixel = picture.GetPixel(x, z);

                //We create a 1x1 cube in the scene
                Transform voxel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;

                //We calculate the position by taking the position of this object as the bottom-left corner of the image.
                //We assume the cubes are 1x1 (they are), if we were to use cubes of smaller or bigger dimensions we would need to multiply the offset.
                Vector3 position = this.transform.position + new Vector3(x, 0, z);

                //Move the voxel to the calculated position
                voxel.position = position;
                //Parent the voxel to this object
                voxel.SetParent(this.transform);

                //Set the color of the voxel to the same color as the pixel
                voxel.GetComponent<MeshRenderer>().material.color = currentPixel;
            }
        }
    }
}
