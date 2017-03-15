using UnityEngine;
using UnityEngine.EventSystems;

public class Camera_Plan_Controles : MonoBehaviour {
    [HideInInspector] public float speed;
    float zoomSpeed;
    public float hauteurMin = 12f;
	public float hauteurMax = 220f;

    #if UNITY_IOS || UNITY_EDITOR
    public float speed_iOS = 0.7f;
    public float zoomSpeed_iOS = 1.2f;
    #endif
    #if UNITY_ANDROID || UNITY_EDITOR
    public float speed_Android = 5f;
    public float zoomSpeed_Android = 300f;
    #endif
    #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
    public float speed_PC = 15f;
    public float zoomSpeed_PC = 650f;
    float rotationY = -90.0f;
	float rotationX = 0.0f;
	[HideInInspector] public float minX = -360.0f;
	[HideInInspector] public float maxX = 360.0f;
	[HideInInspector] public float minY = -90.0f;
	[HideInInspector] public float maxY = 90.0f;
    #endif

	void Start()
	{
        #if UNITY_IOS
		speed = speed_iOS;
		zoomSpeed = speed_iOS;
        #endif
        #if UNITY_ANDROID
        speed = speed_Android;
        zoomSpeed = zoomSpeed_Android;
        #endif
        #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        speed = speed_PC;
        zoomSpeed = zoomSpeed_PC;
        #endif
    }

    void Update()
	{
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

			transform.Translate(0, zoomSpeed * deltaMagnitudeDiff * Time.deltaTime , 0, Space.World);
			Vector3 clampedPosition = transform.position;
			clampedPosition.y = Mathf.Clamp(transform.position.y, hauteurMin, hauteurMax);
			transform.position = clampedPosition;
		}

		/*MOVE*/
		if ((Input.touchCount == 1) && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false))
		{
			Vector3 dir = new Vector3(-1 * Input.touches[0].deltaPosition.x, -1 * Input.touches[0].deltaPosition.y, 0);
			transform.Translate(2f * dir * speed * Time.deltaTime);
		}

        #else
        /*ZOOM*/
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			transform.Translate(0, -zoomSpeed * scroll * Time.deltaTime, 0, Space.World);
			Vector3 clampedPosition = transform.position;
			clampedPosition.y = Mathf.Clamp(transform.position.y, hauteurMin, hauteurMax);
			transform.position = clampedPosition;
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			if ((transform.position.y - zoomSpeed/2 * Time.deltaTime) > hauteurMin)
			{
				transform.Translate(0, - zoomSpeed/2 * Time.deltaTime, 0, Space.World);
			}
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			if ((transform.position.y + zoomSpeed/2 * Time.deltaTime) < hauteurMax)
			{
				transform.Translate(0, zoomSpeed/2 * Time.deltaTime, 0, Space.World);
			}
		}

		/*LOOK*/
		if (Input.GetKey(KeyCode.RightArrow))
		{
			rotationX += -100f * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, minY, maxY);
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			rotationX += 100f * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, minY, maxY);
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}

		/*MOVE*/
		if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
		{
			Vector3 dir = new Vector3(-1 * Input.GetAxis("Mouse X"), -1 * Input.GetAxis("Mouse Y"), 0);
			transform.Translate(5f * dir * speed * Time.deltaTime);
		}
        #endif
	}
}