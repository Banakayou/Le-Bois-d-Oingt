using UnityEngine;
using UnityEngine.EventSystems;

public class ModeMesure : MonoBehaviour {

	[HideInInspector] public GameObject DeleteMesuresButton;
	[HideInInspector] public bool mode_mesure;
	public LayerMask maskNormal;
	public LayerMask maskMesures;
	public GameObject prefabNodeMesure;
	private GameObject[] instancesNodes;
    [HideInInspector] public int nbNodes = 0;
	float timeInstancie;
	public GameObject prefabLigneMesure;
	private GameObject[] instancesLignes;
	private int nbLignes = 0;
	public GameObject prefabSurface;
	private GameObject[] instancesSurfaces;
	bool surfaceDrawn = false;
	public GameObject prefabTexte;
	private GameObject[] instancesTextes;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 targetpos;

    // Use this for initialization
    void Start ()
	{
		instancesNodes = new GameObject[4];
		instancesLignes = new GameObject[5];
		instancesSurfaces = new GameObject[2];
		instancesTextes = new GameObject[6];
		DeleteMesuresButton = GameObject.Find ("Delete_Mesures_Button");
		DeleteMesuresButton.SetActive (false);
	}

    // Update is called once per frame
    void Update ()
	{
		if (mode_mesure)
		{
			rotateBtnMesure();
			if (nbNodes <= 3)
			{
				if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false && (Time.time - timeInstancie > 0.25f))
				{
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit))
					{
						targetpos = hit.point;
						targetpos.y = 15f;
						instancesNodes[nbNodes] = Instantiate(prefabNodeMesure, targetpos, Quaternion.identity) as GameObject;
						nbNodes += 1;
						DeleteMesuresButton.SetActive(true);
						timeInstancie = Time.time;

						//Draw lines
						if (nbNodes > 1)
						{
							surfaceDrawn = false;
							drawLigne(instancesNodes[nbNodes-2].transform, instancesNodes[nbNodes-1].transform);
						}
					}
				}
			}
			if (!surfaceDrawn && nbNodes > 2)
			{
				drawLigne(instancesNodes[0].transform, instancesNodes[nbNodes-1].transform);//lastline
				drawSurface();
			}

			/*MOVE*/
			if (Input.GetMouseButton(1) && EventSystem.current.IsPointerOverGameObject() == false)
			{
				Vector3 dir = new Vector3(-1 * Input.GetAxis("Mouse X"), -1 * Input.GetAxis("Mouse Y"), 0);
				transform.Translate(5f * dir * transform.GetComponent<Camera_Plan_Controles>().speed * Time.deltaTime);
			}
		}
	}


	public void switchMesure()
	{
		mode_mesure = !mode_mesure;
		if (mode_mesure == false)
		{
            DeleteMesuresButton.SetActive(false);
			this.GetComponent<Camera>().orthographic = false;
			DataManager.instance.MesureModeButton.transform.localRotation = new Quaternion(0, 0, 0, 1);
			this.GetComponent<Camera>().cullingMask = maskNormal;
			this.GetComponent<Camera_Plan_Controles> ().enabled = true;
		}
		else
		{
            if (nbNodes > 0)
            {
                DeleteMesuresButton.SetActive(true);
            }
            if (transform.position.y > 60)
			{
				transform.position = new Vector3(transform.position.x, 60, transform.position.z);
			}
			this.GetComponent<Camera>().orthographicSize = transform.position.y / 2;
			this.GetComponent<Camera>().orthographic = true;
			this.GetComponent<Camera>().cullingMask = maskMesures;
			this.GetComponent<Camera_Plan_Controles> ().enabled = false;
		}
	}

	private void rotateBtnMesure()
	{
		DataManager.instance.MesureModeButton.transform.Rotate(0, 0, 75f * Time.deltaTime);
	}

    public void resetMesures()
    {
        for (int i = 0; i < nbNodes; i++)
        {
            Destroy(instancesNodes[i]);
        }
        nbNodes = 0;

        for (int i = 0; i < nbLignes; i++)
        {
            Destroy(instancesLignes[i]);
        }
        nbLignes = 0;

        for (int i = 0; i < instancesSurfaces.Length; i++)
        {
            Destroy(instancesSurfaces[i]);
        }
        surfaceDrawn = false;
        for (int i = 0; i < instancesTextes.Length; i++)
        {
            Destroy(instancesTextes[i]);
        }
        DeleteMesuresButton.SetActive(false);
    }

    private void drawLigne(Transform pos1, Transform pos2)
	{
		if (nbNodes == 4)
		{
			Destroy(instancesLignes[2]);
			Destroy(instancesTextes[2]);
		}
		instancesLignes[nbLignes] = Instantiate(prefabLigneMesure) as GameObject;
		instancesLignes[nbLignes].GetComponent<LineRenderer>().SetPosition(0, pos1.position);
		instancesLignes[nbLignes].GetComponent<LineRenderer>().SetPosition(1, pos2.position);

		instancesTextes[nbLignes] = Instantiate(prefabTexte, ((pos1.position + pos2.position)/2) + new Vector3(0, 1, 0), prefabTexte.transform.localRotation) as GameObject;
		instancesTextes[nbLignes].GetComponent<TextMesh>().text = "⋍" + Mathf.Floor((pos1.position-pos2.position).magnitude).ToString() + " m";
		instancesTextes[nbLignes].GetComponent<TextMesh>().characterSize = 0.7f * transform.position.y / 90f;
		nbLignes += 1;
	}

	private float calculerSurface()
	{
		float temp = 0;
		for (int i = 0; i < nbNodes; i++)
		{
			if (i != nbNodes - 1)
			{
				float A = instancesNodes[i].transform.position.x * instancesNodes [i + 1].transform.position.z;
				float B = instancesNodes[i+1].transform.position.x * instancesNodes[i].transform.position.z;
				temp = temp + (A - B);
			}
			else
			{
				float A = instancesNodes[i].transform.position.x * instancesNodes[0].transform.position.z;
				float B = instancesNodes[0].transform.position.x * instancesNodes[i].transform.position.z;
				temp = temp + (A - B);
			}
		}
		temp *= 0.5f;
		return Mathf.Abs(temp);
	}

	private void drawSurface ()
	{
		for (int x = 0; x < 2; x++)
		{
			Destroy(instancesSurfaces[x]);
			instancesSurfaces[x] = Instantiate(prefabSurface) as GameObject;
			instancesSurfaces[x].name = "Surface" + x;

			Mesh mesh = new Mesh();
			Vector3[] vertex = new Vector3[nbNodes];
			for (int i = 0; i < nbNodes; i++)
			{
				vertex[i] = new Vector3(instancesNodes[i].transform.position.x, instancesNodes[i].transform.position.y, instancesNodes[i].transform.position.z);
			}
			Vector2[] uvs = new Vector2[vertex.Length];
			for (int i = 0; i < vertex.Length; i++)
			{
				if ((i % 2) == 0)
				{
					uvs[i] = new Vector2(0, 0);
				}
				else
				{
					uvs[i] = new Vector2(1, 1);
				}
			}

			int[] tris = new int[3 * (vertex.Length - 2)];
			int C1;
			int C2;
			int C3;
			if (x == 0)
			{
				C1 = 0;
				C2 = 1;
				C3 = 2;
				for (int i = 0; i < tris.Length; i += 3)
				{
					tris[i] = C1;
					tris[i + 1] = C2;
					tris[i + 2] = C3;
					C2++;
					C3++;
				}
			}
			else
			{
				C1 = 0;
				C2 = vertex.Length - 1;
				C3 = vertex.Length - 2;
				for (int i = 0; i < tris.Length; i += 3)
				{
					tris[i] = C1;
					tris[i + 1] = C2;
					tris[i + 2] = C3;
					C2--;
					C3--;
				}
			}
			mesh.vertices = vertex;
			mesh.uv = uvs;
			mesh.triangles = tris;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals ();
			;
			mesh.name = "Mesh" + x;

			instancesSurfaces[x].GetComponent<MeshFilter>().mesh = mesh;
		}
		Destroy(instancesTextes[5]);
		instancesTextes[5] = Instantiate(prefabTexte, instancesSurfaces[0].transform.GetComponent<MeshFilter>().mesh.bounds.center + new Vector3(0, 1, 0), prefabTexte.transform.localRotation) as GameObject;
		instancesTextes[5].GetComponent<TextMesh>().text = "⋍" + Mathf.Floor(calculerSurface()).ToString() + " m²";
		instancesTextes[5].GetComponent<TextMesh> ().characterSize = transform.position.y / 90f;
		surfaceDrawn = true;
	}
}
