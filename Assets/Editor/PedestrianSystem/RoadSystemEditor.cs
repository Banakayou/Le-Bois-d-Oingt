using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(RoadSystem))]
public class RoadSystemEditor : Editor 
{
    private RoadSystem RoadSystem { get; set; }

	private string[]            m_prefabLocation;

    private List<RoadObject> m_allAssets = new List<RoadObject>();
    private List<RoadObject> m_lowAssets = new List<RoadObject>();
    private List<RoadObject> m_mediumAssets = new List<RoadObject>();
    private List<RoadObject> m_highAssets = new List<RoadObject>();

	static public  RoadNode   AnchorNode       { get; set; }
    static public RoadNode EditNode { get; set; }

	static public string  TooltipAnchorLocation      = "Assets/Prefabs/PedestrianSystem/Base/ToolTip-Anchor.prefab";
	static public string  TooltipEditLocation        = "Assets/Prefabs/PedestrianSystem/Base/ToolTip-Edit.prefab";

	private Transform     EditNodeFolder             { get; set; }
	private string        m_editNodeFolderName       = "Edit Folder";
	private Transform     AnchorNodeFolder           { get; set; }
	private string        m_anchorNodeFolderName     = "Anchor Folder";
	private Transform     ResourceNodeFolder         { get; set; }
	private string        m_resourceNodeFolderName   = "Resource Folder";

	void Awake()
	{
        RoadSystem = (RoadSystem)target;

        if (!RoadSystem)
			Debug.LogError("Pedestrian System Error -> \"Pedestrian System\" script must be in the scene. It is missing. Drop in the \"PedestrianSystem\" Prefab to fix this.");

		string sDataPath  = Application.dataPath + "/Prefabs/PedestrianSystem/Resources/Active/Voitures/";
		
		// get the system file paths of all the files in the asset folder
		string[] aFilePaths = Directory.GetFiles(sDataPath);
		
		// enumerate through the list of files loading the assets they represent and getting their type
		
		int count = 0;
		foreach (string sFilePath in aFilePaths) 
		{
			if(sFilePath.Length <= 6)
				continue;
			
			string sAssetPath = sFilePath.Substring(sFilePath.Length - 6, 6);
			
			if(sAssetPath == "prefab")
				count++;
		}
		
		m_prefabLocation = new string[count];
		
		m_highAssets.Clear();
		
		count = 0;
		foreach (string sFilePath in aFilePaths) 
		{
			if(sFilePath.Length <= 6)
				continue;
			
			string sAssetPath = sFilePath.Substring(sFilePath.Length - 6, 6);
			
			if(sAssetPath == "prefab")
			{
				string file = sFilePath.Substring(sDataPath.Length);
				m_prefabLocation[count] = sDataPath + file;
				//Debug.Log("count " + count + " : " + m_prefabLocation[count]);

                RoadObject asset = AssetDatabase.LoadAssetAtPath(m_prefabLocation[count], typeof(RoadObject)) as RoadObject;
				
				if(asset)
				{
                    if (asset.m_assetFrequency == RoadSystem.ObjectFrequency.HIGH)
						m_highAssets.Add(asset);
                    else if (asset.m_assetFrequency == RoadSystem.ObjectFrequency.MEDIUM)
						m_mediumAssets.Add(asset);
                    else if (asset.m_assetFrequency == RoadSystem.ObjectFrequency.LOW)
						m_lowAssets.Add(asset);
					
					m_allAssets.Add(asset);
				}
				
				count++;
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

        if (!RoadSystem || !RoadSystem.Instance)
			return;

		GUILayout.Space(10.0f);
		GUILayout.BeginVertical("box");

		if(GUILayout.Button("Generate Node", GUILayout.Height(49)))
		{
            if (!RoadSystem.Instance.m_nodePrefab)
			{
				Debug.LogError("Node Prefab not set. You can find one in \"Assets\\Plugins\\Pedestrian System\\Prefabs\\Pathing\". Node Generation Cancelled.");
				return;
			}

            RoadNode node = PrefabUtility.InstantiatePrefab(RoadSystem.Instance.m_nodePrefab) as RoadNode;
			Selection.activeGameObject = node.gameObject;
            node.transform.parent = RoadSystem.Instance.transform;

            if (RoadSystem.Instance.m_autoLink)
			{
                if (RoadSystem.Instance.m_linkBothDir)
				{
					if(EditNode)
					{
						EditNode.AddNode( node );
						EditorUtility.SetDirty( EditNode );

						node.AddNode( EditNode );
						EditorUtility.SetDirty( node );

						Vector3 pos = EditNode.transform.position;
						pos.x += 4.0f;
						node.transform.position = pos;
					}
					else if(AnchorNode)
					{
						AnchorNode.AddNode( node );
						EditorUtility.SetDirty( AnchorNode );
						
						node.AddNode( AnchorNode );
						EditorUtility.SetDirty( node );

						Vector3 pos = AnchorNode.transform.position;
						pos.x += 4.0f;
						node.transform.position = pos;
					}
				}
				else if(EditNode)
				{
					EditNode.AddNode( node );
					EditorUtility.SetDirty(EditNode);

					Vector3 pos = EditNode.transform.position;
    			    pos.x += 4.0f;
					node.transform.position = pos;
				}
				else if(AnchorNode)
				{
					AnchorNode.AddNode( node );
					EditorUtility.SetDirty(AnchorNode);

					Vector3 pos = AnchorNode.transform.position;
					pos.x += 4.0f;
					node.transform.position = pos;
				}
			}

			if(EditNode)
			{
				if(AnchorNode)
                    AnchorNode.transform.parent = RoadSystem.transform;

				AnchorNode = EditNode;
				AnchorNode.transform.parent = AnchorNodeFolder.transform;

				EditNode   = node;
				EditNode.transform.parent   = EditNodeFolder.transform;

                RoadSystem.Instance.SetPedestrianNode(RoadSystem.Tooltip.ANCHOR, AnchorNode);
			}
			else
			{
				EditNode   = node;
				EditNode.transform.parent   = EditNodeFolder.transform;
			}

            RoadSystem.Instance.SetPedestrianNode(RoadSystem.Tooltip.EDIT, node);
		}

		GUILayout.EndVertical();
		bool guiChanged = false;

		if(EditNode && AnchorNode)
		{
			GUILayout.BeginHorizontal("box");
			if(GUILayout.Button("Go to 'Edit' Piece", GUILayout.Height(49)))
				Selection.activeObject = EditNode.transform;
			
			if(GUILayout.Button("Go to 'Anchor' Piece", GUILayout.Height(49)))
				Selection.activeObject = AnchorNode.transform;
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10.0f);
		}

        RoadNode useAnchorNode = null;
        RoadNode useEditNode = null;

        if (RoadSystem.Instance.EditNode)
            useEditNode = RoadSystem.Instance.EditNode;
        else if (RoadSystem.Instance.PreviousEditNode)
            useEditNode = RoadSystem.Instance.PreviousEditNode;

        if (RoadSystem.Instance.AnchorNode)
            useAnchorNode = RoadSystem.Instance.AnchorNode;
        else if (RoadSystem.Instance.PreviousAnchorNode)
            useAnchorNode = RoadSystem.Instance.PreviousAnchorNode;

		if(useAnchorNode && useEditNode)
		{
			GUILayout.BeginHorizontal("box");

            if (RoadSystem && RoadSystem.TextureIconAnchorToEdit)
			{
                if (GUILayout.Button(RoadSystem.TextureIconAnchorToEdit))
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

            if (RoadSystem.TextureIconEditToAnchor)
			{
                if (GUILayout.Button(RoadSystem.TextureIconEditToAnchor))
				{
					// link Edit to Anchor
                    RoadSystem.Instance.LinkNode(false);
					guiChanged = true;
				}
			}
			else
			{
				if(GUILayout.Button("Link Edit to Anchor", GUILayout.Height(49)))
				{
					// link Edit to Anchor
                    RoadSystem.Instance.LinkNode(false);
					guiChanged = true;
				}
			}
			GUILayout.EndHorizontal();
		}

        //		if(RoadSystem.Instance.EditNode)
//		{
        //			SetPedestrianNode( RoadSystem.Tooltip.EDIT, RoadSystem.Instance.EditNode );
        //			RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.EDIT, null );
//		}
//		
        //		if(RoadSystem.Instance.AnchorNode)
//		{
        //			SetPedestrianNode( RoadSystem.Tooltip.ANCHOR, RoadSystem.Instance.AnchorNode );
        //			RoadSystem.Instance.SetPedestrianNode( RoadSystem.Tooltip.ANCHOR, null );
//		}

		if(guiChanged)
		{
			EditorUtility.SetDirty(useAnchorNode);
			EditorUtility.SetDirty(useEditNode);
		}
				
		if(EditNodeFolder)
		{
			for(int cIndex = 0; cIndex < EditNodeFolder.childCount; cIndex++)
                EditNodeFolder.GetChild(cIndex).transform.parent = RoadSystem.transform;
			
			if(EditNode)
				EditNode.transform.parent   = EditNodeFolder;
		}
		
		if(AnchorNodeFolder)
		{
			for(int cIndex = 0; cIndex < AnchorNodeFolder.childCount; cIndex++)
                AnchorNodeFolder.GetChild(cIndex).transform.parent = RoadSystem.transform;
			
			if(AnchorNode)
				AnchorNode.transform.parent = AnchorNodeFolder;
		}

		if(AnchorNode)
            SetPedestrianNode(RoadSystem.Tooltip.ANCHOR, AnchorNode);
	
		if(EditNode)
            SetPedestrianNode(RoadSystem.Tooltip.EDIT, EditNode);

		if(GUI.changed)
            EditorUtility.SetDirty(RoadSystem);
	}

	void OnSceneGUI()
	{
		ProcessFolderChecks();
		//CheckForAnchorNode();
		//CheckForEditNode();

        if (RoadSystem && RoadSystem.Instance)
		{
			if(!EditNode)
                RoadSystem.Instance.ShowTooltip(RoadSystem.Tooltip.EDIT, false);
			else
                RoadSystem.Instance.ShowTooltip(RoadSystem.Tooltip.EDIT, true);
			
			if(!AnchorNode)
                RoadSystem.Instance.ShowTooltip(RoadSystem.Tooltip.ANCHOR, false);
			else
                RoadSystem.Instance.ShowTooltip(RoadSystem.Tooltip.ANCHOR, true);

            if (!RoadSystem.Instance.TooltipAnchor)
			{
				if(ResourceNodeFolder && ResourceNodeFolder.childCount > 1)
				{
					for(int cIndex = 1; cIndex < ResourceNodeFolder.childCount; cIndex++)
					{
						if(ResourceNodeFolder.GetChild(cIndex) && ResourceNodeFolder.GetChild(cIndex).name == "Tooltip-Anchor")
							if(ResourceNodeFolder.GetChild(cIndex).gameObject)
								DestroyImmediate(ResourceNodeFolder.GetChild(cIndex).gameObject);
					}
				}
				
				GameObject tooltip = GameObject.Find ("Tooltip-Anchor");
				if (tooltip)
                    RoadSystem.Instance.TooltipAnchor = tooltip;
				else
				{
					GameObject toolTip = AssetDatabase.LoadAssetAtPath(TooltipAnchorLocation, typeof(GameObject)) as GameObject;
                    RoadSystem.Instance.TooltipAnchor = PrefabUtility.InstantiatePrefab(toolTip) as GameObject;
				}
			}
			else
			{
				if(ResourceNodeFolder)
                    RoadSystem.Instance.TooltipAnchor.transform.parent = ResourceNodeFolder;
			}

            if (!RoadSystem.Instance.TooltipEdit)
			{
				if(ResourceNodeFolder && ResourceNodeFolder.childCount > 1)
				{
					for(int cIndex = 1; cIndex < ResourceNodeFolder.childCount; cIndex++)
					{
						if(ResourceNodeFolder.GetChild(cIndex) && ResourceNodeFolder.GetChild(cIndex).name == "Tooltip-Edit")
							if(ResourceNodeFolder.GetChild(cIndex).gameObject)
								DestroyImmediate(ResourceNodeFolder.GetChild(cIndex).gameObject);
					}
				}
				
				GameObject tooltip = GameObject.Find ("Tooltip-Edit");
				if (tooltip)
                    RoadSystem.Instance.TooltipEdit = tooltip;
				else
				{
					GameObject toolTip = AssetDatabase.LoadAssetAtPath(TooltipEditLocation, typeof(GameObject)) as GameObject;
                    RoadSystem.Instance.TooltipEdit = PrefabUtility.InstantiatePrefab(toolTip) as GameObject;
				}
			}
			else
			{
				if(ResourceNodeFolder)
                    RoadSystem.Instance.TooltipEdit.transform.parent = ResourceNodeFolder;
			}

            if (RoadSystem.Instance.EditNode)
			{
                SetPedestrianNode(RoadSystem.Tooltip.EDIT, RoadSystem.Instance.EditNode);
                RoadSystem.Instance.SetPedestrianNode(RoadSystem.Tooltip.EDIT, null);
			}

            if (RoadSystem.Instance.AnchorNode)
			{
                SetPedestrianNode(RoadSystem.Tooltip.ANCHOR, RoadSystem.Instance.AnchorNode);
                RoadSystem.Instance.SetPedestrianNode(RoadSystem.Tooltip.ANCHOR, null);
			}

            if (RoadSystem.Instance.TooltipAnchor)
                RoadSystem.Instance.TooltipAnchor.transform.Rotate(Vector3.up, 1.0f);
            if (RoadSystem.Instance.TooltipEdit)
                RoadSystem.Instance.TooltipEdit.transform.Rotate(Vector3.up, 1.0f); 
		}
	}

	void ProcessFolderChecks( bool a_avoidCreation = false )
	{
		if(!EditNodeFolder)
		{
			bool foundEditNodeFolder = false;
            for (int cIndex = 0; cIndex < RoadSystem.transform.childCount; cIndex++)
			{
                if (RoadSystem.transform.GetChild(cIndex).name == m_editNodeFolderName)
				{
                    EditNodeFolder = RoadSystem.transform.GetChild(cIndex);
					foundEditNodeFolder = true;
					break;
				}
			}
			
			if(!foundEditNodeFolder && !a_avoidCreation)
			{
				EditNodeFolder = new GameObject().transform;
				EditNodeFolder.name = m_editNodeFolderName;
                EditNodeFolder.transform.parent = RoadSystem.transform;
			}
		}

		if(!AnchorNodeFolder)
		{
			bool foundAnchorNodeFolder = false;
            for (int cIndex = 0; cIndex < RoadSystem.transform.childCount; cIndex++)
			{
                if (RoadSystem.transform.GetChild(cIndex).name == m_anchorNodeFolderName)
				{
                    AnchorNodeFolder = RoadSystem.transform.GetChild(cIndex);
					foundAnchorNodeFolder = true;
					break;
				}
			}
			
			if(!foundAnchorNodeFolder && !a_avoidCreation)
			{
				AnchorNodeFolder = new GameObject().transform;
				AnchorNodeFolder.name = m_anchorNodeFolderName;
                AnchorNodeFolder.transform.parent = RoadSystem.transform;
			}
		}

		if(!ResourceNodeFolder)
		{
			bool foundResourceNodeFolder = false;
            for (int cIndex = 0; cIndex < RoadSystem.transform.childCount; cIndex++)
			{
                if (RoadSystem.transform.GetChild(cIndex).name == m_resourceNodeFolderName)
				{
                    ResourceNodeFolder = RoadSystem.transform.GetChild(cIndex);
					foundResourceNodeFolder = true;
					break;
				}
			}
			
			if(!foundResourceNodeFolder && !a_avoidCreation)
			{
				ResourceNodeFolder = new GameObject().transform;
				ResourceNodeFolder.name = m_resourceNodeFolderName;
                ResourceNodeFolder.transform.parent = RoadSystem.transform;
			}
		}		
	}
	
	void CheckForAnchorNode()
	{
		if(!AnchorNodeFolder)
			return;
		
		for(int cIndex = 0; cIndex < AnchorNodeFolder.childCount; cIndex++)
		{
            if (AnchorNodeFolder.GetChild(cIndex).GetComponent<RoadNode>())
			{
                SetPedestrianNode(RoadSystem.Tooltip.ANCHOR, AnchorNodeFolder.GetChild(cIndex).GetComponent<RoadNode>());
			}
		}
	}

	void CheckForEditNode()
	{
		if(!EditNodeFolder)
			return;
		
		for(int cIndex = 0; cIndex < EditNodeFolder.childCount; cIndex++)
		{
			if(EditNodeFolder.GetChild(cIndex).GetComponent<RoadNode>())
			{
                SetPedestrianNode(RoadSystem.Tooltip.EDIT, EditNodeFolder.GetChild(cIndex).GetComponent<RoadNode>());
			}
		}
	}

    public void SetPedestrianNode(RoadNode a_piece)
	{
		EditNode   = a_piece;
		AnchorNode = a_piece;
	}

    void SetPedestrianNode(RoadSystem.Tooltip a_tooltip, RoadNode a_obj)
	{
		switch(a_tooltip)
		{
            case RoadSystem.Tooltip.ANCHOR:
		{
			AnchorNode = a_obj;
            if (RoadSystem.Instance && AnchorNode)
			{
                //RoadSystem.Instance.AnchorNode = AnchorNode;
                RoadSystem.Instance.PositionTooltip(a_tooltip, AnchorNode.gameObject);
			}
		}
			break;
            case RoadSystem.Tooltip.EDIT:
		{
			EditNode = a_obj;

            if (RoadSystem.Instance && EditNode)
			{
                ///RoadSystem.Instance.EditNode = EditNode;
                RoadSystem.Instance.PositionTooltip(a_tooltip, EditNode.gameObject);
			}
		}
			break;
		}
		
	}
}
