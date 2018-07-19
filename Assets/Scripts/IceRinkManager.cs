using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IceRinkManager : NetworkBehaviour   {

    public GameObject _prefabCrack;
    private GameObject[] decals;

    public GameObject _prefabTarget;
    private GameObject[] targets;

    public GameObject _prefabStone;
    private Queue<GameObject> stonePool;
    public int poolSize = 100;
    public float forceDown = 20f;

    public float dropRate;
    private float nextDrop;

    private bool[] isBroken;

    public Material water;

    private float initialCD;





    public int _numRows;
    public int _numCols;
    //public int _numDecals;

	// Use this for initialization
	void Start () {
        //_numCols = 7;
        //_numRows = 7;

        nextDrop = -5.0f;


        decals = new GameObject[_numRows * _numCols];
        targets = new GameObject[_numRows * _numCols];
        stonePool = new Queue<GameObject>();
        isBroken = new bool[_numRows * _numCols];

        for(int i = 0; i< _numCols*_numRows; ++i)
        {
            isBroken[i] = false;

        }
        //isHit = new bool[_numRows * _numCols];
        _prefabCrack.GetComponent<Renderer>().sharedMaterial.renderQueue = 3001;
        fillWithDecals();

		
	}
	






    void fillWithDecals()
    {
        //-0.4 to 0.4 on x
        //0.4 to -0.4 on y

        //scale 0.1 = full on x and z
        float stepCol = (1.00f / _numCols);
        float stepRow = (1.00f / _numRows);
        float localScaleRow = 0.1f / _numRows;
        float localScaleCol = 0.1f / _numCols;
        

        int index = 0;

        for(float k = 0.4f; k > -0.5f; k=k - stepRow)
        {
            for(float i = -0.4f; i < 0.5f; i=i + stepCol)
            {
                decals[index] = Instantiate(_prefabCrack, this.transform, false);
                decals[index].transform.localScale = new Vector3(localScaleCol, 1f, localScaleRow);
                decals[index].transform.localPosition = new Vector3(i, k, -0.1f);

                targets[index] = Instantiate(_prefabTarget, this.transform, false);
                targets[index].transform.localScale = new Vector3(localScaleCol, 1f, localScaleRow);
                targets[index].transform.localPosition = new Vector3(i, k, -0.12f);


                //Debug.Log("i : " + i + "k : " + k );
                //decals[index].SetActive(true);
                ++index;

            }
        }

        for(int i = 0; i<poolSize; i++)
        {
            GameObject newStone = Instantiate(_prefabStone);
            newStone.SetActive(false);
            //newStone.transform.localScale = new Vector3(0.77f, 50f, 0.77f);
            stonePool.Enqueue(newStone);
            //.transform.localPosition = new Vector3(0.2f, 0.2f, -2.12f);


        }

    }

    
    void dropStone(Vector3 localPos,int targetIndex)
    {
        GameObject droppingStone = stonePool.Dequeue();
        droppingStone.SetActive(true);
        droppingStone.name = targetIndex.ToString();
        droppingStone.transform.position = transform.TransformPoint(new Vector3(localPos.x,localPos.y,-30f));
        if (decals[targetIndex].activeSelf)
        {
            isBroken[targetIndex] = true;
        }

    }

    [ClientRpc]
    void RpcdropStone(Vector3 localPos, int targetIndex)
    {
        if (!isBroken[targetIndex])
        {
            GameObject droppingStone = stonePool.Dequeue();
            droppingStone.SetActive(true);
            droppingStone.name = targetIndex.ToString();
            droppingStone.transform.position = transform.TransformPoint(new Vector3(localPos.x, localPos.y, -30f));
            if (decals[targetIndex].activeSelf)
            {
                isBroken[targetIndex] = true;
            }
        }

    }


    void Update()
    {
        nextDrop += Time.deltaTime;
        
        if (isServer && nextDrop > dropRate)
        {
            
            spawnRock();
            nextDrop = 0;

        }

        //if (Time.time > nextDrop)
        //{
        //    nextDrop = Time.time + dropRate;
        //    spawnRock();
        //}

    }


    [ClientRpc]
    void RpcsetTargetActive(int index)
    {
        if (!isBroken[index])
        {
            targets[index].SetActive(true);
        }
    }

    void spawnRock()
    {
        int rIndex = Random.Range(0, (_numCols * _numRows) - 1);

        if (!isBroken[rIndex])
        {
            RpcsetTargetActive(rIndex);
            //dropStone(targets[rIndex].transform.localPosition,rIndex);
            RpcdropStone(targets[rIndex].transform.localPosition, rIndex);
            //orderStack.Enqueue(rIndex);
        }


    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "snowStone")
        {

            int index = IntParseFast(collision.gameObject.name);
            targets[index].SetActive(false);
            //Debug.Log("Collision on " + index);
            if (decals[index].activeSelf)
            {
                //Debug.Log("Hit on active crack");
                decals[index].gameObject.layer = 11;
                decals[index].gameObject.tag = "WaterIce";
                decals[index].GetComponent<Renderer>().material = water;
            }
            else
            {
                decals[index].SetActive(true);
            }

        }

    }


    public static int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }
}

