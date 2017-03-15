using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoadNode))]
[ExecuteInEditMode]
[CanEditMultipleObjects]
public class RoadNodeEditor : Editor 
{
    RoadNode RoadNode;
	RoadSystem   RoadSystem;
	
	void Awake()
	{
        RoadNode = (RoadNode)target;
		RoadNode.CleanupNodes();
	}
	
	public override void OnInspectorGUI () 
	{
		DrawDefaultInspector();
		
		if(!RoadNode)
			return;

		if(!RoadSystem)
		{
			GameObject obj = GameObject.Find ("RoadSystem");
            if (obj && obj.GetComponent<RoadSystem>())
                RoadSystem = obj.GetComponent<RoadSystem>();
		}

		if(!RoadSystem || !RoadSystem.Instance)
			return;

		GUILayout.Space(20.0f);
		
		GUILayout.BeginHorizontal("box");

		if(GUILayout.Button("Return to Pedestrian System"))
		{
			if (RoadSystem && RoadSystem.Instance)
			{
				Selection.activeObject = RoadSystem;
			}
		}

		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal("box");
		
		if(RoadSystem.TextureIconAnchor)
		{
			if(GUILayout.Button(RoadSystem.TextureIconAnchor))
			{
				RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.ANCHOR, RoadNode );
				//Selection.activeObject = PedestrianSystem.Instance;
			}
		}
		else
		{
			if(GUILayout.Button("Select as Anchor", GUILayout.Height(49)))
			{
				RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.ANCHOR, RoadNode );
				//Selection.activeObject = PedestrianSystem.Instance;
			}
		}

		if(RoadSystem.TextureIconEdit)
		{
			if(GUILayout.Button(RoadSystem.TextureIconEdit))
			{
				RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.EDIT, RoadNode );
				//Selection.activeObject = PedestrianSystem.Instance;
			}
		}
		else
		{
			if(GUILayout.Button("Select as Edit", GUILayout.Height(49)))
			{
				RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.EDIT, RoadNode );
				//Selection.activeObject = PedestrianSystem.Instance;
			}
		}
		
		GUILayout.EndHorizontal();
		bool guiChanged = false;

        RoadNode useAnchorNode = null;
        RoadNode useEditNode = null;
		
		if(RoadSystem.Instance.EditNode)
			useEditNode = RoadSystem.Instance.EditNode;
		else if(RoadSystem.Instance.PreviousEditNode)
			useEditNode = RoadSystem.Instance.PreviousEditNode;
		
		if(RoadSystem.Instance.AnchorNode)
			useAnchorNode = RoadSystem.Instance.AnchorNode;
		else if(RoadSystem.Instance.PreviousAnchorNode)
			useAnchorNode = RoadSystem.Instance.PreviousAnchorNode;
		
		if(useAnchorNode && useEditNode &&
		   (useAnchorNode == RoadNode || useEditNode == RoadNode))
		{
			GUILayout.BeginHorizontal("box");
			
			if(RoadSystem && RoadSystem.TextureIconAnchorToEdit)
			{
				if(GUILayout.Button(RoadSystem.TextureIconAnchorToEdit))
				{
					// link Anchor To Edit
					RoadSystem.Instance.LinkNode();
					guiChanged = true;
				}
			}
			else
			{
				if(GUILayout.Button("Link Anchor To Edit", GUILayout.Height(49)))
				{
					// link Anchor To Edit
					RoadSystem.Instance.LinkNode();
					guiChanged = true;
				}
			}
			
			if(RoadSystem.TextureIconEditToAnchor)
			{
				if(GUILayout.Button(RoadSystem.TextureIconEditToAnchor))
				{
					// link Edit to Anchor
					RoadSystem.Instance.LinkNode( false );
					guiChanged = true;
				}
			}
			else
			{
				if(GUILayout.Button("Link Edit to Anchor", GUILayout.Height(49)))
				{
					// link Edit to Anchor
					RoadSystem.Instance.LinkNode( false );
					guiChanged = true;
				}
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal("box");
		if(!RoadNode.GetComponent<PedestrianVehicleCheckTrigger>())
		{
			if(GUILayout.Button("Enable Pedestrian Crossing"))
			{
				RoadNode.gameObject.AddComponent<PedestrianVehicleCheckTrigger>();
				return;
			}
		}
		else
		{
			if(GUILayout.Button("Disable Pedestrian Crossing"))
			{
				if(RoadNode.GetComponent<PedestrianVehicleCheckTrigger>())
					DestroyImmediate(RoadNode.GetComponent<PedestrianVehicleCheckTrigger>());
				if(RoadNode.GetComponent<Rigidbody>())
					DestroyImmediate(RoadNode.GetComponent<Rigidbody>());
				if(RoadNode.GetComponent<Collider>())
					DestroyImmediate(RoadNode.GetComponent<Collider>());

				Debug.LogError("PEDESTRIAN SYSTEM -> The 'Missing Reference Exception' error below can be ignored");
				return;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("Delete Node"))
		{
			DestroyImmediate(RoadNode.gameObject);
			return;
		}
		GUILayout.EndHorizontal();

		if(RoadNode.m_nodes.Count > 0)
		{
			GUILayout.BeginHorizontal("box");
			if(GUILayout.Button("Remove All Links"))
			{
				RoadNode.RemoveAllNodes();
				EditorUtility.SetDirty(RoadNode);
			}
			GUILayout.EndHorizontal();

			for(int pIndex = 0; pIndex < RoadNode.m_nodes.Count; pIndex++)
			{
				GUILayout.BeginHorizontal("box");
				if(GUILayout.Button("Reveal ->", GUILayout.Height(49)))
				{
					RoadSystem.ClearCLRevealObjsFrom();
					RoadSystem.ClearCLRevealObjsTo();
					RoadSystem.AddToCLRevealObjsFrom(RoadNode.transform);
					RoadSystem.AddToCLRevealObjsTo(RoadNode.m_nodes[pIndex].transform);
				}
				if(RoadSystem.TextureIconRemoveAll)
				{
					if(GUILayout.Button(RoadSystem.TextureIconRemoveAll))
					{
						RoadNode.RemoveNode(RoadNode.m_nodes[pIndex]);
						EditorUtility.SetDirty(RoadNode);
						RoadSystem.ClearCLRevealObjsFrom();
						RoadSystem.ClearCLRevealObjsTo();
					}
				}
				GUILayout.EndHorizontal();
			}
		}

		/*
		if(PedestrianSystem.Instance.EditNode && PedestrianNode.NodeExists(PedestrianSystem.Instance.EditNode))
		{
			if(PedestrianSystem.TextureIconRemoveEdit)
			{
				if(GUILayout.Button(PedestrianSystem.TextureIconRemoveEdit))
					PedestrianNode.RemoveNode(PedestrianSystem.Instance.EditNode);
			}
			else
			{
				if(GUILayout.Button("Remove Edit Link"))
					PedestrianNode.RemoveNode(PedestrianSystem.Instance.EditNode);
			}
		}

		if(PedestrianSystem.Instance.AnchorNode && PedestrianNode.NodeExists(PedestrianSystem.Instance.AnchorNode))
		{
			if(PedestrianSystem.TextureIconRemoveAnchor)
			{
				if(GUILayout.Button(PedestrianSystem.TextureIconRemoveAnchor))
					PedestrianNode.RemoveNode(PedestrianSystem.Instance.AnchorNode);
			}
			else
			{
				if(GUILayout.Button("Remove Anchor Link"))
					PedestrianNode.RemoveNode(PedestrianSystem.Instance.AnchorNode);
			}
		}
		*/


		if(RoadSystem.Instance.PreviousAnchorNode == RoadNode)
			RoadSystem.Instance.PositionTooltip(RoadSystem.Tooltip.ANCHOR, RoadNode.gameObject);
		else if(RoadSystem.Instance.PreviousEditNode == RoadNode)
			RoadSystem.Instance.PositionTooltip(RoadSystem.Tooltip.EDIT, RoadNode.gameObject);

		if(guiChanged)
		{
			EditorUtility.SetDirty(useAnchorNode);
			EditorUtility.SetDirty(useEditNode);
		}

		if(GUI.changed)
			EditorUtility.SetDirty(RoadNode);
	}
}
