using UnityEngine;
using System.Collections;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class QuestCheck : MonoBehaviour {

	[HideInInspector]
	public bool QUEST_FIRST_PUZZLE, QUEST_FB_LOGIN, QUEST_FRIEND_SHARE, QUEST_INVITE_FRIENDS, QUEST_UNLOCK_ALL;
	[HideInInspector]
	public bool	QUEST_DAILY_CHAMP, QUEST_10P_T115, QUEST_10P_T130, QUEST_10P_T145, QUEST_CC5_T245, QUEST_CC5_T300;
	[HideInInspector]
	public bool QUEST_DIAMOND_PUZZLE_10, QUEST_DIAMOND_PUZZLE_15, QUEST_DIAMOND_PUZZLE_20, QUEST_DIAMOND_PUZZLE_25, QUEST_DIAMOND_PUZZLE_50, QUEST_DIAMOND_PUZZLE_100;


	// Use this for initialization
	void Start () 
	{
		UpdateQuestStatus();
	}

	public void UpdateQuestStatus()
	{
		QUEST_FIRST_PUZZLE = PlayerPrefs.GetBool(Constants.QUEST_FIRST_PUZZLE, false);
		QUEST_FB_LOGIN = PlayerPrefs.GetBool(Constants.QUEST_FB_LOGIN, false);
		QUEST_FRIEND_SHARE = PlayerPrefs.GetBool(Constants.QUEST_FRIEND_SHARE, false);
		QUEST_INVITE_FRIENDS = PlayerPrefs.GetBool(Constants.QUEST_INVITE_FRIENDS, false);
		QUEST_UNLOCK_ALL = PlayerPrefs.GetBool(Constants.QUEST_UNLOCK_ALL, false);
		QUEST_DAILY_CHAMP = PlayerPrefs.GetBool(Constants.QUEST_DAILY_CHAMP, false);
		QUEST_10P_T115 = PlayerPrefs.GetBool(Constants.QUEST_10P_T115, false);
		QUEST_10P_T130 = PlayerPrefs.GetBool(Constants.QUEST_10P_T130, false);
		QUEST_10P_T145 = PlayerPrefs.GetBool(Constants.QUEST_10P_T145, false);
		QUEST_CC5_T245 = PlayerPrefs.GetBool(Constants.QUEST_CC5_T245, false);
		QUEST_CC5_T300 = PlayerPrefs.GetBool(Constants.QUEST_CC5_T300, false);
		QUEST_DIAMOND_PUZZLE_10 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_10, false);
		QUEST_DIAMOND_PUZZLE_15 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_15, false);
		QUEST_DIAMOND_PUZZLE_20 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_20, false);
		QUEST_DIAMOND_PUZZLE_25 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_25, false);
		QUEST_DIAMOND_PUZZLE_50 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_50, false);
		QUEST_DIAMOND_PUZZLE_100 = PlayerPrefs.GetBool(Constants.QUEST_DIAMOND_PUZZLE_100, false);
		PlayerPrefs.Flush();
	}

	public void PostAchievements()
	{
		if(QUEST_FIRST_PUZZLE)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_FIRST_PUZZLE_ID);
		}
		if(QUEST_FB_LOGIN)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_FB_LOGIN_ID);
		}
		if(QUEST_FRIEND_SHARE)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_FRIEND_SHARE_ID);
		}
		if(QUEST_INVITE_FRIENDS)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_INVITE_FRIENDS_ID);
		}
		if(QUEST_UNLOCK_ALL)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_UNLOCK_ALL_ID);
		}
		if(QUEST_DAILY_CHAMP)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DAILY_CHAMP_ID);
		}
		if(QUEST_10P_T115)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T115_ID);
		}
		if(QUEST_10P_T130)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T130_ID);
		}
		if(QUEST_10P_T145)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_10P_T145_ID);
		}
		if(QUEST_CC5_T245)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_CC5_T245_ID);
		}
		if(QUEST_CC5_T300)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_CC5_T300_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_10)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_10_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_15)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_15_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_20)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_20_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_25)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_25_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_50)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_50_ID);
		}
		if(QUEST_DIAMOND_PUZZLE_100)
		{
			MainDriver.Instance.PostAchievement(Constants.QUEST_DIAMOND_PUZZLE_100_ID);
		}


	}
}
