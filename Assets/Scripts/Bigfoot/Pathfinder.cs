using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;


public class Pathfinder : MonoBehaviour
{
    private List<List<int>> m_Array;
    private Node[,] m_nodes;

    private void Awake()
    {
        
    }

    private void 

    private void Start()
    {
        if(m_Array == null) { 
            Debug.LogError("Pathfinder is missing a Grid reference");
            gameObject.SetActive(false);
            return;
        }
        InitNodes(m_Array.ColumnCount, m_Array.RowCount);
    }
    private void InitNodes(uint x, uint y)
    {
        m_nodes = new Node[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {

            }
        }
    }

    
}


