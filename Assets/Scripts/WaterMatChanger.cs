using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMatChanger : MonoBehaviour {
	public Material m_dayMat;
	public Material m_nightMat;

	public void setDayMaterial()
	{
		this.GetComponent<MeshRenderer>().material = m_dayMat;
	}

	public void setNightMaterial()
	{
		this.GetComponent<MeshRenderer>().material = m_nightMat;
	}
}
