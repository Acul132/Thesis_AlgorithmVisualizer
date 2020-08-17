using UnityEngine;
using System.Collections;

//Wrapper on Texture2D that creates the texture and sets the colour to clear upon initialization
public class CodeSegmentTexture
{
    private Texture2D texture;

    public CodeSegmentTexture()
    {
        texture = new Texture2D( 1, 1 );
        texture.SetPixel( 0, 0, Color.clear);
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
