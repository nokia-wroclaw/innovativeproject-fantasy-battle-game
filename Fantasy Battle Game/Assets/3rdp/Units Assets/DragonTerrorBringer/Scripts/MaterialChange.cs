using UnityEngine;
using System.Collections;

public class MaterialChange : MonoBehaviour 
{
	public Material[] mats;
	public GameObject dragonTerrorBringer;



	void Awake () 
	{
		dragonTerrorBringer.GetComponent<Renderer>().material = mats[0];
	}





	public void MatChangeToGrey ()
	{
		dragonTerrorBringer.GetComponent<Renderer>().material = mats[0];
	}

	public void MatChangeToRed ()
	{
		dragonTerrorBringer.GetComponent<Renderer>().material = mats[1];
	}

	public void MatChangeToGreen ()
	{
		dragonTerrorBringer.GetComponent<Renderer>().material = mats[2];
	}

	public void MatChangeToAlbino ()
	{
		dragonTerrorBringer.GetComponent<Renderer>().material = mats[3];
	}
	
}
