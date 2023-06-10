using System;
using RPG.Attributes;
using UnityEngine;

namespace AI.Sensors
{
    public class VisionMeshGenerator : MonoBehaviour
    {
        [Header ("Vision Field settings")]
        [Tooltip("How smooth is the horizontal arc, segments per all horizontal angle")]
        [SerializeField] int horizontalSegmentsAmount = 10;
        [Tooltip("How smooth is the vertical arc, segments per HALF of vertical angle")]
        [SerializeField] int verticalSegmentsAmount = 2;
        
        [Space(5f)] [Header ("Vision Field data")]
    
        [ReadOnly][SerializeField] float horizontalAngle;
        [ReadOnly][SerializeField] float verticalAngle;
        [ReadOnly][SerializeField] float visionRadius;
        
        //Mesh variables
        Vector3[] _vertices;
        int[] _triangles; 
        Mesh _visionMesh;
        
        //Character
        Character _ch;

        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();
        }
        
        void OnEnable()
        {
            _ch.Events.OnVisionDataChange += RecalculateMesh;
        }

        void OnDisable()
        {
            _ch.Events.OnVisionDataChange -= RecalculateMesh;
        }

        public void RecalculateMesh(CharacterDataStruct.VisionStruct visionStruct)
        {
            SetMeshData();
        }


        public Mesh SetMeshData()
        {
            _visionMesh = new Mesh();
            
            int trianglesAmount = (verticalSegmentsAmount + verticalSegmentsAmount + 1) * horizontalSegmentsAmount * 4 + 2 + 2 + verticalSegmentsAmount * 2 * (2 + 2);
            _vertices = new Vector3[trianglesAmount * 3];
            _triangles = new int[trianglesAmount * 3];
            
            Vector3 forwardRadius = Vector3.forward * _ch.data.vision.Radius;
            float halfHorAngle = _ch.data.vision.HorizontalAngle / 2;

            Vector3 bottomShift = new Vector3(0, _ch.data.vision.BottomVertex, 0);
            Vector3 bottomCenter = Vector3.zero + bottomShift;
            Vector3 bottomLeft = Quaternion.Euler(0, -halfHorAngle, 0) * forwardRadius + bottomShift;
            Vector3 bottomRight = Quaternion.Euler(0, halfHorAngle, 0) * forwardRadius + bottomShift;

            Vector3 topShift = new Vector3(0, _ch.data.vision.TopVertex, 0);
            Vector3 topCenter = Vector3.zero + topShift;
            Vector3 topLeft = Quaternion.Euler(0, -halfHorAngle, 0) * forwardRadius + topShift;
            Vector3 topRight = Quaternion.Euler(0, halfHorAngle, 0) * forwardRadius + topShift;

            int vert = 0;
            
            //left side
            vert = DrawLeftSide(vert, bottomCenter, bottomLeft, topLeft);

            _vertices[vert++] = bottomCenter;
            _vertices[vert++] = topLeft;
            _vertices[vert++] = topCenter;

            //right side
            vert = DrawRightSide(vert, bottomCenter, topRight, bottomRight);

            _vertices[vert++] = bottomCenter;
            _vertices[vert++] = topCenter;
            _vertices[vert++] = topRight;
            
            //middle segment
            float startHorAngle = -halfHorAngle;
            float deltaHorAngle = _ch.data.vision.HorizontalAngle / horizontalSegmentsAmount;
            float deltaVertAngle = _ch.data.vision.VerticalAngle / verticalSegmentsAmount / 2;

            for (int i = 0; i < horizontalSegmentsAmount; i++)
            {
                //MIDDLE part of vision field
                topLeft = Quaternion.Euler(0, startHorAngle + deltaHorAngle * i, 0) * forwardRadius + topShift;
                topRight = Quaternion.Euler(0, startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + topShift;
                
                bottomLeft = Quaternion.Euler(0, startHorAngle + deltaHorAngle * i, 0) * forwardRadius + bottomShift;
                bottomRight = Quaternion.Euler(0, startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + bottomShift;
                
                vert = DrawSquare(vert, bottomRight, topLeft, bottomLeft, topRight);

                //TOP part of vision field (depend on the Vertical angle)
                for (int a = 0; a < verticalSegmentsAmount; a++)
                {
                    topLeft = Quaternion.Euler(0 - deltaVertAngle * (a+1), startHorAngle + deltaHorAngle * i, 0) * forwardRadius + topShift;
                    topRight = Quaternion.Euler(0 - deltaVertAngle * (a+1), startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + topShift;
                
                    bottomLeft = Quaternion.Euler(0 - deltaVertAngle * a, startHorAngle + deltaHorAngle * i, 0) * forwardRadius + topShift;
                    bottomRight = Quaternion.Euler(0 - deltaVertAngle * a, startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + topShift;
                    
                    vert = DrawSquare(vert, bottomRight, topLeft, bottomLeft, topRight);

                    //Draw side triangles on start and in the end
                    if (i == 0)
                        vert = DrawLeftSide(vert, topCenter, bottomLeft, topLeft);

                    if (i == horizontalSegmentsAmount - 1)
                        vert = DrawRightSide(vert, topCenter, topRight, bottomRight);
                    
                    //is this the last top vertical segment? Draw a top triangle
                    if (a == verticalSegmentsAmount - 1)
                    {
                        //top
                        _vertices[vert++] = topCenter;
                        _vertices[vert++] = topLeft;
                        _vertices[vert++] = topRight;
                    }
                }
                
                //BOTTOM part of vision field (depend on the Vertical angle)
                for (int b = 0; b < verticalSegmentsAmount; b++)
                {
                    topLeft = Quaternion.Euler(0 + deltaVertAngle * b, startHorAngle + deltaHorAngle * i, 0) * forwardRadius + bottomShift;
                    topRight = Quaternion.Euler(0 + deltaVertAngle * b, startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + bottomShift;
                
                    bottomLeft = Quaternion.Euler(0 + deltaVertAngle * (b+1), startHorAngle + deltaHorAngle * i, 0) * forwardRadius + bottomShift;
                    bottomRight = Quaternion.Euler(0 + deltaVertAngle * (b+1), startHorAngle + deltaHorAngle * (i+1), 0) * forwardRadius + bottomShift;
                    
                    vert = DrawSquare(vert, bottomRight, topLeft, bottomLeft, topRight);
                    
                    //Draw side triangles on start and in the end
                    if (i == 0)
                        vert = DrawLeftSide(vert, bottomCenter, bottomLeft, topLeft);

                    if (i == horizontalSegmentsAmount - 1)
                        vert = DrawRightSide(vert, bottomCenter, topRight, bottomRight);
                    
                    //is this the last bottom vertical segment? Draw a top triangle
                    if (b == verticalSegmentsAmount - 1)
                    {
                        //bottom
                        _vertices[vert++] = bottomCenter;
                        _vertices[vert++] = bottomRight;
                        _vertices[vert++] = bottomLeft;
                    }
                }
            }
            
            for (int i = 0; i < _triangles.Length; i++)
            {
                _triangles[i] = i;
            }

            _visionMesh.vertices = _vertices;
            _visionMesh.triangles = _triangles;
            _visionMesh.RecalculateNormals();

            return _visionMesh;
        }

        int DrawSquare(int vert, Vector3 bottomRight, Vector3 topLeft, Vector3 bottomLeft, Vector3 topRight)
        {
            _vertices[vert++] = bottomRight;
            _vertices[vert++] = topLeft;
            _vertices[vert++] = bottomLeft;

            _vertices[vert++] = bottomRight;
            _vertices[vert++] = topRight;
            _vertices[vert++] = topLeft;
            return vert;
        }

        int DrawRightSide(int vert, Vector3 bottomCenter, Vector3 topRight, Vector3 bottomRight)
        {
            _vertices[vert++] = bottomCenter;
            _vertices[vert++] = topRight;
            _vertices[vert++] = bottomRight;
            return vert;
        }

        int DrawLeftSide(int vert, Vector3 bottomCenter, Vector3 bottomLeft, Vector3 topLeft)
        {
            _vertices[vert++] = bottomCenter;
            _vertices[vert++] = bottomLeft;
            _vertices[vert++] = topLeft;
            return vert;
        }
    }
}