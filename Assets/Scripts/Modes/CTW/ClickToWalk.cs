using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToWalk : MonoBehaviour {
    private float m_clicTime; //temps du clic
    private bool m_longClic; //clic long
    private bool m_shortclic; //clic court
	private bool m_flagWalking; //signal pour lancer la marche
	private bool m_doneLookAt;
	public float m_proximityDistanceLimit; //distance de la caméra au point d'arrivée
	private RaycastHit m_hit; //données du pt cliqué
	private Ray m_ray;
	private Vector3 m_targetpos;
    private GameObject m_loadingImage; //image de TP
    public Sprite m_loadingSprite1;
    public Sprite m_loadingSprite2;
    public Sprite m_loadingSprite3;
    public Sprite m_loadingSprite4;
    public Sprite m_loadingSprite5;
    private Color m_newColor;
    public GameObject m_prefabArrow; //données de l'indicateur de marche
    private GameObject m_instanceArrow;
	public float m_cameraHeight = 1.7f;

    private static ClickToWalk _instance;
    public static ClickToWalk instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ClickToWalk>();
                //DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Start()
    {
        m_loadingImage = GameObject.Find("Loading");
        m_newColor = m_loadingImage.GetComponent<Image>().color;
        m_newColor.a = 0;
        m_loadingImage.GetComponent<Image>().color = m_newColor;
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

	void Update () {
        //Click to walk
        if (DataManager.instance.MODE_COURANT == DataManager.MODE_PIETON || DataManager.instance.MODE_COURANT == DataManager.MODE_PLAN)
        {
            if (Input.GetAxis("Mouse X") > 0.4f || Input.GetAxis("Mouse X") < -0.4f || Input.GetAxis("Mouse Y") > 0.4f || Input.GetAxis("Mouse Y") < -0.4f || EventSystem.current.IsPointerOverGameObject() == true)
            {
                m_longClic = false;
                m_shortclic = false;
                m_newColor.a = 0;
                m_loadingImage.GetComponent<Image>().color = m_newColor;
            }

            if (Input.GetMouseButtonDown(0))
            {
				m_doneLookAt = true;
				m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (DataManager.instance.MODE_COURANT == DataManager.MODE_ORBITAL || DataManager.instance.MODE_COURANT == DataManager.MODE_PLAN)
                {
					if (Physics.Raycast (m_ray, out m_hit))
					{
						if (m_hit.collider.tag == "Ground")
						{
							m_longClic = true;
						}
					}
                }
                m_clicTime = Time.time;
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_longClic = false;
                m_newColor.a = 0;
                m_loadingImage.GetComponent<Image>().color = m_newColor;
				if ((DataManager.instance.MODE_COURANT == DataManager.MODE_PIETON) && (EventSystem.current.IsPointerOverGameObject() == false) && ((Time.time - m_clicTime) < 0.25f))
                {
                    #if UNITY_ANDROID
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        return;
                    }
                    #endif
					if (Physics.Raycast(m_ray, out m_hit))
					{
						if (m_hit.collider.tag == "Ground")
						{
							m_shortclic = true;
						}
					}
                }
            }

			if (m_longClic) //Depuis mode orbital => teleportation
			{
				if ((Time.time - m_clicTime) > 1.4f) //TP
				{
                    m_longClic = false;
                    m_newColor.a = 0;
                    m_loadingImage.GetComponent<Image>().color = m_newColor;

					m_targetpos = m_hit.point;
					m_targetpos.y += m_cameraHeight;
					DataManager.instance.Camera_pieton.transform.position = m_targetpos;
                    DataManager.instance.setMode(DataManager.MODE_PIETON);
                }
                else if ((Time.time - m_clicTime) > 1.2f)//Display un loading sur la souris
                {
                    m_loadingImage.GetComponent<Image>().sprite = m_loadingSprite5;
                }
                else if ((Time.time - m_clicTime) > 1.0f)
                {
                    m_loadingImage.GetComponent<Image>().sprite = m_loadingSprite4;
                }
                else if ((Time.time - m_clicTime) > 0.8f)
                {
                    m_loadingImage.GetComponent<Image>().sprite = m_loadingSprite3;
                }
                else if ((Time.time - m_clicTime) > 0.6f)
				{
                    m_loadingImage.GetComponent<Image>().sprite = m_loadingSprite2;
				}
	            else if ((Time.time - m_clicTime) > 0.4f)
	            {
                    m_loadingImage.GetComponent<Image>().sprite = m_loadingSprite1;
                    m_newColor.a = 90;
                    m_loadingImage.GetComponent<Image>().color = m_newColor;
                    m_loadingImage.transform.position = Input.mousePosition;
	            }
			}
            if (m_shortclic) //depuis mode pieton => marche
            {
                m_shortclic = false;

                m_targetpos = m_hit.point;
				m_targetpos.y += m_cameraHeight;
				Destroy(m_instanceArrow);
				m_instanceArrow = Instantiate(m_prefabArrow, m_hit.point + new Vector3(0, 0.3f, 0), m_prefabArrow.transform.localRotation) as GameObject;
                m_flagWalking = true;//active la marche
                m_doneLookAt = false;//active le centrage de la vue;
            }
			#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                resetCTW();
            }
			#endif
            //marche
			if (m_flagWalking == true && m_instanceArrow != null)
			{
				if (m_doneLookAt == false)
				{
					Quaternion rotationCible = Quaternion.LookRotation(m_targetpos - DataManager.instance.Camera_pieton.transform.position);
                    DataManager.instance.Camera_pieton.transform.rotation = Quaternion.Slerp (DataManager.instance.Camera_pieton.transform.rotation, rotationCible, Time.deltaTime);
                    DataManager.instance.Camera_pieton.GetComponent<Camera_Pieton_Controles>().normalizeRotation();
                }
				Vector3 offset = m_targetpos - DataManager.instance.Camera_pieton.transform.position;
				if (offset.sqrMagnitude > (m_proximityDistanceLimit * m_proximityDistanceLimit))
	            {
	                DataManager.instance.Camera_pieton.transform.position = Vector3.MoveTowards(DataManager.instance.Camera_pieton.transform.position, m_targetpos, DataManager.instance.Camera_pieton.GetComponent<Camera_Pieton_Controles>().speed * Time.deltaTime);
                    #if UNITY_ANDROID
                    m_flagWalking = true; //sait-on jamais
                    #endif
                }
	            else
	            {
                    resetCTW();
	            }
			}
        }
	}

    public void resetCTW()
    {
        m_flagWalking = false;
        m_doneLookAt = true;
        Destroy(m_instanceArrow);
    }
}
