using UnityEngine;
using UnityEditor;
using System;

public class FindMissingComponents : EditorWindow
{
        //To find missing Components in Scene or prefab
        //How to use: select the prefab or Gameobjects in Scene which you want to detected whether there are some missing Components
        //Result: print how many Gameobjects/components have been detected and how many missing components found
        //Also print the path of missing component like "GameConsistantData has an empty Component attached in position: 4"
        static int go_count = 0, components_count = 0, missing_count = 0;
[MenuItem("Assets/Tool/FindMissingComponents")]
private static void FindMissingComponentInAllSeletedGO()
        {
                GameObject[] toDetectedGameObjects = Selection.gameObjects;
go_count = 0;
components_count = 0;
missing_count = 0;
foreach (GameObject g in toDetectedGameObjects)
                {
                        detectGameObject(g);
                }
                Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
        private static void detectGameObject(GameObject detectedGO)
        {
                go_count++;
Component[] components = detectedGO.GetComponents<Component>();
for (int index = 0; index < components.Length; index++)
                {
                        components_count++;
if (components[index] == null)//Missing Component
                        {
                                missing_count++;
string missingPath = detectedGO.name;
Transform t = detectedGO.transform;
while (t.parent != null)
                                {
                                        missingPath = t.parent.name + "/" + missingPath;
t = t.parent;
                                }
                                Debug.Log(missingPath + " has an empty Component attached in position: " + index, detectedGO);
                        }
                }
                foreach (Transform childT in detectedGO.transform) //Find in child of detectedGameObject
                {
                        detectGameObject(childT.gameObject);
                }
        }
}
