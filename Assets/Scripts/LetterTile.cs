using UnityEngine;
using System.Collections;

public class LetterTile : MonoBehaviour
{
    public TextMesh letter;
    public bool used = false;
    public bool solved = false;
	public bool unsolved = false;
	public bool selected = false;
    [HideInInspector]
    public int horizontal, vertical;

	Animation ShakeAnim;

    void Start()
    {
//        renderer.materials[0].color = WordSearch.Instance.defaultColor;
		ShakeAnim = GetComponentInChildren<Animation>();
    }

    void Update()
    {
//		if(Input.GetKeyDown(KeyCode.Escape)) 
//		{
//			selected = true;
//
//		}
		if(selected)
		{
			ShakeAnim.Play("ShakeLetter");
			selected = false;
//			Debug.Log("Setting true");
		}
//		if (WordSearch.Instance.ready)
//        {
//            if (!used && WordSearch.Instance.current==gameObject)
//            {
//				//If  selcted as current tile add to current word
//                WordSearch.Instance.selected.Add(this.gameObject);
////                renderer.materials[0].color = WordSearch.Instance.selectedColor;
//                WordSearch.Instance.selectedWord += letter.text;
//                used = true;
//            }
//        }
//
//
//        if (solved)
//        {
//			//If tile added to a correct word make it correct color
//            if (renderer.materials[0].color != WordSearch.Instance.correctColor)
//			{
////				renderer.materials[0].color = WordSearch.Instance.correctColor;
//			}
//                
//			//Dont return if same letter is to be used in multiple words
////            return;	
//        }
//
//        if (Input.GetMouseButtonUp(0))
//        {
//            used = false;
////			Debug.Log("GetMouseButtonUp");
////            if (renderer.materials[0].color != WordSearch.Instance.defaultColor)
////                renderer.materials[0].color = WordSearch.Instance.defaultColor;
//        }
//
//		if(unsolved)
//		{
////			Debug.Log("Unspoved = " + letter.text);
//			used = false;
//			//If selected word is wrong make again default color
//			if (!solved && renderer.materials[0].color != WordSearch.Instance.defaultColor)
//			{
////				renderer.materials[0].color = WordSearch.Instance.defaultColor;
//			}
//				
//
//			unsolved = false;
//		}
//
//
//		foreach (Touch touch in Input.touches)
//		{
//			if(Input.touchCount == 1 && (touch.phase == TouchPhase.Ended))
////			if (touch.phase == TouchPhase.Ended)
//			{
//				used = false;
////				if (renderer.materials[0].color != WordSearch.Instance.defaultColor)
////					renderer.materials[0].color = WordSearch.Instance.defaultColor;
//			}
//		}
	}
}
