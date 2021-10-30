using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    private float camMoveToYPosition, camMoveSpeed = 2f;

    public GameObject cubeToCreate, allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRB;

    public Text scoreTxt;

    private bool isLose, firstCube;
    private Coroutine showCubePlace;

    private List<Vector3> AllCubesPositions = new List<Vector3> {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1)
    };

    private Transform mainCam;
    private int prevMaxHor = 0;

    public Color[] colors;
    private Color toCameraColor;

    private void Start()
    {
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 6f + nowCube.y - 1f;
        scoreTxt.text = string.Format("<size=40><color=#FF4F4F>best:</color></size> {0}\n<size=22>now:</size> {1}", PlayerPrefs.GetInt("score"), 0);

        showCubePlace = StartCoroutine(ShowCubePlace());
        allCubesRB = allCubes.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
                if (Input.GetTouch(0).phase != TouchPhase.Began)
                    return;
#endif

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.SetVector(newCube.transform.position);
            Vector3 pos = newCube.transform.position;
            AllCubesPositions.Add(new Vector3(Convert.ToInt32(pos.x), Convert.ToInt32(pos.y), Convert.ToInt32(pos.z)));
            GameObject vfxObj = Instantiate(vfx, pos, Quaternion.identity);
            Destroy(vfxObj, 2f);
            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;
            SpawnPositions();
            MoveCamereChangeBg();
            if (PlayerPrefs.GetString("music") == "On")
                GetComponent<AudioSource>().Play();
        }

        if (!isLose && allCubesRB.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            StopCoroutine(showCubePlace);
            isLose = true;
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);

        if (Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)))
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)))
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)))
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)))
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)))
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)))
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        Vector3 currPos = positions.Find(p => Convert.ToInt32(p.x) == Convert.ToInt32(cubeToPlace.position.x) &&
            Convert.ToInt32(p.y) == Convert.ToInt32(cubeToPlace.position.y) &&
            Convert.ToInt32(p.z) == Convert.ToInt32(cubeToPlace.position.z));
        if (positions.Count > 1)
            positions.Remove(currPos);
        if (positions.Count == 0)
        {
            isLose = true;
            return;
        }

        cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
    }

    private bool IsPositionEmpty(Vector3 pos)
    {
        if (pos.y == 0)
            return false;
        return !(AllCubesPositions.FindAll(p => p.x == pos.x && p.y == pos.y && p.z == pos.z).Count > 0);
    }

    private void MoveCamereChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;
        foreach (Vector3 pos in AllCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Mathf.Abs(Convert.ToInt32(pos.x));
            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Mathf.Abs(Convert.ToInt32(pos.z));
        }

        if (PlayerPrefs.GetInt("score") < maxY - 1)
            PlayerPrefs.SetInt("score", maxY - 1);

        scoreTxt.text = string.Format("<size=40><color=#FF4F4F>best:</color></size> {0}\n<size=22>now:</size> {1}", PlayerPrefs.GetInt("score"), maxY - 1);

        camMoveToYPosition = 6f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if (prevMaxHor != maxHor && maxHor % 3 == 0)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            prevMaxHor = maxHor;
        }

        if (maxY >= 10)
            toCameraColor = colors[2];
        else if (maxY >= 6)
            toCameraColor  = colors[1];
        else if (maxY >= 3)
            toCameraColor = colors[0];
    }
}

struct CubePos
{
    public int x, y, z;

    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }

    public void SetVector(Vector3 position)
    {
        x = Convert.ToInt32(position.x);
        y = Convert.ToInt32(position.y);
        z = Convert.ToInt32(position.z);
    }
}