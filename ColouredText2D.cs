using UnityEngine;
using System.Collections;

//Wrapper on Texture2D that creates the texture and sets the provided colour upon initialization
public class ColouredText2D
{
    private Texture2D texture;

    public ColouredText2D(Color color)
    {
        texture = new Texture2D( 1, 1 );
        texture.SetPixel( 0, 0, color );
        texture.Apply();
    }

    public Texture2D getTexture()
    {
        return texture;
    }

    public void UpdateColor(Color color)
	{
        texture.SetPixel( 0, 0, color );
        texture.Apply();
	}
}
