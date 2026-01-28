using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 6;
    public int height = 6;

    int[,] grid; // 0 = vacío, mayro a 0 = id de hilo

    void Awake()
    {
        Instance = this;
        grid = new int[width, height];
    }

    public bool IsCellFree(int x, int z)
    {
        if (x < 0 || x >= width || z < 0 || z >= height)
            return false;

        return grid[x, z] == 0;
    }

    public void Occupy(int id, int x, int z)
    {
        grid[x, z] = id;
    }

    public void Clear(int x, int z)
    {
        grid[x, z] = 0;
    }
}
