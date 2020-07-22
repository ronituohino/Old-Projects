
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
    public class MeshCut
    {
        private static Mesh_Maker _leftSide = new Mesh_Maker();
        private static Mesh_Maker _rightSide = new Mesh_Maker();

        private static Plane _blade;
        private static Mesh _victim_mesh;

        // capping stuff
        private static List<Vector3> _new_vertices = new List<Vector3>();

        private static int _capMatSub = 1;

        private static readonly Vector2[,] textureUVs = {
                                                        { new Vector2(0, 1), new Vector2(1, 1) }, //up
                                                        { new Vector2(1, 1), new Vector2(1, 0) }, //right
                                                        { new Vector2(1, 0), new Vector2(0, 0) }, //down
                                                        { new Vector2(0, 0), new Vector2(0, 1) }, //left
        };

        static bool cutAnything = false;

        /// <summary>
        /// Cut the specified victim
        /// </summary>
        public static GameObject[] Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection, Material capMaterial)
        {
            cutAnything = false;

            // set the blade relative to victim
            _blade = new Plane(victim.transform.InverseTransformDirection(-normalDirection),
                victim.transform.InverseTransformPoint(anchorPoint));

            // get the victims mesh
            _victim_mesh = victim.GetComponent<MeshFilter>().mesh;

            //_leftSide = new Mesh_Maker();
            //_rightSide = new Mesh_Maker();


            bool[] sides = new bool[3];
            int[] indices;
            int p1, p2, p3;

            Vector3[] vertices = _victim_mesh.vertices;
            Vector3[] normals = _victim_mesh.normals;
            Vector2[] uvs = _victim_mesh.uv;

            int subMeshCount = _victim_mesh.subMeshCount;

            // go throught the submeshes
            for (int sub = 0; sub < subMeshCount; sub++)
            {
                indices = _victim_mesh.GetTriangles(sub);
                int indiceLength = indices.Length;

                for (int i = 0; i < indiceLength; i += 3)
                {
                    p1 = indices[i];
                    p2 = indices[i + 1];
                    p3 = indices[i + 2];

                    sides[0] = _blade.GetSide(vertices[p1]);
                    sides[1] = _blade.GetSide(vertices[p2]);
                    sides[2] = _blade.GetSide(vertices[p3]);


                    // whole triangle
                    if (sides[0] == sides[1] && sides[0] == sides[2])
                    {

                        if (sides[0])
                        { // left side

                            _leftSide.AddTriangle(
                                vertices[p1], vertices[p2], vertices[p3],
                                normals[p1], normals[p2], normals[p3],
                                uvs[p1], uvs[p2], uvs[p3],
                                sub);
                        }
                        else
                        {

                            _rightSide.AddTriangle(
                                vertices[p1], vertices[p2], vertices[p3],
                                normals[p1], normals[p2], normals[p3],
                                uvs[p1], uvs[p2], uvs[p3],
                                sub);
                        }

                    }
                    else
                    { // cut the triangle

                        Cut_this_Face(
                             vertices[p1], vertices[p2], vertices[p3],
                             normals[p1], normals[p2], normals[p3],
                             uvs[p1], uvs[p2], uvs[p3],
                            sub);
                    }
                }
            }

            if (!cutAnything)
            {
                _leftSide._vertices.Clear();
                _leftSide._normals.Clear();
                _leftSide._uvs.Clear();
                _leftSide._subIndices.Clear();

                _rightSide._vertices.Clear();
                _rightSide._normals.Clear();
                _rightSide._uvs.Clear();
                _rightSide._subIndices.Clear();

                _new_vertices.Clear();

                return null;
            }

            // The capping Material will be at the end
            Material[] mats = victim.GetComponent<MeshRenderer>().sharedMaterials;
            //int matsLength = mats.Length;

            if (mats[mats.Length - 1].name != capMaterial.name)
            {
                Material[] newMats = new Material[mats.Length + 1];
                mats.CopyTo(newMats, 0);
                newMats[mats.Length] = capMaterial;
                mats = newMats;
            }
            _capMatSub = mats.Length - 1; // for later use

            // cap the opennings
            Capping();

            // Left Mesh
            Mesh left_HalfMesh = _leftSide.GetMesh();
            //left_HalfMesh.name = "Split Mesh Left";

            // Right Mesh
            Mesh right_HalfMesh = _rightSide.GetMesh();
            //right_HalfMesh.name = "Split Mesh Right";

            // assign the game objects

            //victim.name = "left side";
            victim.GetComponent<MeshFilter>().mesh = left_HalfMesh;

            GameObject leftSideObj = victim;

            GameObject rightSideObj = new GameObject(victim.name, typeof(MeshFilter), typeof(MeshRenderer));
            rightSideObj.transform.position = victim.transform.position;
            rightSideObj.transform.rotation = victim.transform.rotation;
            rightSideObj.GetComponent<MeshFilter>().mesh = right_HalfMesh;

            if (victim.transform.parent != null)
            {
                rightSideObj.transform.parent = victim.transform.parent;
            }

            rightSideObj.transform.localScale = victim.transform.localScale;

            // assign mats
            leftSideObj.GetComponent<MeshRenderer>().materials = mats;
            rightSideObj.GetComponent<MeshRenderer>().materials = mats;

            _leftSide._vertices.Clear();
            _leftSide._normals.Clear();
            _leftSide._uvs.Clear();
            _leftSide._subIndices.Clear();

            _rightSide._vertices.Clear();
            _rightSide._normals.Clear();
            _rightSide._uvs.Clear();
            _rightSide._subIndices.Clear();

            _new_vertices.Clear();

            return new GameObject[] { leftSideObj, rightSideObj };
        }

        /// <summary>
        ///  I have no idea how I made this work
        /// </summary>
        private static void Cut_this_Face(
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
            cutAnything = true;

            bool[] sides = new bool[3];
            sides[0] = _blade.GetSide(v0); // true = left
            sides[1] = _blade.GetSide(v1);
            sides[2] = _blade.GetSide(v2);


            Vector3[] leftPoints = new Vector3[2];
            Vector3[] leftNormals = new Vector3[2];
            Vector2[] leftUvs = new Vector2[2];
            Vector3[] rightPoints = new Vector3[2];
            Vector3[] rightNormals = new Vector3[2];
            Vector2[] rightUvs = new Vector2[2];

            bool didset_left = false;
            bool didset_right = false;

            for (int i = 0; i < 3; i++)
            {
                if (sides[i])
                {
                    if (i == 0)
                    {
                        if (!didset_left)
                        {
                            didset_left = true;

                            leftPoints[0] = v0;
                            leftPoints[1] = leftPoints[0];
                            leftUvs[0] = uv0;
                            leftUvs[1] = leftUvs[0];
                            leftNormals[0] = n0;
                            leftNormals[1] = leftNormals[0];
                        }
                        else
                        {
                            leftPoints[1] = v0;
                            leftUvs[1] = uv0;
                            leftNormals[1] = n0;
                        }
                    }
                    else if (i == 1)
                    {
                        if (!didset_left)
                        {
                            didset_left = true;

                            leftPoints[0] = v1;
                            leftPoints[1] = leftPoints[0];
                            leftUvs[0] = uv1;
                            leftUvs[1] = leftUvs[0];
                            leftNormals[0] = n1;
                            leftNormals[1] = leftNormals[0];
                        }
                        else
                        {
                            leftPoints[1] = v1;
                            leftUvs[1] = uv1;
                            leftNormals[1] = n1;
                        }
                    }
                    else
                    {
                        if (!didset_left)
                        {
                            didset_left = true;

                            leftPoints[0] = v2;
                            leftPoints[1] = leftPoints[0];
                            leftUvs[0] = uv2;
                            leftUvs[1] = leftUvs[0];
                            leftNormals[0] = n2;
                            leftNormals[1] = leftNormals[0];
                        }
                        else
                        {
                            leftPoints[1] = v2;
                            leftUvs[1] = uv2;
                            leftNormals[1] = n2;
                        }
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (!didset_right)
                        {
                            didset_right = true;

                            rightPoints[0] = v0;
                            rightPoints[1] = rightPoints[0];
                            rightUvs[0] = uv0;
                            rightUvs[1] = rightUvs[0];
                            rightNormals[0] = n0;
                            rightNormals[1] = rightNormals[0];
                        }
                        else
                        {
                            rightPoints[1] = v0;
                            rightUvs[1] = uv0;
                            rightNormals[1] = n0;
                        }
                    }
                    else if (i == 1)
                    {
                        if (!didset_right)
                        {
                            didset_right = true;

                            rightPoints[0] = v1;
                            rightPoints[1] = rightPoints[0];
                            rightUvs[0] = uv1;
                            rightUvs[1] = rightUvs[0];
                            rightNormals[0] = n1;
                            rightNormals[1] = rightNormals[0];
                        }
                        else
                        {
                            rightPoints[1] = v1;
                            rightUvs[1] = uv1;
                            rightNormals[1] = n1;
                        }
                    }
                    else
                    {
                        if (!didset_right)
                        {
                            didset_right = true;

                            rightPoints[0] = v2;
                            rightPoints[1] = rightPoints[0];
                            rightUvs[0] = uv2;
                            rightUvs[1] = rightUvs[0];
                            rightNormals[0] = n2;
                            rightNormals[1] = rightNormals[0];
                        }
                        else
                        {
                            rightPoints[1] = v2;
                            rightUvs[1] = uv2;
                            rightNormals[1] = n2;
                        }
                    }
                }
            }


            float normalizedDistance = 0.0f;
            float distance = 0;
            _blade.Raycast(new Ray(leftPoints[0], (rightPoints[0] - leftPoints[0]).normalized), out distance);

            normalizedDistance = distance / (rightPoints[0] - leftPoints[0]).magnitude;
            Vector3 newVertex1 = Vector3.Lerp(leftPoints[0], rightPoints[0], normalizedDistance);
            Vector2 newUv1 = Vector2.Lerp(leftUvs[0], rightUvs[0], normalizedDistance);
            Vector3 newNormal1 = Vector3.Lerp(leftNormals[0], rightNormals[0], normalizedDistance);

            _new_vertices.Add(newVertex1);


            _blade.Raycast(new Ray(leftPoints[1], (rightPoints[1] - leftPoints[1]).normalized), out distance);

            normalizedDistance = distance / (rightPoints[1] - leftPoints[1]).magnitude;
            Vector3 newVertex2 = Vector3.Lerp(leftPoints[1], rightPoints[1], normalizedDistance);
            Vector2 newUv2 = Vector2.Lerp(leftUvs[1], rightUvs[1], normalizedDistance);
            Vector3 newNormal2 = Vector3.Lerp(leftNormals[1], rightNormals[1], normalizedDistance);

            _new_vertices.Add(newVertex2);

            //Vector3[] final_verts;
            //Vector3[] final_norms;
            //Vector2[] final_uvs;

            Vector3 fv0;
            Vector3 fv1;
            Vector3 fv2;

            Vector3 fn0;
            Vector3 fn1;
            Vector3 fn2;

            Vector2 fu0;
            Vector2 fu1;
            Vector2 fu2;

            // first triangle

            //final_verts = new Vector3[] { leftPoints[0], newVertex1, newVertex2 };
            fv0 = leftPoints[0];
            fv1 = newVertex1;
            fv2 = newVertex2;

            //final_norms = new Vector3[] { leftNormals[0], newNormal1, newNormal2 };
            fn0 = leftNormals[0];
            fn1 = newNormal1;
            fn2 = newNormal2;

            //final_uvs = new Vector2[] { leftUvs[0], newUv1, newUv2 };
            fu0 = leftUvs[0];
            fu1 = newUv1;
            fu2 = newUv2;

            //final_verts[0] != final_verts[1] && final_verts[0] != final_verts[2]
            //Vector3.Dot(Vector3.Cross(final_verts[1] - final_verts[0], final_verts[2] - final_verts[0]), final_norms[0]) < 0

            if (fv0 != fv1 && fv0 != fv2)
            {
                if (Vector3.Dot(Vector3.Cross(fv1 - fv0, fv2 - fv0), fn0) < 0)
                {
                    //FlipFace(final_verts, final_norms, final_uvs);
                    FlipFace(ref fv0, ref fv1, ref fv2, ref fn0, ref fn1, ref fn2, ref fu0, ref fu1, ref fu2);
                }

                _leftSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2, submesh);
            }

            // second triangle

            //final_verts = new Vector3[] { leftPoints[0], leftPoints[1], newVertex2 };
            fv0 = leftPoints[0];
            fv1 = leftPoints[1];
            fv2 = newVertex2;

            //final_norms = new Vector3[] { leftNormals[0], leftNormals[1], newNormal2 };
            fn0 = leftNormals[0];
            fn1 = leftNormals[1];
            fn2 = newNormal2;

            //final_uvs = new Vector2[] { leftUvs[0], leftUvs[1], newUv2 };
            fu0 = leftUvs[0];
            fu1 = leftUvs[1];
            fu2 = newUv2;

            if (fv0 != fv1 && fv0 != fv2)
            {
                if (Vector3.Dot(Vector3.Cross(fv1 - fv0, fv2 - fv0), fn0) < 0)
                {
                    //FlipFace(final_verts, final_norms, final_uvs);
                    FlipFace(ref fv0, ref fv1, ref fv2, ref fn0, ref fn1, ref fn2, ref fu0, ref fu1, ref fu2);
                }
                _leftSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2, submesh);
            }

            // third triangle

            //final_verts = new Vector3[] { rightPoints[0], newVertex1, newVertex2 };
            fv0 = rightPoints[0];
            fv1 = newVertex1;
            fv2 = newVertex2;

            //final_norms = new Vector3[] { rightNormals[0], newNormal1, newNormal2 };
            fn0 = rightNormals[0];
            fn1 = newNormal1;
            fn2 = newNormal2;

            //final_uvs = new Vector2[] { rightUvs[0], newUv1, newUv2 };
            fu0 = rightUvs[0];
            fu1 = newUv1;
            fu2 = newUv2;


            if (fv0 != fv1 && fv0 != fv2)
            {
                if (Vector3.Dot(Vector3.Cross(fv1 - fv0, fv2 - fv0), fn0) < 0)
                {
                    //FlipFace(final_verts, final_norms, final_uvs);
                    FlipFace(ref fv0, ref fv1, ref fv2, ref fn0, ref fn1, ref fn2, ref fu0, ref fu1, ref fu2);
                }
                _rightSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2, submesh);
            }

            // fourth triangle

            //final_verts = new Vector3[] { rightPoints[0], rightPoints[1], newVertex2 };
            fv0 = rightPoints[0];
            fv1 = rightPoints[1];
            fv2 = newVertex2;

            //final_norms = new Vector3[] { rightNormals[0], rightNormals[1], newNormal2 };
            fn0 = rightNormals[0];
            fn1 = rightNormals[1];
            fn2 = newNormal2;

            //final_uvs = new Vector2[] { rightUvs[0], rightUvs[1], newUv2 };
            fu0 = rightUvs[0];
            fu1 = rightUvs[1];
            fu2 = newUv2;

            if (fv0 != fv1 && fv0 != fv2)
            {
                if (Vector3.Dot(Vector3.Cross(fv1 - fv0, fv2 - fv0), fn0) < 0)
                {
                    //FlipFace(final_verts, final_norms, final_uvs);
                    FlipFace(ref fv0, ref fv1, ref fv2, ref fn0, ref fn1, ref fn2, ref fu0, ref fu1, ref fu2);
                }
                _rightSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2, submesh);
            }
        }

        private static void FlipFace(
            Vector3[] verts,
            Vector3[] norms,
            Vector2[] uvs)
        {
            Vector3 temp = verts[2];
            verts[2] = verts[0];
            verts[0] = temp;

            temp = norms[2];
            norms[2] = norms[0];
            norms[0] = temp;

            Vector2 temp2 = uvs[2];
            uvs[2] = uvs[0];
            uvs[0] = temp2;
        }

        private static void FlipFace(
            ref Vector3 v0,
            ref Vector3 v1,
            ref Vector3 v2,

            ref Vector3 n0,
            ref Vector3 n1,
            ref Vector3 n2,

            ref Vector2 uv0,
            ref Vector2 uv1,
            ref Vector2 uv2)
        {
            Vector3 temp = v2;
            v2 = v0;
            v0 = temp;

            temp = n2;
            n2 = n0;
            n0 = temp;

            Vector2 temp2 = uv2;
            uv2 = uv0;
            uv0 = temp2;
        }

        private static List<Vector3> capVertTracker = new List<Vector3>();
        private static List<Vector3> capVertpolygon = new List<Vector3>();

        static void Capping()
        {
            Vector3[] vertices = _new_vertices.ToArray();
            int vertexCount = vertices.Length;

            capVertTracker.Clear();

            for (int i = 0; i < vertexCount; i++)
                if (!capVertTracker.Contains(vertices[i]))
                {
                    capVertpolygon.Clear();
                    capVertpolygon.Add(vertices[i]);
                    capVertpolygon.Add(vertices[i + 1]);

                    capVertTracker.Add(vertices[i]);
                    capVertTracker.Add(vertices[i + 1]);


                    bool isDone = false;
                    while (!isDone)
                    {
                        isDone = true;

                        for (int k = 0; k < vertexCount; k += 2)
                        { // go through the pairs

                            int count = capVertpolygon.Count;

                            if (vertices[k] == capVertpolygon[count - 1] && !capVertTracker.Contains(vertices[k + 1]))
                            { // if so add the other

                                isDone = false;
                                capVertpolygon.Add(vertices[k + 1]);
                                capVertTracker.Add(vertices[k + 1]);

                            }
                            else if (vertices[k + 1] == capVertpolygon[count - 1] && !capVertTracker.Contains(vertices[k]))
                            {// if so add the other

                                isDone = false;
                                capVertpolygon.Add(vertices[k]);
                                capVertTracker.Add(vertices[k]);
                            }
                        }
                    }

                    FillCap(capVertpolygon);
                }
        }

        static void FillCap(List<Vector3> vertices)
        {
            int vertexCount = vertices.Count;

            // center of the cap
            Vector3 center = Vector3.zero;
            foreach (Vector3 point in vertices)
                center += point;

            center = center / vertexCount;

            // you need an axis based on the cap
            Vector3 upward = Vector3.zero;
            // 90 degree turn
            upward.x = _blade.normal.y;
            upward.y = -_blade.normal.x;
            upward.z = _blade.normal.z;
            Vector3 left = Vector3.Cross(_blade.normal, upward);

            //Vector3 displacement = Vector3.zero;
            //Vector2 newUV1 = Vector2.zero;
            //Vector2 newUV2 = Vector2.zero;

            int textureNum = Random.Range(0, 4);

            for (int i = 0; i < vertexCount; i++)
            {
                //displacement = vertices[i] - center;
                //newUV1 = Vector3.zero;
                //newUV1.x = Clamp(Vector3.Dot(displacement, left));
                //newUV1.y = Clamp(Vector3.Dot(displacement, upward));

                //displacement = vertices[(i + 1) % vertexCount] - center;
                //newUV2 = Vector3.zero;
                //newUV2.x = Clamp(Vector3.Dot(displacement, left));
                //newUV2.y = Clamp(Vector3.Dot(displacement, upward));

                Vector3[] final_verts = new Vector3[] { vertices[i], vertices[(i + 1) % vertexCount], center };
                Vector3[] final_norms = new Vector3[] { -_blade.normal, -_blade.normal, -_blade.normal };
                Vector2[] final_uvs = new Vector2[] { textureUVs[textureNum, 0], textureUVs[textureNum, 1], new Vector2(0.5f, 0.5f) };

                if (Vector3.Dot(Vector3.Cross(final_verts[1] - final_verts[0], final_verts[2] - final_verts[0]), final_norms[0]) < 0)
                {
                    FlipFace(final_verts, final_norms, final_uvs);
                }

                _leftSide.AddTriangle(final_verts, final_norms, final_uvs,
                    _capMatSub);
                //_leftSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2,
                //    _capMatSub);


                final_norms = new Vector3[] { _blade.normal, _blade.normal, _blade.normal };

                if (Vector3.Dot(Vector3.Cross(final_verts[1] - final_verts[0], final_verts[2] - final_verts[0]), final_norms[0]) < 0)
                {
                    FlipFace(final_verts, final_norms, final_uvs);
                }

                _rightSide.AddTriangle(final_verts, final_norms, final_uvs,
                    _capMatSub);
                //_rightSide.AddTriangle(fv0, fv1, fv2, fn0, fn1, fn2, fu0, fu1, fu2, 
                //    _capMatSub);
            }
        }

        static float Clamp(float value)
        {
            if (value < 0)
            {
                return 0;
            }
            else if (value > 1f)
            {
                return 1f;
            }
            else
            {
                return value;
            }
        }
    }
}