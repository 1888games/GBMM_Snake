using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviourSingleton<GameController>
{


	public float SnakeSpeed = 1f;
	public float StartSpeed = 0.5f;
	public float ChangePerFood = 0.04f;
	public float MaxSpeed = 0.2f;
	

	public Vector2Int SnakeDirection;
	public Vector2Int SnakePosition;
	public List<Vector2Int> SnakeTrail;

	public bool GameReady = false;

	public int FoodCollected = 0;
	
    // Start is called before the first frame update
    void Start()
    {
		Invoke ("Initialise", 0.05f);
    }

    // Update is called once per frame
    void Update()
    {


		if (GameReady) {

			CheckInput ();

		}
    	
        
    }


	void CheckInput () {

		if (Input.GetAxis ("Horizontal") > 0 && SnakeDirection != new Vector2Int(-1, 0)) {
			SnakeDirection = new Vector2Int (1, 0);

		}
		
		if (Input.GetAxis ("Horizontal") < 0 && SnakeDirection != new Vector2Int(1, 0)) {
			SnakeDirection = new Vector2Int (-1, 0);

		}
		
			
		if (Input.GetAxis ("Vertical") > 0 && SnakeDirection != new Vector2Int(0, 1)){
			SnakeDirection = new Vector2Int (0, -1);

		}
		
		if (Input.GetAxis ("Vertical") < 0 && SnakeDirection != new Vector2Int(0, -1)) {
			SnakeDirection = new Vector2Int (0, 1);

		}


	}


	void PositionFood () {

		List<Vector2Int> Tried = new List<Vector2Int> ();

		bool ok = false;
		int attempts = 0;

		while (ok == false && attempts < 100) {

			int x = Random.Range (0, ScreenController.Instance.Columns);
			int y = Random.Range (0, ScreenController.Instance.Rows);

			if (Tried.Contains (new Vector2Int (x, y)) == false) {

				if (ScreenController.Instance.GetPixelName (x, y) == "None" && Vector2Int.Distance(SnakePosition, new Vector2Int(x,y)) > 3) {

					ScreenController.Instance.LightPixel (x, y, "Food");
					//Debug.Log ("FOOD: " + x + " / " + y + " ATT: " + attempts);
		
					ok = true;

				} else {
					Tried.Add (new Vector2Int (x, y));
				}

			}

			attempts++;

			//Debug.Log (attempts);

		}
		
		


	}


	void MoveSnake () {
	
		Vector2Int NextSnakePosition = SnakePosition + SnakeDirection;
		
		if (NextSnakePosition.x > ScreenController.Instance.Columns) {
			NextSnakePosition.x = 0;
		}

		if (NextSnakePosition.x < 0) {
			NextSnakePosition.x = ScreenController.Instance.Columns;
		}
		
		if (NextSnakePosition.y > ScreenController.Instance.Rows) {
			NextSnakePosition.y = 0;
		}

		if (NextSnakePosition.y < 0) {
			NextSnakePosition.y = ScreenController.Instance.Rows;
		}
		
		
		string nextPixel = ScreenController.Instance.GetPixelName (NextSnakePosition.x, NextSnakePosition.y);
		
		DeleteSnake ();

		if (nextPixel == "SnakeTail") {
			ScreenController.Instance.ClearScreen (true);
			Debug.Log ("GAME OVER!!!!!");
			CancelInvoke ();

			Invoke ("ResetGame", 3f);
			return;
		}



		//Debug.Log (nextPixel);

		


		for (int i = SnakeTrail.Count - 1; i > 0; i--) {

			SnakeTrail [i] = SnakeTrail [i - 1];
		}
		
		SnakeTrail [0] = SnakePosition;

		SnakePosition = NextSnakePosition;
		
		
		

		DrawSnake ();
		
		if (nextPixel == "Food") {

			FoodCollected++;
			ScreenController.Instance.PlayNote (112, 127, 250);
		
			AddTailPiece ();
			//Debug.Log (FoodCollected + " Snake Length " + (SnakeTrail.Count + 1).ToString());
			PositionFood ();
			SnakeSpeed -= ChangePerFood;

			if (SnakeSpeed < MaxSpeed) {
				SnakeSpeed = MaxSpeed;
			}
			
		}

		Invoke ("MoveSnake", SnakeSpeed);

	

	}

	void Initialise () {


		ScreenController.Instance.Initialise ();
		

		ResetGame ();
		
	

	}


	void DeleteSnake () {
		
		ScreenController.Instance.HidePixel (SnakePosition.x, SnakePosition.y);

		foreach (Vector2Int snakePiece in SnakeTrail) {

			if (ScreenController.Instance.GetPixelName (snakePiece.x, snakePiece.y) == "SnakeTail") {

				ScreenController.Instance.HidePixel (snakePiece.x, snakePiece.y);
			}
		}

	}

	void AddTailPiece () {

		Vector2Int EndOfTail = SnakeTrail [SnakeTrail.Count - 1];

		for (int x = -1; x < 2; x++) {

			
			for (int y = -1; y < 2; y++) {
			
				if (x != 0 && y != 0) {
					continue;
				}

				Vector2Int Position = EndOfTail + new Vector2Int (x, y);

				//Debug.Log (Position + " " + ScreenController.Instance.GetPixelName (Position.x, Position.y));

				if (ScreenController.Instance.GetPixelName (Position.x, Position.y) == "None") {

					SnakeTrail.Add (Position);
					ScreenController.Instance.LightPixel (Position.x, Position.y, "SnakeTail");
					return;
				}

			}
		}

	}

	void DrawSnake () {
	
		

		ScreenController.Instance.LightPixel (SnakePosition.x, SnakePosition.y, "SnakeHead");

		foreach (Vector2Int snakePiece in SnakeTrail) {

			if (ScreenController.Instance.GetPixelName (snakePiece.x, snakePiece.y) != "Food") {

				ScreenController.Instance.LightPixel (snakePiece.x, snakePiece.y, "SnakeTail");
			}
		}

	}

	void ResetGame () {

		SnakeTrail = new List<Vector2Int> ();

		ScreenController.Instance.ClearScreen ();

		SnakeTrail.Add (SnakePosition - SnakeDirection);
		
		SnakeSpeed = StartSpeed;

		MoveSnake ();
		FoodCollected = 0;
		//AddTailPiece ();
		PositionFood ();
		
		GameReady = true;

	

		
	}
	
	
	
	
	
}
