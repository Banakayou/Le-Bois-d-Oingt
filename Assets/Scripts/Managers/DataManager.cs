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
    [HideInInspector] public GameObject Camera_pieton;
    [HideInInspector] public GameObject Camera_minimap;
    
    [HideInInspector] public GameObject OrbitalModeButton;
    [HideInInspector] public GameObject PietonModeButton;
    [HideInInspector] public GameObject VeilleModeButton;
    
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
        Camera_minimap = GameObject.Find("Camera_MiniMap");
        Camera_minimap.SetActive(false);
        
		OrbitalModeButton = GameObject.Find("Orbital_Mode_Button");
		PietonModeButton = GameObject.Find("Pieton_Mode_Button");
		VeilleModeButton = GameObject.Find("Veille_Mode_Button");

		setMode(MODE_COURANT);
    }
    

    public void setMode(int modeSelect)
    {
        this.GetComponent<FadeCamera>().FadeToBlack();
        
        UIManager.instance.minimap.SetActive(false);

        MODE_COURANT = modeSelect;
        switch (MODE_COURANT)
        {
            case (MODE_ORBITAL):
                OrbitalModeButton.GetComponent<Button> ().interactable = false;
			    PietonModeButton.GetComponent<Button> ().interactable = true;
			    VeilleModeButton.GetComponent<Button> ().interactable = true;

			    Camera_veille.SetActive(false);
		        Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(true);

                ClickToWalk.instance.enabled = true;
                break;
            case (MODE_PLAN):
		        OrbitalModeButton.GetComponent<Button> ().interactable = true;
		        PietonModeButton.GetComponent<Button> ().interactable = true;
		        VeilleModeButton.GetComponent<Button> ().interactable = true;

                Camera_veille.SetActive(false);
                Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(false);

                ClickToWalk.instance.enabled = true;
                break;
	        case (MODE_PIETON):
		        OrbitalModeButton.GetComponent<Button> ().interactable = true;
		        PietonModeButton.GetComponent<Button> ().interactable = false;
		        VeilleModeButton.GetComponent<Button> ().interactable = true;

                Camera_veille.SetActive(false);
                Camera_pieton.SetActive(true);
                Camera_orbitale.SetActive(false);

                UIManager.instance.minimap.SetActive (true);
		        UIManager.instance.animMinimap.enabled = false;
		        ClickToWalk.instance.enabled = true;
                break;
            default: //MODE_VEILLE
                OrbitalModeButton.GetComponent<Button>().interactable = true;
                PietonModeButton.GetComponent<Button>().interactable = true;
                VeilleModeButton.GetComponent<Button>().interactable = false;

                Camera_veille.SetActive(true);
                Camera_pieton.SetActive(false);
                Camera_orbitale.SetActive(false);
                
		        ClickToWalk.instance.enabled = false;
                break;
        }
        this.GetComponent<FadeCamera>().FadeToClear();
    }
    
}