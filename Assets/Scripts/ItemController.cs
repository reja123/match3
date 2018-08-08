using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemController : MonoBehaviour, IPointerDownHandler {

	public bool existCollision=false;
	public GameObject selectedGO;
	public GameObject deleteGO;
	public static  bool isAnimation = false;
	public static ItemController lastSelectObject;

	[HideInInspector]
	public int row;
	[HideInInspector]
	public int column;

	public float speed;

	[HideInInspector]
	public int type;

	public List<ItemController> listForDestroy = new List<ItemController> ();


	public void OnPointerDown(PointerEventData eventData)
	{
		
		if (isAnimation) {
			return;
		}
		
		if (lastSelectObject != null && lastSelectObject != this) {
			var differenceCollumn = Mathf.Abs (lastSelectObject.column - column);
			var differenceRow = Mathf.Abs (lastSelectObject.row - row);
			if (differenceRow > 1 || differenceCollumn > 1 || (differenceCollumn == 1 && differenceRow == 1)) {
				lastSelectObject.selectedGO.SetActive (false);
				lastSelectObject = this;
				selectedGO.SetActive (true);
				//Debug.Log ("Caz Cind nu se poate de mutat");
			} else {
				

				selectedGO.SetActive (true);
			
				isAnimation = true;
				StartCoroutine (StartChangeElements ());
				//Debug.Log ("Caz cind sau mutat");

			}

		} else {
			//Debug.Log ("Caz la prima selectare");
			selectedGO.SetActive (true);
			lastSelectObject = this;
		}
	
	}


	public IEnumerator StartChangeElements()
	{
		yield return new WaitForSeconds(0.1f);
		lastSelectObject.selectedGO.SetActive (false);
		selectedGO.SetActive (false);
		var tempRow = row;
		var tempCollumn = column;
		var temp = transform.position;
		transform.position = lastSelectObject.transform.position;
		row = lastSelectObject.row;
		column = lastSelectObject.column;

		lastSelectObject.transform.position = temp;
		lastSelectObject.row = tempRow;
		lastSelectObject.column = tempCollumn;

		var tempElement= GameController.obj.instantiateItems[row][column];
		GameController.obj.instantiateItems [row] [column] = GameController.obj.instantiateItems [lastSelectObject.row] [lastSelectObject.column];
		GameController.obj.instantiateItems [lastSelectObject.row] [lastSelectObject.column] = tempElement;

		yield return new WaitForEndOfFrame();
		lastSelectObject = null;
		isAnimation = false;
		GameController.obj.FindAndDestroyMatchItems ();

	}
		
		

	void OnCollisionEnter2D(Collision2D col)
	{
		existCollision = true;
	}
		
	void OnCollisionStay2D(Collision2D col)
	{
		existCollision = true;
	}


	void OnCollisionExit2D(Collision2D col)
	{
		existCollision = false;
	}


}
	