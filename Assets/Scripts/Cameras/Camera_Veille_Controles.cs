using UnityEngine;
using UnityEngine.UI;

public class Camera_Veille_Controles : MonoBehaviour {
    private Transform target;

    [HideInInspector] public GameObject MinusSpeedVeille;
    [HideInInspector] public GameObject PlusSpeedVeille;

    private float sensX = 4f;
    private float sensXpalier = 3f;
    private float sensXmin = 1f;
    private float sensXmax = 10f;

	// Use this for initialization
	void Start () {
        MinusSpeedVeille = GameObject.Find("MinusSpeedVeille_Button");
        PlusSpeedVeille = GameObject.Find("PlusSpeedVeille_Button");
        target = GameObject.Find("Centre_Cam_Veille").transform;
    }
	
	// Update is called once per frame
	void Update () {
        DayNightController.instance.m_currentTimeOfDay += (Time.deltaTime / DayNightController.instance.m_secondsInFullDay) * DayNightController.instance.m_timeMultiplier;
        DayNightController.instance.m_sunSlider.value = DayNightController.instance.m_currentTimeOfDay;
        if (DayNightController.instance.m_currentTimeOfDay >= 1)
        {
            DayNightController.instance.m_currentTimeOfDay = 0;
        }

        transform.RotateAround(target.position, Vector3.up, sensX * Time.deltaTime);
		transform.LookAt(target.position);
    }
    
    public void plusSpeed()
    {
        sensX += sensXpalier;
        if (MinusSpeedVeille.GetComponent<Button>().interactable == false)
        {
            MinusSpeedVeille.GetComponent<Button>().interactable = true;
        }
        if (sensX == sensXmax){
            PlusSpeedVeille.GetComponent<Button>().interactable = false;
        }

        DayNightController.instance.m_timeMultiplier += 0.7f;
}

    public void minusSpeed()
    {
        sensX -= sensXpalier;
        if (PlusSpeedVeille.GetComponent<Button>().interactable == false)
        {
            PlusSpeedVeille.GetComponent<Button>().interactable = true;
        }
        if (sensX == sensXmin)
        {
            MinusSpeedVeille.GetComponent<Button>().interactable = false;
        }

        DayNightController.instance.m_timeMultiplier -= 0.7f;
    }
}
