using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Xml;
using UnityEngine.UIElements;
using ProceduralSwordGenerator.SwordDataStructs;
using static UnityEditor.VersionControl.Asset;

namespace ProceduralSwordGenerator.Editor {
    public class ReadCSV {

        private static string scriptableObjectsFolderPath = "Packages/firestarw.procedural_sword_generator/Runtime/Scriptable Objects/";

        private static string csvFolderPath = "Packages/firestarw.procedural_sword_generator/Editor/CSV/";

        private static string csvSwordsPath = "Swords/";
        private static string csvBladesPath = "Blades/";
        private static string csvTangsPath = "Tangs/";
        private static string csvFullersPath = "Fullers/";
        private static string csvGuardsPath = "Guards/";
        private static string csvGripsPath = "Grips/";
        private static string csvPommelsPath = "Pommels/";

        static string[] ReadLinesFromCSV(string path) {
            // Temporary array to return
            string[] lines = new string[0];

            // Gets all CSV files in CSV Folder
            FileInfo[] files = new DirectoryInfo(path).GetFiles("*.csv");
            foreach (FileInfo file in files) {
                // Temporary array to store lines from file
                string[] fileLines = File.ReadAllLines(file.FullName);

                // Combines array to return with lines from file
                lines = lines.Concat(fileLines).ToArray();

            }

            return lines;

        }

        [MenuItem("Procedural Sword Generator/Generate Sword Data")]
        public static void GenerateSwordData() {
            // Creates AllCSVData Scriptable Object
            AllCSVData allCSVData = ScriptableObject.CreateInstance<AllCSVData>();

            // Read CSV's and set in SO
            allCSVData.swords = ReadSwordsCSV(csvFolderPath + csvSwordsPath);
            allCSVData.blades = ReadBladesCSV(csvFolderPath + csvBladesPath);
            allCSVData.tangs = ReadTangsCSV(csvFolderPath + csvTangsPath);
            allCSVData.fullers = ReadFullersCSV(csvFolderPath + csvFullersPath);
            allCSVData.guards = ReadGuardsCSV(csvFolderPath + csvGuardsPath);
            allCSVData.grips = ReadGripsCSV(csvFolderPath + csvGripsPath);
            allCSVData.pommels = ReadPommelsCSV(csvFolderPath + csvPommelsPath);

            // TODO - duplicate id check

            // Creates allSwordData as an asset and saves
            AssetDatabase.CreateAsset(allCSVData, scriptableObjectsFolderPath + "AllCSVData.asset");
            AssetDatabase.SaveAssets();

        }

        private static CSVDataStructs.Sword[] ReadSwordsCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Sword> dataList = new List<CSVDataStructs.Sword>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Sword dataStruct = new CSVDataStructs.Sword();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " ID read failed");
                    continue;

                }

                dataIndex++;

                // Type
                dataStruct.type = data[dataIndex];
                dataIndex++;

                // Blade Id
                if (!int.TryParse(data[dataIndex], out dataStruct.bladeId)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.bladeId)) {
                        //isValid = false;
                        Debug.Log(dataStruct.id + " Blade ID read failed");

                    }

                }

                dataIndex++;

                // Tang Id
                if (!int.TryParse(data[dataIndex], out dataStruct.tangId)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.tangId)) {
                        //isValid = false;
                        Debug.Log(dataStruct.id + " Tang ID read failed");

                    }

                }

                dataIndex++;

                // Fuller TF Id
                if (!StringToIntArray(data[dataIndex].Split(';'), out dataStruct.fullerTopFaceIds)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller TF ID read failed");                  

                }

                dataIndex++;

                // Fuller TF Centre X
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.fullerTopFaceCentreX)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller TF Centre X read failed");

                }

                dataIndex++;

                // Fuller TF Start Y
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.fullerTopFaceStartY)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller TF Start Y read failed");

                }

                dataIndex++;

                // Fuller BF Id
                if (!StringToIntArray(data[dataIndex].Split(';'), out dataStruct.fullerBottomFaceIds)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller BF ID read failed");

                }

                dataIndex++;

                // Fuller BF Centre X
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.fullerBottomFaceCentreX)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller BF Centre X read failed");

                }

                dataIndex++;

                // Fuller BF Start Y
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.fullerBottomFaceStartY)) {
                    //isValid = false;
                    Debug.Log(dataStruct.id + " Fuller BF Start Y read failed");

                }

                dataIndex++;

                // Guard Ids
                string[] guardIds = data[dataIndex].Split(';');
                if (guardIds.Length > 0) {
                    // True Guard Id
                    if (!StringToIntArray(guardIds[0].Split(' '), out dataStruct.trueGuardIds)) {
                        //isValid = false;
                        Debug.Log(dataStruct.id + " True Guard ID read failed");


                    }

                    if (guardIds.Length > 1) {
                        // False Guard Id
                        if (!StringToIntArray(guardIds[1].Split(';'), out dataStruct.falseGuardIds)) {
                            //isValid = false;
                            Debug.Log(dataStruct.id + " False Guard ID read failed");


                        }

                    }

                }


                dataIndex++;

                // Grip Id
                if (!int.TryParse(data[dataIndex], out dataStruct.gripId)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.gripId)) {
                        //isValid = false;
                        Debug.Log(dataStruct.id + " Grip ID read failed");

                    }

                }

                dataIndex++;

                // Pommel Id
                if (!int.TryParse(data[dataIndex], out dataStruct.pommelId)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.pommelId)) {
                        //isValid = false;
                        Debug.Log(dataStruct.id + " Pommel ID read failed");

                    }

                }

                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static bool ReadShapeData(ref int id, ref string[] data, ref int startIndex, out SwordDataStructs.Shape shape) {
            bool isValid = true;

            shape = new SwordDataStructs.Shape();

            // Left Profiles
            if (!ReadProfilesData(ref id, ref data, startIndex, out shape.leftProfiles, out _)) {
                isValid = false;
                Debug.Log(id + " Left Profiles read failed");

            }

            startIndex++;

            // Right Profiles
            if (!ReadProfilesData(ref id, ref data, startIndex, out shape.rightProfiles, out startIndex)) {
                isValid = false;
                Debug.Log(id + " Right Profiles read failed");

            }

            // Cross Sections
            if (!ReadCrossSectionData(ref id, ref data, ref startIndex, out shape.crossSections)) {
                isValid = false;
                Debug.Log(id + " Cross Section read failed");

            }

            return isValid;

        }

        private static bool ReadProfilesData(ref int id, ref string[] data, int startIndex, out SwordDataStructs.Shape.Profile[] profileDatas, out int dataIndex) {
            bool isValid = true;

            float[] x, lengths, widths;

            // Read x
            // Modified StringToFloatArray that if blank sets to 0 as default x is 0
            string[] stringArray = data[startIndex].Split(';');
            x = new float[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++) {
                if (!float.TryParse(stringArray[i], out x[i])) {
                    if (!IfBlank(ref stringArray[i], out x[i])) x[i] = 0;

                }

            }

            startIndex += 2;

            // Read Lengths
            if (!StringToFloatArray(data[startIndex].Split(';'), out lengths)) {
                isValid = false;
                Debug.Log(id + " Profile Length read failed");

            }

            startIndex += 2;

            // Read Widths
            if (!StringToFloatArray(data[startIndex].Split(';'), out widths)) {
                isValid = false;
                Debug.Log(id + " Profile Width read failed");

            }

            startIndex += 2;

            // Shapes
            string[] shapes = data[startIndex].Split(';');

            startIndex++;

            // Create Profile Array
            profileDatas = new SwordDataStructs.Shape.Profile[lengths.Length];

            // Widths should be one element longer than Lengths
            if (lengths.Length == widths.Length - 1) {
                // Set Blade Profile's Length
                for (int i = 0; i < lengths.Length; i++) {
                    profileDatas[i].length = lengths[i];

                }

                // Set Profile's Base and Top Widths
                for (int i = 0; i < widths.Length - 1; i++) {
                    profileDatas[i].baseWidth = widths[i];
                    profileDatas[i].topWidth = i + 1 < widths.Length ? widths[i + 1] : widths[i];

                }

            } else {
                isValid = false;
                Debug.Log(id + " Profile Lengths or Widths array wrong size");

            }

            // X should be at most, 1 longer than lengths
            if (x.Length <= lengths.Length + 1) {
                // Set Profile's Base and Top X
                for (int i = 0; i < x.Length - 1; i++) {
                    profileDatas[i].baseX = x[i];
                    profileDatas[i].topX = x[i + 1] < x.Length ? x[i + 1] : x[i];

                }

                // Set Profile's shape - defaults to "N" if blank or shapes is shorter than number of profiles
                for (int i = 0; i < lengths.Length; i++) {
                    profileDatas[i].shape = i < shapes.Length ? (shapes[i] != "" ? shapes[i] : "N") : "N";

                }

            }

            dataIndex = startIndex;

            return isValid;

        }

        private static bool ReadCrossSectionData(ref int id, ref string[] data, ref int dataIndex, out SwordDataStructs.Shape.CrossSections[] crossSectionsData) {
            bool isValid = true;

            float[] y;

            // Read Y
            if (!StringToFloatArray(data[dataIndex].Split(';'), out y)) {
                isValid = false;
                Debug.Log(id + " Cross Section Y read failed");

            }

            dataIndex++;

            string[] trueX, falseX, trueLeftThicknesses, trueRightThicknesses, falseLeftThicknesses, falseRightThicknesses,
                trueLeftShapes, trueRightShapes, falseLeftShapes, falseRightShapes;

            // Read True Edge X
            trueX = data[dataIndex].Split(';');
            dataIndex++;

            // Read False Edge X
            falseX = data[dataIndex].Split(';');
            dataIndex++;

            // Read True Edge Left Thickness
            trueLeftThicknesses = data[dataIndex].Split(';');
            dataIndex++;

            // Read True Edge Right Thickness
            trueRightThicknesses = data[dataIndex].Split(';');
            dataIndex++;

            // Read False Edge Left Thickness
            falseLeftThicknesses = data[dataIndex].Split(';');
            dataIndex++;

            // Read False Edge Right Thickness
            falseRightThicknesses = data[dataIndex].Split(';');
            dataIndex++;

            // Read True Edge Left Shapes
            trueLeftShapes = data[dataIndex].Split(';');
            dataIndex++;

            // Read True Edge Right Shapes
            trueRightShapes = data[dataIndex].Split(';');
            dataIndex++;

            // Read False Edge Left Shapes
            falseLeftShapes = data[dataIndex].Split(';');
            dataIndex++;

            // Read False Edge Right Shapes
            falseRightShapes = data[dataIndex].Split(';');
            dataIndex++;

            // Create Cross Sections Array
            crossSectionsData = new SwordDataStructs.Shape.CrossSections[y.Length];

            // Set Cross Sections Array
            for (int i = 0; i < y.Length; i++) {
                crossSectionsData[i].y = y[i];

                ReadEdgeCrossSectionData(ref id, ref trueX[i], ref trueLeftThicknesses[i], ref trueRightThicknesses[i],
                    i < trueLeftShapes.Length ? (trueLeftShapes[i] != "" ? trueLeftShapes[i] : "N") : "N",
                    i < trueRightShapes.Length ? (trueRightShapes[i] != "" ? trueRightShapes[i] : "N") : "N", out crossSectionsData[i].trueEdges);

                ReadEdgeCrossSectionData(ref id, ref falseX[i], ref falseLeftThicknesses[i], ref falseRightThicknesses[i],
                    i < falseLeftShapes.Length ? (falseLeftShapes[i] != "" ? falseLeftShapes[i] : "N") : "N",
                    i < falseRightShapes.Length ? (falseRightShapes[i] != "" ? falseRightShapes[i] : "N") : "N", out crossSectionsData[i].falseEdges);

            }

            return isValid;

        }

        private static bool ReadEdgeCrossSectionData(ref int id, ref string x, ref string leftThicknesses, ref string rightThicknesses, string leftShape, string rightShape, out SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] edgeCrossSections) {
            bool isValid = true;

            float[] splitX, splitLeftThickness, splitRightThickness;

            // Read X
            if (!StringToFloatArray(x.Split(' '), out splitX)) {
                isValid = false;
                Debug.Log(id + " Edge Cross Section X Split read failed");

            }

            // Read Left Thicknesses
            if (!StringToFloatArray(leftThicknesses.Split(' '), out splitLeftThickness)) {
                isValid = false;
                Debug.Log(id + " Edge Cross Section Left Thicknesses Split read failed");

            }

            // Read Right Thicknesses
            if (!StringToFloatArray(rightThicknesses.Split(' '), out splitRightThickness)) {
                isValid = false;
                Debug.Log(id + " Edge Cross Section Right Thicknesses Split read failed");

            }

            // Create Edge Cross Sections Array
            edgeCrossSections = new SwordDataStructs.Shape.CrossSections.EdgeCrossSection[splitX.Length];

            // Fill Array
            for (int i = 0; i < splitX.Length; i++) {
                edgeCrossSections[i].x = splitX[i];

                // Left Thicknesses
                ReadCrossSectionThicknessData(i, ref splitLeftThickness, ref leftShape, out edgeCrossSections[i].left);

                // Right Thicknesses
                ReadCrossSectionThicknessData(i, ref splitRightThickness, ref rightShape, out edgeCrossSections[i].right);

            }

            return isValid;

        }

        private static void ReadCrossSectionThicknessData(int index, ref float[] thicknesses, ref string shape, out SwordDataStructs.Shape.CrossSections.CrossSectionThickness thicknessDataStruct) {
            thicknessDataStruct = new Shape.CrossSections.CrossSectionThickness();
            
            thicknessDataStruct.baseThickness = thicknesses[index];
            thicknessDataStruct.topThickness = index + 1 < thicknesses.Length ? thicknesses[index + 1] : thicknesses[index];

            thicknessDataStruct.shape = shape;

        }


        private static CSVDataStructs.Blade[] ReadBladesCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Blade> dataList = new List<CSVDataStructs.Blade>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Blade dataStruct = new CSVDataStructs.Blade();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Blade ID read failed");
                    continue;

                }

                dataIndex++;

                // Shape
                if (!ReadShapeData(ref dataStruct.id, ref data, ref dataIndex, out dataStruct.shape)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Blade Shape read failed");

                }

                // Weight
                if (!float.TryParse(data[dataIndex], out dataStruct.weight)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.weight)) {
                        isValid = false;
                        Debug.Log(dataStruct.id + " Guard Weight read failed");

                    }

                }

                dataIndex++;

                // Material
                dataStruct.material = data[dataIndex];
                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static CSVDataStructs.Tang[] ReadTangsCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Tang> dataList = new List<CSVDataStructs.Tang>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Tang dataStruct = new CSVDataStructs.Tang();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Tang ID read failed");
                    continue;

                }

                dataIndex++;

                // Shape
                if (!ReadShapeData(ref dataStruct.id, ref data, ref dataIndex, out dataStruct.shape)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Tang Shape read failed");

                }

                // Weight
                if (!float.TryParse(data[dataIndex], out dataStruct.weight)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.weight)) {
                        isValid = false;
                        Debug.Log(dataStruct.id + " Tang Weight read failed");

                    }

                }

                dataIndex++;

                // Material
                dataStruct.material = data[dataIndex];
                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static CSVDataStructs.Fuller[] ReadFullersCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Fuller> dataList = new List<CSVDataStructs.Fuller>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Fuller dataStruct = new CSVDataStructs.Fuller();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Fuller ID read failed");
                    continue;

                }

                dataIndex++;

                // Y
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.y)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Fuller Y read failed");

                }

                dataIndex++;

                // Left X
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.leftX)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Fuller Left X read failed");

                }

                dataIndex++;

                // Right X
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.rightX)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Fuller Right X read failed");

                }

                dataIndex++;

                // Left Depth
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.leftDepth)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Fuller Left Depth read failed");

                }

                dataIndex++;

                // Right Depth
                if (!StringToFloatArray(data[dataIndex].Split(';'), out dataStruct.rightDepth)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Fuller Right Depth read failed");

                }

                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static CSVDataStructs.Guard[] ReadGuardsCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Guard> dataList = new List<CSVDataStructs.Guard>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Guard dataStruct = new CSVDataStructs.Guard();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Guard ID read failed");
                    continue;

                }

                dataIndex++;

                // Shape
                if (!ReadShapeData(ref dataStruct.id, ref data, ref dataIndex, out dataStruct.shape)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Guard Shape read failed");

                }

                // Weight
                if (!float.TryParse(data[dataIndex], out dataStruct.weight)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.weight)) {
                        isValid = false;
                        Debug.Log(dataStruct.id + " Guard Weight read failed");

                    }

                }

                dataIndex++;

                // Material
                dataStruct.material = data[dataIndex];
                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static CSVDataStructs.Grip[] ReadGripsCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Grip> dataList = new List<CSVDataStructs.Grip>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Grip dataStruct = new CSVDataStructs.Grip();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Grip ID read failed");
                    continue;

                }

                dataIndex++;

                // Shape
                if (!ReadShapeData(ref dataStruct.id, ref data, ref dataIndex, out dataStruct.shape)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Grip Shape read failed");

                }

                // Weight
                if (!float.TryParse(data[dataIndex], out dataStruct.weight)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.weight)) {
                        isValid = false;
                        Debug.Log(dataStruct.id + " Grip Weight read failed");

                    }

                }

                dataIndex++;

                // Material
                dataStruct.material = data[dataIndex];
                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static CSVDataStructs.Pommel[] ReadPommelsCSV(string csvPath) {
            string[] lines = ReadLinesFromCSV(csvPath);

            // Temporary List to store processed data while validating
            List<CSVDataStructs.Pommel> dataList = new List<CSVDataStructs.Pommel>();

            // Reads data from each line into data list
            bool isValid;
            foreach (string line in lines) {
                isValid = true;

                // Splits line into data
                string[] data = line.Split(',');
                int dataIndex = 0;

                // Creates new data struct and trys to fill
                CSVDataStructs.Pommel dataStruct = new CSVDataStructs.Pommel();

                // Name
                dataStruct.name = data[dataIndex];
                dataIndex++;

                // Id
                if (!int.TryParse(data[dataIndex], out dataStruct.id)) {
                    isValid = false;
                    Debug.Log(dataStruct.name + " Pommel ID read failed");
                    continue;

                }

                dataIndex++;

                // Shape
                if (!ReadShapeData(ref dataStruct.id, ref data, ref dataIndex, out dataStruct.shape)) {
                    isValid = false;
                    Debug.Log(dataStruct.id + " Pommel Shape read failed");

                }

                // Weight
                if (!float.TryParse(data[dataIndex], out dataStruct.weight)) {
                    if (!IfBlank(ref data[dataIndex], out dataStruct.weight)) {
                        isValid = false;
                        Debug.Log(dataStruct.id + " Pommel Weight read failed");

                    }

                }

                dataIndex++;

                // Material
                dataStruct.material = data[dataIndex];
                dataIndex++;

                // Validation Check
                if (isValid) {
                    dataList.Add(dataStruct);

                }

            }

            return dataList.ToArray();

        }

        private static bool StringToIntArray(string[] stringArray, out int[] intArray) {
            intArray = new int[stringArray.Length];
            bool isValid = true;

            for (int i = 0; i < stringArray.Length; i++) {
                if (!int.TryParse(stringArray[i], out intArray[i])) {
                    if (!IfBlank(ref stringArray[i], out intArray[i])) isValid = false;

                }

            }

            return isValid;

        }

        private static bool StringToFloatArray(string[] stringArray, out float[] floatArray) {
            floatArray = new float[stringArray.Length];
            bool isValid = true;

            for (int i = 0; i < stringArray.Length; i++) {
                if (!float.TryParse(stringArray[i], out floatArray[i])) {
                    if (!IfBlank(ref stringArray[i], out floatArray[i])) isValid = false;

                }

            }

            return isValid;

        }

        // Set out to -1 and return if data is blank
        private static bool IfBlank(ref string data, out int blank) {
            blank = -1;

            return data == "";

        }

        private static bool IfBlank(ref string data, out float blank) {
            blank = -1;

            return data == "";

        }

    }

}
