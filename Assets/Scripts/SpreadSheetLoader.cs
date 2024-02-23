using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;

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



}
