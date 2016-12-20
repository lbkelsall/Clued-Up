﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// Room Controller
/// </summary>

public class RoomController : MonoBehaviour {
	/// <summary>
	/// The index of the room.
	/// </summary>
	public int roomIndex;
	/// <summary>
	/// The overlay panel GameObject.
	/// </summary>
	public GameObject overlayPanel;
	/// <summary>
	/// The door quad GameObject.
	/// </summary>
	public GameObject doorQuad;

	private List<GameObject> furnitureInRoom = new List<GameObject>();
	private List<GameObject> cluesInRoom = new List<GameObject>();
	/// <summary>
	/// The main story.
	/// </summary>
	private Story story;

	/// <summary>
	/// Prepares the overlay by turning on and immediately fading out, giving a fade from black effect.
	/// </summary>
	private void setOverlay(){
		overlayPanel.SetActive (true);
		overlayPanel.GetComponent<Image> ().CrossFadeAlpha (0f, 3f, false);
	}
	/// <summary>
	/// Sets the door quad GameObjects image depending on weather condition of story.
	/// </summary>
	private void setDoor(){
		Material[] materialArray = new Material[8];
		//maretialArray for DoorQuad materials
		materialArray [0] = (Material)Resources.Load ("Room2DSunny", typeof(Material)); //finds material located in the resources folder
		materialArray [1] = (Material)Resources.Load ("Room2DRain", typeof(Material));
		materialArray [2] = (Material)Resources.Load ("Room2DSunset", typeof(Material));
		materialArray [3] = (Material)Resources.Load ("Room2DSnow", typeof(Material));

		doorQuad.GetComponent<Renderer> ().material = materialArray [story.getWeather ()];
	}
	/// <summary>
	/// Gets the characters present in this room from story.
	/// </summary>
	private void getCharacters(){
		List<GameObject> charactersInRoom = new List<GameObject> ();
		Debug.Log (roomIndex);
		if (story.getCharactersInRoom (roomIndex).Count > 0) {
			charactersInRoom = story.getCharactersInRoom (roomIndex);
		} else {
			Debug.Log ("No characters this time");
		}

		switch (charactersInRoom.Count) {
		case 1:
			//do stuff to make one character active
			Debug.Log (charactersInRoom [0].GetComponent<Character>().longName + " is in the room!");
			break;
		case 2:
			//do stuff to make two characters active in the same room
			break;
		default:
			break;
		}			
	}

	private void fengShui(){
		switch (roomIndex) {
		case 1:
			GameObject lockers = Instantiate (Resources.Load ("Lockers"), new Vector3 (3.22f, -4f, 2f), Quaternion.Euler (0, 0, 0)) as GameObject;
			lockers.GetComponent<Lockers> ().Initialise ();
			furnitureInRoom.Add (lockers);
			break;
		case 4:
			GameObject cupboardL = Instantiate (Resources.Load ("Cupboard"), new Vector3 (6.11f, -4.88f, 2f), Quaternion.Euler (0, 0, 0)) as GameObject;
			cupboardL.transform.localScale = new Vector3 (0.95f, 1f, 1f);
			cupboardL.GetComponent<Cupboard> ().Initialise ("cupboard-left","cupboard");
			furnitureInRoom.Add (cupboardL);

			GameObject cupboardR = Instantiate (Resources.Load ("Cupboard"), new Vector3 (7.42f, -4.88f, 2f), Quaternion.Euler (0, 0, 0)) as GameObject;
			cupboardR.transform.localScale = new Vector3 (0.9f, 1f, 1f);
			cupboardR.GetComponent<Cupboard> ().Initialise ("cupboard-right","cupboard");
			furnitureInRoom.Add (cupboardR);

			GameObject oven = Instantiate (Resources.Load ("Cupboard"), new Vector3 (-2.83f, -4.84f, 2f), Quaternion.Euler (0, 0, 0)) as GameObject;
			oven.transform.localScale = new Vector3 (0.95f, 1f, 1f);
			oven.GetComponent<Cupboard> ().Initialise ("oven-door","oven");
			furnitureInRoom.Add (oven);
			break;
		default:
			break;
		}
	}


	private void getClues(){
		//USED SO ALL ROOMS HAVE A MICROPHONE. THIS NEEDS TO CHANGE. TODO
		this.cluesInRoom.Add(story.getCluesInRoom (1) [0]);
		Debug.Log ("Clue in room: " + this.cluesInRoom [0].GetComponent<Clue> ().longName);
	}
		
	private List<Vector3> getRoomClueLocations(){
		List<Vector3> locationList = new List<Vector3> ();
		switch (roomIndex) {
		case 4: //kitchen
			locationList.Add (new Vector3 (-5.74f, -3.96f, 1f));
			break;
		default:
			break;
		}

		return locationList;
	}

	/// <summary>
	/// First decides which location to put the clue by summing all possible locations and deciding which to use (either room location or furniture)
	/// Then assigns one clue to this location in a room by either changing clue's location, or testing which kind of furniture it is & moving it there.
	/// </summary>
	private void assignCluesLocations(GameObject clue){
		List<Vector3> roomLocations = getRoomClueLocations (); //possible locations that are not furniture
		int totalLocationsCount = roomLocations.Count + furnitureInRoom.Count;

		int locationIndex = UnityEngine.Random.Range (0, totalLocationsCount);
		if (locationIndex < roomLocations.Count) {	//If clue is in room locations
			clue.gameObject.GetComponent<Transform> ().position = roomLocations [locationIndex];
		} else{
			locationIndex = locationIndex - roomLocations.Count; //change to only include furniture clues
			if (furnitureInRoom [locationIndex].GetComponent<Lockers> () != null) {
				furnitureInRoom [locationIndex].GetComponent<Lockers> ().addClue (clue);
			}else if (furnitureInRoom [locationIndex].GetComponent<Cupboard> () != null) {
				furnitureInRoom [locationIndex].GetComponent<Cupboard> ().addClue (clue);
			}
		}
	}

	/// <summary>
	/// Walk character in, get reference to story and do initialisation for scene
	/// </summary>
	void Start () {
		setOverlay ();
		GameObject detective = GameObject.Find ("Detective");
		detective.GetComponent<Detective> ().walkIn();

		story = GameObject.Find("Story").GetComponent<Story>(); // references persistant object story

		//Make detective slightly lower in screen
		Vector3 pos = detective.transform.position;
		pos.y = -6.5f;
		detective.transform.position = pos;

		switch (roomIndex) {
		case 1:
			setDoor ();
			break;
		default:
			break;
		}

		getCharacters ();
		fengShui ();  
		getClues ();
		assignCluesLocations (cluesInRoom[0]);
	}
}
