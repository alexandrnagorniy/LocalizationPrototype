using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{
    public Dropdown langDropdown;
    public List<string> textes = new List<string>();
    public List<TranslatorObject> objects = new List<TranslatorObject>();
    
    public string languageName;
    public List<string> datas = new List<string>();

    private List<Data> myData = new List<Data>();
    public FirebaseDatabase reference;
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance;

        langDropdown.options.Clear();

        reference.RootReference.Child("Languages").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log(task);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                string result = snapshot.GetRawJsonValue();

                foreach (var lang in snapshot.Children)
                {
                    Dropdown.OptionData t = new Dropdown.OptionData();

                    t.text = (string) lang.Key;
                    langDropdown.options.Add(t);

                    var datas = lang.Child("datas");

                    Data d = new Data();
                    foreach (var value in datas.Children)
                    {
                        d.datas.Add((string) value.Value);
                    }
                    myData.Add(d);
                }
            }
        });
        if (PlayerPrefs.HasKey("CurrentLanguage"))
        {
            StartCoroutine(Load());
        }
    }

    IEnumerator Load()
    {
        yield return new WaitUntil(() => langDropdown.options.Count > 0);
        langDropdown.value = PlayerPrefs.GetInt("CurrentLanguage");
        SelectLanguage();
    }

    public void SelectLanguage()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetComponent<Text>().text = myData[langDropdown.value].datas[i];
        }
        PlayerPrefs.SetInt("CurrentLanguage", langDropdown.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddLanguage(datas, languageName);
    }

    void AddLanguage(List<string> _data, string _languageName)
    {
        Data data = new Data();
        for (int i = 0; i < _data.Count; i++)
        {
            data.datas.Add(_data[i]);
        }
        string json = JsonUtility.ToJson(data);
        reference.RootReference.Child("Languages").Child(_languageName).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception.ToString());
                //UnityMainThreadDispatcher.Instance().Enqueue(() => MessageController.Instanse.ShowMessage("Error",task.Exception.ToString()));
                //UnityMainThreadDispatcher.Instance().Enqueue(() => AdminPanel.Instanse.HideLoading());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                //UnityMainThreadDispatcher.Instance().Enqueue(() => _callback.Invoke());
                Debug.Log("Finished update database...");
            }
        });
    }
}

[System.Serializable]
public class Data
{
    public List<string> datas = new List<string>();
}