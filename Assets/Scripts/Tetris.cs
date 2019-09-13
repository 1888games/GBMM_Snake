using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour {



	public class Shape {

		public string Name;
		public Vector2Int Position;
		public int Rotation;
		public string PlacedName;
		public bool Drawn;

		public Shape (string name) {

			PlacedName = "BlockInPlay";
			Position = new Vector2Int (3, -1);
			Rotation = Random.Range (0, 3);
			Name = name;

			Drawn = false;



		}

	}

	Dictionary<string, Dictionary<int, List<Vector3Int>>> Shapes;
	List<string> ShapeNames;

	public Shape ShapeInPlay;

	public bool GameIsActive = false;

	public float DropTime = 1f;
	public float MaxDropTime = 2f;
	public float MinDropTime = 0.5f;

	public float PreviousXAxis = 0f;
	public float PreviousYAxis = 0f;

	Dictionary<int, int> RowsToDrop;

	public int BestScore = 0;
	public int Score = 0;
	public bool Initialised = false;

	// Start is called before the first frame update
	void Start () {

		SetupShapes ();


		BestScore = PlayerPrefs.GetInt ("BestTetris", 0);


		Invoke ("Initialise", 0.05f);



	}




	// Update is called once per frame
	void Update () {


		if (GameIsActive) {

			if (Input.GetKeyDown (KeyCode.Space) || Input.GetButtonDown ("Fire1")) {

				RotateBlock ();
			}

			if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetAxis ("Horizontal") < 0f && PreviousXAxis >= 0f) {

				ShiftBlock (-1);

			}

			if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetAxis ("Horizontal") > 0f && PreviousXAxis <= 0f) {

				ShiftBlock (1);

			}

			if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetAxis ("Vertical") > 0f && PreviousYAxis <= 0f) {

				DropShape ();

			}

			PreviousXAxis = Input.GetAxis ("Horizontal");
			PreviousYAxis = Input.GetAxis ("Vertical");


		} else {

			if (Input.anyKeyDown && Initialised) {

				CancelInvoke ();
				ResetGame ();
			}
		}

	}


	void ShiftBlock (int moveBy) {

		int newXPosition = ShapeInPlay.Position.x + moveBy;

		if (newXPosition < -1) {
			Debug.Log ("Too far left");
			return;
		}

		if (newXPosition > ScreenController.Instance.Columns - 1) {
			Debug.Log ("Too far right");
			return;
		}

		List<Vector3Int> shapeData = Shapes [ShapeInPlay.Name] [ShapeInPlay.Rotation];

		Debug.Log (newXPosition);

		

			if (newXPosition == -1) {

			foreach (Vector3Int row in shapeData) {

				if (row.x == 1) {
					return;
				}
			}
		}

		if (newXPosition == ScreenController.Instance.Columns - 1) {

			foreach (Vector3Int row in shapeData) {

				if (row.z == 1) {
					return;
				}
			}
		}

		DeleteShape (ShapeInPlay);

		bool MoveShape = true;

		for (int y = 0; y < shapeData.Count; y++) {
			Vector3Int row = shapeData [y];

			if (ShapeInPlay.Position.y + y >= 0) {

				if (row.x == 1 && ScreenController.Instance.GetPixelName (newXPosition + 0, ShapeInPlay.Position.y + y) != "None") {
					MoveShape = false;
				}

				if (row.y == 1 && ScreenController.Instance.GetPixelName (newXPosition + 1, ShapeInPlay.Position.y + y) != "None") {
					MoveShape = false;
				}

				if (row.z == 1 && ScreenController.Instance.GetPixelName (newXPosition + 2, ShapeInPlay.Position.y + y) != "None") {
					MoveShape = false;
				}

			}

		}


		if (MoveShape) {
			ShapeInPlay.Position.x += moveBy;
		}

		DrawShape (ShapeInPlay);
			

	}


	void RotateBlock () {

		if (ShapeInPlay != null) {

			DeleteShape (ShapeInPlay);

			int originalRotation = ShapeInPlay.Rotation;

			ShapeInPlay.Rotation++;

			if (ShapeInPlay.Rotation == 4) {
				ShapeInPlay.Rotation = 0;
			}

			bool success = DrawShape (ShapeInPlay);

			if (success == false) {
				Debug.Log ("Abandon...");
				DeleteShape (ShapeInPlay);
				ShapeInPlay.Rotation = originalRotation;
				DrawShape (ShapeInPlay);
			}

		}

	}


	void Initialise () {

		ScreenController.Instance.Initialise ();

		//ResetGame ();


		DisplayHighScore ();

		Initialised = true;

		

	}


	void DropShape () {

		int newYPosition = ShapeInPlay.Position.y + 1;


		List<Vector3Int> shapeData = Shapes [ShapeInPlay.Name] [ShapeInPlay.Rotation];

		bool doDrop = true;

		for (int y = 0; y < shapeData.Count; y++) {
			Vector3Int row = shapeData [y];

			if (row.x + row.y + row.z > 0 && newYPosition + y > ScreenController.Instance.Rows) {

				doDrop = false;
				Debug.Log ("OUT OF ROWS....");

			}

			if (row.x == 1) {
				if (ScreenController.Instance.GetPixelName (ShapeInPlay.Position.x, newYPosition + y) == "BlockPlaced") {
					doDrop = false;
				}

			}

			if (row.y == 1) {
				if (ScreenController.Instance.GetPixelName (ShapeInPlay.Position.x + 1, newYPosition + y) == "BlockPlaced") {
					doDrop = false;
				}
			}

			if (row.z == 1) {
				if (ScreenController.Instance.GetPixelName (ShapeInPlay.Position.x + 2, newYPosition + y) == "BlockPlaced") {
					doDrop = false;
				}
			}
		}


		if (doDrop) {

			DeleteShape (ShapeInPlay);
			ShapeInPlay.Position.y++;
			DrawShape (ShapeInPlay);
			CancelInvoke ();
			Invoke ("DropShape", DropTime);


		} else {

			bool gameOver = false;
			for (int y = 0; y < shapeData.Count; y++) {
				Vector3Int row = shapeData [y];

				if (row.x + row.y + row.z > 0 && ShapeInPlay.Position.y + y < 0) {
					gameOver = true;
				}

			}

			if (gameOver) {

				CancelInvoke ();
				GameOver ();

			} else {


				ShapeInPlay.PlacedName = "BlockPlaced";
				DeleteShape (ShapeInPlay);
				DrawShape (ShapeInPlay);
				CancelInvoke ();
				CheckCompletedRows ();
				NewShape ();

			}

		}


	}


	void GameOver () {

		ScreenController.Instance.ClearScreen (true);
		Debug.Log ("GAME OVER!!!!!");
		CancelInvoke ();
		GameIsActive = false;

		PlayerPrefs.SetInt ("BestTetris", BestScore);

		Initialised = false;

		Invoke ("DisplayScore", 1.5f);
		Invoke ("DisplayHighScore", 4f);


	}


	void DisplayScore () {

		ScreenController.Instance.DisplayNumber (Score);
		Initialised = true;

	}


	void DisplayHighScore () {

		ScreenController.Instance.DisplayNumber (BestScore);


	}

	void CheckCompletedRows () {

		int rowsComplete = 0;

		RowsToDrop = new Dictionary<int, int> ();

		for (int y = ScreenController.Instance.Rows; y >= 0; y--) {

			RowsToDrop.Add (y, rowsComplete);

			bool rowComplete = true;

			for (int x = 0; x <= ScreenController.Instance.Columns; x++) {

				if (ScreenController.Instance.GetPixelName (x, y) != "BlockPlaced") {
					rowComplete = false;

				}


			}


			if (rowComplete) {
				rowsComplete++;
				RowsToDrop [y] = -1;
			}


		}

		foreach (int y in RowsToDrop.Keys) {

			int rowsToDropThisRow = RowsToDrop [y];

			for (int x = 0; x <= ScreenController.Instance.Columns; x++) {

				bool pixelHere = ScreenController.Instance.GetPixelName (x, y) == "BlockPlaced";

				if (rowsToDropThisRow != 0) {
					ScreenController.Instance.HidePixel (x, y);

					if (rowsToDropThisRow > 0 && pixelHere) {

						ScreenController.Instance.LightPixel (x, y + rowsToDropThisRow, "BlockPlaced");
					}
				}


			}




		}

		Score = Score + rowsComplete * rowsComplete;


		if (Score > BestScore) {
			BestScore = Score;
		}

		DropTime = Mathf.Max (MinDropTime, DropTime - 0.025f);



	}

	void NewShape () {

		string name = ShapeNames [Random.Range (0, ShapeNames.Count)];

		Shape newShape = new Shape (name);

		ShapeInPlay = newShape;



		DrawShape (newShape);

		Invoke ("DropShape", DropTime);


	}

	void DeleteShape (Shape shape) {

		List<Vector3Int> shapeData = Shapes [shape.Name] [shape.Rotation];

		for (int y = 0; y < shapeData.Count; y++) {

			Vector3Int row = shapeData [y];

			if (row.x == 1) {
				ScreenController.Instance.HidePixel (shape.Position.x, shape.Position.y + y);
			}

			if (row.y == 1) {
				ScreenController.Instance.HidePixel (shape.Position.x + 1, shape.Position.y + y);
			}

			if (row.z == 1) {
				ScreenController.Instance.HidePixel (shape.Position.x + 2, shape.Position.y + y);
			}

		}
	}

	bool DrawShape (Shape shape) {

		List<Vector3Int> shapeData = Shapes [shape.Name] [shape.Rotation];

		for (int y = 0; y < shapeData.Count; y++) {

			Vector3Int row = shapeData [y];

			if (row.x + row.y + row.z > 0 && shape.Position.y + y > ScreenController.Instance.Rows) {
				return false;
			}

			if (row.x == 1) {

				if (shape.Position.x < 0 || shape.Position.x >= ScreenController.Instance.Columns + 1) {
					return false;
				}

				if (shape.PlacedName == "BlockInPlay") {
					ScreenController.Instance.FlashPixel (shape.Position.x, shape.Position.y + y, shape.PlacedName, 0.1f);
				} else {
					ScreenController.Instance.LightPixel (shape.Position.x, shape.Position.y + y, shape.PlacedName);
				}
			}

			if (row.y == 1) {

				if (shape.Position.x + 1 < 0 || shape.Position.x + 1 >= ScreenController.Instance.Columns + 1) {
					return false;
				}
				if (shape.PlacedName == "BlockInPlay") {
					ScreenController.Instance.FlashPixel (shape.Position.x + 1, shape.Position.y + y, shape.PlacedName, 0.1f);
				} else {
					ScreenController.Instance.LightPixel (shape.Position.x + 1, shape.Position.y + y, shape.PlacedName);
				}
			}

			if (row.z == 1) {

				if (shape.Position.x + 2 < 0 || shape.Position.x + 2 >= ScreenController.Instance.Columns + 1) {
					return false;
				}

				if (shape.PlacedName == "BlockInPlay") {
					ScreenController.Instance.FlashPixel (shape.Position.x + 2, shape.Position.y + y, shape.PlacedName, 0.1f);
				} else {
					ScreenController.Instance.LightPixel (shape.Position.x + 2, shape.Position.y + y, shape.PlacedName);
				}
			}

		}

		return true;


	}

	void ResetGame () {

		ScreenController.Instance.ClearScreen ();

	


		DropTime = MaxDropTime;
		Score = 0;

		NewShape ();

		GameIsActive = true;

	}

	void SetupShapes () {

		Shapes = new Dictionary<string, Dictionary<int, List<Vector3Int>>> ();
		ShapeNames = new List<string> ();

		string shapeName = "Box";

		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 0, 0));

		shapeName = "r";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (1, 0, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 1));

		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));

		shapeName = "L";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 1));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 0, 0));

		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));


		shapeName = "Line";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));

		shapeName = "S";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 0, 1));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 0));

		Shapes [shapeName] [3].Add (new Vector3Int (1, 0, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));

		shapeName = "T";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 1));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 1, 0));

		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));

		shapeName = "Z";
		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [0].Add (new Vector3Int (0, 0, 0));

		Shapes [shapeName] [1].Add (new Vector3Int (0, 0, 1));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 1));
		Shapes [shapeName] [1].Add (new Vector3Int (0, 1, 0));

		Shapes [shapeName] [2].Add (new Vector3Int (0, 0, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [2].Add (new Vector3Int (0, 1, 1));

		Shapes [shapeName] [3].Add (new Vector3Int (0, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [3].Add (new Vector3Int (1, 0, 0));







	}
}


