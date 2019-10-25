using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslatorObject : MonoBehaviour
{
    private Text myText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeText(string text)
    {
        myText.text = text;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
