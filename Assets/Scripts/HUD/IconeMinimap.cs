using UnityEngine;

public class IconeMinimap : MonoBehaviour {
	Quaternion rotation;
	float scale;
	public float scaleMin;
	public float scaleMax;
	public float speedPulse;

	void Awake() {
		rotation = transform.rotation;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		scale += speedPulse * Time.deltaTime;
		if (scale > scaleMax)
		{
			scale = scaleMax;
			speedPulse *= -1f;
		}
		else if (scale < scaleMin)
		{
			scale = scaleMin;
			speedPulse *= -1f;
		}
		transform.localScale = new Vector3 (scale * 60f, scale * 45f, 0.1f);
	}

	void LateUpdate(){
		transform.rotation = rotation;
        transform.position = new Vector3(transform.parent.position.x, transform.position.y, transform.parent.position.z);
	}
}
