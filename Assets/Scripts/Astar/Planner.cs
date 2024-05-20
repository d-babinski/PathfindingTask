using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
namespace Astar
{
    public class Planner : MonoBehaviour
    {
        [SerializeField] private Path path = null;

        [SerializeField] private Transform startTransform = null;
        [FormerlySerializedAs("goalTransform")][SerializeField] private Transform targetTransform = null;

        [FormerlySerializedAs("defaultTile")][SerializeField] private Tile tilePrefab = null;
        [SerializeField] private Material defaultMaterial = null;
        [SerializeField] private Material obstacleMaterial = null;

        private int mapWidth = 5;
        private int mapHeight = 5;
        
        private Tile[,] visualGrid = new Tile[0, 0];
        private Node[,] nodeGrid = new Node[0, 0];
        
        private Vector2 start = new Vector2();
        private Vector2 target = new Vector2();
        
        private int[,] map = {};

        public void Start()
        {
            Tile.OnTileClicked += swapTile;
            Tile.OnTileMiddleClicked += OnStartReplaced;
            Tile.OnTileRightClicked += OnTargetReplaced;
            GenerateNewGrid(mapHeight, mapWidth);
        }

        public void GenerateNewGrid(int _newWidth, int _newHeight)
        {
            cleanCurrentMap();
            mapHeight = _newHeight;
            mapWidth = _newWidth;
            initializeMap(mapWidth, mapHeight);
            setStart(0,0);
            setTarget(mapWidth - 1, mapHeight - 1);
            calculatePath((int)start.x, (int)start.y, (int)target.x, (int)target.y, nodeGrid);
        }
        
        private void cleanCurrentMap()
        {
            nodeGrid = new Node[0, 0];
            
            foreach (Tile _tile in visualGrid)
            {
                Destroy(_tile.gameObject);
            }
            
            visualGrid = new Tile[0, 0];
        }
        
        private void initializeMap(int _width, int _height)
        {
            map = new int[_width, _height];
            visualGrid = renderGrid(map);
            nodeGrid = Astar.CreateNodeGridFromObstacleMap(map);
            Astar.GenerateSuccessors(nodeGrid);
        }
        
        private void OnStartReplaced(int _newX, int _newY)
        {
            setStart(_newX, _newY);
            calculatePath((int)start.x, (int)start.y, (int)target.x, (int)target.y, nodeGrid);
        }

        private void setStart(int _x, int _y)
        {
            start.x = _x;
            start.y = _y;
            startTransform.position = new Vector3(_x, 1f, _y);
        }

        private void OnTargetReplaced(int _newX, int _newY)
        {
            setTarget(_newX, _newY);
            calculatePath((int)start.x, (int)start.y, (int)target.x, (int)target.y, nodeGrid);
        }
        
        private void setTarget(int _x, int _y)
        {
            target.x = _x;
            target.y = _y;
            targetTransform.position = new Vector3(_x, 1f, _y);
        }

        private void swapTile(int _x, int _y)
        {
            int _newVal = map[_x, _y] == 1 ? 0 : 1;
            map[_x, _y] = _newVal;
            visualGrid[_x, _y].GetComponent<MeshRenderer>().material = map[_x, _y] == 1 ? obstacleMaterial : defaultMaterial;
            visualGrid[_x, _y].X = _x;
            visualGrid[_x, _y].Y = _y;

            nodeGrid[_x, _y].IsObstacle = _newVal == 1;

            Astar.UpdateGridCellAndItsNeighbours(_x, _y, nodeGrid);
            calculatePath((int)start.x, (int)start.y, (int)target.x, (int)target.y, nodeGrid);
        }
        
        private void calculatePath(int _sx, int _sy, int _tx, int _ty, Node[,] _grid)
        {
            Astar.FindPath(_sx, _sy, _tx, _ty, _grid);
            Node _targetNode = _grid[_tx, _ty];
            path.AssignPath(extractPath(_targetNode));
        }

        private Tile[,] renderGrid(int[,] _grid)
        {
            Tile[,] _visualGrid = new Tile[_grid.GetLength(0), _grid.GetLength(1)];

            for (int x = 0; x < _visualGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _visualGrid.GetLength(1); y++)
                {
                    _visualGrid[x, y] = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    _visualGrid[x, y].X = x;
                    _visualGrid[x, y].Y = y;
                    _visualGrid[x, y].GetComponent<MeshRenderer>().material = _grid[x, y] == 1 ? obstacleMaterial : defaultMaterial;
                }
            }
            return _visualGrid;
        }

        private Vector3[] extractPath(Node _targetNode)
        {
            List<Vector3> _path = new List<Vector3>();
            Node _processedNode = _targetNode;

            while (_processedNode.Parent != null)
            {
                _path.Add(new Vector3(_processedNode.X, 1f, _processedNode.Y));
                _processedNode = _processedNode.Parent;
            }

            _path.Add(new Vector3(_processedNode.X, 1f, _processedNode.Y));
            _path.Reverse();
            
            return _path.ToArray();
        }
    }
}
