using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BallReset : MonoBehaviour {

    private Vector3 originPos;
    private Rigidbody rigidbody;

    public List<GameObject> stars;
    private int starCount = 0;
    public GameObject platform;
    public Vector3 platformCenter;
    public Vector3 platformExtents;

    private string sceneName; 

    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        originPos = gameObject.transform.position;

        platformCenter = platform.GetComponent<MeshCollider>().bounds.center;
        platformExtents = platform.GetComponent<MeshCollider>().bounds.extents;

        sceneName = SceneManager.GetActiveScene().name;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            Reset();
        } else if (col.gameObject.CompareTag("Star"))
        {
            col.gameObject.SetActive(false);
            starCount++;

        } else if (col.gameObject.CompareTag("Goal"))
        {
            if (starCount >= stars.Count)
            {
                col.gameObject.SetActive(false);
                LevelUp();
            } else
            {
                Reset();
            }
        }
    }

    public void checkCheat(Vector3 currPos)
    {
        

        if (currPos.x > platformCenter.x + platformExtents.x ||
            currPos.x < platformCenter.x - platformExtents.x ||
            currPos.y > platformCenter.y + platformExtents.y * 3 ||
            currPos.y < platformCenter.y - platformExtents.y * 0.5 ||
            currPos.z > platformCenter.z + platformExtents.z ||
            currPos.z < platformCenter.z - platformExtents.z)
        {
            foreach (var star in stars)
            {
                star.SetActive(false);
            }
        }
    }

    void Reset()
    {
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.angularVelocity = new Vector3(0, 0, 0);
        gameObject.transform.position = originPos;

        foreach (var star in stars)
        {
            star.SetActive(true);
        }

        starCount = 0;
        
    }

    void LevelUp()
    {
        if (sceneName == "Level1")
        {
            SteamVR_LoadLevel.Begin("Level2");
        }
        else if (sceneName == "Level2")
        {
            SteamVR_LoadLevel.Begin("Level3");
        }
        else if (sceneName == "Level3")
        {
            SteamVR_LoadLevel.Begin("Level4");
        }
        else if (sceneName == "Level4")
        {
            SteamVR_LoadLevel.Begin("Level1");
        }
    }
}
