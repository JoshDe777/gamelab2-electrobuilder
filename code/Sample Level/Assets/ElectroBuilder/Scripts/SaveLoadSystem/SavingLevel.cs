using UnityEditor;
using UnityEngine;
using TMPro;
using System;

namespace ElectroBuilder
{
    namespace SaveLoadSystem
    {
        public class SavingLevel : MonoBehaviour
        {
            private GameObject levelScene;

            private SaveLoadEventHandler saveLoadEvents;

            [SerializeField]
            GameObject saveLoadManager;

            [SerializeField] private TMP_InputField textFieldInput;
            private string inputLevelName => textFieldInput.text;


            private void Start()
            {
                textFieldInput.text = GameObject.FindGameObjectWithTag("LevelLayoutObject").name;

                saveLoadEvents = FindObjectOfType<SaveLoadEventHandler>();
            }

            void Update()
            {
                //Check for new levelScene Objekt after Save/Start
                if (levelScene == null)
                {
                    levelScene = GameObject.FindGameObjectWithTag("LevelLayoutObject");
                    textFieldInput.text = levelScene.name;
                }
            }

            #region SaveLogic
            //Generate unique Path and Save the selected Objekt
            private void saveSceneObjekt()
            {
                string localPath = "Assets/Resources/Level/" + inputLevelName + ".prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                PrefabUtility.SaveAsPrefabAsset(levelScene, localPath);
            }


            //Changes all the Materials of the Child Objects of the Layout, by going through each one and changing the Material to an non (Instance) version from it
            private void prepareMaterialsForSave()
            {
                foreach (Transform Child in levelScene.transform)
                {
                    if (Child.GetComponent<MeshRenderer>() != null)
                    {
                        Child.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + Child.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", ""));
                    }

                    if (Child.childCount > 0)
                    {
                        foreach (Transform deeperChild in Child)
                        {
                            if (deeperChild.GetComponent<MeshRenderer>() != null)
                            {
                                deeperChild.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + deeperChild.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", ""));
                            }
                            else if(deeperChild.GetComponent<LineRenderer>() != null)
                            {
                                deeperChild.GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/" + deeperChild.GetComponent<LineRenderer>().material.name.Replace(" (Instance)", ""));
                            }
                        }
                    }
                }
            }

            private void saveSceneAndClearWorkspace()
            {
                prepareMaterialsForSave();
                saveSceneObjekt();
                Destroy(levelScene);
            }

            private void createNewLevelLayoutObject()
            {
                GameObject newScene = new GameObject();
                newScene.tag = "LevelLayoutObject";
                newScene.name = "NewLevel";
            }
            #endregion

            #region SaveLevelUI 
            public void onSaveLevelSaveAndNewButtonPress()
            {
                saveLoadEvents.InvokeSaveLevelEvent();
                saveSceneAndClearWorkspace();
                createNewLevelLayoutObject();
                saveLoadEvents.InvokeNewLevelEvent();
                SaveLoadLevelUI sllui = saveLoadManager.GetComponent<SaveLoadLevelUI>();
                sllui.Hide();
            }

            public void onSaveLevelSaveAndKeepButtonPress()
            {
                saveLoadEvents.InvokeSaveLevelEvent();
                saveSceneObjekt();
                SaveLoadLevelUI sllui = saveLoadManager.GetComponent<SaveLoadLevelUI>();
                sllui.Hide();
            }

            //Used with Save-And-New Button Press
            public void changeInputFieldTextToSceneName()
            {
                textFieldInput.text = GameObject.FindGameObjectWithTag("LevelLayoutObject").name;
            }

            public void newLevelButtonPress()
            {
                saveLoadEvents.InvokeNewLevelEvent();
                Destroy(levelScene);
                createNewLevelLayoutObject();
            }
            #endregion

        }
    }
}



