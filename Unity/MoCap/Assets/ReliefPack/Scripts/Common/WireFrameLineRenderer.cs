//
// taken from:
// http://forum.unity3d.com/threads/8814-Wireframe-3D/page3
//
	using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
     
    public class WireFrameLineRenderer : MonoBehaviour
    {
        //****************************************************************************************
        //  Material options
        //****************************************************************************************
        public Color LineColor;
        public bool ZWrite = true;
        public bool AWrite = true;
        public bool Blend = true;
        public int Fidelity = 3;
       
        //****************************************************************************************
        // Line Data
        //****************************************************************************************
        private Vector3[] Lines;
        private List<Line> LinesArray = new List<Line>();
        private Material LineMaterial;
       
       
        //*****************************************************************************************
        // Helper class, Line is defined as two Points
        //*****************************************************************************************
        public class Line
        {
            public Vector3 PointA;
            public Vector3 PointB;
           
            public Line (Vector3 a, Vector3 b)
            {
                PointA = a;
                PointB = b;
            }
           
            //*****************************************************************************************
            // A == B if   Aa&&Ab == Ba&&Bb or Ab&&Ba == Aa && Bb
            //*****************************************************************************************
            public static bool operator == (Line lA, Line lB )
            {
                if( lA.PointA == lB.PointA && lA.PointB == lB.PointB )
                {
                    return true;
                }
               
                if ( lA.PointA == lB.PointB && lA.PointB == lB.PointA )
                {
                    return true;   
                }
                   
               
                return false;
            }
		
		    public override bool Equals(System.Object obj)
		    {
		        if (obj == null)
		        {
		            return false;
		        }
		
		        Line lB = obj as Line;
		        if ((System.Object)lB == null)
		        {
		            return false;
		        }
		
                if( PointA == lB.PointA && PointB == lB.PointB )
                {
                    return true;
                }
               
                if ( PointA == lB.PointB && PointB == lB.PointA )
                {
                    return true;   
                }
               
                return false;
		    }		
		    public bool Equals(Line lB)
		    {
                if( PointA == lB.PointA && PointB == lB.PointB )
                {
                    return true;
                }
               
                if ( PointA == lB.PointB && PointB == lB.PointA )
                {
                    return true;   
                }
               
                return false;
			}
		
		    public override int GetHashCode()
		    {
		        return 0;
		    }		
           
            //*****************************************************************************************
            // A != B if   !(Aa&&Ab == Ba&&Bb or Ab&&Ba == Aa && Bb)
            //*****************************************************************************************
            public static bool operator != (Line lA, Line lB )
            {
                return !( lA == lB );
            }
        }
       
        //*****************************************************************************************
        // Parse the mesh this is attached to and save the line data
        //*****************************************************************************************
        public void Start ()
        {      
            LineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Front Fog { Mode Off } } } }"); 
            LineMaterial.hideFlags = HideFlags.HideAndDontSave;
            LineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
         
            MeshFilter filter = GetComponent<MeshFilter>();
            Mesh mesh = filter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
           
            for (int i = 0; i < triangles.Length/3; i++)
            {
                int j = i*3;
                Line lineA = new Line( vertices[triangles[j]], vertices[triangles[j+1]] );
                Line lineB = new Line( vertices[triangles[j+1]], vertices[triangles[j+ 2]] );
                Line lineC = new Line( vertices[triangles[j+2]], vertices[triangles[j]] );
     
                if( Fidelity == 3 )
                {
                    AddLine( lineA );
                    AddLine( lineB );
                    AddLine( lineC );
                }
                else if ( Fidelity == 2 )
                {
                    AddLine( lineA );
                    AddLine( lineB );
                }
                else if( Fidelity == 1 )
                {
                    AddLine(lineA); 
                }
            }
        }
       
        //****************************************************************************************
        // Adds a line to the array if the equivalent line isn't stored already
        //****************************************************************************************
        public void AddLine(Line l)
        {
            bool found = false;
            foreach( Line line in LinesArray )
            {
                if( l == line )
                { found = true; break; }
            }
           
            if( !found )
            { LinesArray.Add( l ); }
        }
     
        //****************************************************************************************
        // Deferred rendering of wireframe, this should let materials go first
        //****************************************************************************************
        public void OnRenderObject()
        {  
            LineMaterial.SetPass(0);
           
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(LineColor);
           
            foreach( Line line in LinesArray )
            {
                GL.Vertex( line.PointA );
                GL.Vertex( line.PointB );
            }
                 
            GL.End();
            GL.PopMatrix();
        }
    }