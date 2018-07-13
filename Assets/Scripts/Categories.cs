using UnityEngine;
using System.Collections;

public class Categories 
{



}

public enum EGameMode
{
	None,
	TutorialMode,
	FullMode,
	KidsMode,
	Count
}

public enum EGameType
{
	None, 
	ThemeSelection,
	TimeChallenge,
	QuickGame,
	DailyChallenge,
	KidsGame,
	Count
}

public struct Level
{
	public string category;
	public string iconImg;
	public int puzzleCount;
};

public struct SubLevel
{
	public string category;
	public string bgImg;
	public string iconImg;
};

public struct Quest
{
	public int  diamondCnt;
	public string details;
};
