using UnityEngine;
using UnityEngine.EventSystems;

public class Camera_Orbitale_Controles : MonoBehaviour
{
	public Transform target;
	float distance;
    public float distanceStart;
    public float distanceMin = 30f;
	public float distanceMax = 250f;
	float _x;
	float _y;
	[HideInInspector] public float Ymax = 85f;
	[HideInInspector] public float Ymin = 15f;

    float sensX;
    float sensY;
    float zoomSpeed;
    #if UNITY_IOS || UNITY_EDITOR
    public float sensX_iOS = 0.7f;
    public float sensY_iOS = 0.7f;
    public float zoomSpeed_iOS = 1.2f;
    #endif
    #if UNITY_ANDROID || UNITY_EDITOR
    public float sensX_Android = 10f;
    public float sensY_Android = 10f;
    public float zoomSpeed_Android = 300f;
    #endif
    #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
    public float sensX_PC = 30f;
    public float sensY_PC = 30f;
    public float zoomSpeed_PC = 650f;
    #endif

    void Start () {
		distance = distanceStart;
		_y = 25;
		_x = 330;

		RotateCamera();
		transform.LookAt(target);

        #if UNITY_IOS
		sensX = sensX_iOS;
		sensY = sensY_iOS;
		zoomSpeed = zoomSpeed_iOS;
        #endif
        #if UNITY_ANDROID
        sensX = sensX_Android;
        sensY = sensY_Android;
        zoomSpeed = zoomSpeed_Android;
        #endif
        #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        sensX = sensX_PC;
        sensY = sensY_PC;
        zoomSpeed = zoomSpeed_PC;
        #endif
    }

    void Update() {
#if UNITY_IOS || UNITY_ANDROID
        /*ZOOM*/
		if (Input.touchCount == 2)
		{
		// Store both touches.
		Touch touchZero = Input.GetTouch(0);
		Touch touchOne = Input.GetTouch(1);

		// Find the position in the previous frame of each touch.
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		// Find the magnitude of the vector (the distance) between the touches in each frame.
		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

		// Find the difference in the distances between each frame.
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		distance += zoomSpeed * deltaMagnitudeDiff * Time.deltaTime;
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		}

		/*LOOK*/
		if ((Input.touchCount == 1) && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false))
		{
		_x += Input.touches[0].deltaPosition.x * sensX * Time.deltaTime;
		_y -= Input.touches[0].deltaPosition.y * sensY * Time.deltaTime;
		_y = Mathf.Clamp(_y, Ymin, Ymax);

		RotateCamera();
		}

        #else
        /*ZOOM*/
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel") ;
			distance += -zoomSpeed * scroll * Time.deltaTime;
			distance = Mathf.Clamp(distance, distanceMin, distanceMax);
			RotateCamera();
		}

		/*LOOK*/
		if (Input.GetMouseButton(0) && (EventSystem.current.IsPointerOverGameObject() == false))
		{
			_x += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
			_y -= Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
			_y = Mathf.Clamp(_y, Ymin, Ymax);
			RotateCamera();
		}

		/*MOVE*/
		if (Input.GetKey(KeyCode.RightArrow))
		{
			target.transform.position += 20f * transform.right * Time.deltaTime;
			RotateCamera();
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			target.transform.position += -20f * transform.right * Time.deltaTime;
			RotateCamera();
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Vector3 forw = transform.forward;
			forw.y = 0;
			target.transform.position += 20f * forw * Time.deltaTime;
			RotateCamera();
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Vector3 forw = transform.forward;
			forw.y = 0;
			target.transform.position += -20f * forw * Time.deltaTime;
			RotateCamera();
		}
        
		#endif
	}

	void RotateCamera() {
		Quaternion rotation = Quaternion.Euler(_y, _x, 0);
		Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
		transform.rotation = rotation;
		transform.position = position;
	}
}