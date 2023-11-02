using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ElectroBuilder
{
    namespace SaveLoadSystem
    {
        public class LoadingLevel : MonoBehaviour
        {
            [SerializeField] private Transform scrollViewContent;
            
            [SerializeField] private Font textFont;

            [SerializeField] private ReadSelectedLevel readSelectedLevel;

            private SaveLoadEventHandler saveLoadEventHandler;

            private string lastSelected;

            private void Start()
            {
                //On Event from ReadSelectedLevel changing the displayed text in the selectedTextBox
                readSelectedLevel.OnSelectionChanged += changeShowSelectedText;
                saveLoadEventHandler = FindObjectOfType<SaveLoadEventHandler>();
            }

            //creates a Text Box foreach already Saved Level into the content Space of the Load Menu
            public void loadLevelIntoContent()
            {
                Object[] SavedLevels = Resources.LoadAll("Level", typeof(GameObject));

                foreach (GameObject Level in SavedLevels)
                {
                    GameObject newLevelTextBox = new GameObject(Level.name);
                    newLevelTextBox.transform.parent = scrollViewContent;


                    Text textBoxLevelName = newLevelTextBox.AddComponent<Text>();
                    textBoxLevelName.rectTransform.sizeDelta = new Vector2(600, 50);
                    textBoxLevelName.font = textFont;
                    textBoxLevelName.alignment = TextAnchor.MiddleCenter;
                    textBoxLevelName.color = Color.black;
                    textBoxLevelName.fontSize = 31;
                    textBoxLevelName.text = Level.name;
                }
            }

            //Clearing all children in Scrolview Content
            //--> Preventing multiple Textboxes of the same Level-Gameobjekt 
            public void clearContentSpace()
            {
                foreach (Transform child in scrollViewContent)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            public void loadingLevel()
            {
                saveLoadEventHandler.InvokeLoadLevelEvent();
                Destroy(GameObject.FindGameObjectWithTag("LevelLayoutObject"));
                GameObject chosenLevel = Resources.Load<GameObject>("Level/" + readSelectedLevel.selcetedLevelName);
                Instantiate(chosenLevel, chosenLevel.transform.position, chosenLevel.transform.rotation);
            }

            public void deleteSavedLevelObjekt()
            {
                AssetDatabase.DeleteAsset("Assets/Resources/Level/" + readSelectedLevel.selcetedLevelName + ".prefab");
                foreach (Transform child in scrollViewContent)
                {
                    if (child.name.Equals(readSelectedLevel.selcetedLevelName))
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }

            //changing the text in selectedTextBox
            private void changeShowSelectedText()
            {
                Color ElectrOrange = new Color(220F / 255F, 86F / 255F, 54F / 255F);

                //If no Level has been selcted yet
                if (lastSelected == null && readSelectedLevel.selcetedLevelName != null)
                {
                    //Same block again and again: Find Parent Object (Content), find Childobject with selected Name, get and change settings in the text component
                    GameObject contentParent = FindObjectOfType<ReadSelectedLevel>().gameObject;
                    GameObject textBox = contentParent.transform.Find(readSelectedLevel.selcetedLevelName).gameObject;
                    textBox.GetComponent<Text>().resizeTextForBestFit = true;
                    textBox.GetComponent<Text>().fontStyle = FontStyle.Bold;
                    textBox.GetComponent<Text>().color = ElectrOrange;
                }

                //If the same level is deselcted or reselceted
                if (lastSelected != null && readSelectedLevel.selcetedLevelName != null && lastSelected == readSelectedLevel.selcetedLevelName)
                {
                    GameObject contentParent = FindObjectOfType<ReadSelectedLevel>().gameObject;
                    GameObject textBox = contentParent.transform.Find(readSelectedLevel.selcetedLevelName).gameObject;
                    //Switching the settings to the opposit
                    textBox.GetComponent<Text>().resizeTextForBestFit = !textBox.GetComponent<Text>().resizeTextForBestFit;
                    if (textBox.GetComponent<Text>().fontStyle == FontStyle.Bold)
                    {
                        //Deselect
                        textBox.GetComponent<Text>().fontStyle = FontStyle.Normal;
                        textBox.GetComponent<Text>().color = Color.black;
                        readSelectedLevel.ResetSelectedLevelName();
                    }
                    else
                    {
                        textBox.GetComponent<Text>().fontStyle = FontStyle.Bold;
                        textBox.GetComponent<Text>().color = ElectrOrange;
                    }
                }

                //If another level is selcted while one already was selected
                if (lastSelected != null && readSelectedLevel.selcetedLevelName != null && lastSelected != readSelectedLevel.selcetedLevelName)
                {
                    GameObject contentParent = FindObjectOfType<ReadSelectedLevel>().gameObject;
                    GameObject textBox = contentParent.transform.Find(readSelectedLevel.selcetedLevelName).gameObject;
                    textBox.GetComponent<Text>().resizeTextForBestFit = true;
                    textBox.GetComponent<Text>().fontStyle = FontStyle.Bold;
                    textBox.GetComponent<Text>().color = ElectrOrange;

                    GameObject contentParentLast = FindObjectOfType<ReadSelectedLevel>().gameObject;
                    GameObject textBoxLast = contentParentLast.transform.Find(lastSelected).gameObject;
                    textBoxLast.GetComponent<Text>().resizeTextForBestFit = false;
                    textBoxLast.GetComponent<Text>().fontStyle = FontStyle.Normal;
                    textBoxLast.GetComponent<Text>().color = Color.black;
                }

                lastSelected = readSelectedLevel.selcetedLevelName;
            }
        }
    }
}




