using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using Heyzap;

public class GameOver : MonoBehaviour {

	public Text timeTaken;
	public Text bestTime;
	public Text diamondEarnedText;
	public Text puzzleName;
	public Image diamondIcon;
	public Text rewardText;
	public Text gameTimeLabel, bestTimeLabel;
	public Text tutorialTitleText;

	public RectTransform settingPanel, diamondInfoPanel;
	public RectTransform resultPanel, tutorialResultPanel, rewardPanel;

	public RectTransform rateDialog;



	int noOfDiamonds = 0;
	int keyLevelCount = 0;

//	bool hasGotDiamond;


	void OnEnable()
	{
	}
	
	void OnDisable()
	{
	}

	// Use this for initialization
	void Start () 
	{
		settingPanel.transform.gameObject.SetActive(false);
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			tutorialTitleText.text = "Tutorial puzzle completed";
			//			noOfDiamonds = 1;
			tutorialResultPanel.gameObject.SetActive(true);
		}
		else
		{
			MainDriver.Instance.gameCount++;
			puzzleName.text =  GameData.gamePuzzleName;
			
			if(GameData.gameMode == EGameMode.FullMode)
			{
				diamondInfoPanel.gameObject.SetActive(true);
				diamondIcon.gameObject.SetActive(true);
				if(GameData.isTimeChallenge)
				{
					//Time challnege
					gameTimeLabel.text = "GAME TIME";
					bestTimeLabel.text = "GIVEN TIME";
					
					if(GameData.isChallengeCompleted)
					{
						noOfDiamonds = 1;
						MainDriver.Instance.puzzleSolved++;
						timeTaken.text = String.Format("{0:D2} : {1:D2}", GameData.timeTaken/60, GameData.timeTaken%60);
						int timeChallengeReward = 1;
						int totalKeys  = PlayerPrefs.GetInt(Constants.KEY_CURRENT_DIAMONDS, 0);
						PlayerPrefs.SetInt(Constants.KEY_CURRENT_DIAMONDS, totalKeys+1);
						diamondEarnedText.text = "Time challenge completed";
						rewardText.text =  timeChallengeReward + " DIAMOND";
						rewardPanel.gameObject.SetActive(true);
						
						if(!MainDriver.Instance.questChecker.QUEST_FIRST_PUZZLE)
						{
							PlayerPrefs.SetBool(Constants.QUEST_FIRST_PUZZLE, true);
							MainDriver.Instance.questChecker.UpdateQuestStatus();
							MainDriver.Instance.PostAchievement(Constants.QUEST_FIRST_PUZZLE_ID);
							MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
						}
					}
					else
					{
						timeTaken.text = "Time Up";
						//Failed Time challenge
						if(GameData.timeLimit == 60)
						{
							diamondEarnedText.text = "1 Min challenge failed";
						}
						else if(GameData.timeLimit == 75)
						{
							diamondEarnedText.text = "1:15 Min challenge failed";
						}
						else if(GameData.timeLimit == 90)
						{
							diamondEarnedText.text = "1:30 Min challenge failed";
						}
						
					}
					resultPanel.gameObject.SetActive(true);
				}
				else
				{
					MainDriver.Instance.puzzleSolved++;
					//Normal game
					gameTimeLabel.text = "GAME TIME";
					bestTimeLabel.text = "BEST TIME";
					timeTaken.text = String.Format("{0:D2} : {1:D2}", GameData.timeTaken/60, GameData.timeTaken%60);
					noOfDiamonds = GetDiamondForSeconds(GameData.timeTaken);
					if(noOfDiamonds == 1)
					{
//						if(MainDriver.Instance.puzzleSolved == 1)
						if(!MainDriver.Instance.questChecker.QUEST_FIRST_PUZZLE)
						{
							//							diamondEarnedText.text =  "First puzzle reward "+ noOfDiamonds + " Diamond";
							diamondEarnedText.text =  "You have got "+ noOfDiamonds + " Diamond";
							tutorialTitleText.text = "First puzzle completed";
							tutorialResultPanel.gameObject.SetActive(true);

							PlayerPrefs.SetBool(Constants.QUEST_FIRST_PUZZLE, true);
							MainDriver.Instance.questChecker.UpdateQuestStatus();
							MainDriver.Instance.PostAchievement(Constants.QUEST_FIRST_PUZZLE_ID);
							MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
							
						}
						else
						{
							diamondEarnedText.text =  "You have got "+ noOfDiamonds + " Diamond";
							resultPanel.gameObject.SetActive(true);
							rewardPanel.gameObject.SetActive(true);
						}
						
						
					}
					else
					{
						diamondEarnedText.text = "Finish puzzle under 2 Min to get a Diamond";
						resultPanel.gameObject.SetActive(true);
					}
					
//					if(!MainDriver.Instance.questChecker.QUEST_FIRST_PUZZLE)
//					{
//						PlayerPrefs.SetBool(Constants.QUEST_FIRST_PUZZLE, true);
//						MainDriver.Instance.questChecker.UpdateQuestStatus();
//						MainDriver.Instance.PostAchievement(Constants.QUEST_FIRST_PUZZLE_ID);
//						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
//					}
					
				}
				
				
			}
			else
			{
				//Kids mode
				MainDriver.Instance.puzzleSolved++;
				diamondInfoPanel.gameObject.SetActive(false);
				timeTaken.text = String.Format("{0:D2} : {1:D2}", GameData.timeTaken/60, GameData.timeTaken%60);
				diamondIcon.gameObject.SetActive(false);
				diamondEarnedText.text = "";
				resultPanel.gameObject.SetActive(true);
			}
			
			
			SaveLevelData();
			if(MainDriver.Instance.gameCount > 3)
			{
				HZInterstitialAd.Show();
			}
			
			MainDriver.Instance.PostScoreToLeaderBoard();
			UniRate.Instance.LogEvent(true);
			
		}
		
		
		
	}
	
	public void RateDialog()
	{
		Invoke("RateDialogShow", 0.25f);
	}
	
	void RateDialogShow()
	{
		rateDialog.transform.gameObject.SetActive(true);
		rateDialog.transform.GetComponent<Animator>().SetTrigger("Show");
	}
	
	
	// Update is called once per frame
//	void Update () 
//	{
//		if(Input.GetKeyDown(KeyCode.Escape)) 
//		{
//			//			LobbyPressed();
//		}
//	}
	
	
	#region Button Actions
	public void PlayAgain()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		PlayerPrefs.Flush();
		Application.LoadLevelAsync("GamePlay");
	}
	
	public void Settings()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		settingPanel.transform.gameObject.SetActive(true);
	}
	
	public void RateME()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		UniRate.Instance.RateIfNetworkAvailable();
	}
	
	public void ShareScore()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		if(GameData.isTimeChallenge && !GameData.isChallengeCompleted)
		{
			string timeChallengeType = "";
			if(GameData.timeLimit == 60)
			{
				timeChallengeType = "1:00 Min challenge";
			}
			else if(GameData.timeLimit == 75)
			{
				timeChallengeType = "1:15 Min challenge";
			}
			else if(GameData.timeLimit == 90)
			{
				timeChallengeType = "1:30 Min challenge";
			}
			MainDriver.Instance.ShareWhenTimeChallengeFailed(GameHud.wordsFoundCount, timeChallengeType);
		}
		else
		{
			string timeText = String.Format("{0:D2} : {1:D2}", GameData.timeTaken/60, GameData.timeTaken%60);
			MainDriver.Instance.ShareUniversal(timeText);
		}
		
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
	
	public void TutorialOkPressed()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		if(GameData.gameMode == EGameMode.TutorialMode)
		{
			GameData.gameMode = EGameMode.FullMode;
			Application.LoadLevelAsync("MainLobby");
		}
		else
		{
			resultPanel.gameObject.SetActive(true);
			tutorialResultPanel.gameObject.SetActive(false);
		}
	}
	
	public void GetIt()
	{
		if(GameData.isMusicON)
		{
			MainDriver.Instance.PlayButtonSound();
		}
		rewardPanel.gameObject.SetActive(false);
	}
	
	#endregion
	
	
	
	void SaveLevelData()
	{
		string bestTimeKey = Utility.KeyForBestTimeOfPuzzle();
		
		int previousBestTime = PlayerPrefs.GetInt(bestTimeKey, 0);
		//		Debug.Log("previousBestTime = " + previousBestTime);
		//		Debug.Log("GameData.timeTaken = " + GameData.timeTaken);
		
		//Save For last level time
		if(GameData.isTimeChallenge)
		{
			if(GameData.isChallengeCompleted)
			{
				if(GameData.timeTaken <= previousBestTime || previousBestTime == 0)
				{
					PlayerPrefs.SetInt(bestTimeKey, GameData.timeTaken);
				}
			}
		}
		else
		{
			if(GameData.timeTaken <= previousBestTime || previousBestTime == 0)
			{
				PlayerPrefs.SetInt(bestTimeKey, GameData.timeTaken);
			}
		}
		
		
		int currentBest = PlayerPrefs.GetInt(bestTimeKey, 0);
		
		if(GameData.isTimeChallenge)
		{
			bestTime.text =  String.Format("{0:D2} : {1:D2}", GameData.timeLimit/60, GameData.timeLimit%60);
		}
		else
		{
			bestTime.text =  String.Format("{0:D2} : {1:D2}", currentBest/60, currentBest%60);
		}
		
		
		if(GameData.gameMode == EGameMode.FullMode)
		{
			//Daily Challenge Quest check
			if(GameData.gameType == EGameType.DailyChallenge && !MainDriver.Instance.questChecker.QUEST_DAILY_CHAMP)
			{
				MainDriver.Instance.dailyChallangeCnt++;
				PlayerPrefs.SetInt(Constants.DAILY_CHALLENGE_COUNT, MainDriver.Instance.dailyChallangeCnt);
				if(MainDriver.Instance.dailyChallangeCnt == 10)
				{
					PlayerPrefs.SetBool(Constants.QUEST_DAILY_CHAMP, true);
					MainDriver.Instance.questChecker.UpdateQuestStatus();
					MainDriver.Instance.PostAchievement(Constants.QUEST_DAILY_CHAMP_ID);
					MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
				}
			}
			
			//Time quests check
			if(GameData.timeTaken <= 75 && !MainDriver.Instance.questChecker.QUEST_10P_T115)
			{
				//10 Puzzles Under 1:15 each
				if(!GameData.isTimeChallenge || (GameData.isTimeChallenge && GameData.isChallengeCompleted))
				{
					MainDriver.Instance.puzzleIn115Cnt++; 
					PlayerPrefs.SetInt(Constants.PUZZLE_UNDER_115, MainDriver.Instance.puzzleIn115Cnt);
					if(MainDriver.Instance.puzzleIn115Cnt == 10)
					{
						PlayerPrefs.SetBool(Constants.QUEST_10P_T115, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T115_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
			}
			
			if(GameData.timeTaken <= 90 && !MainDriver.Instance.questChecker.QUEST_10P_T130)
			{
				//10 Puzzles Under 1:30 each
				if(!GameData.isTimeChallenge || (GameData.isTimeChallenge && GameData.isChallengeCompleted))
				{
					MainDriver.Instance.puzzleIn130Cnt++; 
					PlayerPrefs.SetInt(Constants.PUZZLE_UNDER_130, MainDriver.Instance.puzzleIn130Cnt);
					if(MainDriver.Instance.puzzleIn130Cnt == 10)
					{
						PlayerPrefs.SetBool(Constants.QUEST_10P_T130, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T130_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
			}
			
			if(GameData.timeTaken <= 105 && !MainDriver.Instance.questChecker.QUEST_10P_T145)
			{
				//10 Puzzles Under 1:45 each
				if(!GameData.isTimeChallenge || (GameData.isTimeChallenge && GameData.isChallengeCompleted))
				{
					MainDriver.Instance.puzzleIn145Cnt++; 
					PlayerPrefs.SetInt(Constants.PUZZLE_UNDER_145, MainDriver.Instance.puzzleIn145Cnt);
					if(MainDriver.Instance.puzzleIn145Cnt == 10)
					{
						PlayerPrefs.SetBool(Constants.QUEST_10P_T145, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T145_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
			}
			
			
			//Consecutive Puzzle Check
			// 3 Mint Check
			if(GameData.timeTaken <= 180 && !MainDriver.Instance.questChecker.QUEST_CC5_T300)
			{
				if(!GameData.isTimeChallenge || (GameData.isTimeChallenge && GameData.isChallengeCompleted))
				{
					MainDriver.Instance.ccPuzzle300Cnt++;
					PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_300, MainDriver.Instance.ccPuzzle300Cnt);
					if(MainDriver.Instance.ccPuzzle300Cnt == 5)
					{
						PlayerPrefs.SetBool(Constants.QUEST_CC5_T300, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_CC5_T300_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(GameData.isTimeChallenge && !GameData.isChallengeCompleted)
				{
					MainDriver.Instance.ccPuzzle300Cnt = 0;
					PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_300, 0);
				}

			}
			else if(GameData.timeTaken > 180 && !MainDriver.Instance.questChecker.QUEST_CC5_T300)
			{
				MainDriver.Instance.ccPuzzle300Cnt = 0;
				PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_300, 0);
			}

			
			// 2:45 Check
			if(GameData.timeTaken <= 165 && !MainDriver.Instance.questChecker.QUEST_CC5_T245)
			{
				if(!GameData.isTimeChallenge || (GameData.isTimeChallenge && GameData.isChallengeCompleted))
				{
					MainDriver.Instance.ccPuzzle245Cnt++;
					PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_245, MainDriver.Instance.ccPuzzle245Cnt);
					if(MainDriver.Instance.ccPuzzle245Cnt == 5)
					{
						PlayerPrefs.SetBool(Constants.QUEST_CC5_T245, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_CC5_T245_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(GameData.isTimeChallenge && !GameData.isChallengeCompleted)
				{
					MainDriver.Instance.ccPuzzle245Cnt = 0;
					PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_245, 0);
				}

			}
			else if(GameData.timeTaken > 165 && !MainDriver.Instance.questChecker.QUEST_CC5_T245)
			{
				MainDriver.Instance.ccPuzzle245Cnt = 0;
				PlayerPrefs.SetInt(Constants.PUZZLE_CONSECUTIVE_245, 0);
			}

		}
		
		//		Debug.Log("NO of diamond = " + noOfDiamonds );
		
		PlayerPrefs.SetInt(Constants.KEY_GAME_COUNT, MainDriver.Instance.gameCount);
		PlayerPrefs.SetInt(Constants.KEY_PUZZLE_SOLVED, MainDriver.Instance.puzzleSolved);

		if(noOfDiamonds == 1)
		{
			MainDriver.Instance.currentDiamondCnt += noOfDiamonds;
			MainDriver.Instance.lifeTimeDiamondCnt += noOfDiamonds;
			PlayerPrefs.SetInt(Constants.KEY_CURRENT_DIAMONDS, MainDriver.Instance.currentDiamondCnt);
			PlayerPrefs.SetInt(Constants.KEY_LIFETIME_DIAMONDS, MainDriver.Instance.lifeTimeDiamondCnt);
			
			string themeDiamondKey = Utility.KeyForDiamondsOfTheme(GameData.mainThemeIndex);
			//			Debug.Log("themeDiamondKey = " + themeDiamondKey);
			int themeDiamondCnt = PlayerPrefs.GetInt(themeDiamondKey, 0);
			themeDiamondCnt += noOfDiamonds;
			PlayerPrefs.SetInt(themeDiamondKey, themeDiamondCnt);
			
			string puzzleDiamondKey = Utility.KeyForDiamondsOfPuzzle(GameData.mainThemeIndex, GameData.puzzleID);
			//			Debug.Log("puzzleDiamondKey = " + puzzleDiamondKey);
			int puzzleDiamondCnt = PlayerPrefs.GetInt(puzzleDiamondKey, 0);
			if(puzzleDiamondCnt == 0 && noOfDiamonds == 1)
			{
				//Quest check
				MainDriver.Instance.diamondPuzzleCnt++;
				PlayerPrefs.SetInt(Constants.PUZZLE_WITH_DIAMONDS, MainDriver.Instance.diamondPuzzleCnt);
				if(MainDriver.Instance.diamondPuzzleCnt == 10)
				{
					//Earn a diamond on 10 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_10)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_10, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_10_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(MainDriver.Instance.diamondPuzzleCnt == 15)
				{
					//Earn a diamond on 15 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_15)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_15, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_15_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(MainDriver.Instance.diamondPuzzleCnt == 20)
				{
					//Earn a diamond on 20 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_20)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_20, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_20_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(MainDriver.Instance.diamondPuzzleCnt == 25)
				{
					//Earn a diamond on 25 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_25)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_25, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_25_ID);
						MainDriver.Instance.AwardDiamonds(Constants.QUEST_AWARD);
					}
				}
				else if(MainDriver.Instance.diamondPuzzleCnt == 50)
				{
					//Earn a diamond on 50 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_50)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_50, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_50_ID);
						MainDriver.Instance.AwardDiamonds(25);
					}
				}
				else if(MainDriver.Instance.diamondPuzzleCnt == 100)
				{
					//Earn a diamond on 50 Different Puzzles
					if(!MainDriver.Instance.questChecker.QUEST_DIAMOND_PUZZLE_100)
					{
						PlayerPrefs.SetBool(Constants.QUEST_DIAMOND_PUZZLE_100, true);
						MainDriver.Instance.questChecker.UpdateQuestStatus();
						MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_100_ID);
						MainDriver.Instance.AwardDiamonds(50);
					}
				}
				
				
			}
			puzzleDiamondCnt += noOfDiamonds;
			PlayerPrefs.SetInt(puzzleDiamondKey, puzzleDiamondCnt);
		}
		
		PlayerPrefs.Flush();
		
	}
	
	
	
	
	int GetDiamondForSeconds(int timeTaken)
	{
		if(GameData.gameMode == EGameMode.KidsMode)
		{
			return 0;
		}
		if(timeTaken <= 120)
		{
			//If level finished within 2:00 minute award 1 Key
			return 1;
			
		}
		else
		{
//			if(MainDriver.Instance.puzzleSolved == 1)
//			{
//				return 1;
//			}
			if(!MainDriver.Instance.questChecker.QUEST_FIRST_PUZZLE)
			{
				return 1;
			}
			return 0;
		}
	}
	
	
}
