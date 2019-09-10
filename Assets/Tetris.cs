using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviourSingleton<Tetris> {

	public class Shape {

		public string Name;
		public Vector2Int Position;
		public int Rotation;
		public bool Placed;

		public Shape (string name) {

			Placed = false;
			Position = new Vector2Int (3, -2);
			Rotation = Random.Range (0, 3);
			Name = name;

		}
	
	}

	Dictionary<string, Dictionary<int, List<Vector3Int>>> Shapes;
	List<string> ShapeNames;

	public List<Shape> ShapesInPlay;

	// Start is called before the first frame update
	void Start () {

		SetupShapes ();

		Invoke ("Initialise", 0.05f);

	}




	// Update is called once per frame
	void Update () {

	}


	void Initialise () {

		ScreenController.Instance.Initialise ();

		ResetGame ();

	}


	void NewShape () {

		string name = ShapeNames [Random.Range (0, ShapeNames.Count)];

		ShapesInPlay.Add (new Shape (name));


	}

	void DrawShape () {
		
		
	


	}

	void ResetGame () {

		ShapesInPlay = new List<Shape> ();

		NewShape ();

	}

	void SetupShapes () {

		Shapes = new Dictionary<string, Dictionary<int, List<Vector3Int>>> ();

		string shapeName = "Box";

		Shapes.Add (shapeName, new Dictionary<int, List<Vector3Int>> ());
		ShapeNames.Add (shapeName);

		Shapes [shapeName].Add (0, new List<Vector3Int> ());
		Shapes [shapeName].Add (1, new List<Vector3Int> ());
		Shapes [shapeName].Add (2, new List<Vector3Int> ());
		Shapes [shapeName].Add (3, new List<Vector3Int> ());

		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));
		Shapes [shapeName] [0].Add (new Vector3Int (1, 1, 0));

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


