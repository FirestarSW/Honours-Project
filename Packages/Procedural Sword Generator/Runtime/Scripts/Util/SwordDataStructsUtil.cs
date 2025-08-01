using ProceduralSwordGenerator.SwordDataStructs;
using System.Linq;
using UnityEngine;

namespace ProceduralSwordGenerator {
    public class SwordDataStructsUtil : MonoBehaviour {
        static public float CalculateProfileLength(ref SwordDataStructs.Shape.Profile[] profiles) {
            // Loops over profiles and adds their lengths together
            float length = 0;
            if (profiles != null) {
                foreach (var profile in profiles) {
                    length += profile.length;

                }

            }

            return length;

        }

        static public float[] CalculateProfileYChanges(ref SwordDataStructs.Shape.Profile[] profiles) {
            // Contains every y change in profiles
            float[] profileYChanges = new float[profiles.Length + 1];

            // For each profile, add y / total length
            float totalLength = 0;
            for (int i = 0; i < profileYChanges.Length - 1; i++) {
                profileYChanges[i] = totalLength;
                totalLength += profiles[i].length;
            }

            profileYChanges[profileYChanges.Length - 1] = totalLength;

            return profileYChanges;

        }

        static public float[] CalculateCrossSectionYChanges(ref SwordDataStructs.Shape.CrossSections[] crossSections) {
            // Contians every y change in cross sections
            float[] crossYChanges = new float[crossSections.Length + 1];

            // For each cross, add y
            for (int i = 0; i < crossSections.Length; i++) {
                crossYChanges[i] = crossSections[i].y;

            }

            return crossYChanges;

        }

        // Returns profile which y is in range of, if not found, then return last profile. If no profiles returns blank profile
        // out bool to check if profile is found, could be better to use int 0-2 to represent 3 above returns
        static public SwordDataStructs.Shape.Profile FindCurrentProfile(float y, ref SwordDataStructs.Shape.Profile[] profiles, out bool profileFound, out float profileActualY) {
            if (profiles != null) {
                float totalY = 0;
                // Loops over profiles
                foreach (var profile in profiles) {
                    // If current Y is greater than totalY (profile start y) and is less than totalY + profile length (profile end y)
                    if (y >= totalY && y < totalY + profile.length) { // This might need changed to be same as find cross
                        // Profile found and return profile
                        profileFound = true;

                        profileActualY = totalY;

                        return profile;

                        //  Else add profile length to total Y
                    } else {
                        totalY += profile.length;

                    }

                }

                // If profiles isnt empty, return last profile
                if (profiles.Length > 0) {
                    // If program reaches here, profile was not found but last profile was returned
                    profileFound = true;

                    profileActualY = totalY;

                    return profiles.Last();

                }

            }

            // If program reaches here, profile was not found, return blank profile
            profileFound = false;

            profileActualY = 0;

            // If profiles is null or empty, return blank
            return new SwordDataStructs.Shape.Profile();

        }

        // Returns cross section which y is in range of, if not found, then return last cross section. If no cross sections returns blank cross section
        // out bool to check if cross is found, could be better to use int 0-2 to represent 3 above returns
        static public SwordDataStructs.Shape.CrossSections FindCurrentCrossSection(float y, ref SwordDataStructs.Shape.CrossSections[] crossSections, out bool crossFound, out float crossActualY) {
            if (crossSections != null) {
                float totalY = 0;

                // Loops over all but the last cross section
                for (int i = 0; i < crossSections.Length - 1; i++) {
                    // If current y is less than next sections actual y and current y is greater or equal to totaly
                    if (y < totalY + crossSections[i + 1].y && y >= totalY) {
                        // Cross found and return cross
                        crossFound = true;
                        crossActualY = totalY;
                        return crossSections[i];

                        // Else add next cross y to total y
                    } else {
                        totalY += crossSections[i + 1].y;

                    }

                }

                // If program reaches here, cross was not found or there is only one cross

                // If cross sections is not empty - return last cross
                if (crossSections.Length > 0) {
                    crossFound = true;

                    crossActualY = totalY;

                    return crossSections.Last();

                }

            }

            // If program reaches here, cross was not found, return blank cross section
            crossFound = false;

            crossActualY = 0;

            // If Cross sections is null or empty, return blank
            return new SwordDataStructs.Shape.CrossSections();

        }

        // Returns cross section after which y is in range of, if not found, then return last cross section. If no cross sections returns blank cross section
        // out bool to check if cross is found, could be better to use int 0-2 to represent 3 above returns
        static public SwordDataStructs.Shape.CrossSections FindNextCrossSection(float y, ref SwordDataStructs.Shape.CrossSections[] crossSections, out bool crossFound, out float crossActualY) {

            if (crossSections != null) {
                float totalY = 0;
                // Loops over cross sections
                foreach (var cross in crossSections) {
                    // If current Y is greater than totalY (cross start y) and is less than totalY + cross y (cross end y) (or y = 0 for first one)
                    if (y >= totalY && y < totalY + cross.y) {
                        // Cross found and return cross
                        crossFound = true;

                        crossActualY = totalY + cross.y;

                        return cross;

                        //  Else add cross Y to total Y
                    } else {
                        totalY += cross.y;

                    }

                }

                // If cross sections isnt empty, return last cross section
                if (crossSections.Length > 0) {
                    // If program reaches here, cross was not found but last cross section was returned
                    crossFound = true;

                    crossActualY = totalY;

                    return crossSections.Last();

                }

            }

            // If program reaches here, cross was not found, return blank cross section
            crossFound = false;

            crossActualY = 0;

            // If Cross sections is null or empty, return blank
            return new SwordDataStructs.Shape.CrossSections();

        }

        static public float CalculateCrossSectionWidth(ref SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] edges) {
            // Loops over edges and adds their widths together
            float width = 0;
            if (edges != null) {
                foreach (var edge in edges) {
                    width += edge.x;

                }

            }

            return width;

        }

        public static float CalculateCrossSectionDepth(float x, ref SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] edges, bool edgeTop) {
            if (edges != null) {
                //x = Mathf.Abs(x);

                float totalX = 0;
                // Loops over edges
                foreach (var edge in edges) {
                    // If current X is greater than totalX (cross start x) and is less than totalX + cross width (cross end x)
                    if (x >= totalX && x < totalX + edge.x) {
                        // Calculates depth at x - totalX between start and end of edge
                        return MathsUtil.CalculateYOnLinearLine(x - totalX, new Vector2(0, edgeTop ? edge.right.baseThickness : edge.left.baseThickness), new Vector2(edge.x, edgeTop ? edge.right.topThickness : edge.left.topThickness));

                    } else {
                        totalX += edge.x;

                    }

                }

                // If program reaches here, edge was not found, return last thickness
                if (edges.Length > 0) {
                    return edgeTop ? edges.Last().right.topThickness : edges.Last().left.topThickness;

                }

            }

            return 0;

        }

        public static float CalculateCrossSectionDepthWithFactor(float widthFactor, ref SwordDataStructs.Shape.CrossSections.EdgeCrossSection[] edges, bool edgeTop) {
            // Returns the Depth for current cross section at cross section width * widthFactor (x)
            return CalculateCrossSectionDepth(CalculateCrossSectionWidth(ref edges) * widthFactor, ref edges, edgeTop);

        }

    }

}
