using ProceduralSwordGenerator.SwordDataStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProceduralSwordGenerator {
    public class SwordPartMesh : MonoBehaviour {
        Vector3[] vertices;
        Vector2[] uVs;
        int[] triangles;

        Dictionary<float, List<float>> volumesForY;

        [SerializeField] Vector3 incrementBy = new Vector3(0.5f, 0.5f, 0.5f);

        [SerializeField] public Material material;

        Mesh mesh;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        string meshName;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake() {
            if (mesh) {
                SetupMesh();

            }

        }

        public void SetupMesh() {
            // Get Mesh, if thre is not one, create one
            mesh = GetComponent<Mesh>(); if (!mesh) { mesh = new Mesh(); mesh.Clear(); }

            // Get Mesh Filter, if there is not one, add one
            meshFilter = GetComponent<MeshFilter>(); if (!meshFilter) { meshFilter = gameObject.AddComponent<MeshFilter>(); }

            // Get Mesh Renderer, if there is not one, add one
            meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = material;

            mesh.name = meshName;

            mesh.vertices = vertices;
            mesh.uv = uVs;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();

            meshFilter.sharedMesh = mesh;

        }

        public void CreateMesh(ref SwordDataStructs.Shape shape, int xDirection, string name, string materialPath) {
            volumesForY = new Dictionary<float, List<float>>();

            meshName = name;

            if (materialPath != null && materialPath != "") {
                material = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));

            }

            List<int>[] faces = CreateVertices(ref shape, xDirection);
            
            List<int> tempTriangles = new List<int>();
            CreateTriangles(ref tempTriangles, ref faces, xDirection);

        }

        public float CalcCentreMass(float mass, float length) {
            float totalVolume = CalcTotalVolume();

            float factor = totalVolume != 0 ? mass / totalVolume : 0;

            float sumOfYM = 0;
            foreach (var yKey in volumesForY) {
                float totalVolumeForY = 0;
                foreach (var volume in yKey.Value) {
                    totalVolumeForY += volume;

                }

                sumOfYM += yKey.Key * totalVolumeForY * factor;

            }

            float centreMass = mass != 0 ? sumOfYM / mass : 0;

            return centreMass >= 0 ? length - centreMass : -(length + centreMass);

        }

        // Creates Vertices and stores y change indexes in a list<int> for each face
        // xDirection used to change when currentVertex(x, y, z) is added to vertices - negatives used to flip:
        // 1 = x, y, z; 2 = y, x, z;
        List<int>[] CreateVertices(ref SwordDataStructs.Shape shape, int xDirection) {
            // Stores 4 lists - each list is one face of the blade
            // First 4 = top true, bottom true, top false, bottom false
            // Each List contains the index of each y change, eg. y=0 i=0, y=1 i=20, y=2 i=36, ect.
            List<int>[] faces = new List<int>[4];
            for (int i = 0; i < faces.Length; i++) { faces[i] = new List<int>(); }

            // Temporary List to store vertices before converting into vertices array
            List<Vector3> tempVertices = new List<Vector3>();

            Vector3 currentVertex = new Vector3(0, 0, 0);

            // Loops over first 4 faces of sword, ie. top true, bottom true, top false, bottom false - order may change
            for (int i = 0; i < faces.Length; i++) {
                currentVertex = new Vector3(0, 0, 0);

                // Bools to tell what the current face is
                bool edge = i <= 1;
                bool top = i % 2 == 0;

                // Gets maxY possible for current face
                float maxY = SwordDataStructsUtil.CalculateProfileLength(ref edge ? ref shape.leftProfiles : ref shape.rightProfiles);

                // Loops over length of face (Y) - exits when increments greater than length
                bool exitY = false;
                while (!exitY) {
                    // Find profile for current y
                    SwordDataStructs.Shape.Profile currentProfile = SwordDataStructsUtil.FindCurrentProfile(currentVertex.y, ref edge ? ref shape.leftProfiles : ref shape.rightProfiles, out _, out float currentProfileActualY);

                    float maxX = 0, profileCurrentCentreX = 0, profileWidth = 0;

                    // Gets maxX for current Y ( width of profile at current Y)
                    // If last profile - use top width
                    if (currentProfileActualY >= maxY) {
                        profileCurrentCentreX = MathsUtil.CalculateXOnLinearLine(currentVertex.y, new Vector2(currentProfile.topX, currentProfileActualY), new Vector2(currentProfile.topX, currentProfileActualY));

                        Vector2 a = new Vector2(currentProfile.topWidth + profileCurrentCentreX, currentProfileActualY);
                        Vector2 b = new Vector2(currentProfile.topWidth + profileCurrentCentreX, currentProfileActualY);

                        profileWidth = MathsUtil.CalculateXOnLinearLine(currentVertex.y, a, b);

                        if (currentProfile.shape == "R") {
                            MathsUtil.GetEquationOfCircle(a, b, out float radius, out Vector2 origin);

                            maxX = MathsUtil.CalcXOnCircle(currentVertex.y, radius, origin);

                            maxX = Mathf.Abs(maxX);

                        } else {
                            maxX = profileWidth;

                        }

                        // Else calculate as normal
                    } else {
                        profileCurrentCentreX = MathsUtil.CalculateXOnLinearLine(currentVertex.y, new Vector2(currentProfile.baseX, currentProfileActualY), new Vector2(currentProfile.topX, currentProfileActualY + currentProfile.length));

                        Vector2 a = new Vector2(currentProfile.baseWidth + profileCurrentCentreX, currentProfileActualY);
                        Vector2 b = new Vector2(currentProfile.topWidth + profileCurrentCentreX, currentProfileActualY + currentProfile.length);

                        profileWidth = MathsUtil.CalculateXOnLinearLine(currentVertex.y, a, b);

                        if (currentProfile.shape == "R") {
                            MathsUtil.GetEquationOfCircle(a, b, out float radius, out Vector2 origin);

                            maxX = MathsUtil.CalcXOnCircle(currentVertex.y, radius, origin);

                            maxX = Mathf.Abs(maxX);

                        } else {
                            maxX = profileWidth;

                        }

                    }

                    //maxX = Mathf.Abs(maxX);

                    // Adds index of the y change to be added to vertices
                    faces[i].Add(tempVertices.Count);

                    // If current y is out of range of width of face, set to length of face and exit at end of loop 
                    if (currentVertex.y >= maxY) {
                        currentVertex.y = maxY;
                        exitY = true;

                    }

                    // Resets current vertex x and z
                    currentVertex.x = profileCurrentCentreX;
                    currentVertex.z = 0;                    

                    // Finds cross section for current y
                    SwordDataStructs.Shape.CrossSections currentCrossSection = SwordDataStructsUtil.FindCurrentCrossSection(currentVertex.y, ref shape.crossSections, out _, out float currentCrossActualY);

                    SwordDataStructs.Shape.CrossSections nextCrossSection = SwordDataStructsUtil.FindNextCrossSection(currentVertex.y, ref shape.crossSections, out _, out float nextCrossActualY);


                    // Selects if current edge is true or false
                    SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] currentEdges = edge ? currentCrossSection.trueEdges : currentCrossSection.falseEdges;


                    // Add check for nullptr

                    // Loops over width of face (X) - exits when increments greater than width
                    bool exitX = false;
                    while (!exitX) {
                        // If current x is out of range of width of face, set to width of face and exit at end of loop 
                        if (currentVertex.x >= maxX) {
                            currentVertex.x = maxX;
                            exitX = true;

                        }

                        // Calculate Z
                        currentVertex.z = CalcZ(new Vector2(currentVertex.x * (profileWidth / maxX) - profileCurrentCentreX, currentVertex.y), ref nextCrossSection, ref currentEdges, profileWidth - profileCurrentCentreX, edge, top, currentCrossActualY, nextCrossActualY);

                        // Adds currentVertex to vertices - flips x and z dependent on which face
                        if (xDirection == 1) {
                            tempVertices.Add(new Vector3(edge ? currentVertex.x : -currentVertex.x, currentVertex.y, top ? currentVertex.z : -currentVertex.z));

                        } else if (xDirection == -1) {
                            tempVertices.Add(new Vector3(edge ? currentVertex.x : -currentVertex.x, -currentVertex.y, top ? currentVertex.z : -currentVertex.z));

                        } else if (xDirection == 2) {
                            tempVertices.Add(new Vector3(currentVertex.y, !edge ? currentVertex.x : -currentVertex.x, top ? currentVertex.z : -currentVertex.z));

                        } else if (xDirection == -2) {
                            tempVertices.Add(new Vector3(-currentVertex.y, !edge ? currentVertex.x : -currentVertex.x, !top ? currentVertex.z : -currentVertex.z));

                        } 

                        // Increments x for next loop
                        currentVertex.x += incrementBy.x;


                    }

                    // Increments y for next loop
                    currentVertex.y += incrementBy.y;

                }

                // Adds index of last vertex to the end of faces[i]
                faces[i].Add(tempVertices.Count);

            }

            // Converts temporary list to array - number of vertices are not planned to be changed
            vertices = tempVertices.ToArray();

            return faces;

        }

        float CalcZ(Vector2 currentXY, ref SwordDataStructs.Shape.CrossSections nextCrossSection, ref SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] currentEdges, float profileWidthAtY, bool edge, bool top, float currentCrossActualY, float nextCrossActualY) {

            // Calculates factor based on current x and width of profile at y
            float widthFactor = MathsUtil.CalculateFactor(currentXY.x, profileWidthAtY);

            // Calculate Z for current cross section at width * widthFactor
            float currentCrossDepth = SwordDataStructsUtil.CalculateCrossSectionDepthWithFactor(widthFactor, ref currentEdges, edge == top);

            // Calculate Z for next cross section at width * widthFactor
            float nextCrossDepth = SwordDataStructsUtil.CalculateCrossSectionDepthWithFactor(widthFactor, ref edge ? ref nextCrossSection.trueEdges : ref nextCrossSection.falseEdges, edge == top);

            // Calculates length factor of current y along current cross section
            float lengthFactor = MathsUtil.CalculateFactor(currentXY.y - currentCrossActualY, nextCrossActualY - currentCrossActualY);

            // Lerp between currentCross depth and nextCross depth using length factor
            return Mathf.Lerp(currentCrossDepth, nextCrossDepth, lengthFactor);

        }

        void CreateTrianglesForFace(ref List<int> face, ref List<int> tempTriangles, bool clockwise) {
            // Loops over all but last 2 y changes
            // For each y change index - need to check that last row isnt a row of vertices with no way to make triangles
            for (int yChangeIndex = 0; yChangeIndex < face.Count - 2; yChangeIndex++) {
                // Loops over vertex indexes between y and next y change
                for (int i = face[yChangeIndex]; i < (face[yChangeIndex + 1] - 1); i++) {
                    int d = i + face[yChangeIndex + 1] - face[yChangeIndex];
                    int c = d + 1;

                    // yChangeIndex + 2 is in length of face 
                    if ((yChangeIndex + 2) < face.Count) {
                        // If d is in next next yChange - set to end of next yChange - c must also be the same
                        if (d >= face[yChangeIndex + 2]) {
                            d = face[yChangeIndex + 2] - 1;
                            c = d;



                            // If d is not in next next yChange, check that c is not - set to end of next yChange
                        } else if (c >= face[yChangeIndex + 2]) {
                            c = face[yChangeIndex + 2] - 1;

                        // If on last iteration and next y change is longer than current y - set to end of next yChange
                        } else if (i == (face[yChangeIndex + 1] - 2) && face[yChangeIndex + 2] - face[yChangeIndex + 1] > face[yChangeIndex + 1] - face[yChangeIndex]) {
                            c = face[yChangeIndex + 2] - 1;

                        }

                        // Else yChangeIndex + 2 is out of range
                    } else {
                        // Check if d is out of range of faces indexes // This doesnt work - face.Last is last y change and not last possible y
                        if (d >= face.Last()) {
                            d = face.Last();
                            c = d;

                            // If d is not in next yChange, check c is not
                        } else if (c >= face.Last()) {
                            c = face.Last();

                        }

                    }

                    AddTriangles(i, i + 1, c, d, ref tempTriangles, clockwise);

                    // If does not contain y, create list for y
                    if (!volumesForY.ContainsKey(vertices[tempTriangles[i]].y)) volumesForY.Add(vertices[tempTriangles[i]].y, new List<float>());

                    volumesForY[vertices[tempTriangles[i]].y].Add(CalcVolumeOfTriangleFace(vertices[tempTriangles[i]], vertices[tempTriangles[i + 1]], vertices[tempTriangles[c]]));
                    volumesForY[vertices[tempTriangles[i]].y].Add(CalcVolumeOfTriangleFace(vertices[tempTriangles[i]], vertices[tempTriangles[c]], vertices[tempTriangles[d]]));

                }

            }

        }

        // Need to check for when z is z * -1 as volume is calculated currently assuming z starts from 0
        float CalcVolumeOfTriangleFace(Vector3 A, Vector3 B, Vector3 C) {
            A.x = Mathf.Abs(A.x);
            A.y = Mathf.Abs(A.y);
            A.z = Mathf.Abs(A.z);

            B.x = Mathf.Abs(B.x);
            B.y = Mathf.Abs(B.y);
            B.z = Mathf.Abs(B.z);

            C.x = Mathf.Abs(C.x);
            C.y = Mathf.Abs(C.y);
            C.z = Mathf.Abs(C.z);

            float height = Mathf.Min(A.z, B.z, C.z);

            float volume = Mathf.Abs(MathsUtil.CalcVolumeOfTriPrism(A, B, C, height));

            A.z -= height;
            B.z -= height;
            C.z -= height;

            // If remaining height of each point is ~= to 0, return volume
            if (Mathf.Approximately(A.z, 0) && Mathf.Approximately(B.z, 0) && Mathf.Approximately(C.z, 0)) return volume;

            // Changes C to be shortest length
            if (A.z == 0) {
                Vector3 temp = C;
                C = A;
                A = temp;

            } else if (B.z == 0) {
                Vector3 temp = C;
                C = B;
                B = temp;                

            }

            // If A.z and B.z are the same - calculate quarter of square based pyramid
            if (Mathf.Approximately(A.z, B.z)) {
                height = Mathf.Abs(C.y - B.y);

                // Quarter of pyramid volume
                volume += Mathf.Abs(MathsUtil.CalcVolumneOfSquarePyramid((B - A).magnitude * 2, height));

            // They are different
            } else {
                // Changes B to be longest
                if (B.z < A.z) {
                    Vector2 temp = B;
                    B = A;
                    A = temp;

                }

                // Half height
                height = A.z;

                // Volume of last triangle * half height
                volume += Mathf.Abs(MathsUtil.CalcVolumeOfTriPrism(A, B, C, height * 0.5f));

                float oa = B.z != 0 ? Mathf.Abs(C.y - B.y) / B.z : 0;
                float pyrHeight = Mathf.Abs((B.z - A.z) * Mathf.Tan(Mathf.Atan(oa)));

                A.z -= height;
                B.z -= height;

                volume += Mathf.Abs(MathsUtil.CalcVolumeOfTriPrism(A, B, new Vector2(C.x, pyrHeight), height * 0.5f));

                volume += Mathf.Abs(MathsUtil.CalcVolumneOfRectPyramid(B.z * 2, (B - A).magnitude * 2, pyrHeight) * 0.25f);

            }


            return volume;

        }

        void CreateSideTriangles(ref List<int> face1, ref List<int> face2, ref List<int> tempTriangles, bool clockwise) {
            // If faces arent null
            if (face1 != null && face2 != null) {
                int a, b, c, d;

                // For bottom face

                // First in face
                a = face2[0];
                d = face1[0];

                // Last in first row
                b = 1 < face2.Count ? face2[1] - 1 : face2.Last();
                c = 1 < face1.Count ? face1[1] - 1 : face1.Last();

                AddTriangles(a, b, c, d, ref tempTriangles, clockwise);

                // For top face

                // First in last row
                a = face1[face1.Count >= 2 ? face1.Count - 2 : 0];
                d = face2[face2.Count >= 2 ? face2.Count - 2 : 0];

                // Last in last row
                b = face1.Last() - 1;
                c = face2.Last() - 1;

                AddTriangles(a, b, c, d, ref tempTriangles, clockwise);

                // For sides
                // Loops over each yChange of longest face
                for (int yChangeIndex = 1; yChangeIndex < (face1.Count >= face2.Count ? face1.Count : face2.Count) - 1; yChangeIndex++) {
                    // One before y change - last in row
                    a = face1[yChangeIndex] - 1; // add check
                    b = face2[yChangeIndex] - 1;

                    if (a < 0) a = 0; if (b < 0) b = 0;

                    // One before next y change - last in next row
                    int nextYChange = yChangeIndex + 1;
                    c = nextYChange < face2.Count ? face2[nextYChange] - 1 : face2.Last();
                    d = nextYChange < face1.Count ? face1[nextYChange] - 1 : face1.Last();

                    AddTriangles(a, b, c, d, ref tempTriangles, clockwise);

                }

            }

        }

        void AddTriangles(int a, int b, int c, int d, ref List<int> tempTriangles, bool clockwise) {
            // For clockwise
            if (clockwise) {
                tempTriangles.Add(a);
                tempTriangles.Add(b);
                tempTriangles.Add(c);
                tempTriangles.Add(d);
                tempTriangles.Add(a);
                tempTriangles.Add(c);

                // For anit clockwise
            } else {
                tempTriangles.Add(a);
                tempTriangles.Add(c);
                tempTriangles.Add(b);
                tempTriangles.Add(a);
                tempTriangles.Add(d);
                tempTriangles.Add(c);

            }

        }

        void CreateTriangles(ref List<int> tempTriangles, ref List<int>[] faces, int xDirection) {
            CreateSideTriangles(ref faces[0], ref faces[1], ref tempTriangles, xDirection != -1);
            CreateSideTriangles(ref faces[3], ref faces[2], ref tempTriangles, xDirection != -1);

            // For each face in faces
            for (int i = 0; i < faces.Length; i++) {
                // Create triangles for face - clockwise pattern = 1, 0, 0, 1
                CreateTrianglesForFace(ref faces[i], ref tempTriangles, (i % 4 == 0 || i % 4 == 3) == (xDirection != -1));

            }

            triangles = tempTriangles.ToArray();

        }

        float CalcTotalVolume() {
            float totalVolume = 0;
            foreach (var yKey in volumesForY) {
                foreach (var v in yKey.Value) {
                    totalVolume += v;

                }

            }

            return totalVolume;

        }

    }

}
