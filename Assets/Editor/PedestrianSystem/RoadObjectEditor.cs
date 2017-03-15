using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoadObject))]
[ExecuteInEditMode]
[CanEditMultipleObjects]
public class RoadObjectEditor : Editor 
{
	RoadObject   PedestrianObject;
	RoadSystem   PedestrianSystem;
	
	void Awake()
	{
        PedestrianObject = (RoadObject)target;
	}
	
	public override void OnInspectorGUI () 
	{
		DrawDefaultInspector();
		
		if(!PedestrianObject)
			return;
		
		if(!PedestrianSystem)
		{
            GameObject obj = GameObject.Find("RoadSystem");
            if (obj && obj.GetComponent<RoadSystem>())
                PedestrianSystem = obj.GetComponent<RoadSystem>();
		}
		
	}
}
