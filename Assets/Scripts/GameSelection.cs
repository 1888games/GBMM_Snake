using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelection : MonoBehaviourSingleton<GameSelection> {

	public GameObject CurrentGame;

	public GameObject TetrisPrefab;
	public GameObject SnakePrefab;

	public string Mode = "Snake";

	// Start is called before the first frame update
	void Start () {

		Invoke ("LaunchGame", 0.2f);

	}

	void LaunchGame () {

		if (CurrentGame != null) {
			Destroy (CurrentGame);
		}


		if (Mode == "Snake") {

			CurrentGame = Instantiate (SnakePrefab);

		} else {

			CurrentGame = Instantiate (TetrisPrefab);
		}



	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.S)) {
			Mode = "Snake";
			LaunchGame ();
		}

		if (Input.GetKeyDown (KeyCode.T)) {
			Mode = "Tetris";
			LaunchGame ();
		}
	}

	
}
