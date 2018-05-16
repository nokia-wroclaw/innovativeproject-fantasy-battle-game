using System;
using UnityEngine;
using UnityEngine.UI;

public class DSCController : MonoBehaviour{
    public Text Brief;
    public Text Lore;
    public Text Name;

    // Use this for initialization
    private void Start(){
        foreach (Transform child in transform){
            var obj = child.gameObject.GetComponent<Text>();
            if (obj.name == "DSC_Name")
                Name = obj;
            else if (obj.name == "DSC_Brief")
                Brief = obj;
            else if (obj.name == "DSC_Lore") Lore = obj;

            obj.text = "";
        }
    }

    
    
    public void UpdateStatus(IDescription description){
        if (description != null){
            setStatus(description.GetName(),description.GetBrief(),description.GetLore());
        }
        else{
            if (Name!=null && Brief!=null && Lore!=null){
                setStatus();
            }
        }
    }

    private void setStatus(String name="", String brief="", String lore=""){
        Name.text = name;
        Brief.text = brief;
        Lore.text = lore;
    }
}