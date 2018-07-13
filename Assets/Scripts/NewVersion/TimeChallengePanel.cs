using UnityEngine;
using System.Collections;


public class TimeChallengePanel : MonoBehaviour {


	public RectTransform  challengePanel, timeChallengePanel;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			BackToChallenge();
		}
	}

	public  void BackToChallenge()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		challengePanel.gameObject.SetActive(true);
		timeChallengePanel.gameObject.SetActive(false);
	}

	public void ChallengeGame(int time)
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}

		if(time == 60)
		{
			GameData.timeLimit = 60;
		}
		else if(time == 75)
		{
			GameData.timeLimit = 75;
		}
		else if(time == 90)
		{
			GameData.timeLimit = 90;
		}
		
		GameData.isTimeChallenge = true;
		GameData.gameType = EGameType.TimeChallenge;
		
		//Load a random game
		GameData.gameMode = EGameMode.FullMode;


		int index = UnityEngine.Random.Range(0, MainDriver.Instance.unlockedThemes.Count);
		GameData.mainThemeIndex = MainDriver.Instance.unlockedThemes[index];
		
		int puzzzleId = UnityEngine.Random.Range(0, Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1].Length);
		GameData.puzzleID = puzzzleId + 1;
		
		GameData.gamePuzzleName = Reader.Instance.SubLevelDetails[GameData.mainThemeIndex-1][puzzzleId];
		GameData.puzzleFileName = "P_"+ GameData.mainThemeIndex + "_"+ GameData.puzzleID;
		Application.LoadLevelAsync("GamePlay");
	}
}
