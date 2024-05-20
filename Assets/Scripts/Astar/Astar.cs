using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    public class Node
    {
        public int X, Y;
        public bool IsObstacle = false;
        public float TotalCost => CostSoFar + HeuristicCost;
        public float CostSoFar = 0f;
        public float HeuristicCost = 0f;
        public Node Parent = null;
        
        public List<Node> Successors = new List<Node>();

        public static int CheaperNode(Node _a, Node _b)
        {
            if (_a.TotalCost < _b.TotalCost)
            {
                return -1;
            }

            if (Math.Abs(_a.TotalCost - _b.TotalCost) < Mathf.Epsilon)
            {
                return 0;
            }

            return 1;
        }
        
        public void Cleanup()
        {
            HeuristicCost = 0f;
            CostSoFar = 0f;
            Parent = null;
        }
    }

    public class Heuristics
    {
        public static float GetHeuristicCost(Node _a, Node _b)
        {
            return Mathf.Abs(_a.X - _b.X) + Mathf.Abs(_a.Y - _b.Y);
        }
    }
    
    public class Astar
    {
        public static Node[,] CreateNodeGridFromObstacleMap(int[,] _map)
        {
            Node[,] _nodeGrid = new Node[_map.GetLength(0), _map.GetLength(1)];

            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    Node _node = new Node
                    {
                        X = x,
                        Y = y,
                        IsObstacle = _map[x, y] == 1,
                    };

                    _nodeGrid[x, y] = _node;
                }
            }

            return _nodeGrid;
        }
        
        public static Node FindPath(int _startX, int _startY, int _targetX, int _targetY, Node[,] _grid)
        {
            foreach (var _node in _grid)
            {
                _node.Cleanup();
            }
            
            List<Node> _open = new();
            List<Node> _closed = new();

            Node _targetNode = _grid[_targetX, _targetY];

            Node _startNode = _grid[_startX, _startY];
            _startNode.HeuristicCost = Heuristics.GetHeuristicCost(_grid[_startX, _startY], _grid[_targetX, _targetY]);
            
            _open.Add(_startNode);

            while (_open.Count > 0)
            {
                _open.Sort(Node.CheaperNode);

                Node _currentNode = _open[0];

                _open.Remove(_currentNode);
                
                if (_currentNode == _targetNode)
                {
                    break;
                }

                foreach (var _successor in _currentNode.Successors)
                {
                    float _successorCost = _currentNode.CostSoFar + Heuristics.GetHeuristicCost(_successor, _currentNode);

                    if (_open.Contains(_successor))
                    {
                        if (_successorCost >= _successor.CostSoFar)
                        {
                            continue;
                        }
                    }
                    else if (_closed.Contains(_successor))
                    { 
                        if (_successorCost >= _successor.CostSoFar)
                        {
                            continue;
                        }

                        _closed.Remove(_successor);
                        _open.Add(_successor);
                    }
                    else
                    {
                        _open.Add(_successor);
                        _successor.HeuristicCost = Heuristics.GetHeuristicCost(_successor, _targetNode);
                    }

                    _successor.CostSoFar = _successorCost;
                    _successor.Parent = _currentNode;
                }
                
                _closed.Add(_currentNode);
            }

            return _targetNode;
        }
        
        public static Node[,] GenerateSuccessors(Node[,] _grid)
        {
            foreach (var _node in _grid)
            {
                GenerateSuccessorsForSingleNode(_node, _grid);
            }

            return _grid;
        }

        public static void UpdateGridCellAndItsNeighbours(int _x, int _y, Node[,] _grid)
        {
            _updateCell( _x, _y);
            _updateCell( _x-1, _y);
            _updateCell( _x+1, _y);
            _updateCell( _x, _y -1);
            _updateCell( _x, _y + 1);

            void _updateCell(int _nx,int _ny)
            {
                if (_nx < 0 || _nx >= _grid.GetLength(0) || _ny < 0 || _ny >= _grid.GetLength(1))
                {
                    return;
                }

                if (_grid[_nx, _ny].IsObstacle == true)
                {
                    _grid[_nx, _ny].Successors = new List<Node>();
                    return;
                }

                GenerateSuccessorsForSingleNode(_grid[_nx, _ny], _grid);
            }
        }
        
        public static Node GenerateSuccessorsForSingleNode(Node _node, Node[,] _grid)
        {
            _node.Successors = new List<Node>();
            
            int _x = _node.X;
            int _y = _node.Y;

            _addToSuccessorsIfInBoundsAndNotObstacle( _x-1, _y);
            _addToSuccessorsIfInBoundsAndNotObstacle( _x+1, _y);
            _addToSuccessorsIfInBoundsAndNotObstacle( _x, _y -1);
            _addToSuccessorsIfInBoundsAndNotObstacle( _x, _y + 1);

            void _addToSuccessorsIfInBoundsAndNotObstacle(int _nx, int _ny )
            {
                if (_nx < 0 || _nx >= _grid.GetLength(0) || _ny < 0 || _ny >= _grid.GetLength(1))
                {
                    return;
                }

                if (_grid[_nx, _ny].IsObstacle == true)
                {
                    return;
                }
                
                _node.Successors.Add(_grid[_nx, _ny]);
            }

            return _node;
        }
    }
}
