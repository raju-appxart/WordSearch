using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using PlayerPrefs = PreviewLabs.PlayerPrefs;


public class GameHud : MonoBehaviour {

	public Canvas HudCanvas;
	public RectTransform gamePlayHud;
	public RectTransform tutorialHud;
	public RectTransform pauseMenu;
	public RectTransform settingPanel;

	public GameObject rootObject;
	public Text timeElapsedText;
	public Text bestTimeLabel;
	public Text wordsFound;

	public Text wordFormedGame, wordFormedTutorial;
	public Text[] puzzleWords, tutorialWords;


	//Pause panel dsiplay info
	public Text puzzleName;
	public Text pauseWordsFound, pauseTime;



	private float startTime, endTime;
	public static int wordsFoundCount;
	bool paused, alertON;
	int time = 0;

	string timerText, bestTimeText, wordFoundText;


	void OnEnable()
	{
	}
	
	void OnDisable()
	{

	}

	// Use this for initialization
	void Start () 
	{
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			tutorialHud.gameObject.SetActive(true);
		}
		else
		{
			wordsFoundCount = 0;
			string bestTimeKey = Utility.KeyForBestTimeOfPuzzle();
			int bestTime = PlayerPrefs.GetInt(bestTimeKey, 0);
			if(bestTime == 0)
			{
				bestTimeLabel.text =  "?";
			}
			else
			{
				bestTimeLabel.text = String.Format("{0:D2}:{1:D2}", bestTime/60, bestTime%60);
			}
			
			wordsFound.text =  wordsFoundCount + "/" + WordSearch.totalWords ;
			pauseWordsFound.text = "" + wordsFoundCount + "/" + WordSearch.totalWords ;
			
			puzzleName.text = GameData.gamePuzzleName;
			

			if(GameData.isTimeChallenge)
			{
				timeElapsedText.text = String.Format("{0:D2}:{1:D2}", GameData.timeLimit/60, GameData.timeLimit%60);
			}
			else
			{
				timeElapsedText.text = String.Format("{0:D2}:{1:D2}", time/60, time%60);
			}

			Invoke("CurrentTime",1.0f);
			gamePlayHud.transform.gameObject.SetActive(true);
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			if(Camera.main.GetComponent<WordSearch>().paused == false)
			{    
				PauseGame();
			}
		}
	}

	public void AssignPuzzlewords( List<string>Words)
	{
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			for(int i = 0; i< tutorialWords.Length; i++)
			{
				if(i < Words.Count)
				{
					tutorialWords[i].text = Words[i];
				}
				else
				{
					tutorialWords[i].text = "";
				}
			}
		}
		else
		{
			wordsFound.text =  wordsFoundCount + "/" + WordSearch.totalWords ;
			pauseWordsFound.text = "" + wordsFoundCount + "/" + WordSearch.totalWords ;
			for(int i = 0; i< puzzleWords.Length; i++)
			{
				if(i < Words.Count)
				{
					puzzleWords[i].text = Words[i];
				}
				else
				{
					puzzleWords[i].text = "";
				}
			}
			startTime = Time.time;
		}

		InvokeRepeating ("ResetWords", 0.6f, 3.0f);
	}

	void ResetWords()
	{
		for(int i = 0; i< puzzleWords.Length; i++)
		{
			puzzleWords[i].enabled = false;
			puzzleWords[i].enabled = true;
		}
	}

	public void WordFound(string wordFound)
	{
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			for(int i = 0; i< tutorialWords.Length; i++)
			{
				if(wordFound == tutorialWords[i].text)
				{
					tutorialWords[i].color = new Color(245.0f/255, 195.0f/255, 85.0f/255);
					tutorialWords[i].fontStyle = FontStyle.Italic;
					break;
				}
			}
		}
		else
		{
			for(int i = 0; i< puzzleWords.Length; i++)
			{
				if(wordFound == puzzleWords[i].text)
				{
					puzzleWords[i].color = new Color(245.0f/255, 195.0f/255, 85.0f/255);
					puzzleWords[i].fontStyle = FontStyle.Italic;
					wordsFoundCount++;
					wordsFound.text = wordsFoundCount + "/" + WordSearch.totalWords ;
					pauseWordsFound.text = "" + wordsFoundCount + "/" + WordSearch.totalWords ;
					break;
				}
			}
		}

	}

	private void CurrentTime()
	{
//		TimeSpan t = TimeSpan.FromSeconds(Mathf.RoundToInt(Time.time - startTime));
//		GameData.timeTaken = t.Minutes*60 + t.Seconds;
//		timeElapsedText.text = String.Format(timerText + " {0:D2}:{1:D2}", t.Minutes, t.Seconds);
//		return String.Format(timerText + " {0:D2}:{1:D2}", t.Minutes, t.Seconds);

		time++;
		GameData.timeTaken = time;

		if(GameData.isTimeChallenge)
		{
			timeElapsedText.text = String.Format("{0:D2}:{1:D2}", (GameData.timeLimit-time)/60, (GameData.timeLimit-time)%60);
		}
		else
		{
			timeElapsedText.text = String.Format("{0:D2}:{1:D2}", time/60, time%60);
		}
		
		pauseTime.text = String.Format("{0:D2}:{1:D2}", time/60, time%60);

		if(GameData.isTimeChallenge)
		{
			if(GameData.timeTaken >= GameData.timeLimit)
			{
				GameData.isChallengeCompleted = false;
				Application.LoadLevel("GameOver");
			}
		}
//		timeElapsedText.text = String.Format(timerText + " {0:D2}:{1:D2}", time/60, time%60);

		if(!paused)
		Invoke("CurrentTime", 1);
	}

	
	#region Hud Buttons Actions
	public void PauseGame()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		Camera.main.GetComponent<WordSearch>().paused = true;
		Camera.main.GetComponent<WordSearch>().GamePaused();
		rootObject.SetActive(false);
		gamePlayHud.transform.gameObject.SetActive(false);
		pauseMenu.transform.gameObject.SetActive(true);
		paused = true;
		CancelInvoke();
	}

	public void ResumeGame()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		rootObject.SetActive(true);
		Camera.main.GetComponent<WordSearch>().GameResumed();
		gamePlayHud.transform.gameObject.SetActive(true);
		pauseMenu.transform.gameObject.SetActive(false);
		Camera.main.GetComponent<WordSearch>().paused = false;
		paused = false;
		Invoke("CurrentTime",0.5f);
	}

	public void RestartGame()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		Application.LoadLevel(Application.loadedLevel);
	}

	public void Settings()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		settingPanel.transform.gameObject.SetActive(true);
	}



	public void LobbyPressed()
	{
		if(GameData.isMusicON && (GameData.gameType == EGameType.QuickGame || GameData.gameType == EGameType.TimeChallenge
		                          || GameData.gameType == EGameType.DailyChallenge))
		{
			MainDriver.Instance.PlayButtonSound();
		}
		GameData.isTimeChallenge = false;
		Application.LoadLevelAsync("MainLobby");
	}


	#endregion


	void OnApplicationFocus(bool focusStatus)
	{
//		Debug.Log("OnApplicationFocus = " + focusStatus);
		if(focusStatus)
		{
//			//Application Resumed
//			ResumeGame();
		}
		else
		{
			//Application Backgrounded
			if(!pauseMenu.transform.gameObject.activeSelf && !settingPanel.transform.gameObject.activeSelf)
			{
				if(GameData.gameMode == EGameMode.TutorialMode)
				{

				}
				else
				{
					PauseGame();
				}

			}

		}
	}


}
