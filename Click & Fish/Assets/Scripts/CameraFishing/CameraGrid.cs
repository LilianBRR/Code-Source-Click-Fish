using UnityEngine;

public class CameraGrid : MonoBehaviour
{
    [SerializeField] private Camera _gridCamera;
    [SerializeField] private float _distanceFromCamera = 10f;

    private const int COLUMNS = 4;
    private const int ROWS = 3;
    private Vector3[] _gridPositions = new Vector3[12];

    void Awake()
    {
        if (_gridCamera == null) _gridCamera = _gridCamera.GetComponent<Camera>();

        CalculateGridPositions();
    }

    public void CalculateGridPositions()
    {
        float cellWidth = 1f / COLUMNS;
        float cellHeight = 1f / ROWS;

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLUMNS; col++)
            {
                int index = row * COLUMNS + col;

                float viewportX = (col + 0.5f) * cellWidth;
                float viewportY = (row + 0.5f) * cellHeight;

                Vector3 viewportPoint = new Vector3(viewportX, viewportY, _distanceFromCamera);
                _gridPositions[index] = _gridCamera.ViewportToWorldPoint(viewportPoint);
            }
        }
    }

    public Vector3 GetGridPosition(int index)
    {
        if (index >= 0 && index < _gridPositions.Length)
            return _gridPositions[index];

        Debug.LogWarning("Index hors limites !");
        return Vector3.zero;
    }

    public Vector3 GetGridPosition(int column, int row)
    {
        int index = row * COLUMNS + column;
        return GetGridPosition(index);
    }

    private void OnDrawGizmosSelected()
    {
        if (_gridCamera == null) _gridCamera = GetComponent<Camera>();

        CalculateGridPositions();

        Gizmos.color = Color.green;
        foreach (Vector3 pos in _gridPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.05f);
        }
    }
}