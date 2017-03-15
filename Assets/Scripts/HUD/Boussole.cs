using UnityEngine;

public class Boussole : MonoBehaviour {
    public Transform target;
	float angle_target_h;

    void Start()
    {
        
    }
    
    void Update() {
		Vector3 target_pos_local = Camera.main.transform.InverseTransformPoint(target.position);
		if (DataManager.instance.MODE_COURANT == DataManager.MODE_PLAN)
        {
            angle_target_h = Mathf.Atan2(target_pos_local.x, target_pos_local.y) * Mathf.Rad2Deg;
        }
        else
        {
            angle_target_h = Mathf.Atan2(target_pos_local.x, target_pos_local.z) * Mathf.Rad2Deg;
        }
		transform.eulerAngles = new Vector3(0, 0, -angle_target_h);
    }
}
