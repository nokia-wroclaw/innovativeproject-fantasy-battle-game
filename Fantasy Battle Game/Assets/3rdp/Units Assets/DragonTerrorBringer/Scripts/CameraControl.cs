using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	public Animator anim;
	int cameraAnim;
	int cameraAnimBack;

	void Start () 
	{
		anim = GetComponent<Animator>();
		cameraAnim = Animator.StringToHash("CameraAnim");
		cameraAnimBack = Animator.StringToHash("CameraAnimBack");
	}
	
	public void CameraAnim ()
	{
		anim.SetTrigger(cameraAnim);
	}

	public void CameraAnimBack ()
	{
		anim.SetTrigger(cameraAnimBack);
	}
	
}
