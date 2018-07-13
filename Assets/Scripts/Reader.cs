using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;

public class Reader 
{

//	string questFileName = "Eng_QuestXml";
	string questFileName = "AllQuestXml";
	public List<Quest> QuestDataList;

//	string mainLevelFileName = "EnglishLevels";
	string mainLevelFileName = "EnglishThemes";
	public List<Level> ThemeDataList;

//	string subLevelFileName = "englishPuzzle.txt";
	string subLevelFileName = "AllPuzzle.txt";
	public List <string[]> SubLevelDetails;

	string kidsLevelFileName = "KidsLevels";
	public List <string> kidsLevelDetail;

	private static readonly Reader instance = new Reader ();


	public static Reader Instance 
	{
		get 
		{
			return instance;
		}
	}
	
//	static Reader ()
//	{
//		
//	}
	
	private Reader ()
	{
		QuestDataList = new List<Quest>();
		ThemeDataList = new List<Level>();
		SubLevelDetails = new List<string[]>();
		kidsLevelDetail = new List<string>();

		ReadDataAsPerLanguage();
	}


	public void ReadDataAsPerLanguage()
	{
//		GetFilesAsPerLanguage();
		ReadAllCategories();
		ReadSubCategories();
		ReadAllQuests();
		ReadKidsLevel();
	}


	void ReadAllQuests()
	{
		try
		{
			//Clear previous data 
			QuestDataList.Clear();
//			Debug.Log("ReadAllQuests");
			//Read all quest details
			XmlDocument questXmlDoc = new XmlDocument();
			TextAsset questTextXml = (TextAsset)Resources.Load("LevelDetails/"+questFileName, typeof(TextAsset));
			questXmlDoc.LoadXml(questTextXml.text);

			foreach(XmlElement node in questXmlDoc.SelectNodes("Quests/Quest"))
			{
				Quest questDetail = new Quest();
				questDetail.diamondCnt = int.Parse(node.SelectSingleNode("count").InnerText);
				questDetail.details = node.SelectSingleNode("details").InnerText;
				QuestDataList.Add(questDetail);
//				Debug.Log("Quest = " + questDetail.details);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
		}
		
	}

	void ReadAllCategories()
	{
		try
		{
			//Clear previous data 
			ThemeDataList.Clear();
			
			//Read all Categories details for current language
			XmlDocument questXmlDoc = new XmlDocument();
			TextAsset questTextXml = (TextAsset)Resources.Load("LevelDetails/"+mainLevelFileName, typeof(TextAsset));
			questXmlDoc.LoadXml(questTextXml.text);
			
			foreach(XmlElement node in questXmlDoc.SelectNodes("Levels/Level"))
			{
				Level levelDetail = new Level();
				levelDetail.category = node.SelectSingleNode("category").InnerText;
				levelDetail.iconImg = node.SelectSingleNode("icon").InnerText;
				levelDetail.puzzleCount = int.Parse(node.SelectSingleNode("count").InnerText);
				ThemeDataList.Add(levelDetail);
//				Debug.Log("Theme category  = " + levelDetail.category + " Count - " + levelDetail.puzzleCount);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
		}
	}


	void ReadSubCategories()
	{
		try
		{
			SubLevelDetails.Clear();

			//Read all sub categories for current language
			TextAsset subLevelCsv = Resources.Load("SubLevels/" + subLevelFileName) as TextAsset;
			StringReader reader = new StringReader (subLevelCsv.text);
			string line = reader.ReadLine (); 
			int row = 0;
			
			using(reader)
			{
				while(line != null)
				{
					string[] entries = line.Split(',');
					if (entries.Length >= 0)
					{
						SubLevelDetails.Add(entries);
					}
					line = reader.ReadLine();
				}
				reader.Close();
			}
			
		}
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
		}
	}


	void ReadKidsLevel()
	{
		try
		{
			//Clear previous data 
			kidsLevelDetail.Clear();
			
			//Read all Categories details for current language
			XmlDocument questXmlDoc = new XmlDocument();
			TextAsset questTextXml = (TextAsset)Resources.Load("LevelDetails/"+kidsLevelFileName, typeof(TextAsset));
			questXmlDoc.LoadXml(questTextXml.text);
			
			foreach(XmlElement node in questXmlDoc.SelectNodes("Levels/Level"))
			{
				string levelname = node.SelectSingleNode("Name").InnerText;
//				Debug.Log("levelname = " +levelname);
				kidsLevelDetail.Add(levelname);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
		}
	}


}
