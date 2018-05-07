using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DSCController : MonoBehaviour {
	public Text Name;
	public Text Brief;
	public Text Lore;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			var obj = child.gameObject.GetComponent<Text>();
			if(obj.name == "DSC_Name"){
				Name = obj;
			}
			else if(obj.name == "DSC_Brief"){
				Brief = obj;
			}
			else if(obj.name == "DSC_Lore"){
				Lore = obj;
			}

			obj.text="";
		}
	}
	
	public void UpdateStatus(IDescription description){
		Name.text=description.GetName();
		Brief.text=description.GetBrief();
		Lore.text=description.GetLore();
	}
}
