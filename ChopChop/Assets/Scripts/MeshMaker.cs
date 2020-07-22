
//    MIT License
//    
//    Copyright (c) 2017 Dustin Whirle
//    
//    My Youtube stuff: https://www.youtube.com/playlist?list=PL-sp8pM7xzbVls1NovXqwgfBQiwhTA_Ya
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace BLINDED_AM_ME
{
    public class Mesh_Maker
    {
        // Mesh Values
        public List<Vector3> _vertices = new List<Vector3>();
        public List<Vector3> _normals = new List<Vector3>();
        public List<Vector2> _uvs = new List<Vector2>();
        public List<List<int>> _subIndices = new List<List<int>>();

        public int VertCount
        {
            get
            {
                return _vertices.Count;
            }
        }

        public void AddTriangle(
            Vector3 v0,
            Vector3 v1,
            Vector3 v2,

            Vector3 n0,
            Vector3 n1,
            Vector3 n2,

            Vector2 uv0,
            Vector2 uv1,
            Vector2 uv2,
            int submesh)
        {
            int vertCount = _vertices.Count;

            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);

            _normals.Add(n0);
            _normals.Add(n1);
            _normals.Add(n2);

            _uvs.Add(uv0);
            _uvs.Add(uv1);
            _uvs.Add(uv2);

            int subIndiceCount = _subIndices.Count;

            if (subIndiceCount < submesh + 1)
            {
                for (int i = subIndiceCount; i < submesh + 1; i++)
                {
                    _subIndices.Add(new List<int>());
                }
            }

            _subIndices[submesh].Add(vertCount);
            _subIndices[submesh].Add(vertCount + 1);
            _subIndices[submesh].Add(vertCount + 2);
        }

        public void AddTriangle(
            Vector3[] verticies,
            Vector3[] normals,
            Vector2[] uvs,
            int submesh)
        {
            int vertCount = _vertices.Count;

            _vertices.Add(verticies[0]);
            _vertices.Add(verticies[1]);
            _vertices.Add(verticies[2]);

            _normals.Add(normals[0]);
            _normals.Add(normals[1]);
            _normals.Add(normals[2]);

            _uvs.Add(uvs[0]);
            _uvs.Add(uvs[1]);
            _uvs.Add(uvs[2]);

            int subIndiceCount = _subIndices.Count;

            if (subIndiceCount < submesh + 1)
            {
                for (int i = subIndiceCount; i < submesh + 1; i++)
                {
                    _subIndices.Add(new List<int>());
                }
            }

            _subIndices[submesh].Add(vertCount);
            _subIndices[submesh].Add(vertCount + 1);
            _subIndices[submesh].Add(vertCount + 2);
        }

        /// <summary>
        /// Creates and returns a new mesh
        /// </summary>
        public Mesh GetMesh()
        {
            Mesh shape = new Mesh();
            shape.name = "Generated Mesh";
            shape.SetVertices(_vertices);
            shape.SetNormals(_normals);
            shape.SetUVs(0, _uvs);
            shape.SetUVs(1, _uvs);

            int subIndiceCount = _subIndices.Count;

            shape.subMeshCount = subIndiceCount;

            for (int i = 0; i < subIndiceCount; i++)
                shape.SetTriangles(_subIndices[i], i);

            return shape;
        }
    }
}