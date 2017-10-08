using UnityEngine;
using System;

public class DataTranslator : MonoBehaviour {

    private static string KILL_SYMBOL = "[KILLS]";
    private static string DEATHS_SYMBOL = "[DEATHS]";

    public static string ValuesToData (int kills, int deaths) {
        return KILL_SYMBOL + kills + "/" + DEATHS_SYMBOL + deaths;
    }

    public static int DataToKills (string data) {
        return int.Parse(DataToValue(data, KILL_SYMBOL));
    }

    public static int DataToDeaths (string data) {
        return int.Parse(DataToValue(data, DEATHS_SYMBOL));
    }

    private static string DataToValue(string data, string symbol) {
        
		string[] pieces = data.Split('/');
		foreach (string piece in pieces)
		{

            if (piece.StartsWith(symbol, StringComparison.Ordinal))
			{
                return piece.Substring(symbol.Length);
			}
		}

        return "";
        Debug.LogError("Symbol not found in " + data);
    }
}
