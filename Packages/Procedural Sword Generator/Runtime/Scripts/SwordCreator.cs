using ProceduralSwordGenerator.CSVDataStructs;
using ProceduralSwordGenerator.SwordDataStructs;
using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordCreator : MonoBehaviour {

        [SerializeField] bool createOneSword = false;

        [SerializeField] public bool createSwordReplica = true;

        [SerializeField] public bool createReplicaParts = true;

        [SerializeField] public bool realisticMaterial = true;

        [SerializeField] public float swordScale = 1.0f;

        [SerializeField] AllCSVData allCSVData;

        [SerializeField] string soFolderPath = "Packages/firestarw.procedural_sword_generator/Runtime/Scriptable Objects/";
        [SerializeField] string soSwordsFolder = "Swords/";

        [SerializeField] string materialsFolderPath = "Packages/firestarw.procedural_sword_generator/Runtime/Materials/";

        public Tuple<string, string>[] materials; // Contains name, path

        int swordNum = 0; // Change to read from so to keep persistant between plays - need better system to reuse id's from deleted swords

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            allCSVData = (AllCSVData)AssetDatabase.LoadAssetAtPath(soFolderPath + "AllCSVData.asset", typeof(AllCSVData));

            // Get guids of materials in materialFoldersPaths
            string[] materialGUIDS = AssetDatabase.FindAssets("t:material", new string[] { materialsFolderPath });

            materials = new Tuple<string,string>[materialGUIDS.Length];

            for (int i = 0; i < materialGUIDS.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(materialGUIDS[i]);
                string name = path.Replace(materialsFolderPath, "").Replace(".mat", "");

                // Splits subfolders from name - name should be last
                string[] temp = name.Split("/");

                materials[i] = new Tuple<string, string>(temp.Last(), path);

            }

        }

        // Update is called once per frame
        void Update() {
            if (createOneSword) {
                CreateSword();

            }

        }

        public Sword CreateSword() {
            // Create Sword gameobject
            GameObject sword = new GameObject("Sword " + swordNum);

            // Get random sword from AllCSV's
            CSVDataStructs.Sword swordToMake = allCSVData.swords[UnityEngine.Random.Range(0, allCSVData.swords.Length)];

            SwordData swordData;

            // Get properties from file

            // Create Sword Data Struct
            SwordDataStructs.Sword swordDataStruct = new SwordDataStructs.Sword();

            // Fill sword data struct
            swordDataStruct.name = createSwordReplica && createReplicaParts ? swordToMake.name : sword.name;
            swordDataStruct.id = swordNum;

            swordDataStruct.blade = CreateBlade(swordToMake.bladeId, swordToMake.tangId, swordToMake.fullerTopFaceIds, swordToMake.fullerTopFaceCentreX, swordToMake.fullerTopFaceStartY, swordToMake.fullerBottomFaceIds, swordToMake.fullerBottomFaceCentreX, swordToMake.fullerBottomFaceStartY, swordToMake.name);

            if (createSwordReplica) {
                if (createReplicaParts) {
                    sword.name = swordToMake.name;

                }

                swordDataStruct.trueGuards = CreateGuards(swordToMake.trueGuardIds, swordToMake.name);
                swordDataStruct.falseGuards = CreateGuards(swordToMake.falseGuardIds, swordToMake.name);
                swordDataStruct.grip = CreateGrip(swordToMake.gripId, swordToMake.name);
                swordDataStruct.pommel = CreatePommel(swordToMake.pommelId, swordToMake.name);

            } else {
                swordToMake = allCSVData.swords[UnityEngine.Random.Range(0, allCSVData.swords.Length)];
                swordDataStruct.trueGuards = CreateGuards(swordToMake.trueGuardIds, swordToMake.name);
                swordDataStruct.falseGuards = CreateGuards(swordToMake.falseGuardIds, swordToMake.name);

                swordToMake = allCSVData.swords[UnityEngine.Random.Range(0, allCSVData.swords.Length)];
                swordDataStruct.grip = CreateGrip(swordToMake.gripId, swordToMake.name);

                swordToMake = allCSVData.swords[UnityEngine.Random.Range(0, allCSVData.swords.Length)];
                swordDataStruct.pommel = CreatePommel(swordToMake.pommelId, swordToMake.name);

            }

            // Create SwordData SO
            swordData = ScriptableObject.CreateInstance<SwordData>();
            swordData.swordData = swordDataStruct;

            AssetDatabase.CreateAsset(swordData, soFolderPath + soSwordsFolder + sword.name + ".asset");
            AssetDatabase.SaveAssets();

            // Get material paths
            swordData.swordData.blade.bladeMaterialPath = GetMaterialPath(swordData.swordData.blade.bladeMaterial);
            swordData.swordData.grip.materialPath = GetMaterialPath(swordData.swordData.grip.material);
            swordData.swordData.pommel.materialPath = GetMaterialPath(swordData.swordData.pommel.material);

            for (int i = 0; i < swordData.swordData.trueGuards.Length; i++) swordData.swordData.trueGuards[i].materialPath = GetMaterialPath(swordData.swordData.trueGuards[i].material);
            for (int i = 0; i < swordData.swordData.falseGuards.Length; i++) swordData.swordData.falseGuards[i].materialPath = GetMaterialPath(swordData.swordData.falseGuards[i].material);

            // Adds sword script which will initialise its properties
            Sword swordScript = sword.AddComponent<Sword>();

            // Setup sword
            swordScript.Setup(swordData);

            sword.transform.position = new Vector3((swordNum + 1) * 15, 0, 0);

            sword.transform.localScale = new Vector3(swordScale, swordScale, swordScale);

            sword.AddComponent<SwordPhysics>();

            swordNum++;
            createOneSword = false;

            return swordScript;

        }

        string GetMaterialPath(string partMaterial) {
            string materialPath = "";

            if (partMaterial != null) {
                string[] partMaterials = partMaterial.Split(" ");

                bool exit = false;

                string folderName = "";
                string materialName = "";

                // For each word in part materials
                foreach (string pMaterial in partMaterials) {

                    // Check against each material in materials
                    foreach (var material in materials) {
                        // Folders inside Materials/
                        string[] folders = material.Item2.Replace(materialsFolderPath, "").Split("/");

                        if (folders.Length > 0) {
                            // If not matching - next loop
                            if (folders.Contains("Realistic") != realisticMaterial) continue;

                            // For each folder, check if folder name is in part material
                            for (int i = 1; i < folders.Length - 1; i++) {

                                // If folder name is in part name - store folder name and break
                                if (pMaterial.Contains(folders[i])) {
                                    materialName = pMaterial;
                                    folderName = folders[i] + "/";
                                    exit = true;
                                    break;

                                }

                            }

                        }

                        if (exit) break;

                    }

                    if (exit) break;

                }


                // If folderName and materialName are not blank
                if (folderName != "/" && materialName != "") {
                    bool materialFound = false;

                    // For each material
                    foreach (var material in materials) {
                        // If not matching - next loop
                        if (material.Item2.Contains("Realistic") != realisticMaterial) continue;

                        // If correct folder
                        if (material.Item2.Contains(folderName)) {

                            // If name matches, set materialPath and break
                            if (material.Item1 == materialName) {
                                materialPath = material.Item2;
                                materialFound = true;
                                break;

                            }

                        }


                    }

                    // If material was not found - Get first material in same folder
                    if (!materialFound) {
                        foreach (var material in materials) {
                            // If not matching - next loop
                            if (material.Item2.Contains("Realistic") != realisticMaterial) continue;

                            // If correct folder
                            if (material.Item2.Contains(folderName)) {
                                materialPath = material.Item2;
                                materialFound = true;
                                break;

                            }


                        }

                    }

                }

            }

            // If material path is blank and materials isnt empty, set to first material
            if (materialPath == "" && materials.Length > 0) materialPath = materials.First().Item2;

            return materialPath;

        }

        void CreateShape(ref SwordDataStructs.Shape shape) {
            if (!createReplicaParts) {
                float probability = 70;

                if (shape.leftProfiles != null) {
                    if (UnityEngine.Random.Range(0, 99) < probability) {
                        float factor = UnityEngine.Random.Range(0.8f, 1.2f);
                        for (int i = 0; i < shape.leftProfiles.Length; i++) {
                            shape.leftProfiles[i].baseWidth *= factor;
                            shape.leftProfiles[i].topWidth *= factor;

                        }

                        if (shape.rightProfiles != null) {
                            for (int i = 0; i < shape.rightProfiles.Length; i++) {
                                shape.rightProfiles[i].baseWidth *= factor;
                                shape.rightProfiles[i].topWidth *= factor;

                            }

                        }

                        foreach (var cross in shape.crossSections) {
                            if (cross.trueEdges != null) {
                                for (int i = 0; i < cross.trueEdges.Length; i++) {
                                    cross.trueEdges[i].x *= factor;

                                }

                            }

                            if (cross.falseEdges != null) {
                                for (int i = 0; i < cross.falseEdges.Length; i++) {
                                    cross.falseEdges[i].x *= factor;

                                }

                            }

                        }

                    }

                }

            }

            shape.totalLength = CalcProfileTotalLength(ref shape);

            CalcCrossSectionThickness(ref shape.crossSections, out shape.baseThickness, out shape.tipThickness);

        }

        float CalcProfileTotalLength(ref SwordDataStructs.Shape shape) {
            float leftTotalLength = SwordDataStructsUtil.CalculateProfileLength(ref shape.leftProfiles);
            float rightTotalLength = SwordDataStructsUtil.CalculateProfileLength(ref shape.rightProfiles);

            // Return longer total length
            return leftTotalLength > rightTotalLength ? leftTotalLength : rightTotalLength;

        }

        void CalcCrossSectionThickness(ref SwordDataStructs.Shape.CrossSections[] crossSections, out float baseThickness, out float tipThickness) {
            baseThickness = 0;
            tipThickness = 0;

            // Cross Sections
            if (crossSections != null) {
                float trueEdgeThickness = 0;
                float falseEdgeThickness = 0;

                // First True Edge
                if (crossSections.First().trueEdges != null) {
                    trueEdgeThickness =
                    crossSections.First().trueEdges.First().left.baseThickness // First cross section - first true edge - left of edge - base thickness
                        + crossSections.First().trueEdges.First().right.baseThickness; // First cross section - first true edge - right of edge - base thickness

                }

                // First False Edge
                if (crossSections.First().falseEdges != null) {
                    falseEdgeThickness =
                    crossSections.First().falseEdges.First().left.baseThickness // First cross section - first false edge - left of edge - base thickness
                        + crossSections.First().falseEdges.First().right.baseThickness; // First cross section - first false edge - right of edge - base thickness

                }

                // Base Thickness
                baseThickness = trueEdgeThickness > falseEdgeThickness ? trueEdgeThickness : falseEdgeThickness;

                // Last True Edge
                if (crossSections.First().trueEdges != null) {
                    trueEdgeThickness =
                    crossSections.Last().trueEdges.First().left.baseThickness // Last cross section - first true edge - left of edge - base thickness
                        + crossSections.Last().trueEdges.First().right.baseThickness; // Last cross section - first true edge - right of edge - base thickness


                }

                // Last False Edge
                if (crossSections.First().falseEdges != null) {
                    falseEdgeThickness =
                    crossSections.Last().falseEdges.First().left.baseThickness // Last cross section - first false edge - left of edge - base thickness
                        + crossSections.Last().falseEdges.First().right.baseThickness; // Last cross section - first false edge - right of edge - base thickness

                }

                // Tip Thickness
                tipThickness = trueEdgeThickness > falseEdgeThickness ? trueEdgeThickness : falseEdgeThickness;

            }

        }

        SwordDataStructs.Blade CreateBlade(int bladeId, int tangId, int[] fullerTopFaceIds, float[] fullerTopFaceCentreX, float[] fullerTopFaceStartY, int[] fullerBottomFaceIds, float[] fullerBottomFaceCentreX, float[] fullerBottomFaceStartY, string name) {
            SwordDataStructs.Blade blade = new SwordDataStructs.Blade();

            blade.name = name + " Blade";

            if (bladeId != -1) {
                CSVDataStructs.Blade csvBlade = Array.Find(allCSVData.blades, blade => blade.id == bladeId);

                // Blade Shape
                blade.shape = csvBlade.shape;
                CreateShape(ref blade.shape);

                // Blade Weight
                blade.bladeWeight = csvBlade.weight;

                // Blade Material
                blade.bladeMaterial = csvBlade.material;

                // Tang
                blade.tang = CreateTang(tangId);

                // Fullers
                blade.topFaceFullers = CreateFullers(fullerTopFaceIds, fullerTopFaceCentreX, fullerTopFaceStartY);
                blade.bottomFaceFullers = CreateFullers(fullerBottomFaceIds, fullerBottomFaceCentreX, fullerBottomFaceStartY);


                // Total Length
                blade.totalLength = blade.shape.totalLength + blade.tang.shape.totalLength;

                // Total Weight
                blade.totalWeight = blade.bladeWeight + blade.tang.weight;

            }

            return blade;

        }

        SwordDataStructs.Tang CreateTang(int id) {
            SwordDataStructs.Tang tang = new SwordDataStructs.Tang();

            if (id != -1) {
                CSVDataStructs.Tang csvTang = Array.Find(allCSVData.tangs, tang => tang.id == id);

                // Tang Shape
                tang.shape = csvTang.shape;
                CreateShape(ref tang.shape);

                // Tang Weight
                tang.weight = csvTang.weight;

                // Tang Material
                tang.material = csvTang.material;

            }

            return tang;

        }

        SwordDataStructs.Fuller[] CreateFullers(int[] ids, float[] centreX, float[] startY) {
            SwordDataStructs.Fuller[] fullers = new SwordDataStructs.Fuller[ids.Length];

            CSVDataStructs.Fuller[] csvFullers = new CSVDataStructs.Fuller[ids.Length];

            // For each fuller in csvFullers
            for (int i = 0; i < csvFullers.Length; i++) {
                if (ids[i] != -1) {
                    csvFullers[i] = Array.Find(allCSVData.fullers, fuller => fuller.id == ids[i]);

                    SwordDataStructs.Fuller fuller = new SwordDataStructs.Fuller();

                    CSVDataStructs.Fuller csvFuller = csvFullers[i];

                    fuller.centreX = centreX[i];
                    fuller.startY = startY[i];

                    fuller.leftSide = CreateFullerSides(ref csvFuller, ref csvFuller.leftX, ref csvFuller.leftDepth);
                    fuller.rightSide = CreateFullerSides(ref csvFuller, ref csvFuller.rightX, ref csvFuller.rightDepth);

                    fullers[i] = fuller;

                }

            }

            return fullers;

        }

        SwordDataStructs.Fuller.FullerSide[] CreateFullerSides(ref CSVDataStructs.Fuller fuller, ref float[] x, ref float[] depth) {
            if (x != null) {
                SwordDataStructs.Fuller.FullerSide[] side = new SwordDataStructs.Fuller.FullerSide[x.Length];

                for (int i = 0; i < side.Length; i++) {
                    side[i].y = fuller.y[i];

                    side[i].x = x[i];
                    side[i].depth = i < depth.Length ? depth[i] : 0;

                }

                return side;

            } else {
                Debug.Log("Im null");

                return null;

            }

        }

        SwordDataStructs.Guard[] CreateGuards(int[] ids, string name) {
            SwordDataStructs.Guard[] guards = new SwordDataStructs.Guard[ids.Length];

            CSVDataStructs.Guard[] csvGuards = new CSVDataStructs.Guard[ids.Length];


            for (int i = 0; i < ids.Length; i++) {
                SwordDataStructs.Guard guard = new SwordDataStructs.Guard();

                if (ids[i] != -1) {
                    csvGuards[i] = Array.Find(allCSVData.guards, guard => guard.id == ids[i]);
                    guard = CreateGuard(csvGuards[i], name);

                }

                guards[i] = guard;

            }

            return guards;

        }

        SwordDataStructs.Guard CreateGuard(CSVDataStructs.Guard guardToMake, string name) {
            SwordDataStructs.Guard guard = new SwordDataStructs.Guard();

            guard.name = name + " Guard";

            // Guard Shape
            guard.shape = guardToMake.shape;
            CreateShape(ref guard.shape);

            // Guard Weight
            guard.weight = guardToMake.weight;

            // Guard Material
            guard.material = guardToMake.material;

            return guard;

        }

        SwordDataStructs.Grip CreateGrip(int id, string name) {
            SwordDataStructs.Grip grip = new SwordDataStructs.Grip();

            grip.name = name + " Grip";

            if (id != -1) {
                CSVDataStructs.Grip csvGrip = Array.Find(allCSVData.grips, grip => grip.id == id);

                // Guard Shape
                grip.shape = csvGrip.shape;
                CreateShape(ref grip.shape);

                grip.weight = csvGrip.weight;
                grip.material = csvGrip.material;

            }

            return grip;

        }

        SwordDataStructs.Pommel CreatePommel(int id, string name) {
            SwordDataStructs.Pommel pommel = new SwordDataStructs.Pommel();

            pommel.name = name + " Pommel";

            if (id != -1) {
                CSVDataStructs.Pommel csvPommel = Array.Find(allCSVData.pommels, pommel => pommel.id == id);

                // Guard Shape
                pommel.shape = csvPommel.shape;
                CreateShape(ref pommel.shape);

                pommel.weight = csvPommel.weight;
                pommel.material = csvPommel.material;

            }

            return pommel;

        }

    }

}