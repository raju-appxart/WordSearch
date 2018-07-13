using UnityEngine;
using System.Collections;

/// <summary>
/// Utility.
/// 
/// </summary>
public class Utility : MonoBehaviour {


	public static string KeyForBestTimeOfPuzzle()
	{
		string key = "";
		if(GameData.gameMode == EGameMode.FullMode)
		{
			key = GameData.mainThemeIndex + "_" + GameData.puzzleID  + "_B" ;

		}
		else if(GameData.gameMode == EGameMode.KidsMode)
		{
			key = "K_" + GameData.kidsIndex + "_B" ;
		}
//		Debug.Log("KEY game = " + key);
		return key;
	}

	public static string KeyForBestTimeOfPuzzle(int themeId, int puzzleID)
	{
		string key = "";
		if(GameData.gameMode == EGameMode.FullMode)
		{
			key = themeId + "_" + puzzleID  + "_B" ;
			
		}
		else if(GameData.gameMode == EGameMode.KidsMode)
		{
			key = "K_" + GameData.kidsIndex + "_B" ;
		}
		//		Debug.Log("KEY game = " + key);
		return key;
	}

	public static string KeyForBestTimeOfKidsPuzzle(int index)
	{
//		else if(GameData.gameMode == EGameMode.KidsMode)
		{
			string key = "";
			key = "K_" + index + "_B" ;
//			Debug.Log("KEY game = " + key);
			return key;
		}
	}


	public static string KeyForDiamondsOfPuzzle(int themeId, int puzzleId)
	{
		string key = themeId + "_" + puzzleId  + "_D" ;
		return key;
	}

	public static string KeyForDiamondsOfTheme(int themeId)
	{
		string key = themeId + "_D" ;
		return key;
	}

	public static string KeyForThemeLock(int themeId)
	{
		string key = "Lock_"  + themeId ;
		return key;
	}

	string CurrentLevelId()
	{
		string key = "P_"+ GameData.mainThemeIndex + "_" + GameData.puzzleID;
		return key;
	}


}
