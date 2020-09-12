﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGR : MonoBehaviour
{
    private static MGR p_instance = null;
    public static MGR Instance { get { return p_instance; } }

    public Material mat;
    public Material mat_Obstacle;

    public GameObject GO_Plane;
    public GameObject GO_Case;

    public GameObject GO_Player;
    public GameObject GO_Bot;

    public GameObject[,] arrGO_Cases;

    int[,] layout;
    float[,] matInf;
    public Vector2Int vec2_Depart;
    public Vector2Int vec2_Arrive;

    public int n_PlayerSpeed;
    public int n_BotSpeed;

    public Vector3 vec3_BotDst;
    public bool letsgo;





    //public int length;
    //public int width;
    void Awake()
    {
        if (p_instance == null)
            //if not, set instance to this
            p_instance = this;
        //If instance already exists and it's not this:
        else if (p_instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        arrGO_Cases = new GameObject[10, 10];
        layout = new int[10, 10]
        {
        {1  ,0  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,0  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,0  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,0  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,0  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 },
        {1  ,1  ,1  ,1  ,1  ,1 ,1 ,1 ,1 ,1 }
        };



        Instantiate(GO_Plane.transform, new Vector3(5, 0, 5), Quaternion.identity);
        for(int i = 0; i< 10;i++)
            for(int j = 0; j <10; j++)
            {
                arrGO_Cases[i,j] = (GameObject)Instantiate(GO_Case, new Vector3(i+0.5f, 0.5f, j+0.5f), Quaternion.identity);
                arrGO_Cases[i, j].GetComponent<CaseController>().setPos(i, j);
                arrGO_Cases[i, j].gameObject.name = "Case_" + i + "_" + j; 
                if(layout[i,j] == 1)
                {
                    arrGO_Cases[i, j].GetComponent<CaseController>().setPoids(1.0f);
                }
                else
                {
                    arrGO_Cases[i, j].GetComponent<CaseController>().setPoids(Mathf.Infinity);
                }
            }

        Instantiate(GO_Player.transform, new Vector3(9.5f, 0.5f, 9.5f), Quaternion.identity);

        Instantiate(GO_Bot.transform, new Vector3(vec2_Depart.x + .5f, 0.5f, vec2_Depart.y + .5f), Quaternion.identity);
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if(e.button == 0 && e.isMouse)
        {
            if (arrGO_Cases[vec2_Arrive.x, vec2_Arrive.y].GetComponent<CaseController>().poids < Mathf.Infinity)
            {
                Dijkstra();
            }
        }
        if(e.isKey)
        {
            Dijkstra();
        }

    }

    public void Dijkstra()
    {

        List<Vector2Int> P = new List<Vector2Int>();
        /*Initialisation*/
        float[,] dst = new float[10,10];
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                dst[i, j] = Mathf.Infinity;
            }
        }


        dst[vec2_Depart.x, vec2_Depart.y] = 0;


        List<GameObject> Q = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (arrGO_Cases[i, j].GetComponent<CaseController>().poids < Mathf.Infinity)
                {
                    arrGO_Cases[i, j].GetComponent<Renderer>().material = mat;
                }
            }
        }

        Vector2Int[,] predecesseurs = new Vector2Int[10, 10];
        while(P.Count != 10*10) { 
            float mindst = Mathf.Infinity;
            Vector2Int minCase = new Vector2Int(vec2_Depart.x, vec2_Depart.y);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (dst[i, j] != Mathf.Infinity)
                    {
                        //Debug.Log("dst[" + i + "," + j + "] : " + dst[i, j]);
                    }
                    if(!P.Contains(new Vector2Int(i,j)) && mindst> dst[i,j])
                    {
                        mindst = dst[i, j];
                        minCase = new Vector2Int(i, j);
                    }
                }
            }
            //Debug.Log("MinCase : " + minCase + " dst : " + mindst);
            P.Add(arrGO_Cases[minCase.x, minCase.y].GetComponent<CaseController>().GetPos());
            

            List<GameObject> CSnghb = arrGO_Cases[minCase.x,minCase.y].GetComponent<CaseController>().arrGO_neighbour;
            foreach (GameObject GO in CSnghb)
            {
                Vector2Int posGO = GO.GetComponent<CaseController>().GetPos();

                if(dst[posGO.x, posGO.y] > (dst[minCase.x, minCase.y] + arrGO_Cases[posGO.x, posGO.y].GetComponent<CaseController>().poids))
                {
                    dst[posGO.x, posGO.y] = dst[minCase.x, minCase.y] + arrGO_Cases[posGO.x, posGO.y].GetComponent<CaseController>().poids;
                    predecesseurs[posGO.x, posGO.y] = new Vector2Int(minCase.x, minCase.y);
                }
            }


        }

        List<Vector2Int> chemin = new List<Vector2Int>();
        Vector2Int s = vec2_Arrive;
        while (s != vec2_Depart)
        {
            chemin.Add(s);
            arrGO_Cases[s.x, s.y].GetComponent<Renderer>().material = mat_Obstacle;
            arrGO_Cases[s.x, s.y].GetComponent<Renderer>().material.color = Color.blue;
            s = predecesseurs[s.x, s.y];
        }
        arrGO_Cases[vec2_Arrive.x, vec2_Arrive.y].GetComponent<Renderer>().material = mat_Obstacle;
        arrGO_Cases[vec2_Arrive.x, vec2_Arrive.y].GetComponent<Renderer>().material.color = Color.cyan;
        arrGO_Cases[vec2_Depart.x, vec2_Depart.y].GetComponent<Renderer>().material = mat_Obstacle;
        arrGO_Cases[vec2_Depart.x, vec2_Depart.y].GetComponent<Renderer>().material.color = Color.green;

        vec3_BotDst = new Vector3(chemin[chemin.Count -1].x +.5f,0.5f, chemin[chemin.Count - 1].y+0.5f);
        letsgo = true;

    }

}
