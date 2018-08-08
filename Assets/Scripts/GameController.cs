using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	public Text totalScoreText;
	public Text scoreInfoText;
	public float totalScore;
	public float cellSize=100f;
	public float cellSpacing=5;
	public float offsetYForCreate = 100;
	public static GameController obj;
	public int gameSize = 9;
	public Object[] typeItems;
	public Transform itemsParent;
	public GameObject initialSpawnPoint;
	public ItemController[][] instantiateItems;
	int[] countMatchInGroup;
	Vector3[] spawnPointsPosition;
	int rnd;
	int group;
	float currentOffset;
	List<ItemsCoordonate> listForRemove = new List<ItemsCoordonate> ();


	void Awake ()
	{
		//asd
		obj = this;

		#region setSpawnPointPosition
		spawnPointsPosition = new Vector3[gameSize];
		spawnPointsPosition [0] = initialSpawnPoint.transform.position;

		for (int i = 1; i < gameSize; i++) {
			spawnPointsPosition [i] = new Vector3 (spawnPointsPosition [i - 1].x + (cellSize / 2 + cellSpacing), spawnPointsPosition [i - 1].y, spawnPointsPosition [i - 1].z);
		}
		#endregion
		CreateItems ();
	}
		

	public ItemController CreateElement(int row, int column, float offset)
	{
		Vector3 newPosition = new Vector3 ();
		newPosition = spawnPointsPosition [column];
		newPosition.y += offset;

		int rnd = Random.Range (0, 5);
		var temp = (GameObject)Instantiate (typeItems [rnd], newPosition, Quaternion.identity, itemsParent);
		ItemController itemController = temp.GetComponent<ItemController> ();

		itemController.row = row;
		itemController.column = column;
		itemController.type = rnd;
		instantiateItems [row] [column] = itemController;
		return itemController;
	}


	public IEnumerator WaitEndFall()
	{
		yield return new WaitForSeconds (0.1f);
		bool isEndFall=false;
		bool execute = false;
		while (isEndFall == false) {
			yield return new WaitForSeconds (0.1f);
			execute = false;
			for (int i = 0; i < gameSize; i++) {
				if (!instantiateItems [0] [i].existCollision) {
					execute = true;
					break;
				}
			}

			if (!execute) {
				isEndFall = true;
			}
		}
			
		GameController.obj.FindAndDestroyMatchItems ();


	}

	public void CreateItems()
	{
		Vector3 newPosition = new Vector3 ();
		instantiateItems =new ItemController[gameSize][];
		for (int i = 0; i < gameSize; i++) {
			instantiateItems [i] = new ItemController[gameSize];
		}

		for (int i = 0; i < gameSize; i++) {
			for (int j = 0; j < gameSize; j++) {

				newPosition = spawnPointsPosition [j];
				newPosition.y -= currentOffset;


				bool find=false;
				do {
					rnd = Random.Range (0, 5);
					find = GameController.obj.CheckNewElement (i, j, rnd);


				} while (find==false);

			
				var temp = (GameObject)Instantiate (typeItems [rnd], newPosition, Quaternion.identity, itemsParent);
				ItemController itemController = temp.GetComponent<ItemController> ();

				instantiateItems [i] [j] = itemController;
				itemController.row = i;
				itemController.column = j;
				itemController.type = rnd;

			}
			currentOffset += offsetYForCreate;
		}

	}


	public void FindAndDestroyMatchItems ()
	{
		listForRemove = new List<ItemsCoordonate> ();
		ItemController.isAnimation = true;
		group = 0;
		FindMatchItems (true);
		FindMatchItems (false);
		CalculateScore ();
		InvokeRepeating("DestroyItems",0.3f,0);

	}
		

	public void CalculateScore()
	{
		if (group > 0) {
			countMatchInGroup = new int[group];  
			foreach (var element in listForRemove) {
				countMatchInGroup [element.group]++;
			}
			float score = 0;
			string result = "";
			for (int i = 0; i < countMatchInGroup.Length; i++) {
				if (countMatchInGroup [i] == 3) {
					score += 15 * 3;
					result += "15*3";
				} else {
					if (countMatchInGroup [i] == 4) {
						score += 20 * 4;
						result += "20*4";
					} else {
						score += 30 * countMatchInGroup [i];
						result += "30*" + countMatchInGroup [i].ToString (); 
					}

				}
				if (i != countMatchInGroup.Length - 1) {
					result += "+";
				}


			}

			totalScore += score;
			totalScoreText.text = totalScore.ToString ();
			scoreInfoText.text = result;
		}


	}

	public void FindMatchItems (bool vertical)
	{
		
		int countElements = 0;
		int typeElement = 0;
		List <ItemsCoordonate> tempList = new List<ItemsCoordonate> ();

		for (int i = 0; i < gameSize; i++) {
			countElements = 0;
			for (int j = 0; j < gameSize; j++) {
				int i1;
				int j1;

				if (vertical) {
					i1 = j;
					j1 = i;
				} else {
					i1 = i;
					j1 = j;
				}

				if (countElements == 0) {
					tempList = new List<ItemsCoordonate> ();
					typeElement = instantiateItems [i1] [j1].type;
					countElements = 1;

					ItemsCoordonate a = new ItemsCoordonate ();
					a.row = i1;
					a.column = j1;
					tempList.Add (a);

				} else if (typeElement == instantiateItems [i1] [j1].type) {
					countElements++;
					ItemsCoordonate a = new ItemsCoordonate ();
					a.row = i1;
					a.column = j1;
					tempList.Add (a);

					if (j == gameSize - 1) {

						if (countElements >= 3) {
							
							AddInListForRemove (tempList,group);
							group++;
							tempList = new List<ItemsCoordonate> ();
							typeElement = instantiateItems [i1] [j1].type;
							countElements = 1;
							a = new ItemsCoordonate ();
							a.row = i1;
							a.column = j1;
							tempList.Add (a);
						}

					}

				} else if (typeElement != instantiateItems [i1] [j1].type) {


					if (countElements >= 3) {
						AddInListForRemove (tempList,group);
						group++;
					} 

					tempList = new List<ItemsCoordonate> ();
					typeElement = instantiateItems [i1] [j1].type;
					countElements = 1;
					ItemsCoordonate a = new ItemsCoordonate ();
					a.row = i1;
					a.column = j1;
					tempList.Add (a);

				}

			}
		}

	}


	public bool ExistItemInListForRemove(ItemsCoordonate item)
	{
		foreach (var element in listForRemove) {
			if (element.column == item.column && element.row==item.row) {
				return true;
			}
		}
		return false;

	}


	void AddInListForRemove (List<ItemsCoordonate> list,int localGroup)
	{

		foreach (var element in list) {
			if (ExistItemInListForRemove (element)) {
				localGroup = element.group;
				group -= 1;
				list.Remove (element);
				Debug.Log ("sa Executat");
				break;
			}
		}


		foreach (var element in list) {
			element.group = group;
			listForRemove.Add (element);
		}

		foreach (var element in listForRemove) {
			instantiateItems [element.row] [element.column].deleteGO.SetActive (true);
		}

	}


	void DestroyItems ()
	{

		foreach (var element in listForRemove) {
			if (instantiateItems [element.row] [element.column] != null) {
				Destroy (instantiateItems [element.row] [element.column].gameObject);
			}
				instantiateItems [element.row] [element.column] = null;

		}
			
		if (listForRemove.Count > 0) {
			SetNewElementInGridIfExist ();
			StartCoroutine (WaitEndFall ());
		} else {
			ItemController.isAnimation = false;
		}
	}



	public void SetNewElementInGridIfExist ()
	{

		ItemController[] tempColumn = null;

		int index = gameSize-1;
		for (int i = 0; i < gameSize; i++) {
			index = gameSize-1;
			tempColumn = new ItemController[gameSize];
			for (int j = gameSize - 1; j >= 0; j--) {
				if (instantiateItems [j] [i] != null) {
					tempColumn [index] = instantiateItems [j] [i];
					index -= 1;
				}
			
			}
			int count = 0;
			for (int k = gameSize-1; k >=0 ; k--) {
				instantiateItems [k] [i] = tempColumn [k];
				if (instantiateItems [k] [i] != null) {
					instantiateItems [k] [i].row = k;
					instantiateItems [k] [i].column = i;
				} else {
					count++;
					CreateElement (k, i, count * offsetYForCreate);
				
				}
			}
				
		}
			
	}
		

	public bool CheckNewElement (int i, int j, int value)
	{
		int count = 1;

		if (j > 0) {
			for (int k = j - 1; k >= 0; k--) {
				if (instantiateItems [i] [k].type == value) {
					count++;
				} else {
					break;
				}
			}


			if (count >= 3)
				return false;
		}

		count = 1;
		if (i > 0) {
			for (int k = i - 1; k >= 0; k--) {
				if (instantiateItems [k] [j].type == value) {
					count++;
				} else {
					break;
				}
			}


			if (count >= 3)
				return false;

		}

		return true;

	}


	[System.Serializable]
	public class ItemsCoordonate
	{
		public int row = 0;
		public int column = 0;
		public int group=0;

	}


}
