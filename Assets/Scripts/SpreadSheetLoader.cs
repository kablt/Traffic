using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;
using UnityEngine.Analytics;

public class SpreadSheetLoader : MonoBehaviour
{
    //���� �������� ��Ʈ�� TSV�������� �о�� �� �ֵ��� �ּҸ� ������ݴϴ�.
    public static string GetSheetDataAddress(string address, string range , long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

    public readonly string ADDRESS = "https://docs.google.com/spreadsheets/d/1KxbUY44Xhzr-_a3SLNwS9IdXE656ht2c5BZAw5U9jj4";

    public readonly string RANGE = "A2:D7";
    public readonly long SHEET_ID = 0;

    //�о�� ��Ʈ�� �����͸� �ӽ������س����ϴ�.
    private string loadString = string.Empty;
    //���� �������� ��Ʈ�� TSV��� �ּҸ� �̿��� �����͸� �о�ϴ�.
    private IEnumerator LoadData(Action<string> onMessageReceived)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetSheetDataAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();
        //������ �ε� �Ϸ�
        Debug.Log(www.downloadHandler.text);
        if(onMessageReceived != null)
        {
            onMessageReceived(www.downloadHandler.text);
        }
        yield return null;
    }

    public string StartLoader()
    {
        StartCoroutine(LoadData(output => loadString = output));

        return loadString;
    }

    T GetData<T>(string[] datas, string childType = "")
    {
        object data;
        if (string.IsNullOrEmpty(childType) || Type.GetType(childType) == null)
        {
            data = Activator.CreateInstance(typeof(T));
        }
        else
        {
            data = Activator.CreateInstance(Type.GetType(childType));
        }

        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance);
        for(int i = 0; i< datas.Length; i++)
        {
            try 
            {
                Type type = fieldInfos[i].FieldType;
                if (string.IsNullOrEmpty(datas[i]))
                {
                    continue;
                }

                if(type == typeof(int))
                {
                    fieldInfos[i].SetValue(data, int.Parse(datas[i]));
                }
                else if(type == typeof(float))
                {
                    fieldInfos[i].SetValue(data, float.Parse(datas[i]));
                }
                else if(type == typeof(bool))
                {
                    fieldInfos[i].SetValue(data, bool.Parse(datas[i]));
                }
                else if(type == typeof(string))
                {
                    fieldInfos[i].SetValue(data, datas[i]);
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"SpreadSheet Load Error : {e.Message}");
            }

        }
        return (T)data;

        List<T> GetDatas<T>(string data)
        {
            List<T> returnList = new List<T>();
            string[] splitedData = data.Split('\n');
            foreach(string element in splitedData)
            {
                string[] datas = element.Split('\t');
                returnList.Add(GetData<T>(datas));
            }
            return returnList;
        }

    }

}
