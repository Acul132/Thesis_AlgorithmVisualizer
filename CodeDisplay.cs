using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CodeDisplay : MonoBehaviour
{
    //Dictionary to hold the algorithm segments and allow for
    //easy lookup of an algorithm given its name
    private Dictionary<string,string[]> codeSegments;

    //Pseudocode for bubble sort
    private string[] bubbleCode = {"  for i = 0 to arrayLength-1",
                                   "      for j = 0 to arrayLength-i-1",
                                   "          if leftSide > rightSide",
                                   "              swap(leftSide,rightSide)"};

    //Pseudocode for selection sort
    private string[] selectionCode = {"  for i = 0 to arrayLength-1",
                                   "      minIndex = i",
                                   "      for j = i+1 to arrayLength",
                                   "          if arr[j] < arr[minIndex]",
                                   "              minIndex = j",
                                   "      swap(arr[minIndex],arr[i])"};

    //Pseudocode for insertion sort
    private string[] insertionCode = {"  for i = 1 to arrayLength",
                                   "      key = arr[i]",
                                   "      j = i - 1",
                                   "      while(j >= 0 && arr[i] > key",
                                   "          shiftRight()",
                                   "          j = j - 1",
                                   "      arr[j+1] = key"};

    private string currentSelected; //Currently selected algorithm

    //Relevent references for rendering
    private CodeSegmentTexture[] codeTextures;
    private Rect[] codePositions;
    private Rect containerRect;
    private int currentFontSize;
    private static GUIStyle _staticRectStyle;
    private static Texture2D _staticRectTexture;
    private Color containerColor;

    private bool display;   //Flag to determine when to display the code display
    
    public Font codeFont;   //The font for the code display must be provided

    public void Start()
    {
        display = false;    //Do not display the code display before the values have been loaded

        codeSegments = new Dictionary<string, string[]> //Dictionary is created to reference the appropriate algorithms
        {
            { "bubble", bubbleCode },
            { "selection", selectionCode },
            { "insertion", insertionCode }
        };

        currentSelected = "bubble";
        containerRect = GetContainerRect();
        containerColor = new Color(0.30f,0.30f,0.30f,1.0f);

        InitArrays();
        InitUI();
        display = true;     //Display the code display after all values have been instanciated
    }

    //Unity life cycle method for IMGUI elements
    private void OnGUI(){
        if (display)    //Only render the dislay when the display flag is set to true
        {
            GUIDrawRect(containerRect);
            for(int i = 0; i < codeSegments[currentSelected].Length; i++)
            {
                GUIDrawLabel(codePositions[i], codeTextures[i], codeSegments[currentSelected][i]);
            }
        }
    }

    //Method for rendering the code segements
    public void GUIDrawLabel( Rect position, CodeSegmentTexture texture, string codeLine )
    { 
        if( _staticRectStyle == null )      //This ensures that a GUIStyle does not get created for each code segement as it should be reused
        {
            _staticRectStyle = new GUIStyle();
        }
 
        _staticRectStyle.normal.background = texture.getTexture();
        _staticRectStyle.alignment = TextAnchor.MiddleLeft;
        _staticRectStyle.fontSize = currentFontSize;
        _staticRectStyle.normal.textColor = Color.white;
        _staticRectStyle.font = codeFont;

        GUI.Label( position, codeLine , _staticRectStyle );
    }

    //Method for rendering the container for the code segements
    public void GUIDrawRect( Rect position )
    {
        if( _staticRectTexture == null )
        {
            _staticRectTexture = new Texture2D( 1, 1 );
        }
        if( _staticRectStyle == null )
        {
            _staticRectStyle = new GUIStyle();
        }

        _staticRectTexture.SetPixel( 0, 0, containerColor);
        _staticRectTexture.Apply();
 
        _staticRectStyle.normal.background = _staticRectTexture;

        GUI.Box( position, GUIContent.none , _staticRectStyle);
    }

    //Initializes the arrays that store texture and position data
    private void InitArrays()
	{
        codeTextures = new CodeSegmentTexture[codeSegments[currentSelected].Length];
        codePositions = new Rect[codeSegments[currentSelected].Length];
	}

    //Generate the data required to render to the screen
    private void InitUI()
	{
        currentFontSize = GetFontSize(GetLongestLine());    //Font size must be re-calculated to ensure it will fit inside the display
        GeneratePositions();
        GenerateTextures();
	}

    //Creates the container for the display
    private Rect GetContainerRect()
    {
        float width, xOffset;

        if(Screen.width > 600)  //The display will cover either half or a third of the screen horizontally depending on screenw idth
        {
            width = Screen.width / 3;
            xOffset = width * 2;
        }
        else
        {
            width = Screen.width / 2;
            xOffset = width;
        }

        float height = (Screen.height / 2) * 0.7f;     //The height of the display is always 70% of the room between the bars and the bottom settings bar
        float yOffset = Screen.height / 2 + 40;
        return new Rect(xOffset,yOffset,width,height);
    }

    //Generates the code segements Rect values (position and size)
    private void GeneratePositions()
    {
        display = false;

        float width = containerRect.width;
        float height = containerRect.height / codePositions.Length;
        float xOffset = containerRect.x;

        for(int i = 0; i < codePositions.Length; i++)
        {
            float yOffset = (i * height) + containerRect.y;
            codePositions[i] = new Rect(xOffset,yOffset,width,height);
        }
    }

    //Generates the code segements textures (colour)
    private void GenerateTextures()
    {
        display = false;

        for(int i = 0; i < codeTextures.Length; i++)
        {
            codeTextures[i] = new CodeSegmentTexture();
        }
    }

    //Determines which line in the algorithm has the most amount of characters
    private string GetLongestLine()
    {
        string longestLine = codeSegments[currentSelected][0];
        for(int i = 1; i < codeSegments[currentSelected].Length; i++)
        {
            if(codeSegments[currentSelected][i].Length > longestLine.Length)
                longestLine = codeSegments[currentSelected][i];
        }
        return longestLine;
    }

    //Determines the font size necessary for the display, provided the longest line in the algorithm
    private int GetFontSize(string line)
    {
        int fontSize = 4; 
        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent(line);
        style.fontSize = fontSize;
        style.font = codeFont;

        //Loop until the additional font size is too large for the display (i.e. the previous size will 100% fit)
        while(style.CalcSize(content).x < containerRect.width
            && style.CalcSize(content).y < containerRect.height/codeSegments[currentSelected].Length)
        {
            fontSize += 2;
            style.fontSize = fontSize;
        } 

        return fontSize -= 2;   //Must subtract 2 before returning as the loops final value is 2 units too large
    }

    //Reset the selected code segments (set the background of each code segment to clear)
    public void ResetDisplay()
	{
        for(int i = 0; i < codeTextures.Length; i++)
        {
            codeTextures[i].UpdateColor(Color.clear);
        }
	}

    //public member method used to highlight the provided code segement 
    public void DisplayStep(int stepIndex)
    {
        //Return if step index is out of the array bounds
        if(stepIndex < 0 || stepIndex > codeTextures.Length-1)
            return;

        for(int i = 0; i < codeTextures.Length; i++)
        {
            if(i == stepIndex)
                codeTextures[i].UpdateColor(new Color(0.56f,0.92f,0.46f,0.4f));
            else
                codeTextures[i].UpdateColor(Color.clear);
        }
    }

    //public member method used to highlight the provided code segements
    public void DisplayStep(int[] stepIndices)
    {
        for(int i = 0; i < codeTextures.Length; i++)
        {
            if(stepIndices.Contains(i))
                codeTextures[i].UpdateColor(new Color(0.56f,0.92f,0.46f,0.4f));
            else
                codeTextures[i].UpdateColor(Color.clear);
        }
    }

    //public member method used to change which algorithm is currently selected by the code display 
    public void ChangeCode(string algorithm)
	{
        //Check to ensure that the provided key is a valid option
        if(!codeSegments.ContainsKey(algorithm))
            return;

        currentSelected = algorithm;
        InitArrays();
        InitUI();
	}

    //public member method used to toggle the display on and off (used when entering the settings menu for example)
    public void ToggleDisplay()
    {
        ResetDisplay();
        display = !display;
    }
}
