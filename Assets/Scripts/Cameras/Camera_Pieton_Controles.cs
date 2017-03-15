using UnityEngine;
using UnityEngine.EventSystems;

public class Camera_Pieton_Controles : MonoBehaviour {
    public float minY = -45.0f;
    public float maxY = 80.0f;

    float sensX;
    float sensY;

    #if UNITY_IOS || UNITY_EDITOR
    public float sensX_iOS = 1f;
    public float sensY_iOS = 0.9f;
    #endif
    #if UNITY_ANDROID || UNITY_EDITOR
    public float sensX_Android = 20f;
    public float sensY_Android = 15f;
    #endif
    #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
    public float sensX_PC = 40f;
    public float sensY_PC = 35f;
    #endif

    [HideInInspector] public float rotationY = 0.0f;
    [HideInInspector] public float rotationX = 0.0f;

	public float speed = 7f;

	// Use this for initialization
    
	void Start () {
        #if UNITY_IOS
		sensX = sensX_iOS;
		sensY = sensY_iOS;
        #endif
        #if UNITY_ANDROID
        sensX = sensX_Android;
		sensY = sensY_Android;
        #endif
        #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        sensX = sensX_PC;
        sensY = sensY_PC;
        #endif
    }

    private void OnEnable()
    {
        normalizeRotation();
        ClickToWalk.instance.resetCTW();
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_ANDROID || UNITY_IOS
		/*LOOK*/
        if ((Input.touchCount == 1) && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false))
        {
            rotationX += Input.touches[0].deltaPosition.x * sensX * Time.deltaTime;
            rotationY += Input.touches[0].deltaPosition.y * sensY * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        #else
        /*LOOK*/
        if (Input.GetMouseButton(0) && (EventSystem.current.IsPointerOverGameObject() == false))
        {
            rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }

		/*MOVE*/
		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position += -1 * transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Vector3 forw = transform.forward;
			forw.y = 0;
			transform.position += forw * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Vector3 forw = transform.forward;
			forw.y = 0;
			transform.position += -1 * forw * speed * Time.deltaTime;
		}
        #endif
    }

    public void normalizeRotation()
    {
        rotationX = transform.localEulerAngles.y;
        rotationY = - transform.localEulerAngles.x;
        if (rotationY < minY)
        {
            rotationY += 360f;
        }
        if (rotationY > maxY)
        {
            rotationY -= 360f;
        }
    }
}
