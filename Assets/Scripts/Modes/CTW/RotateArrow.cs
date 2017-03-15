using UnityEngine;

public class RotateArrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0, 35f * Time.deltaTime, 0);
	}
}
