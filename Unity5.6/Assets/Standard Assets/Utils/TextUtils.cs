using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
public class TextUtils
{
    public static string readLine(string Src, ref int offsetPos)
    {
        char[] line = new char[1024];
        int pos = offsetPos;
        while (pos < Src.Length)
        {
            char ch = Src[pos];
            if (ch == '\r' || ch == '\n')
            {
                pos++;
                if (Src[pos] == '\n')
                    pos++;
                break;
            }
            pos++;
        }
        int length = pos - offsetPos;
        Src.CopyTo(offsetPos, line, 0, pos - offsetPos);
        offsetPos = pos;
        return new string(line, 0, length);
    }
    public static bool isSepChar(char ch)
    {
        if (ch == ' ' ||
           ch == '\r' ||
           ch == '\n')
            return true;
        return false;
    }
    public static List<string> splitLine(string Src)
    {
        int pos = 0;
        List<string> values = new List<string>();
        StringBuilder sp = new StringBuilder();
        while (pos < Src.Length)
        {
            if (isSepChar(Src[pos]))
            {
                pos++;
                if (sp.Length > 0)
                    values.Add(sp.ToString());
                sp.Remove(0, sp.Length);
                continue;
            }
            sp.Append(Src[pos]);
            pos++;
        }
        return values;
    }
}
