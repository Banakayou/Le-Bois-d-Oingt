using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Century : MonoBehaviour {
    public Font font17;
    public Font font13s;
	[HideInInspector] public GameObject city13s;
	[HideInInspector] public Animator animatorCity13s;
    [HideInInspector] public bool statusCity13s;

	[HideInInspector] public GameObject city2017;
	[HideInInspector] public Animator animatorCity2017;
    [HideInInspector] public bool statusCity2017;

	[HideInInspector] public Text centuryString;
    private bool inTransition = false;

	// Use this for initialization
	void Start () {
		city13s = GameObject.Find("13S - Alexis_FBX");
		animatorCity13s = city13s.GetComponent<Animator>();
		animatorCity13s.enabled = false;
		statusCity13s = true;

		city2017 = GameObject.Find("2017 - Alexis_FBX");
		animatorCity2017 = city2017.GetComponent<Animator>();
		animatorCity2017.enabled = false;
		statusCity2017 = false;

		centuryString = GameObject.Find ("NomProjet").GetComponent<Text>();
        changeFontsTo13s();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (inTransition)
        {
            centuryString.color = new Color(centuryString.color.r, centuryString.color.g, centuryString.color.b, Mathf.PingPong(2*Time.time, 1));
        }
    }

	public void ToggleCentury()
	{
        if (statusCity13s == true)
        {
            Toggle13s();
            Invoke("Toggle2017", Time.deltaTime * 380);
        }
        else
        {
            Toggle2017();
            Invoke("Toggle13s", Time.deltaTime * 380);
        }
        inTransition = true;
        Invoke("StopFlash", Time.deltaTime * 1200);
	}

    private void StopFlash()
    {
        inTransition = false;
        centuryString.color = Color.white;
    }

	private void Toggle13s()
	{
		animatorCity13s.enabled = true;
		if (statusCity13s == false) {
			animatorCity13s.Play ("13so");
			statusCity13s = true;
            changeFontsTo13s();
        } else if (statusCity13s == true) {
			animatorCity13s.Play ("13s");
			statusCity13s = false;
		}
	}

	private void Toggle2017()
	{
		animatorCity2017.enabled = true;
		if (statusCity2017 == false)
		{
			animatorCity2017.Play("2017o");
			statusCity2017 = true;
            changeFontsTo2017();
        }
		else if (statusCity2017 == true)
		{
			animatorCity2017.Play("2017");
			statusCity2017 = false;
		}
	}

    private void changeFontsTo2017()
    {
        centuryString.GetComponent<Text>().font = font17;
        centuryString.GetComponent<Text>().text = "2017";
        DayNightController.instance.m_horloge.GetComponent<Text>().font = font17;
        DayNightController.instance.m_horloge.GetComponent<Text>().fontSize = 30;
    }

    private void changeFontsTo13s()
    {
        centuryString.GetComponent<Text>().font = font13s;
        centuryString.GetComponent<Text>().text = "13ème siècle";
        DayNightController.instance.m_horloge.GetComponent<Text>().font = font13s;
        DayNightController.instance.m_horloge.GetComponent<Text>().fontSize = 20;
    } 
}
