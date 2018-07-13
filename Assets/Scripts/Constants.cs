using UnityEngine;
using System.Collections;

public class Constants  {

	//Play Services ID

	#if UNITY_ANDROID

	#if AMAZON
	public const string LEADER_BOARD_ID = "puzzle_solved";	//Amazon

	//For Amazon
	public const string QUEST_FIRST_PUZZLE_ID = "learner";
	public const string QUEST_FB_LOGIN_ID = "go_social";
	public const string QUEST_FRIEND_SHARE_ID = "sharing";
	public const string QUEST_INVITE_FRIENDS_ID = "invite_friends";
	public const string QUEST_UNLOCK_ALL_ID = "wordsearch_champ";
	public const string QUEST_DAILY_CHAMP_ID = "daily_champ";
	public const string QUEST_10P_T115_ID = "ten_oneminfif";
	public const string QUEST_10P_T130_ID = "ten_oneminthir";
	public const string QUEST_10P_T145_ID = "ten_oneminfour";
	public const string QUEST_CC5_T245_ID = "five_two";
	public const string QUEST_CC5_T300_ID = "five_three";
	public const string QUEST_DIAMOND_PUZZLE_10_ID = "ten_ir";
	public const string QUEST_DIAMOND_PUZZLE_15_ID = "onefive_ir";
	public const string QUEST_DIAMOND_PUZZLE_20_ID = "twoo_ir";
	public const string QUEST_DIAMOND_PUZZLE_25_ID = "twofif_ir";
	public const string QUEST_DIAMOND_PUZZLE_50_ID = "fifity_r";
	public const string QUEST_DIAMOND_PUZZLE_100_ID = "huhd_r";

	#else
	public const string LEADER_BOARD_ID = "CgkI2sf28r8WEAIQBw";

	public const string QUEST_FIRST_PUZZLE_ID = "CgkI2sf28r8WEAIQAQ";
	public const string QUEST_FB_LOGIN_ID = "CgkI2sf28r8WEAIQAg";
	public const string QUEST_FRIEND_SHARE_ID = "CgkI2sf28r8WEAIQAw";
	public const string QUEST_INVITE_FRIENDS_ID = "CgkI2sf28r8WEAIQBA";
	public const string QUEST_UNLOCK_ALL_ID = "CgkI2sf28r8WEAIQBQ";
	public const string QUEST_DAILY_CHAMP_ID = "CgkI2sf28r8WEAIQBg";
	public const string QUEST_10P_T115_ID = "CgkI2sf28r8WEAIQCA";
	public const string QUEST_10P_T130_ID = "CgkI2sf28r8WEAIQCQ";
	public const string QUEST_10P_T145_ID = "CgkI2sf28r8WEAIQCg";
	public const string QUEST_CC5_T245_ID = "CgkI2sf28r8WEAIQCw";
	public const string QUEST_CC5_T300_ID = "CgkI2sf28r8WEAIQDA";
	public const string QUEST_DIAMOND_PUZZLE_10_ID = "CgkI2sf28r8WEAIQDQ";
	public const string QUEST_DIAMOND_PUZZLE_15_ID = "CgkI2sf28r8WEAIQDg";
	public const string QUEST_DIAMOND_PUZZLE_20_ID = "CgkI2sf28r8WEAIQDw";
	public const string QUEST_DIAMOND_PUZZLE_25_ID = "CgkI2sf28r8WEAIQEA";
	public const string QUEST_DIAMOND_PUZZLE_50_ID = "CgkI2sf28r8WEAIQEQ";
	public const string QUEST_DIAMOND_PUZZLE_100_ID = "CgkI2sf28r8WEAIQEg";
	#endif

	#elif UNITY_IOS
	public const string LEADER_BOARD_ID = "puzzle_solved";

	public const string QUEST_FIRST_PUZZLE_ID = "learner";
	public const string QUEST_FB_LOGIN_ID = "go_social";
	public const string QUEST_FRIEND_SHARE_ID = "sharing";
	public const string QUEST_INVITE_FRIENDS_ID = "invite_friends";
	public const string QUEST_UNLOCK_ALL_ID = "wordsearch_champ";
	public const string QUEST_DAILY_CHAMP_ID = "daily_champ";
	public const string QUEST_10P_T115_ID = "ten_oneminfif";
	public const string QUEST_10P_T130_ID = "ten_oneminthir";
	public const string QUEST_10P_T145_ID = "ten_oneminfour";
	public const string QUEST_CC5_T245_ID = "five_two";
	public const string QUEST_CC5_T300_ID = "five_three";
	public const string QUEST_DIAMOND_PUZZLE_10_ID = "ten_ir";
	public const string QUEST_DIAMOND_PUZZLE_15_ID = "onefive_ir";
	public const string QUEST_DIAMOND_PUZZLE_20_ID = "twoo_ir";
	public const string QUEST_DIAMOND_PUZZLE_25_ID = "twofif_ir";
	public const string QUEST_DIAMOND_PUZZLE_50_ID = "fifity_r";
	public const string QUEST_DIAMOND_PUZZLE_100_ID = "huhd_r";
	#endif

	public const int QUEST_AWARD = 5;

	public const int FREE_THEMES = 5;
//	public const int  LOCKED_THEMES = 16;
	public const int TOTAL_THEMES = 33;

	//Settings Keys for Player Prefs
	public const string KEY_SOUND = "Sound";
	public const string KEY_NOTIFICATIONS = "Notif";
	public const string KEY_GAME_COUNT = "Games";
	public const string KEY_PUZZLE_SOLVED = "Solved";
	public const string KEY_CURRENT_DIAMONDS = "Diamonds";
	public const string KEY_LIFETIME_DIAMONDS = "LifeDiam";





	//Quests related constants 
	public const string QUEST_FIRST_PUZZLE = "Q1";
	public const string QUEST_FB_LOGIN = "Q2";
	public const string QUEST_FRIEND_SHARE = "Q3";
	public const string QUEST_INVITE_FRIENDS = "Q4";
	public const string QUEST_UNLOCK_ALL = "Q5";
	public const string QUEST_DAILY_CHAMP = "Q6";
	public const string QUEST_10P_T115 = "Q7";
	public const string QUEST_10P_T130 = "Q8";
	public const string QUEST_10P_T145 = "Q9";
	public const string QUEST_CC5_T245 = "Q10";
	public const string QUEST_CC5_T300 = "Q11";
	public const string QUEST_DIAMOND_PUZZLE_10 = "Q12";
	public const string QUEST_DIAMOND_PUZZLE_15 = "Q13";
	public const string QUEST_DIAMOND_PUZZLE_20 = "Q14";
	public const string QUEST_DIAMOND_PUZZLE_25 = "Q15";
	public const string QUEST_DIAMOND_PUZZLE_50 = "Q16";
	public const string QUEST_DIAMOND_PUZZLE_100 = "Q17";

	public const string UNLOCKED_THEMES_COUNT = "unlthm";
	public const string DAILY_CHALLENGE_COUNT = "dcc";
	public const string PUZZLE_WITH_DIAMONDS = "pwd";
	public const string PUZZLE_UNDER_115 = "pu115";
	public const string PUZZLE_UNDER_130 = "pu130";
	public const string PUZZLE_UNDER_145 = "pu145";
	public const string PUZZLE_CONSECUTIVE_245 = "pcc245";
	public const string PUZZLE_CONSECUTIVE_300 = "pcc300";


	//Noification related constants
	public const int H4 = 4;
	public const int H8 = 8;
	public const int D1 = 1;
	public const int D3 = 3;
	public const int D7 = 7;
	public const int D14 = 14;
	public const int D30 = 30;

}
