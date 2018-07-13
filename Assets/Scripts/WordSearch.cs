using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Vectrosity;

public class WordSearch : MonoBehaviour {

//	public bool useDictionary;                      // Should we use the dictionary?
	[HideInInspector]
	public TextAsset dictionary;                    // If useDictionary is true, this is the dictionary that will be used.
	public string[] words;                          // Define words that should show up in puzzle. It's overwritten with dictionary words if useDictionary is true.
//	public int maxWordsInPuzzle;                      // Maximum number of words that will show up in puzzle. Ignored if useDictionary is false.
	public int maxWordLength;                       // Maximum length of the single word that will shop up in crossword. Ignored if useDictionary is false.
	public bool hint;              				// If hint is true, letters of the words listed will be in upper case.

	public int horizontal, vertical;                // defines the size of the grid.
	public float sensibility;                       // How much is the tile sensible on the mouse over.

	public GameObject tile, current;
	public GameObject rootObj;
	
	public GameObject gameHud;

	public GameObject popingLetter;

	public bool paused;
	

	[HideInInspector]
	public bool correct = false;


//	private List<GameObject> tiles = new List<GameObject>();
	private GameObject[] allTiles = new GameObject[121];
	private GameObject temp;
	private int solved = 0;
	public static int totalWords = 0;
	private string[,] wordMatrix;
	private List<string> placedWords = new List<string>();

	private Ray ray_start, ray_move, ray_end;
	private RaycastHit hit;
	private int marker = 0;


	//Tile dimensions
	private Vector3 tileScale;						 //Tile World scale
	private float tileScreenScale;
	private float sideOffset, topOffset;						 // Offset from screen 
	private float spacing;                           // Spacing between the tiles

	//Level Details
	private XmlDocument xmlDoc;
	private TextAsset textXml;

	private int maxWordsInPuzzle;                      // Maximum number of words that will show up in puzzle. Ignored if useDictionary is false.
	List<string> puzzleWordsList = new List<string>();

	//For placement for letters related
	private int []permitted = new int[] {1,1,1,1,1,1,1,1};
	private int []used = new int[1000];
	private int [,]dirs  = new int[8,2] {{1,0},{0,1},{1,1},{1,-1},{-1,0},{0,-1},{-1,1},{-1,-1}};


	//Word Check & Line draw related
	GameObject startTile, potentialEnd;
	Text wordFormed;
	PopLetter popLetterTile;
	int layerMask;

	//Vectrosity Line Draw
	public Material lineMaterial;
	VectorLine borderLine ;
	public VectorLine currentSelectionLine;
	string currentCapName;
	List<VectorLine> selectionLines;
	List<string> endCaps;
	public Texture2D capTex, lineTex;
	List<Color32> lineColors ;
	VectorLine bottomBorder;

	private static WordSearch instance;
	public static WordSearch Instance
	{
		get
		{
			return instance;
		}
	}
	
	void Awake()
	{
		instance = this;
		selectionLines = new List<VectorLine>();
		endCaps = new List<string>();
	}

	void OnDestroy()
	{
//		VectorLine.Destroy(ref bottomBorder);
		for(int i = 0; i<endCaps.Count; i++)
		{
//			Debug.Log("Cap OnDestroy = " + endCaps[i]);
			VectorLine.RemoveEndCap(endCaps[i]);
		}
		VectorLine.Destroy(selectionLines);
		if(currentSelectionLine != null)
		{
//			Debug.Log("Detcroyinjdhdvhdhhdhvhdhj");
			VectorLine.Destroy (ref currentSelectionLine);
		}
//		if(borderLine != null )
//		{
//			VectorLine.Destroy(ref borderLine);
//		}
	}

	// Use this for initialization
	void Start () 
	{
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			maxWordsInPuzzle = 2;
		}
		else
		{
			maxWordsInPuzzle = 9;
		}


		//First of all set up dimension of Letter Tile as per screen size & number of row/columns
		SetDimensionsOfTile ();

		SetDictionaryToBeUsed();

		popLetterTile = popingLetter.GetComponent<PopLetter>();

//		layerMask = 1 << LayerMask.NameToLayer("Puzzle");
//		layerMask = 5;
//		Debug.Log("int layerMask = " + layerMask);

		//Set max lenght of a word
		Mathf.Clamp(maxWordLength, 0, vertical < horizontal ? horizontal : vertical);

		//Check vale for Sensibility
		Mathf.Clamp01(sensibility);

		//Create matrix of required size
		wordMatrix = new string[horizontal, vertical];


		int counter = 0;

		//Get words from dictionary
		if(dictionary != null)
		{
			words = dictionary.text.Split('\n');
		}


		//Shuffle total words available
		Shuffle(words);

		//Add all valid words to a List
		if(words.Length > 0)
		{
			while (puzzleWordsList.Count < maxWordsInPuzzle )
			{
				if (words[counter].Length <= maxWordLength)
				{
					if(!puzzleWordsList.Contains(words[counter]))
					{
						puzzleWordsList.Add(words[counter]);
					}
					else
					{
						//					Debug.Log("GameData.subLevelDictionary");
					}
					
				}
				counter++;
				
				//If less words are there of lenght 11 in file
				if(counter >= words.Length)
				{
					break;
				}
			}
		}

		StartLoadingPuzzleGrid ();

		//Tell GameHud all words of puzzle
		totalWords = placedWords.Count;
		gameHud.SendMessage("AssignPuzzlewords", placedWords );
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			wordFormed = gameHud.GetComponent<GameHud>().wordFormedTutorial;
		}
		else
		{
			wordFormed = gameHud.GetComponent<GameHud>().wordFormedGame;
		}

		wordFormed.text = "";

		                                                                                              
		//Instantiate tiles
		for (int i = 0; i < horizontal; i++)
		{
			for (int j = 0; j < vertical; j++)
			{
				temp = Instantiate(tile, 
				                   Camera.main.ScreenToWorldPoint(new Vector3((sideOffset*Screen.width + (spacing+tileScreenScale)*(j)*Screen.width) + (tileScreenScale/2.0f)*Screen.width, 
				                                           (Screen.height - topOffset*Screen.height - (spacing+tileScreenScale)*(i)*Screen.width - (tileScreenScale/2.0f)*Screen.width), 
				                                           -Camera.main.transform.position.z)),
				                   Quaternion.identity) as GameObject;
				temp.name = "tile_" + i.ToString() + "_" + j.ToString();

				temp.transform.localScale = tileScale;
				temp.transform.parent = rootObj.transform;
				BoxCollider boxCollider = temp.GetComponent<BoxCollider>() as BoxCollider;
				boxCollider.size = new Vector3(sensibility, sensibility, sensibility);
				if(i == 0)
				{
					//If first row
					boxCollider.size = new Vector3(sensibility, sensibility + 0.6f, sensibility);
					boxCollider.center = new Vector3(0, 0.3f, 0);
				}
				if(j == 0)
				{
					//If first column
					Vector3 size = boxCollider.size;
					size.x += 0.6f;
					boxCollider.size = size;
					Vector3 center = boxCollider.center;
					center.x -= 0.3f;
					boxCollider.center = center;
				}
				if(j == vertical-1)
				{
					//If last column
					Vector3 size = boxCollider.size;
					size.x += 0.6f;
					boxCollider.size = size;
					Vector3 center = boxCollider.center;
					center.x += 0.3f;
					boxCollider.center = center;
				}
				if(i == horizontal-1)
				{
					//If last row
					Vector3 size = boxCollider.size;
					size.y += 0.6f;
					boxCollider.size = size;
					Vector3 center = boxCollider.center;
					center.y -= 0.3f;
					boxCollider.center = center;
				}

				if(temp.GetComponent<LetterTile>().letter.font == null)
				{
					Debug.Log("FONT NULLLLLLLL");
				}

				temp.GetComponent<LetterTile>().letter.text = wordMatrix[i,j].ToUpper() ; //"";
				temp.GetComponent<LetterTile>().horizontal = i;
				temp.GetComponent<LetterTile>().vertical = j;
//				tiles.Add(tile);

				allTiles[i*horizontal + j] = temp;
//				wordMatrix[i, j] = "";

			}
		}

		paused = false;
		currentSelectionLine = null;
		SetSelectionLine();

	}


	void SetDictionaryToBeUsed()
	{
		string filePath = "Puzzles/";
		if(GameData.gameMode == EGameMode.KidsMode)
		{
			filePath += "Kids/";
		}


		filePath += GameData.puzzleFileName;
//		Debug.Log("File patha = " + filePath);
//		dictionary = (TextAsset)Resources.Load( "LevelWords/" + GameData.subLevelDictionary, typeof(TextAsset));
		try
		{
			dictionary = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
			if(dictionary == null)
			{
				Debug.Log("Puzzle file NUll at path = " + filePath);
			}
			else
			{
//				Debug.Log("Not");
			}
				
		}
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
		}

			
	}

	void SetSelectionLine () 
	{
		if(currentSelectionLine != null)
		{
			VectorLine.Destroy (ref currentSelectionLine);
		}

		VectorLine.SetCamera3D(Camera.main);
		currentCapName = "cap" + solved;
		VectorLine.SetEndCap (currentCapName, EndCap.Mirror, lineTex, capTex);
		endCaps.Add(currentCapName);

		LineType lineType = LineType.Discrete;
		Joins joins = Joins.None;
		float lineWidth = Screen.width*0.07f;
		if(currentSelectionLine == null)
		{
			lineColors = new List<Color32>();
//			currentSelectionLine = new VectorLine("Line", new List<Vector3>(), lineMaterial, lineWidth, lineType, joins);
			currentSelectionLine = new VectorLine("Line", new List<Vector3>(), lineWidth, lineType, joins);
			currentSelectionLine.material = lineMaterial;
			if(currentSelectionLine.endCap == null)
			{
				currentSelectionLine.endCap = currentCapName;
			}
		}

	}

	void RemoveWrongSelection()
	{
//		Debug.Log("Cap destroy = " + currentCapName);
		VectorLine.RemoveEndCap(currentCapName);
		endCaps.Remove(currentCapName);
		VectorLine.Destroy (ref currentSelectionLine);
	}

	#region Grid Setup

	//Calculate world scale & tile scale accordilngly
	void SetDimensionsOfTile ()
	{
		float availableBoardWidth = Screen.width;
		float availableBoardHeight = Screen.height * 0.6f;
		float tileFactor = 1;
		
		//Set up these parameters for adjusting tiles according to screen size
		sideOffset = 0.05f;
		topOffset = 0.20f;	//0.22f
		float totalWidth = (1.0f - 2 * sideOffset) / horizontal;
		
		spacing = totalWidth * 0.2f;	//Space between tiles is 20 % of tile width on screen
		tileScreenScale = totalWidth * 0.8f + spacing/horizontal;
		
		if(availableBoardHeight < availableBoardWidth)
		{
			tileFactor = (availableBoardHeight/availableBoardWidth);
			float pixelDiff = (availableBoardWidth - availableBoardHeight);
			pixelDiff = pixelDiff*0.5f;
			//			Debug.Log(" tileFactor = " + tileFactor);
			totalWidth = totalWidth*tileFactor;
			sideOffset = sideOffset + pixelDiff/Screen.width;
			//			Debug.Log("sideOffset = " + sideOffset);
			spacing = totalWidth * 0.2f;	
			tileScreenScale = totalWidth * 0.8f + spacing/horizontal;
		}
		
		Vector3 leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height,-Camera.main.transform.position.z));
		Vector3 tileWidthTop = Camera.main.ScreenToWorldPoint(new Vector3(tileScreenScale * Screen.width, (Screen.height),-Camera.main.transform.position.z));
		float tileWorldWidth = Mathf.Abs (tileWidthTop.x - leftTop.x);
		tileScale = new Vector3 (tileWorldWidth, tileWorldWidth, tileWorldWidth);
		
		popingLetter.transform.localScale = tileScale*1.3f;

	}

	// Center the whole game
	private void CenterBackground()
	{
		rootObj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2.0f, 
		                                                                 (Screen.height*0.35f), 
		                                                                 -Camera.main.transform.position.z));

	}


	#endregion


	// Update is called once per frame
	void Update () 
	{
		if(paused)
		{
			return;
		}

		// We have started with selecting
		if(Input.GetMouseButtonDown(0))
		{
			//Touch started
			ray_start = Camera.main.ScreenPointToRay(Input.mousePosition);
			// try to raycast an image...
			if (Physics.Raycast(ray_start, out hit))
			{
				// ...and select it if it's hit
				current = hit.transform.gameObject;
				startTile = hit.transform.gameObject;
				wordFormed.text = GetStringFromTiles( startTile,  startTile );
				if(GameData.isMusicON)
				{
					MainDriver.Instance.PlayWordSelectionSound();
				}

//				SetSelectionLine();
				if(currentSelectionLine != null)
				{
					Vector3 startPos = startTile.transform.position;
					startPos.z += 5;
					currentSelectionLine.points3.Add(startPos);
					popingLetter.gameObject.SetActive(true);
					startPos.y += startTile.transform.localScale.y * 1.5f;
					startPos.z = -1;
					popingLetter.transform.position = startPos;
					popLetterTile.letter.text = GetStringFromTiles( startTile,  startTile );
				}
				else
				{
					Debug.Log("currentSelectionLine is NUll");
				}

			}

		}

		if (Input.GetMouseButton(0) )
		{
			//Touch Moved
			ray_move = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray_move, out hit))
			{
				// ...and select it if it's hit
				current = hit.transform.gameObject;
				GameObject endTile = hit.transform.gameObject;
				if(!endTile.name.Contains("tile"))
					return;
				if(ValidateRelativeStartTileTo(startTile, endTile)) 
				{
					popLetterTile.letter.text = GetStringFromTiles( endTile,  endTile );
					DrawSelectionLine(endTile);
					potentialEnd = endTile;
					wordFormed.text = GetStringFromTiles( startTile,  endTile);
//					if(endTile.GetComponent<LetterTile>().selected != true)
					endTile.GetComponent<LetterTile>().selected = true;
				}
				else if(ValidateErrorCorrectedNode(endTile) != null)
				{
					endTile = ValidateErrorCorrectedNode(endTile);
//					popingLetter.transform.position = endTile.transform.position;
					popLetterTile.letter.text = GetStringFromTiles( endTile,  endTile );
					DrawSelectionLine(endTile);
					potentialEnd = endTile;
					wordFormed.text = GetStringFromTiles( startTile,  endTile);
				}
			}
		}


		// Check the selected word
		if (Input.GetMouseButtonUp(0))
		{
			//Touch ended
			if(potentialEnd != null && (currentSelectionLine != null) )
			{

				if(currentSelectionLine.points3.Count %2 != 0)
				{
					Vector3 pos = potentialEnd.transform.position;
					pos.z += 5;
					currentSelectionLine.points3.Add(pos);
				}
				DrawSelectionLine(potentialEnd);
				Check(wordFormed.text);
				popingLetter.gameObject.SetActive(false);
			}
//			DrawSelectionLine(potentialEnd);
			startTile = null;
			potentialEnd = null;
			wordFormed.text = "";
		}


	}


	#region Word Check Mechanism
	// Check the selected word
	private void Check(string word)
	{
		if (!correct)
		{
//			foreach (KeyValuePair<string, bool> p in placedWords)
			foreach (string p in placedWords)
			{
//				if (word.ToLower() == p.Key.Trim().ToLower())
//				if (word.ToLower() == p.ToLower())
				if (word == p)
				{
					correct = true;
//					Debug.Log("Word found = " + word + " Count placed ="+ placedWords.Count);
					//Play correct word sound
					if(GameData.isMusicON)
					{
						MainDriver.Instance.PlayCorrectWordSound();
					}
				}
			}
		}
		
		if (correct)
		{
			gameHud.SendMessage("WordFound", word);
			placedWords.Remove(word);

			solved++;
			if(lineColors != null)
			{
				for (var i = 0; i < lineColors.Count; i++) {
					lineColors[i] = new Color(0, 1, 0, 0.42f);	//0.42f aalfa
					lineColors[i] = new Color(225.0f/255, 184.0f/255, 216.0f, 0.38f);
				}
				currentSelectionLine.SetColors (lineColors);

			}

			selectionLines.Add(currentSelectionLine);
			currentSelectionLine = null;
			if( (placedWords.Count == 0) || (solved == totalWords) )	
			{
				if(GameData.isTimeChallenge)
				{
					if(GameData.timeTaken <= GameData.timeLimit)
					{
						GameData.isChallengeCompleted = true;
					}
				}
				Application.LoadLevel("GameOver");
			}
		}
		else
		{
			RemoveWrongSelection();
		}

		correct = false;
		SetSelectionLine();
	}


	bool ValidateRelativeStartTileTo(GameObject start, GameObject end)
    {
		Vector2 startTileID = CordinatesForTile(start);
		Vector2 endTileID = CordinatesForTile(end);
		int startX = (int)startTileID.x;
		int startY = (int)startTileID.y;

		int endX = (int)endTileID.x;
		int endY = (int)endTileID.y;

		Vector3 endPos = end.transform.position;
		endPos.z = -1;

		if((startX == endX) && (startY != endY))
		{
			endPos.y += end.transform.localScale.y * 1.5f;
			popingLetter.transform.position = endPos;
			return true;
		}
		else if((startX != endX) && (startY == endY))
		{
//			Debug.Log("Vertical Vertical Vertical");
			endPos.y += end.transform.localScale.y * 1.4f;
			if(endY > 5)
			{
				endPos.x -= end.transform.localScale.x * 1.6f;
				popingLetter.transform.position = endPos;
			}
			else
			{
				endPos.x += end.transform.localScale.x * 1.6f;
				popingLetter.transform.position = endPos;
			}

			return true;
		}
		else if(Mathf.Abs(startX - endX) == Mathf.Abs(startY - endY) )
		{
//			Debug.Log("Moving XXXX");
			if(endX > startX && endY > startY)
			{
				////
//				Debug.Log("Moving LEft to right down side");
				endPos.x += end.transform.localScale.x*0.8f ;
				endPos.y += end.transform.localScale.y * 1.2f;
				popingLetter.transform.position = endPos;
			}
			else if (startX > endX && endY < startY)
			{
				///
//				Debug.Log("Moving RIght to Left UPP Side");
				endPos.x += end.transform.localScale.x*0.8f ;
				endPos.y += end.transform.localScale.y * 1.2f;
				popingLetter.transform.position = endPos;
			}
			else if (endX > startX && endY < startY)
			{
//				Debug.Log("Moving RIght to Left down Side");
				endPos.x -= end.transform.localScale.x *0.8f;
				endPos.y += end.transform.localScale.y * 1.2f;
				popingLetter.transform.position = endPos;
			}
			else if (startX > endX && endY > startY)
			{
//				Debug.Log("Moving LEft to rightt UPP Side");
				endPos.x -= end.transform.localScale.x *0.8f ;
				endPos.y += end.transform.localScale.y * 1.2f;
				popingLetter.transform.position = endPos;
			}

			return true;
		}
		return false;

    }

	GameObject ValidateErrorCorrectedNode(GameObject end)
	{
//		Debug.Log("ValidateErrorCorrectedNode");
		Vector2 startTileID = CordinatesForTile(startTile);
		Vector2 endTileID = CordinatesForTile(end);

		float xdifference = endTileID.x - startTileID.x;
		float ydifference = endTileID.y - startTileID.y;
		float xAbsoluteDifference = Mathf.Abs(xdifference);
		float yAbsoluteDifference = Mathf.Abs(ydifference);

		float swipeAngle = Mathf.Rad2Deg * (Mathf.Atan2 (yAbsoluteDifference, xAbsoluteDifference));

		if (swipeAngle < 0) 
		{
			swipeAngle = 360 + swipeAngle;
		}
//		Debug.Log("swipeAngle = " + swipeAngle);
		int tileNumber = -1;

//		if(swipeAngle <= 180.0f/16)
		if(swipeAngle <= 25.0f)
		{
//			Debug.Log("staright 0 ");
			tileNumber = horizontal * (int)endTileID.x + (int)startTileID.y;
		}
//		else if (swipeAngle >= 3.5f*180/16 && swipeAngle <= 5.5f*180/16)
		else if (swipeAngle > 25 && swipeAngle <= 65)
		{
//			Debug.Log("staright 45");
			Vector2 gradient;
			
			if ((ydifference > 0) == (xdifference > 0)) 
			{
				gradient = new Vector2(1, 1);
			}
			else
			{
				gradient = new Vector2(-1, 1);
			}

			Vector2 vectorDifference = new Vector2( endTileID.x - startTileID.x, endTileID.y - startTileID.y);
			float numarator = (vectorDifference.x * gradient.x) + (vectorDifference.y * gradient.y);
			float denominator = (gradient.x * gradient.x)+(gradient.y*gradient.y);
			Vector2 closestPoint = new Vector2(startTileID.x + Mathf.Round(gradient.x*numarator/denominator), startTileID.y + Mathf.Round(gradient.y*numarator/denominator));
			if (closestPoint.x <= 10 && closestPoint.x>=0 && closestPoint.y<=10 && closestPoint.y>=0) 
			{
				tileNumber = horizontal * (int)closestPoint.x + (int)closestPoint.y;
			}
		}
//		else if (swipeAngle >=7*180.0f/16 && swipeAngle <= 180.0f/2)
		else if (swipeAngle > 65 && swipeAngle <= 90)
		{
//			Debug.Log("staright  90 ");
			tileNumber = horizontal * (int)startTileID.x + (int)endTileID.y;
		}
		else
		{
//			Debug.Log("Swipe angle = " + swipeAngle);
		}

		if (tileNumber >=0 && tileNumber < allTiles.Length ) 
		{
			
			GameObject candidateTile = allTiles[tileNumber];
			endTileID = CordinatesForTile(end);
			int startX = (int)startTileID.x;
			int startY = (int)startTileID.y;
			int endX = (int)endTileID.x;
			int endY = (int)endTileID.y;
//			if(ValidateSelection(startTileID, CordinatesForTile(candidateTile)))
//			if(ValidateSelection(startTileID, endTileID))
			{
				Vector3 endPos = candidateTile.transform.position;
				endPos.z = -1;
				if((startX == endX) && (startY != endY))
				{
//					Debug.Log("Moving LEft RIght Error Node");
					endPos.y += candidateTile.transform.localScale.y * 1.5f;
					popingLetter.transform.position = endPos;
				}
				else if((startX != endX) && (startY == endY))
				{
//					Debug.Log("Moving UP DOwn");
					if(endY > 6)
					{
						endPos.x -= end.transform.localScale.x * 1.5f;
						popingLetter.transform.position = endPos;
					}
					else
					{
						endPos.x += end.transform.localScale.x * 1.5f;
						popingLetter.transform.position = endPos;
					}
					
				}
				return candidateTile;
			}
//			else
//			{
//				Debug.Log("Not valid");
//			}
			
		}
//		Debug.Log("Return = " + tileNumber);
		return null;
	}

	bool ValidateSelection(Vector2 startTileCord, Vector2 endTileCord)
	{
		return ((startTileCord.x == endTileCord.x && startTileCord.y != endTileCord.y) 
		        || (startTileCord.x != endTileCord.x && startTileCord.y == endTileCord.y) 
		        || (Mathf.Abs(startTileCord.x-endTileCord.x) == Mathf.Abs(startTileCord.y-endTileCord.y)));
	}
	
	
	string GetStringFromTiles(GameObject start, GameObject end )
	{

		Vector2 startTileID = CordinatesForTile(start);
		Vector2 endTileID = CordinatesForTile(end);

		string wordFormed = GetStringFromCordinates(startTileID, endTileID);
//		Debug.Log("wordFormed = = " + wordFormed);
		wordFormed = wordFormed.ToUpper();
		return wordFormed;
	}

	Vector2 CordinatesForTile(GameObject tile)
	{
		if(tile != null)
		{
			string startTileName = tile.name;
//			Debug.Log("startTileName  = " + startTileName);
			if(startTileName.Split('_').Length != 3)
			{
//				Debug.Log("startTileName  = " + startTileName);
				return Vector2.zero;
			}

			int row = int.Parse( startTileName.Split('_')[1]);
			int col = int.Parse( startTileName.Split('_')[2]);
			
			return new Vector2(row, col);
		}
		return Vector2.zero;
	}

	string GetStringFromCordinates(Vector2 start, Vector2 end)
	{
		Vector2 point = new Vector2(start.x, start.y);
		Vector2 delta = GetDeltaBetweenPoints(start, end);

		string visString = "";
		visString += wordMatrix[(int)start.x, (int)start.y];
		while (!(point.x == end.x && point.y == end.y)) {
			point.x += delta.x;
			point.y += delta.y;
			string tileText = wordMatrix[(int)point.x, (int)point.y];
			visString += tileText;
//			if (point.x<=0 || point.x>=horizontal-1 || point.y<=0 || point.y>=vertical-1) 
			if (point.x<0 || point.x>horizontal-1 || point.y<0 || point.y>vertical-1) 
			{
				return visString;
			}
		}
		return visString;
	}

	Vector2 GetDeltaBetweenPoints(Vector2 start, Vector2 end)
	{
		Vector2 delta = Vector2.zero;
		delta.x = end.x - start.x;
		delta.y = end.y - start.y;

		if (delta.x!=0) {
			delta.x = delta.x/(Mathf.Abs(delta.x));
			
		}
		if (delta.y!= 0) {
			delta.y = delta.y/Mathf.Abs(delta.y);
		}
		return delta;
	}


	void DrawSelectionLine(GameObject currentTarget)
	{
		Vector3 pos = currentTarget.transform.position;
		pos.z += 5;
		if(currentSelectionLine != null)
		{
			if (currentSelectionLine.points3.Count%2 == 0) 
			{
//								lineColors.Add (Color.white);
			}
			
			if (currentSelectionLine.points3.Count %2 != 0)
			{
				currentSelectionLine.points3.Add(pos);
				if(lineColors != null)
				{
					lineColors.Add (Color.yellow);
				}
			}
			
			
			if ( currentSelectionLine.points3.Count >= 2) 
			{	
				// Only draw when continuous, or when discrete && there are at least 2 points
				currentSelectionLine.points3[currentSelectionLine.points3.Count-1] = pos;
//				currentSelectionLine.Draw();
				currentSelectionLine.Draw3D();
			}
		}
	}

	#endregion


	#region Puzzle Words Placement
	void StartLoadingPuzzleGrid ()
	{
		int i,j;
		for (i=0; i<horizontal; i++) 
		{
			for (j=0; j<vertical; j++) 
			{
				wordMatrix[i,j] = "";
			}
		}

		for (i=0; i<puzzleWordsList.Count; i++) 
		{
//			Debug.Log("Trying to place  Word = " + puzzleWordsList[i]);
			PlaceWordInPuzzle(i);
		};
//		Debug.Log("Placed Word = " + placedWords.Count);
		FillBoard ();

	}

	void PlaceWordInPuzzle(int word)
	{
		System.Random rnd = new System.Random ();
		int []tried = new int[8];
		int i,x,y,start_dir,start_x,start_y;
		for (i=0; i<8; i++) 
		{
//			tried[i] = !permitted[i];
			tried[i] = 0;
		}

		i= start_dir = rnd.Next(0, 8); //rnd.Next(8);

		if(IsDirectionCached(i))
		{
//			Debug.Log("Duplicate");
			while(IsDirectionCached(i))
			{
				i= start_dir = rnd.Next(0, 8);
			}
//			Debug.Log("ADDD dumplicatre also");
		}
		else
		{
			directionCache.Add(i);
		}


		System.Random rnd1 = new System.Random ();
		System.Random rnd2 = new System.Random ();

		do
		{
//			Debug.Log("Tried for word = " + puzzleWordsList[word]);
			if (tried[i] == 0) 
			{
				tried[i] = 1;
				y= start_y = rnd1.Next(0, vertical); 
				do
				{
					x=start_x = rnd2.Next(0, horizontal); 
					do
					{
						if (TryPlaceWord(x, y, i, word, 0) == 1) 
						{
//							Debug.Log("Placed = " + puzzleWordsList[word] + "Direction  = " + i);
							TryPlaceWord(x, y, i, word, 1);
							//Add as placed word in puzzle 
							placedWords.Add(puzzleWordsList[word].ToUpper());
							goto success;
						};
						x = (x+1) % horizontal;
						
					}while (x!= start_x);
					y = (y+1)%vertical;
				}while (y!=start_y);
			}
			i=(i+1)%8;

		}while(i != start_dir);
//		Debug.Log("Failed tried word = " + i);
		used[word] = -1;
		return; 
	success:
//		Debug.Log("Success tried word = " + i);
		used[word] =1;
		return;  
	}

	int TryPlaceWord(int row, int col, int dir, int word , int place )
	{
		int len;
		int cnt;
		char [] pickWord = puzzleWordsList[word].ToCharArray();
		len = pickWord.Length;
		if (dirs[dir,0] == 1 && (len + row) > horizontal) return(0);
		if (dirs[dir,0] == -1 && (len - 1) > row) return(0);
		if (dirs[dir,1] == 1 && (len + col) > vertical) return(0);
		if (dirs[dir,1] == -1 && (len - 1) > col) return(0);

		for (cnt = 0; cnt<len; cnt++) 
		{
			if ( !string.IsNullOrEmpty( wordMatrix[row,col].ToString())  &&  wordMatrix[row,col] != pickWord[cnt].ToString()) 
			{
				return 0;
			};
			if (place == 1) 
			{
				wordMatrix[row,col] = pickWord[cnt].ToString();
			}
//			Debug.Log("Row = " + row +" Col = "+col );
			row += dirs[dir,0];
			col += dirs[dir,1];

		}

		return (1);
	}

	void FillBoard()
	{
		if(puzzleWordsList.Count<1)
		{
			return;
		}

		System.Random rnd = new System.Random ();
		int word,spot;
		int i,j;
		for (i=0; i<horizontal; i++) 
		{
			for (j=0; j<vertical; j++) 
			{
				if (wordMatrix[i,j] == "") 
				{
					do 
					{
						word  = rnd.Next(puzzleWordsList.Count);
					} while (used[word]==-1);
					string tempStr = puzzleWordsList[word];
					spot = rnd.Next(tempStr.Length);
					if(GameData.gameMode == EGameMode.TutorialMode)
					{
						tempStr = "O";
						spot = rnd.Next(tempStr.Length);
						wordMatrix[i,j] = (tempStr.ToCharArray())[spot].ToString();
					}
					else
					{
						wordMatrix[i,j] = (tempStr.ToCharArray())[spot].ToString();
					}

				};
			}
		}
	}

	private List<int> directionCache = new List<int>();

	bool IsDirectionCached(int dir)
	{
		if(directionCache.Contains(dir))
			return true;
		else 
			return false;
	}

	// Shuffles words of current puzzle in string array
	private void Shuffle(string[] words)
	{
		for (int i = 0; i < words.Length; i++)
		{
			words[i] = words[i].TrimEnd();
		}
		for (int t = 0; t < words.Length; t++)
		{
			string tmp = words[t];
			int r = UnityEngine.Random.Range(t, words.Length);
			words[t] = words[r];
			words[r] = tmp;
		}
	}
	
	#endregion


	public void GamePaused()
	{
		for(int i = 0; i < selectionLines.Count; i++)
		{
			selectionLines[i].active = false;
		}
//		bottomBorder.active = false;
//		borderLine.active = false;
		popingLetter.gameObject.SetActive(false);
	}

	public void GameResumed()
	{
//		bottomBorder.active = true;
//		borderLine.active = true;
		for(int i = 0; i < selectionLines.Count; i++)
		{
			selectionLines[i].active = true;
		}
//		popingLetter.gameObject.SetActive(false);
	}

	Vector2 GetScreenPos(GameObject tile)
	{
		if(tile != null)
		{
			Vector3 scrPos = Camera.main.WorldToScreenPoint(tile.transform.position);
			return new Vector2(scrPos.x, scrPos.y);
		}
		return Vector2.zero;
	}

}
