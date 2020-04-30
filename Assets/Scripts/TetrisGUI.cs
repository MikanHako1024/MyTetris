using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGUI : MonoBehaviour {

	public TetrisTilemap tetrisTilemap;

	// ？GUI的坐标是从左上角开始到右下角的 ...
	public Rect rect1 = new Rect(10, 10, 50, 30);
	public Rect rect2 = new Rect(70, 10, 50, 30);
	public Rect rect3 = new Rect(130, 10, 50, 30);
	GUIStyle style3 = new GUIStyle();
	
	//public int score {
	//	get { return 0; }
	//}
	public string scoreText {
		//get { return "0"; }
		get { return tetrisTilemap.GetScore().ToString(); }
	}

	private void Start () {
		tetrisTilemap = FindObjectOfType<TetrisTilemap>();
		style3.alignment = TextAnchor.MiddleCenter;
	}

	private void OnGUI() {
		/*
		if (GUI.Button(rect1, "开始")) {
			//Debug.Log("开始");
			tetrisTilemap.StartGame();
		}
		//else if (GUI.Button(rect2, "暂停")) {
		if (GUI.Button(rect2, "暂停")) {
			//Debug.Log("暂停");
			tetrisTilemap.PauseGame();
		}
		GUI.Label(rect3, "分数", style3);
		*/

		GUIButton1();
		GUIButton2();
		GUILabel1();
	}

	private void GUIButton1() {
		if (tetrisTilemap.isGameStart) {
			if (GUI.Button(rect1, "结束")) {
				tetrisTilemap.StopGame();
			}
		}
		else {
			if (GUI.Button(rect1, "开始")) {
				tetrisTilemap.StartGame();
			}
		}
	}

	private void GUIButton2() {
		if (tetrisTilemap.isGamePause) {
			if (GUI.Button(rect2, "继续")) {
				tetrisTilemap.ContinueGame();
			}
		}
		else {
			if (GUI.Button(rect2, "暂停")) {
				tetrisTilemap.PauseGame();
			}
		}
	}

	private void GUILabel1() {
		//GUI.Label(rect3, score.ToString(), style3);
		GUI.Label(rect3, scoreText, style3);
	}
}
