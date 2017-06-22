using UnityEngine;
using UnityEngine.UI;

public class DayNightController : MonoBehaviour {
	//Elements de la map
	//
	//public GameObject water;
	//
    public Light m_sun;
    public Transform m_stars;
	[HideInInspector] public GameObject[] m_lamps;
    [HideInInspector] public GameObject[] m_flams;
    public float m_lampsViewDistance = 250f;

	//Système Jour/Nuit
	[HideInInspector] public bool m_summer; //false = hiver, true = été
	public float m_secondsInFullDay = 120f;
    public float m_timeMultiplier = 1f;
    [HideInInspector] [Range(0, 1)] public float m_currentTimeOfDay = 0;
	[Range(0, 1)] public const float m_initialTimeOfDay = 0.681f;

	[HideInInspector] float m_currentHour;
	[HideInInspector] float m_currentMinute;
	[HideInInspector] string m_currentMinuteString;
	//Elements du HUD
    [HideInInspector] public GameObject m_horloge;
    [HideInInspector] public Slider m_sunSlider;

	//Données de calcul
    public AnimationCurve m_summerSourceIntensity;
	public AnimationCurve m_winterSourceIntensity;
	public AnimationCurve m_summerAmbiantIntensity;
	public AnimationCurve m_winterAmbiantIntensity;
	public Gradient m_summerNightDayColor;
	public Gradient m_winterNightDayColor;
	public AnimationCurve m_summerExposure;
	public AnimationCurve m_winterExposure;

    private static DayNightController _instance;
    public static DayNightController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DayNightController>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
        m_horloge = GameObject.Find("Horloge");
    }

    void Start() {
		m_sunSlider = UIManager.instance.sunSlider.GetComponent<Slider>();
		m_lamps = GameObject.FindGameObjectsWithTag("Lampadaire");
        m_flams = GameObject.FindGameObjectsWithTag("flam");
        m_currentTimeOfDay = m_sunSlider.value; UpdateSun(); UpdateClock();
        m_sunSlider.onValueChanged.AddListener(delegate { m_currentTimeOfDay = m_sunSlider.value; UpdateSun(); UpdateClock();});
        
    }
    

	void UpdateClock()
	{
		m_currentHour = 24 * m_currentTimeOfDay;
		m_currentMinute = Mathf.Floor(60 * (m_currentHour - Mathf.Floor(m_currentHour)));
		if (m_currentMinute < 30)
		{
			m_currentMinuteString = "00";
		}
		else { m_currentMinuteString = "30"; }
		m_horloge.GetComponent<Text>().text = Mathf.Floor(m_currentHour).ToString("00") + "h" + m_currentMinuteString;
	}

    void UpdateSun()
	{
		m_sun.transform.localRotation = Quaternion.Euler((m_currentTimeOfDay * 180f), (-m_currentTimeOfDay * 150f), 0);
		m_stars.transform.rotation = m_sun.transform.localRotation;

		if (m_summer)
        {
			m_sun.intensity = m_summerSourceIntensity.Evaluate(m_currentTimeOfDay);
			RenderSettings.ambientIntensity = m_summerAmbiantIntensity.Evaluate(m_currentTimeOfDay);
			m_sun.color = m_summerNightDayColor.Evaluate(m_currentTimeOfDay);
			RenderSettings.skybox.SetFloat("_Exposure", m_summerExposure.Evaluate(m_currentTimeOfDay));
        }
        else
        {
			m_sun.intensity = m_winterSourceIntensity.Evaluate(m_currentTimeOfDay);
			RenderSettings.ambientIntensity = m_winterAmbiantIntensity.Evaluate(m_currentTimeOfDay);
			m_sun.color = m_winterNightDayColor.Evaluate(m_currentTimeOfDay);
			RenderSettings.skybox.SetFloat("_Exposure", m_winterExposure.Evaluate(m_currentTimeOfDay));
        }
        
		if (m_sun.intensity > 1.3f)
        {
            foreach (GameObject go in m_lamps)
            {
                go.GetComponentInChildren<Light>().enabled = false;
            }
            foreach (GameObject go in m_flams)
            {
                go.SetActive(false);
            }
            m_stars.gameObject.SetActive(false);
			//water.GetComponent<WaterMatChanger>().setDayMaterial();
        }
        else
        {
			foreach (GameObject go in m_lamps)
            {
                Vector3 distLamp = go.transform.position - Camera.main.transform.position;//dist de la cam
				Light lampe = go.GetComponent<Light>();
				if (distLamp.sqrMagnitude < (m_lampsViewDistance * m_lampsViewDistance))
				{
					lampe.enabled = true;
				}
				else
				{
					lampe.enabled = false;
				}
            }
            foreach (GameObject go in m_flams)
            {
                go.SetActive(true);
            }
			m_stars.gameObject.SetActive(true);
			//water.GetComponent<WaterMatChanger>().setNightMaterial();
        }
    }
    
}