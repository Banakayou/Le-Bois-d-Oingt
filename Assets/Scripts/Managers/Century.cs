using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Century : MonoBehaviour {

	[HideInInspector] public GameObject city13s;
	[HideInInspector] public Animator animCity13s;
	[HideInInspector] public bool statusCity13s;

	[HideInInspector] public GameObject city2017;
	[HideInInspector] public Animator animCity2017;
	[HideInInspector] public bool statusCity2017;

	[HideInInspector] public GameObject centuryString;

	// Use this for initialization
	void Start () {
		city13s = GameObject.Find("13S - Alexis_FBX");
		animCity13s = city13s.GetComponent<Animator>();
		animCity13s.enabled = false;
		statusCity13s = true;

		city2017 = GameObject.Find("2017 - Alexis_FBX");
		animCity2017 = city2017.GetComponent<Animator>();
		animCity2017.enabled = false;
		statusCity2017 = true;
		Toggle2017 ();

		centuryString = GameObject.Find ("NomProjet");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleCentury()
	{
		Toggle13s ();
		Toggle2017 ();
	}

	private void Toggle13s()
	{
		animCity13s.enabled = true;
		if (statusCity13s == false) {
			animCity13s.Play ("13so");
			statusCity13s = true;
			centuryString.GetComponent<Text> ().text = "13ème siècle";
		} else if (statusCity13s == true) {
			animCity13s.Play ("13s");
			statusCity13s = false;
		}
	}

	private void Toggle2017()
	{
		animCity2017.enabled = true;
		if (statusCity2017 == false)
		{
			animCity2017.Play("2017o");
			statusCity2017 = true;
			centuryString.GetComponent<Text> ().text = "2017";
		}
		else if (statusCity2017 == true)
		{
			animCity2017.Play("2017");
			statusCity2017 = false;
		}
	}
}
