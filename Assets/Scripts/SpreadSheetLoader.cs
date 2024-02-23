using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Reflection;

public class SpreadSheetLoader : MonoBehaviour
{
    //구글 스프레드 시트를 TSV형식으로 읽어올 수 있도록 주소를 만들어줍니다.
    public static string GetSheetDataAddress(string address, string range , long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

    public readonly string ADDRESS = "https://docs.google.com/spreadsheets/d/1KxbUY44Xhzr-_a3SLNwS9IdXE656ht2c5BZAw5U9jj4";

    public readonly string RANGE = "A2:D7";
    public readonly long SHEET_ID = 0;

    //읽어온 스트링 데이터를 임시저장해놓습니다.
    private string loadString = string.Empty;
    //구글 스프레드 시트의 TSV얻는 주소를 이용해 데이터를 읽어옯니다.
    private IEnumerator LoadData(Action<string> onMessageReceived)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetSheetDataAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();
        //데이터 로딩 완료
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
