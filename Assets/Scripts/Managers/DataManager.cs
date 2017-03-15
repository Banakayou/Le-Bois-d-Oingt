using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public const int MODE_ORBITAL = 0;
    public const int MODE_VEILLE = 1;
    public const int MODE_PLAN = 2;
    public const int MODE_PIETON = 3;
    [HideInInspector] public int MODE_COURANT = MODE_ORBITAL;

    [HideInInspector] public GameObject Camera_orbitale;
    [HideInInspector] public GameObject Camera_veille;
    [HideInInspector] public GameObject Camera_plan;
    [HideInInspector] public GameObject Camera_pieton;
    [HideInInspector] public GameObject Camera_minimap;

    [HideInInspector] public GameObject PlanModeButton;
    [HideInInspector] public GameObject OrbitalModeButton;
    [HideInInspector] public GameObject PietonModeButton;
    [HideInInspector] public GameObject VeilleModeButton;
	[HideInInspector] public GameObject MesureModeButton;
    [HideInInspector] public GameObject ConstructionModeButton;
    [HideInInspector] public GameObject SunButton;
    
	private static DataManager _instance;
	public static DataManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<DataManager>();
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

        Application.targetFrameRate = 90;
    }

	void Start()
	{
        Camera_orbitale = GameObject.Find("Camera_Orbitale");
        Camera_veille = GameObject.Find("Camera_Veille");
        Camera_pieton = GameObject.Find("Camera_Pieton");
        Camera_plan = GameObject.Find("Camera_Plan");
        Camera_minimap = GameObject.Find("Camera_MiniMap");
        Camera_minimap.SetActive(false);

        PlanModeButton = GameObject.Find("Plan_Mode_Button");
		OrbitalModeButton = GameObject.Find("Orbital_Mode_Button");
		PietonModeButton = GameObject.Find("Pieton_Mode_Button");
		VeilleModeButton = GameObject.Find("Veille_Mode_Button");
		MesureModeButton = GameObject.Find ("Mesure_Mode_Button");
        ConstructionModeButton = GameObject.Find("Construction_Mode_Button");
        SunButton = GameObject.Find ("Sun_Button");

		setMode(MODE_COURANT);
    }

    void Update()
    {
        
    }

    public void setMode(int modeSelect)
    {
        this.GetComponent<FadeCamera>().FadeToBlack();

        if (UIManager.instance.speedCameraVeillePanel.activeSelf)
        {
            UIManager.instance.ToggleSpeedCameraVeillePanel();
        }
        if (UIManager.instance.sunSlider.activeSelf)
        {
            UIManager.instance.ToggleSunSlider();
        }
        if (UIManager.instance.statusMinimap == true)
        {
            UIManager.instance.animMinimap.Stop();
        }
        UIManager.instance.minimap.SetActive(false);
        if (Camera_plan.GetComponent<ModeMesure>().mode_mesure == true)
        {
			Camera_plan.GetComponent<ModeMesure>().switchMesure();
        }

        MODE_COURANT = modeSelect;
        switch (MODE_COURANT)
        {
            case (MODE_ORBITAL):
                PlanModeButton.GetComponent<Button> ().interactable = true;
                OrbitalModeButton.GetComponent<Button> ().interactable = false;
			    PietonModeButton.GetComponent<Button> ().interactable = true;
			    VeilleModeButton.GetComponent<Button> ().interactable = true;
			    MesureModeButton.SetActive(false);
                //
                ConstructionModeButton.SetActive(false);
                //
                SunButton.SetActive (true);

			    Camera_veille.SetActive(false);
		        Camera_plan.SetActive(false);
		        Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(true);

                ClickToWalk.instance.enabled = true;
                break;
            case (MODE_PLAN):
		        PlanModeButton.GetComponent<Button> ().interactable = false;
		        OrbitalModeButton.GetComponent<Button> ().interactable = true;
		        PietonModeButton.GetComponent<Button> ().interactable = true;
		        VeilleModeButton.GetComponent<Button> ().interactable = true;
		        MesureModeButton.SetActive (true);
                ConstructionModeButton.SetActive(false);
                SunButton.SetActive (true);

                Camera_veille.SetActive(false);
                Camera_plan.SetActive(true);
                Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(false);

                ClickToWalk.instance.enabled = true;
                break;
	        case (MODE_PIETON):
		        PlanModeButton.GetComponent<Button> ().interactable = true;
		        OrbitalModeButton.GetComponent<Button> ().interactable = true;
		        PietonModeButton.GetComponent<Button> ().interactable = false;
		        VeilleModeButton.GetComponent<Button> ().interactable = true;
		        MesureModeButton.SetActive(false);
                ConstructionModeButton.SetActive(false);
		        SunButton.SetActive(true);

                Camera_veille.SetActive(false);
                Camera_plan.SetActive(false);
                Camera_pieton.SetActive(true);
                Camera_orbitale.SetActive(false);

                UIManager.instance.minimap.SetActive (true);
		        UIManager.instance.animMinimap.enabled = false;
		        ClickToWalk.instance.enabled = true;
                break;
            default: //MODE_VEILLE
                PlanModeButton.GetComponent<Button>().interactable = true;
                OrbitalModeButton.GetComponent<Button>().interactable = true;
                PietonModeButton.GetComponent<Button>().interactable = true;
                VeilleModeButton.GetComponent<Button>().interactable = false;
		        MesureModeButton.SetActive(false);
                ConstructionModeButton.SetActive(false);
		        SunButton.SetActive(false);

                Camera_veille.SetActive(true);
                Camera_plan.SetActive(false);
                Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(false);

                UIManager.instance.ToggleSpeedCameraVeillePanel();
                UIManager.instance.ToggleSunSlider();
		        ClickToWalk.instance.enabled = false;
                break;
        }
        this.GetComponent<FadeCamera>().FadeToClear();
    }

    void OnApplicationQuit()
    {
        DayNightController.instance.resetTimeOfDay();
    }

}