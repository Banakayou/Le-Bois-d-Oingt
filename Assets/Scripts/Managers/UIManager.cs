using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;


#if UNITY_ANDROID
public class ScreenShotBridgeANDROID 
{
	static string path = "com.plugin.screenshot.ScreenShotPlugin";
	public delegate void ScreenShotDelegate(bool screenShotStatus);
	public static IEnumerator SaveScreenShot(string fileName,string albumName,bool isScreenShotWithDateTime,ScreenShotDelegate callBack)
	{
		yield return new WaitForEndOfFrame ();
		Application.CaptureScreenshot (fileName+".png");
        yield return new WaitForSeconds (2f);
		if (Application.platform == RuntimePlatform.Android) {
						string origin = System.IO.Path.Combine (Application.persistentDataPath, fileName + ".png");
						string destination = "/sdcard/" + albumName + "/";
						if (!System.IO.Directory.Exists ("/sdcard/" + albumName)) {
								System.IO.Directory.CreateDirectory (destination);
						}
						if (System.IO.File.Exists (origin)) {
								string finalFileName = "";
								if (isScreenShotWithDateTime)
										finalFileName = destination + "" + fileName + System.DateTime.Now.ToString ("yyyyMMddHHmmssfff") + ".png";
								else {
										int totalScreenShots = PlayerPrefs.GetInt ("TotalScreenShots", 0);
										finalFileName = destination + "" + fileName + "_" + totalScreenShots + ".png";
										totalScreenShots++;
										PlayerPrefs.SetInt ("TotalScreenShots", totalScreenShots);
								}
								System.IO.File.Move (origin, finalFileName);
								AndroidJavaClass playerClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
								AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject> ("currentActivity");		
								AndroidJavaClass pluginClass = new AndroidJavaClass (path);
								pluginClass.CallStatic ("RefreshGallery", new object[2] {activity, finalFileName});
						}
				}
		callBack(true);
	}
}
#endif
#if UNITY_IOS
public class ScreenShotBridgeIOS 
{
	public delegate void ScreenShotDelegate(bool screenShotStatus);
	[DllImport("__Internal")]
	private static extern bool _SaveScreenshotToGallery(string albumName);
	public static IEnumerator SaveScreenShot(string albumName,ScreenShotDelegate callBack)
	{
		yield return new WaitForEndOfFrame ();
		Application.CaptureScreenshot ("Screenshot.png");
        yield return new WaitForSeconds (2f);
		bool screenShotStatus = _SaveScreenshotToGallery (albumName);
		callBack(screenShotStatus);
	}
}
#endif

public class UIManager : MonoBehaviour {
    [HideInInspector] public GameObject HUD;
    [HideInInspector] public GameObject imagesNonContractuelles;

    [HideInInspector] public GameObject credits;
	private Animator animCredits;
   	private bool statusCredits;

    [HideInInspector] public GameObject minimap;
	[HideInInspector] public Animator animMinimap;
	[HideInInspector] public bool statusMinimap;
    
	[HideInInspector] public GameObject sunSlider;
    
    string folderPath = "";

	#if UNITY_IOS || UNITY_ANDROID
    bool isScreenShotSave;
    void ScreenShotStatus(bool status)
    {
        isScreenShotSave = status;
    }
	#endif


    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UIManager>();
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
    }

	// Use this for initialization
	void Start () {
        HUD = GameObject.Find("HUD");
        imagesNonContractuelles = GameObject.Find("Images non-contractuelles");

        credits = GameObject.Find("Credits");
		animCredits = credits.GetComponent<Animator>();
		animCredits.enabled = false;
		statusCredits = false;
		credits.SetActive(false);

		minimap = GameObject.Find("MiniMap");
		animMinimap = minimap.GetComponent<Animator>();
		animMinimap.enabled = false;
		statusMinimap = false;

        sunSlider = GameObject.Find("Sun_Slider");
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    public void ToggleCredits(bool active)
    {         
        animCredits.enabled = true;
        if (active == true && statusCredits == false)
        {
            HideHUD();
            credits.SetActive(active);
            animCredits.Play("CreditsFadeIn");
            statusCredits = true;
        }
        else if (active == false && statusCredits == true)
        {
            RevealHUD();
            animCredits.Play("CreditsFadeOut");
            statusCredits = false;
            credits.SetActive(active);
        }
		animMinimap.enabled = false;
    }

    public void ToggleMinimap()
	{
		animMinimap.enabled = true;
		if (statusMinimap == false)
		{
			animMinimap.Play("minimapReduce");
			statusMinimap = true;
            DataManager.instance.Camera_minimap.SetActive(true);
            if (DataManager.instance.MODE_COURANT == DataManager.MODE_PIETON)
            {
                DataManager.instance.Camera_pieton.GetComponentInChildren<IconeMinimap>().enabled = true;
                DataManager.instance.Camera_pieton.GetComponentInChildren<SpriteRenderer>().enabled = true;
                DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<IconeMinimap>().enabled = false;
                DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            if (DataManager.instance.MODE_COURANT == DataManager.MODE_TOUR)
            {
                DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<IconeMinimap>().enabled = true;
                DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<SpriteRenderer>().enabled = true;
                DataManager.instance.Camera_pieton.GetComponentInChildren<IconeMinimap>().enabled = false;
                DataManager.instance.Camera_pieton.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
		else if (statusMinimap == true)
		{
			DataManager.instance.Camera_pieton.GetComponentInChildren<IconeMinimap>().enabled = false;
			DataManager.instance.Camera_pieton.GetComponentInChildren<SpriteRenderer>().enabled = false;
            DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<IconeMinimap>().enabled = false;
            DataManager.instance.Camera_tour.transform.parent.GetComponentInChildren<SpriteRenderer>().enabled = false;
            animMinimap.Play("minimapEnlarge");
			statusMinimap = false;
            DataManager.instance.Camera_minimap.SetActive(false);
        }
	}
    

    public void TakeScreenshot()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        HideHUD();
        int AA = QualitySettings.antiAliasing;
        QualitySettings.antiAliasing = 0;
        yield return new WaitForEndOfFrame();

        //prendre photo
        #if UNITY_IOS
        StartCoroutine(ScreenShotBridgeIOS.SaveScreenShot(folderPath,ScreenShotStatus));
        isScreenShotSave = false;
        #endif

        #if UNITY_ANDROID
		folderPath = "DCIM/Archipratique/";
        StartCoroutine(ScreenShotBridgeANDROID.SaveScreenShot("Archipratique", folderPath, true, ScreenShotStatus));	
        isScreenShotSave = false;
        #endif

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        Application.CaptureScreenshot(folderPath + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", 1);
    	#endif

        yield return new WaitForEndOfFrame();
        //regen hud
        if (AA != 0)
        {
            QualitySettings.antiAliasing = AA;
        }

        RevealHUD();
    }

    public void HideHUD()
    {
        for (int i = 0; i < HUD.transform.childCount; i++)
        {
            HUD.transform.GetChild(i).gameObject.SetActive(false);
        }
        imagesNonContractuelles.SetActive(true);

    }
    public void RevealHUD()
    {
        for (int i = 0; i < HUD.transform.childCount; i++)
        {
            HUD.transform.GetChild(i).gameObject.SetActive(true);
        }
        credits.SetActive(false);
        
        
        if (DataManager.instance.MODE_COURANT != DataManager.MODE_PIETON)
        {
            minimap.SetActive(false);
        }
        
    }
}

