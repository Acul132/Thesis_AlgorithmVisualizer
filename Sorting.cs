using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Sorting : MonoBehaviour
{
    //String map to associated dropdown index with correct algorithm name
    private string[] algoMap;

    //Initial variables for the bar display
    [SerializeField] private int numOfBars = 10;
    [SerializeField] private int barOffset = 10;
    [SerializeField] private int maxHeight = 20;
    [SerializeField] private int minHeight = 1;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private string algorithm = "bubble";

    //References to gameobjects
    public Slider numOfBarsSlider;
    public Slider maxHeightSlider;
    public Slider minHeightSlider;
    public Slider delaySlider;
    public TMP_Dropdown algoDropdown;
    public CodeDisplay codeDisplay;
    public Button playPauseButton;

    //Primary arrays for the bar display (values and rendering information)
    private Rect[] barRects;
    private int[] values;
    private ColouredText2D[] barTextures;

    //Flags
    private bool isSorting, isSettingsOpened, paused;

    //Alternative sprites for the play/pause button
    private Sprite playButtonSprite;
    private Sprite pauseButtonSprite;

    //Objects required for rendering
    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;


    public void Start()
    {
        //Initialize all necessary objets
        algoMap = new string[]{"bubble", "selection", "insertion", "merge"};
        barRects = new Rect[numOfBars];
        barTextures = new ColouredText2D[numOfBars];
        values = new int[numOfBars];
        isSorting = false;
        paused = true;
        isSettingsOpened = false;

        //After initialization, display the bars to the screen
        GenerateRandomValues();
        RenderBars();

        //Load in the correct sprites for the play and pause button
        playButtonSprite = Resources.Load<Sprite>("play");
        pauseButtonSprite = Resources.Load<Sprite>("stop");

        //Set the initial values of the sliders in the settings menu
        numOfBarsSlider.value = numOfBars;
        maxHeightSlider.value = maxHeight;
        minHeightSlider.value = minHeight;
    }

    //Public function used to reset or refresh the bars
    public void resetBars()
    {
        barRects = new Rect[numOfBars];
        barTextures = new ColouredText2D[numOfBars];
        values = new int[numOfBars];
        GenerateRandomValues();
        RenderBars();
        isSorting = false;
        paused = true;
        playPauseButton.GetComponent<Image>().sprite = playButtonSprite;
    }

    //Fills the values array with random integer values between the two provided bounds
    private void GenerateRandomValues()
    {
        for(int x = 0; x < numOfBars; x++)
        {
            values[x] = Random.Range(minHeight, maxHeight+1);
        }
    }

    //Unity life cycle method used to display the IMGUI objects (bars)
    //Called once every frame
    private void OnGUI(){
        if (!isSettingsOpened)  //Do not render to the screen if the settings menu is open
        {
            for(int i = 0; i < barRects.Length; i++)
            {
                GUIDrawRect(barRects[i], barTextures[i], values[i]);
            }
        }
    }

    //Draws a single bar to the screen (called once for every bar)
    public void GUIDrawRect( Rect position, ColouredText2D texture, int value )
    { 
        if( _staticRectStyle == null )  //Ensures that a new GUIStyle is not created for each bar
        {
            _staticRectStyle = new GUIStyle();
        }
 
        _staticRectStyle.normal.background = texture.getTexture();
        _staticRectStyle.alignment = TextAnchor.LowerCenter;
        _staticRectStyle.fontSize = 40 - numOfBars;

        //Do not display the numbers if there are 25 or more bars to display
        if(numOfBars < 25)  
            GUI.Box( position, value.ToString() , _staticRectStyle );
        else
            GUI.Box( position, GUIContent.none , _staticRectStyle );
 
    }

    //Method for determining the correct x,y,width, and height values for each bar
    private void RenderBars()
    {
        int width = Screen.width - 20; //-20 for padding
        int height = Screen.height - 20; // -20 for padding

        int barWidth = (width - (numOfBars * barOffset))/numOfBars; 

        for(int i = 0; i < numOfBars; i++)
        {
            //Must convert division to float for floating point arithmetic
            float barHeight = -(int)(height/2 * ((float)values[i] / maxHeight)); 
            int xOffset = (i * (barWidth + barOffset)) + 20;
            int yOffset = (height / 2) + 30;
            barRects[i] = new Rect(xOffset,yOffset,barWidth,barHeight);
            //Only set the colour to white if it is the first isntance created of the bar
            if(barTextures[i] == null)  
                barTextures[i] = new ColouredText2D(Color.white);
        }
    }

    //Handler method for the delay slider
    public void UpdateDelay(){
        delay = delaySlider.value;
    }

    //Handler method for the setting button
    public void SettingsOpened()
    {
        if (isSettingsOpened) //Update from slider values
        {
            if(!algorithm.Equals(algoMap[algoDropdown.value])){
                algorithm = algoMap[algoDropdown.value];
                codeDisplay.ChangeCode(algorithm);  //Update the algorithm being displayed if there is a change in algorithm
            }
            numOfBars = (int)numOfBarsSlider.value;
            maxHeight = (int)maxHeightSlider.value;
            minHeight = (int)minHeightSlider.value;
            resetBars();    //Refresh the bars values every time the settings menu is closed
        }
        isSettingsOpened = !isSettingsOpened;
    }

    //Handle what happens when the space bar or play/pause key is pressed
    public void HandleStartStop(){
        //If an algorithm is not currently being sorted, start sorting the correct one
        if (!isSorting){ 
            isSorting = true;
            if (algorithm == "bubble")
                StartCoroutine(BubbleSort());
            else if(algorithm == "insertion")
                StartCoroutine(InsertionSort());
            else if(algorithm == "selection")
                StartCoroutine(SelectionSort()); 
        }

        paused = !paused;

        if(playPauseButton.image.sprite.name == "play")
            playPauseButton.GetComponent<Image>().sprite = pauseButtonSprite;
        else
            playPauseButton.GetComponent<Image>().sprite = playButtonSprite;
    }

    //Unity life cycle method, used in this instance to check for keyboard input
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            HandleStartStop();
        }
    }

    //Coroutine for the bubble sort algorithm
    //Displays the algorithm whiile checking for pauses and delaying between each step
    IEnumerator BubbleSort()
    {
        for (int p = 0; p <= numOfBars - 2; p++)
        {
            codeDisplay.DisplayStep(0);     //Update current step on the code display by calling a member method 
            yield return new WaitForSecondsRealtime(delay);     //Sleep the coroutine for the provided amount of delay
            while(paused)yield return null;     //Check the paused flag, if true, sleep until it is set to false

            for (int i = 0; i <= numOfBars - 2; i++)
            {
                codeDisplay.DisplayStep(1);
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                //Setting the bars that are currently being selected to green for clarity
                barTextures[i].UpdateColor(Color.green);    
                barTextures[i+1].UpdateColor(Color.green);

                int curVal = values[i];
                int nextVal = values[i + 1];

                codeDisplay.DisplayStep(2);
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                if (curVal > nextVal)
                {
                    int t = nextVal;
                    values[i+1] = curVal;
                    values[i] = t;
                    RenderBars();   //Must call RenderBars() any time there has been an update to the values array 

                    codeDisplay.DisplayStep(3);
                    yield return new WaitForSecondsRealtime(delay);
                    while(paused)yield return null;
                }
                //Reseting the colour of the bars
                barTextures[i].UpdateColor(Color.white);
                barTextures[i+1].UpdateColor(Color.white);
            }
        }
        codeDisplay.ResetDisplay();
    }

    //Coroutine for the insertion sort algorithm
    //Displays the algorithm whiile checking for pauses and delaying between each step
    IEnumerator InsertionSort()
    {
        for (int i = 1; i < values.Length; ++i)
        {
            codeDisplay.DisplayStep(0);
            yield return new WaitForSecondsRealtime(delay);
            while(paused)yield return null;

            int key = values[i];
            int j = i - 1;
            barTextures[i].UpdateColor(Color.green);

            codeDisplay.DisplayStep(new int[]{1,2});
            yield return new WaitForSecondsRealtime(delay);
            while(paused)yield return null;

            while ( j >= 0 && values[j] > key) {

                codeDisplay.DisplayStep(3);
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                barTextures[j].UpdateColor(Color.green);
                values[j + 1] = values[j];
                RenderBars();
                
                j = j-1;

                codeDisplay.DisplayStep(new int[]{4,5});
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                barTextures[j+1].UpdateColor(Color.white);
            }
            
            
            values[j + 1] = key;
            barTextures[i].UpdateColor(Color.white);
            RenderBars();

            codeDisplay.DisplayStep(6);
            yield return new WaitForSecondsRealtime(delay);
            while(paused)yield return null;
        }
        codeDisplay.ResetDisplay();
    }

    //Coroutine for the selection sort algorithm
    //Displays the algorithm whiile checking for pauses and delaying between each step
    IEnumerator SelectionSort()
    {
        for (int i = 0; i < values.Length - 1; i++)
        {
            int minIndex = i;
            barTextures[i].UpdateColor(Color.green);

            codeDisplay.DisplayStep(new int[]{0,1});
            yield return new WaitForSecondsRealtime(delay);
            while(paused)yield return null;

            for(int j = i + 1; j < values.Length; j++){
                codeDisplay.DisplayStep(2);
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                barTextures[j].UpdateColor(Color.green);
                codeDisplay.DisplayStep(3);
                yield return new WaitForSecondsRealtime(delay);
                while(paused)yield return null;

                if(values[j] < values[minIndex]){
                    minIndex = j;

                    codeDisplay.DisplayStep(4);
                    yield return new WaitForSecondsRealtime(delay);
                    while(paused)yield return null;
                }
                barTextures[j].UpdateColor(Color.white);
            }

            barTextures[i].UpdateColor(Color.white);

            int temp = values[minIndex];
            values[minIndex] = values[i];
            values[i] = temp;
            RenderBars();
            

            codeDisplay.DisplayStep(5);
            yield return new WaitForSecondsRealtime(delay);
            while(paused)yield return null;
        }
        codeDisplay.ResetDisplay();
    }
}
