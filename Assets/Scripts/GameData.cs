using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour {

	static public EGameMode gameMode;
	static public EGameType gameType;

	static public int mainThemeIndex;
	static public int puzzleID;
	static public int kidsIndex;
	static public string puzzleFileName;
	static public string gamePuzzleName;
	static public bool isTimeChallenge, isChallengeCompleted;
	static public int timeLimit;

	static public int totalKeys;

	static public bool  en8_lock, en9_lock, en10_lock, en11_lock, en12_lock;


	//Language
	static public int timeTaken;
	static public bool isMusicON;
	static public bool isNotifcationON;
}
