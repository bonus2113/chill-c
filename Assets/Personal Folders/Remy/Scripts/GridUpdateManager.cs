using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridUpdateManager : MonoBehaviour {

    public interface IGridObject
    {
        void UpdateMe();
    }

    private static GridUpdateManager s_Instance = null;
    public static GridUpdateManager Instance
    {
        get
        {
            return s_Instance;
        }
    }

    void Awake()
    {
        if (s_Instance != null)
        {
            Debug.LogWarning("GridUpdateManager already exists! Fix yo' shit boi!");
            Destroy(this.gameObject);
            return;
        }
        s_Instance = this;
    }

    void OnDestroy()
    {
        if (s_Instance == this)
        {
            s_Instance = null;
        }
    }

    private Vector2 m_CurrentCenterCell = new Vector2(11, -3);

    private float c_CellWidth = 30.0f;

    private Dictionary<int, Dictionary<int, List<Transform>>> m_Grid = new Dictionary<int, Dictionary<int, List<Transform>>>();

    //assumes all objects registered implement IGridObject
    public void RegisterInGrid(Transform obj)
    {
        Vector3 worldPos = obj.position;
        int cellX = (int)(worldPos.x / c_CellWidth);
        int cellY = (int)(worldPos.z / c_CellWidth);

        //add to grid
        AddToCell(cellX, cellY, obj);

        //Debug.Log("Object registered: " + obj.gameObject.name);
    }

    private void AddToCell(int x, int y, Transform obj)
    {
        if (!m_Grid.ContainsKey(x))
        {
            m_Grid.Add(x, new Dictionary<int, List<Transform>>());
            //Debug.Log("Cell x created: " + x);
        }

        if (!m_Grid[x].ContainsKey(y))
        {
            m_Grid[x].Add(y, new List<Transform>());
            //Debug.Log("Cell y created: " + y);
        }

        m_Grid[x][y].Add(obj);

        //Debug.Log("Object: " + obj.gameObject.name + " added to cell: " + x + ", " + y);
    }

    private void UpdateItemsInCell(Vector2 cell)
    {
        int iX = (int)cell.x;
        int iY = (int)cell.y;

        if (m_Grid.ContainsKey(iX))
        {
            if (m_Grid[iX].ContainsKey(iY))
            {
                for (int i = 0; i < m_Grid[iX][iY].Count; i++)
                {
                    m_Grid[iX][iY][i].GetComponent<IGridObject>().UpdateMe();
                }
            }
        }
    }

    private Vector2 GetPlayerCell()
    {
        var playerPos = Player.Instance.transform.position;
        Vector2 outVec = new Vector2((int)(playerPos.x/c_CellWidth), (int)(playerPos.z/c_CellWidth));

        return outVec;
    }
	
	// Update is called once per frame
	void Update () {

        Vector2 playerCell = GetPlayerCell();
        if (m_CurrentCenterCell != playerCell)
        {
            m_CurrentCenterCell = playerCell;
            Debug.Log("Player cell changed to: " + m_CurrentCenterCell);
            Debug.Log("Grid items in cell: " + GetCellCount(m_CurrentCenterCell));
        }
        //9-cell update
        //"top"
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2(-1, 1));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 0, 1));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 1, 1));

        //"center"
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2(-1, 0));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 0, 0));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 1, 0));

        //"bot"
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2(-1, -1));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 0, -1));
        UpdateItemsInCell(m_CurrentCenterCell + new Vector2( 1, -1));
    }

    private int GetCellCount(Vector2 cell)
    {
        int iX = (int)cell.x;
        int iY = (int)cell.y;


        if (m_Grid.ContainsKey(iX))
        {
            if (m_Grid[iX].ContainsKey(iY))
            {
                return m_Grid[iX][iY].Count;
            }
        }

        return 0;
    }
}
