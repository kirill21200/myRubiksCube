using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RotateCube : MonoBehaviour
{
    private short rot_dir;
    private bool isRotating;
    //private bool isShuffled;
    private bool isTimer;
    //private bool isWin;



    [HideInInspector] public List<Transform> U;
    [HideInInspector] public List<Transform> R;
    [HideInInspector] public List<Transform> L;
    [HideInInspector] public List<Transform> D;
    [HideInInspector] public List<Transform> F;
    [HideInInspector] public List<Transform> B;
    [HideInInspector] public List<List<Transform>> sides;

    public Transform RelativeCube;
    public Transform Pref;

    private Transform[] Cubes;
    private Transform[] StartCubes;
    public Transform TheCenter;

    public Transform RotationCube;

    private float speedRotateX = 100;
    private float speedRotateY = 100;

    [HideInInspector] public List<Transform> side;
    private string touched;

    private Vector2 f0start;

    private Vector3[] steps =
    {
        Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.back, Vector3.forward
    };

    Vector3 targetScaleVector = new Vector3(0.2f, 0.2f, 0.2f);
    float animationSpeed = 0.1f;

    [SerializeField] private Text timerText;
    private float _timeLeft = 0f;
    private IEnumerator StartTimer()
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
    }
    private void UpdateTimeText()
    {
        float minutes = Mathf.FloorToInt(_timeLeft / 60);
        float seconds = Mathf.FloorToInt(_timeLeft % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    //private bool IfEquals()
    //{
    //    for (int x = 0; x < StartCubes.Length; x++)
    //    {
    //        if (StartCubes[x].tag == "Cube" && Cubes[x].tag == "Cube")
    //        {
    //            Debug.Log(StartCubes[x].localPosition);
    //            Debug.Log(Cubes[x].localPosition);
    //            if (StartCubes[x].localPosition != Cubes[x].localPosition)
    //                return false;
    //        }      
    //    }
    //    return true;
    //}
    public IEnumerator Boom()
    {
        foreach (Transform child in RelativeCube)
        {
            Vector3 start = child.lossyScale;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * animationSpeed;
                child.localScale = Vector3.Lerp(start, targetScaleVector, t);
            }
            child.localScale = targetScaleVector;
            Destroy(child.gameObject);
            yield return new WaitForSeconds(0.9f);
        }


        yield break;
    }

    void DoShuffle()
    {
        short random_step;
        short random_dir;
        for (int i = 0; i < Random.Range(15, 30); i++)
        {
            random_step = (short)Random.Range(0, steps.Length - 1);
            random_dir = (short)(Random.Range(0, 1) * 2 - 1);

            foreach (var v in sides[random_step])
            {
                v.RotateAround(TheCenter.transform.position, steps[random_step] * random_dir, 90);
                FindPositions(Cubes);
            }

        }
    }

    public IEnumerator Rotation(Transform v, Vector3 RotVector)
    {
        isRotating = true;
        short _angle = 0;
        while (_angle != 90)
        {
            _angle += 10;
            v.RotateAround(TheCenter.transform.position, RotVector, 10);
            yield return new WaitForSeconds(0.00000001f);
        }

        FindPositions(Cubes);
        isRotating = false;
    }

    public bool FindPositions(Transform[] Cubes)
    {
        if (isRotating)
            return false;

        U = new List<Transform>();
        D = new List<Transform>();
        L = new List<Transform>();
        R = new List<Transform>();
        F = new List<Transform>();
        B = new List<Transform>();

        sides = new List<List<Transform>>();
        sides.Add(U);
        sides.Add(D);
        sides.Add(L);
        sides.Add(R);
        sides.Add(F);
        sides.Add(B);

        foreach (var Cube in Cubes)
        {
            if (Cube.tag == "Cube")
            {
                double coord = Math.Round(Cube.localPosition.y);
                if (coord == 1)
                    U.Add(Cube);
                else if (coord == -1)
                    D.Add(Cube);

                coord = Math.Round(Cube.localPosition.x);
                if (coord == 1)
                    R.Add(Cube);
                else if (coord == -1)
                    L.Add(Cube);

                coord = Math.Round(Cube.localPosition.z);
                if (coord == 1)
                    B.Add(Cube);
                else if (coord == -1)
                    F.Add(Cube);
            }
        }

        return true;
    }

    private void Start()
    {
        _timeLeft = StaticScript.Time;
        isRotating = false;
        Cubes = RelativeCube.GetComponentsInChildren<Transform>();
        StartCubes = Pref.GetComponentsInChildren<Transform>(); 
        FindPositions(Cubes);
        DoShuffle();
    }

    void Update()
    {
        if (_timeLeft < 0)
        {
            StartCoroutine(Boom());
            timerText.text = "����� �����!";
        }
        if (!isRotating /*&& !isWin*/ && Input.touchCount == 1)
        {
            if (!isTimer) StartCoroutine(StartTimer());
            isTimer = true;
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                f0start = Input.GetTouch(0).position;
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1))
                {
                            if ( !side.Contains(hit.collider.transform))
                            {
                                side.Add(hit.collider.transform);
                            }

                }
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.name == "Front") touched = "f";
                    if (hit.collider.name == "Back") touched = "b";
                    if (hit.collider.name == "Right") touched = "r";
                    if (hit.collider.name == "Left") touched = "l";
                    if (hit.collider.name == "Up") touched = "u";
                    if (hit.collider.name == "Down") touched = "d";
                }
            }
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                Vector2 f0position = Input.GetTouch(0).position;
                Vector2 dir = f0start - f0position;
                double direct = dir.x - dir.y;
                if (direct < 0)
                    rot_dir = -1;
                else
                    rot_dir = 1;
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1))
                {
                            if (!side.Contains(hit.collider.transform))
                            {
                                side.Add(hit.collider.transform);
                            }

                }
                if (side.Count > 1)
                {
                    foreach (var f in sides)
                    {
                        if (f.Contains(side[0]) && f.Contains(side[1]))
                        {
                            if (f == U)
                            {
                                if (touched == "f" || touched == "b" || touched == "r" || touched == "l")
                                {
                                    foreach (Transform v in U)
                                        StartCoroutine(Rotation(v, TheCenter.up * rot_dir));
                                }
                            }
                            if (f == D)
                            {
                                if (touched == "f" || touched == "b" || touched == "r" || touched == "l")
                                {
                                    foreach (Transform v in D)
                                        StartCoroutine(Rotation(v, TheCenter.up * rot_dir));
                                }
                            }
                            if (f == F)
                            {
                                if (touched == "u" || touched == "d" || touched == "r" || touched == "l")
                                {
                                    foreach (Transform v in F)
                                        StartCoroutine(Rotation(v, TheCenter.forward * rot_dir));
                                }
                            }
                            if (f == B)
                            {
                                if (touched == "u" || touched == "d" || touched == "r" || touched == "l")
                                {
                                    foreach (Transform v in B)
                                        StartCoroutine(Rotation(v, TheCenter.forward * rot_dir));
                                }
                            }
                            if (f == R)
                            {
                                if (touched == "f" || touched == "b" || touched == "u" || touched == "d")
                                {
                                    foreach (Transform v in R)
                                        StartCoroutine(Rotation(v, TheCenter.right * rot_dir));
                                }
                            }
                            if (f == L)
                            {
                                if (touched == "f" || touched == "b" || touched == "u" || touched == "d")
                                {
                                    foreach (Transform v in L)
                                        StartCoroutine(Rotation(v, TheCenter.right * rot_dir));
                                }
                            }
                        }
                    }
                    //if (IfEquals())
                    //{
                    //    _timeLeft = 0;
                    //    timerText.text = "������� �������!";
                    //    isWin = true;
                    //}
                    side.Clear();
                    touched = "";
                }                   
            }           
        }
        if (!isRotating&& Input.touchCount == 2)
        {
            RelativeCube.RotateAround(RotationCube.transform.position, RotationCube.up, -Input.GetTouch(0).deltaPosition.x * speedRotateX * Mathf.Deg2Rad * Mathf.Deg2Rad);
            RelativeCube.RotateAround(RotationCube.transform.position, RotationCube.right, Input.GetTouch(0).deltaPosition.y * speedRotateY * Mathf.Deg2Rad * Mathf.Deg2Rad);
        }
    }   
}
