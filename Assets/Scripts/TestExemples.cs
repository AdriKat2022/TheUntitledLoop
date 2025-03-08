using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestExemples : MonoBehaviour
{
    [SerializeField] GameObject ChoicesHolder;
    [SerializeField] TMP_Text text;

    [SerializeField] Story story;

    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        SetUpNode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Next(int index)
    {
        foreach (Transform trans in ChoicesHolder.transform)
        {
            Destroy(trans.gameObject);
        }

        story.ChooseNextNode(index);
        SetUpNode();
    }

    private void SetUpNode()
    {
        StoryNode node = story.GetCurrentNode();
        text.text = node.getText();

        int i = 0;
        foreach(NextNode nn in node.GetNextNodes())
        {
            GameObject instance = Instantiate(prefab, ChoicesHolder.transform);
            MyButton button = instance.GetComponent<MyButton>();

            button.te = this;
            button.id = i++;
        }
        
    }
}
