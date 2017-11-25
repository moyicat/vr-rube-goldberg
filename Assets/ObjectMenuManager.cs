using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {

    public List<GameObject> objList;
    public List<GameObject> prefabList;

    private int currObj = 0;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            objList.Add(child.gameObject);
        }
        objList[currObj].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MenuLeft()
    {
        objList[currObj].SetActive(false);
        currObj--;
        if (currObj < 0)
        {
            currObj = objList.Count - 1;
        }
        objList[currObj].SetActive(true);
    }

    public void MenuRight()
    {
        objList[currObj].SetActive(false);
        currObj++;
        if (currObj >= objList.Count)
        {
            currObj = 0;
        }
        objList[currObj].SetActive(true);
    }

    public void SpawnCurrentObject()
    {
        Instantiate(prefabList[currObj], objList[currObj].transform.position, objList[currObj].transform.rotation);
    }
}
