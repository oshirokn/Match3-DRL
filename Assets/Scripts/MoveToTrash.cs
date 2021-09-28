using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTrash : MonoBehaviour
{
    public bool moveToTrash;
    public float maxSpeed = 10f;
    public float smoothTime = 1f;
    private Vector3 endMarker;
    private Vector3 velocity = Vector3.zero;
    private Board board;
    private GameObject trashcan;
    private SoundManager soundManager;

    void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        switch (gameObject.tag)
        {
            case "Blue":
                endMarker = new Vector3(5.35f, -4.82f, 0f);
                break;
            case "Brown":
                endMarker = new Vector3(5.42f, -2.45f, 0f);
                break;
            case "Green":
                endMarker = new Vector3(3.13f, -2.46f, 0f);
                break;
            case "Orange":
                endMarker = new Vector3(3.85f, -4.81f, 0f);
                break;
            case "Pink":
                endMarker = new Vector3(0.8499998f, -4.79f, 0f);
                break;
            case "Red":
                endMarker = new Vector3(0.8600001f, -2.43f, 0f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveToTrash == false)
        {
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, endMarker,ref velocity, smoothTime, maxSpeed);
            if (Vector3.Distance(gameObject.transform.position, endMarker) < 1f)
            {
                switch (gameObject.tag)
                {
                    //Blue = carton
                    case "Blue":
                        trashcan = GameObject.Find("Trash-Carton").transform.Find("Trashcan").gameObject;
                        break;
                        // Brown = Plastic
                    case "Brown":
                        trashcan = GameObject.Find("Trash-Plastic").transform.Find("Trashcan").gameObject;
                        break;
                        // Green = biowaste
                    case "Green":
                        trashcan = GameObject.Find("Trash-Biowaste").transform.Find("Trashcan").gameObject;
                        break;
                        // Orange = Paper
                    case "Orange":
                        trashcan = GameObject.Find("Trash-Paper").transform.Find("Trashcan").gameObject;
                        break;
                        // Pink = glass
                    case "Pink":
                        trashcan = GameObject.Find("Trash-Glass").transform.Find("Trashcan").gameObject;
                        break;
                        // red = metal
                    case "Red":
                        trashcan = GameObject.Find("Trash-Metal").transform.Find("Trashcan").gameObject;
                        break;
                }
                trashcan.GetComponent<Animator>().SetTrigger("trigger");
                StartCoroutine(RemoveTrigger());
                IEnumerator RemoveTrigger()
                {
                    soundManager.PlayTrashcanNoise();
                    yield return new WaitForSeconds(0);
                    trashcan.GetComponent<Animator>().ResetTrigger("trigger");
                    Destroy(gameObject);
                    moveToTrash = true;
                }
                //board.trashCanOff();
            }
        }
    }
}
