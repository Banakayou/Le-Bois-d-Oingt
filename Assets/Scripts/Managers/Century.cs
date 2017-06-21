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

    public Sprite century_13s;
    public Sprite capture_13s;
    public Sprite clock_13s;
    public Sprite info_13s;
    public Sprite boussole_13s;
    public Sprite auto_13s;
    public Sprite pieton_13s;
    public Sprite orbit_13s;

    public Sprite century_17;
    public Sprite capture_17;
    public Sprite clock_17;
    public Sprite info_17;
    public Sprite boussole_17;
    public Sprite auto_17;
    public Sprite pieton_17;
    public Sprite orbit_17;

    // Use this for initialization
    void Start () {
		city13s = GameObject.Find("13eme");
		animatorCity13s = city13s.GetComponent<Animator>();
		animatorCity13s.enabled = false;
		statusCity13s = true;

		city2017 = GameObject.Find("2017");
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
            Invoke("Toggle2017", Time.deltaTime * 360);
        }
        else
        {
            Toggle2017();
            Invoke("Toggle13s", Time.deltaTime * 360);
        }
        inTransition = true;
        Invoke("StopFlash", Time.deltaTime * 700);
	}

    private void StopFlash()
    {
        inTransition = false;
        centuryString.color = new Color(centuryString.color.r, centuryString.color.g, centuryString.color.b, 1);
    }

	private void Toggle13s()
	{
		animatorCity13s.enabled = true;
		if (statusCity13s == false) {
			animatorCity13s.Play ("13so");
			statusCity13s = true;
            changeFontsTo13s();
            changeIconsTo13s();
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
            changeIconsTo17();
        }
		else if (statusCity2017 == true)
		{
			animatorCity2017.Play("2017");
			statusCity2017 = false;
		}
	}

    private void changeFontsTo2017()
    {
        centuryString.transform.parent.GetComponent<Text>().font = font17;
        centuryString.transform.parent.GetComponent<Text>().fontSize = 70;
        centuryString.GetComponent<Text>().font = font17;
        centuryString.GetComponent<Text>().fontSize = 70;
        centuryString.GetComponent<Text>().text = "2017";
        centuryString.color = Color.black;
        DayNightController.instance.m_horloge.GetComponent<Text>().font = font17;
        DayNightController.instance.m_horloge.GetComponent<Text>().fontSize = 30;
    }

    private void changeFontsTo13s()
    {
        centuryString.transform.parent.GetComponent<Text>().font = font13s;
        centuryString.transform.parent.GetComponent<Text>().fontSize = 52;
        centuryString.GetComponent<Text>().font = font13s;
        centuryString.GetComponent<Text>().fontSize = 52;
        centuryString.GetComponent<Text>().text = "13ème siècle";
        centuryString.color = Color.black;
        DayNightController.instance.m_horloge.GetComponent<Text>().font = font13s;
        DayNightController.instance.m_horloge.GetComponent<Text>().fontSize = 20;
    } 

    private void changeIconsTo13s()
    {
        DataManager.instance.OrbitalModeButton.GetComponent<Image>().sprite = orbit_13s;
        DataManager.instance.PietonModeButton.GetComponent<Image>().sprite = pieton_13s;
        DataManager.instance.VeilleModeButton.GetComponent<Image>().sprite = auto_13s;
        this.GetComponent<Image>().sprite = century_13s;
        DayNightController.instance.m_horloge.transform.parent.GetComponent<Image>().sprite = clock_13s;
        GameObject.Find("Capture_Button").GetComponent<Image>().sprite = capture_13s;
        GameObject.Find("Info_Button").GetComponent<Image>().sprite = info_13s;
        GameObject.Find("Contour").GetComponent<Image>().sprite = boussole_13s;
        ColorBlock cb = GameObject.Find("ImageButtonMinimap").transform.parent.GetComponent<Button>().colors;
        cb.normalColor = cb.pressedColor = cb.highlightedColor = new Color(148, 111, 54, 255);
        GameObject.Find("ImageButtonMinimap").transform.parent.GetComponent<Button>().colors = cb;
    }

    private void changeIconsTo17()
    {
        DataManager.instance.OrbitalModeButton.GetComponent<Image>().sprite = orbit_17;
        DataManager.instance.PietonModeButton.GetComponent<Image>().sprite = pieton_17;
        DataManager.instance.VeilleModeButton.GetComponent<Image>().sprite = auto_17;
        this.GetComponent<Image>().sprite = century_17;
        DayNightController.instance.m_horloge.transform.parent.GetComponent<Image>().sprite = clock_17;
        GameObject.Find("Capture_Button").GetComponent<Image>().sprite = capture_17;
        GameObject.Find("Info_Button").GetComponent<Image>().sprite = info_17;
        GameObject.Find("Contour").GetComponent<Image>().sprite = boussole_17;
        ColorBlock cb = GameObject.Find("ImageButtonMinimap").transform.parent.GetComponent<Button>().colors;
        cb.normalColor = cb.pressedColor = cb.highlightedColor = new Color(238, 185, 83, 255);
        GameObject.Find("ImageButtonMinimap").transform.parent.GetComponent<Button>().colors = cb;
    }
}
