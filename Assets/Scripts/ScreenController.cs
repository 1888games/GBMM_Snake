using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Common;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;


public class ScreenController : MonoBehaviourSingleton<ScreenController> {

	public List<GameObject> GameboyGameObjects;

	public Image [,] Backlights;

	public int OutputMode = 0;

	public Dictionary<int, bool> Channel1Outputs;
	public Dictionary<int, bool> Channel15Outputs;
	public Dictionary<int, bool> Channel16Outputs;

	public bool Channel1Output = false;
	public bool Channel15Output = false;
	public bool Channel16Output = true;





	private KeyCode [] keyCodes = {
			KeyCode.Alpha0,
			 KeyCode.Alpha1,
			 KeyCode.Alpha2,
			 KeyCode.Alpha3,
			 KeyCode.Alpha4,
			 KeyCode.Alpha5,
			 KeyCode.Alpha6,
			 KeyCode.Alpha7,
			 KeyCode.Alpha8,
			 KeyCode.Alpha9,
		 };

	public int Rows;
	public int Columns;

	public List<string> MidiDevices;

	public OutputDevice MidiDevice;

	int MidiDeviceID = -1;

	Dictionary<string, int> Velocities;

	public Text DeviceNameText;
	public Text Output1Text;
	public Text Output15Text;
	public Text Output16Text;



	// Start is called before the first frame update
	void Start () {

		MidiDevices = new List<string> ();

		try {

			foreach (var outputDevice in OutputDevice.GetAll ()) {
				Debug.Log (outputDevice.Name);
				MidiDevices.Add (outputDevice.Name);
				MidiDeviceID = 0;
			}

			if (MidiDeviceID >= 0) {
				MidiDevice = OutputDevice.GetByName (MidiDevices [MidiDeviceID]);
				DeviceNameText.text = MidiDevices [MidiDeviceID] + " (" + MidiDeviceID + ")";
			}

		} catch (Exception e) {

			Debug.Log (e.ToString ());
		}

		Velocities = new Dictionary<string, int> ();

		Velocities.Add ("SnakeHead", 127);
		Velocities.Add ("SnakeTail", 90);
		Velocities.Add ("Food", 60);
		Velocities.Add ("GameOver", 75);

		Channel1Outputs = new Dictionary<int, bool> ();
		Channel15Outputs = new Dictionary<int, bool> ();
		Channel16Outputs = new Dictionary<int, bool> ();

		Channel1Outputs.Add (0, false);
		Channel1Outputs.Add (1, false);
		Channel1Outputs.Add (2, true);
		Channel1Outputs.Add (3, true);
		Channel1Outputs.Add (4, true);

		Channel15Outputs.Add (0, false);
		Channel15Outputs.Add (1, true);
		Channel15Outputs.Add (2, true);
		Channel15Outputs.Add (3, true);
		Channel15Outputs.Add (4, false);


		Channel16Outputs.Add (0, true);
		Channel16Outputs.Add (1, false);
		Channel16Outputs.Add (2, true);
		Channel16Outputs.Add (3, false);
		Channel16Outputs.Add (4, true);

		Output1Text.text = "1 - " + Channel1Outputs [OutputMode].ToString ();
		Output15Text.text = "15 - " + Channel15Outputs [OutputMode].ToString ();
		Output16Text.text = "16 - " + Channel16Outputs [OutputMode].ToString ();


	}

	public void Initialise () {

		Backlights = new Image [8, 8];

		float currentY = GameboyGameObjects [0].GetComponent<RectTransform> ().anchoredPosition.y;
		int currentRow = 0;
		int currentCol = 0;
		float thisY = 0f;

		foreach (GameObject go in GameboyGameObjects) {

			Image image = go.GetComponent<Image> ();

			thisY = go.GetComponent<RectTransform> ().anchoredPosition.y;

			if (currentY != thisY) {
				Columns = currentCol - 1;
				currentCol = 0;
				currentRow++;
			}

			currentY = thisY;


			Backlights [currentCol, currentRow] = image;

			currentCol++;

			image.enabled = false;
			image.name = "None";


		}

		Rows = currentRow;


	}

	public void ClearScreen (bool enabled = false) {


		for (int x = 0; x <= Columns; x++) {

			for (int y = 0; y <= Rows; y++) {

				if (enabled == false) {
					HidePixel (x, y);
				} else {
					LightPixel (x, y, "GameOver");
				}
			}


		}


	}



	public string GetPixelName (int x, int y) {

		if (PixelExists (x, y)) {

			return Backlights [x, y].name;
		}

		return "OutOfBounds";

	}



	public void FlipPixel (int x, int y) {

		if (PixelExists (x, y)) {

			Backlights [x, y].enabled = !Backlights [x, y].enabled;
		}


	}




	public int PixelToNoteNumber (int x, int y) {

		return 36 + ((Rows - y) * 8) + x;



	}

	bool PixelExists (int x, int y) {

		if (y < 0 || x < 0) {
			return false;
		}

		if (x > Columns) {
			return false;
		}

		if (y > Rows) {
			return false;
		}

		Image image = Backlights [x, y];


		return image != null;
	}


	public async void PlayNote (int noteNumber, int velocity, int stopAfter = 0) {

		if (MidiDeviceID >= 0) {


			NoteOnEvent NoteOn = new NoteOnEvent ();

			NoteOn.Channel = (FourBitNumber)15;
			NoteOn.NoteNumber = (SevenBitNumber)noteNumber;
			NoteOn.Velocity = (SevenBitNumber)velocity;

			if (Channel1Outputs [OutputMode]) {
				NoteOn.Channel = (FourBitNumber)0;
				MidiDevice.SendEvent (NoteOn);
			}

			if (Channel15Outputs [OutputMode]) {
				NoteOn.Channel = (FourBitNumber)14;
				MidiDevice.SendEvent (NoteOn);
			}

			if (Channel16Outputs [OutputMode]) {
				NoteOn.Channel = (FourBitNumber)15;
				MidiDevice.SendEvent (NoteOn);
			}



			Debug.Log ("Play " + noteNumber + " ID: " + MidiDeviceID);

			if (stopAfter > 0) {
				await Task.Delay (stopAfter);
				StopNote (noteNumber);
			}



		}


	}

	void StopNote (int noteNumber) {

		if (MidiDeviceID >= 0) {

			NoteOffEvent NoteOff = new NoteOffEvent ();

			NoteOff.Channel = (FourBitNumber)15;
			NoteOff.NoteNumber = (SevenBitNumber)noteNumber;
			NoteOff.Velocity = (SevenBitNumber)127;


			if (Channel1Outputs [OutputMode]) {
				NoteOff.Channel = (FourBitNumber)0;
				MidiDevice.SendEvent (NoteOff);
			}

			if (Channel15Outputs [OutputMode]) {
				NoteOff.Channel = (FourBitNumber)14;
				MidiDevice.SendEvent (NoteOff);
			}

			if (Channel16Outputs [OutputMode]) {
				NoteOff.Channel = (FourBitNumber)15;
				MidiDevice.SendEvent (NoteOff);
			}

			Debug.Log ("Stop " + noteNumber + " ID: " + MidiDeviceID);




		}


	}





	public void LightPixel (int x, int y, string name) {

		if (PixelExists (x, y)) {

			Backlights [x, y].enabled = true;
			Backlights [x, y].name = name;

			Debug.Log (name);

			PlayNote (PixelToNoteNumber (x, y), Velocities [name]);

			//Debug.Log ("Note on: " + PixelToNoteNumber (x, y));

		}

	}


	public void HidePixel (int x, int y) {

		if (PixelExists (x, y)) {

			Backlights [x, y].enabled = false;
			Backlights [x, y].name = "None";

			StopNote (PixelToNoteNumber (x, y));

			//Debug.Log ("Note off: " + PixelToNoteNumber (x, y));

		}

	}






	// Update is called once per frame
	void Update () {

		if (Backlights != null) {
			int x = UnityEngine.Random.Range (0, Columns + 1);
			int y = UnityEngine.Random.Range (0, Rows + 1);

			//FlipPixel (x, y);
		}

		if (Input.GetKeyDown (KeyCode.Space)) {

			OutputMode++;

			if (OutputMode > Channel16Outputs.Count - 1) {
				OutputMode = 0;
			}

			Output1Text.text = "1 - " + Channel1Outputs [OutputMode].ToString ();
			Output15Text.text = "15 - " + Channel15Outputs [OutputMode].ToString ();
			Output16Text.text = "16 - " + Channel16Outputs [OutputMode].ToString ();


		}

		for (int i = 0; i < keyCodes.Length; i++) {
			if (Input.GetKeyDown (keyCodes [i])) {

				int numberPressed = i;
				Debug.Log (numberPressed);

				if (MidiDevices.Count - 1 >= numberPressed) {
					MidiDeviceID = numberPressed;
					MidiDevice.TurnAllNotesOff ();
					MidiDevice.Dispose ();
					MidiDevice = OutputDevice.GetByName (MidiDevices [MidiDeviceID]);
					DeviceNameText.text = MidiDevices [MidiDeviceID] + " (" + MidiDeviceID + ")";
				}

			}
		}
	}

}
