using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;

public class ESOLang
{
    #region Idea
    /*
     * var1 = value;
     * var1 => newValue;
     * 
     * object obj1 {
     *  vis pubVar1 = value;
     *  hid privVar1 = value;
     * }
     *     
    */
    #endregion

    private string[] code;
    private Dictionary<string, float> variables;

    public class UndefinedVariableException : Exception {}
    public class MismatchedParentheticalException : Exception { }
    public class SysEvalException : Exception { }

    public class parenthetical
    {
        public int startIndex;
        public int endIndex;
        public string expression;
        
        public parenthetical(int startIndex, int endIndex, string expression)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.expression = expression;
        }
    }

    public enum mathOperation
    {
        sqrt = 0,
        sin = 1,
        cos = 2,
        tan = 3,
        arcsin = 4,
        arccos = 5,
        arctan = 6,
        abs = 7
    }

    #region privateMemberFunctions
    private float sysEval(string expression)
    {
        try
        {
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(float), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (float)(loDataTable.Rows[0]["Eval"]);
        }
        catch (Exception e)
        {
            throw new SysEvalException();
        }
    }
    private string formatVarsAsString()
    {
        string result = "";
        foreach (KeyValuePair<string, float> entry in variables)
        {
            result += entry.Key + " : " + entry.Value.ToString() + ", ";
        }
        return result;
    }

    private float getVal(string varName)
    {
        if (variables.ContainsKey(varName))
            return variables[varName];
        throw new UndefinedVariableException();
    }

    #region Math

    private parenthetical getMathParenthetical(mathOperation operation, string expression)
    {
        parenthetical newParenthetical = findParenthetical(Enum.GetName(typeof(mathOperation), operation), expression);
        return newParenthetical;
    }

    private string performMath2(parenthetical mathParenthetical, mathOperation operation, string expression)
    {
        string mathString = "";
        switch (operation)
        {
            case mathOperation.sqrt:
                mathString = Mathf.Sqrt(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.sin:
                mathString = Mathf.Sin(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.cos:
                mathString = Mathf.Cos(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.tan:
                mathString = Mathf.Tan(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.arcsin:
                mathString = Mathf.Asin(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.arccos:
                mathString = Mathf.Acos(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.arctan:
                mathString = Mathf.Atan(sysEval(mathParenthetical.expression)).ToString();
                break;
            case mathOperation.abs:
                mathString = Mathf.Abs(sysEval(mathParenthetical.expression)).ToString();
                break;
        }
        expression = expression.Substring(0, mathParenthetical.startIndex - 1) + mathString +
                     expression.Substring(mathParenthetical.endIndex + 1);
        return expression;
    }

    private string performMath1(mathOperation operation, string expression)
    {
        string result = "";
        try
        {
            result = performMath2(getMathParenthetical(operation, expression), operation, expression);
        }
        catch (SysEvalException e)
        {
        }
        return result;
    }
    #endregion

    private string doMath(string expression)
    {

        return expression;
    }

    private float eval(string toEval)
    {
        // REPLACE ALL VARIABLES WITH THEIR VALUES
        foreach (KeyValuePair<string, float> entry in variables)
        {
            toEval = toEval.Replace(entry.Key, entry.Value.ToString());
        }

        toEval = doMath(toEval);

        // RETURN
        float result = sysEval(toEval);
        return result;
    }

    private parenthetical findParenthetical(string preface, string expression)
    {
        int indexOfStart = expression.IndexOf(preface + "(") + preface.Length;
        int indexOfEnd = expression.IndexOf(')');

        if (indexOfStart - preface.Length > -1)
        {
            Debug.Log("parentheses found: " + indexOfStart.ToString());
            /*
            if (indexOfEnd != -1)
                throw new MismatchedParentheticalException();*/
            return new parenthetical(indexOfStart, indexOfEnd, expression.Substring(indexOfStart + 1, indexOfEnd));
        }
        Debug.Log("absolutely no parentheses found");
        return null;
    }
    #endregion

    public ESOLang(string [] code)
    {
        this.code = code;
        variables = new Dictionary<string, float>();
    }
    public string execute()
    {
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = code[i].Trim().Replace(" ", "");
            Debug.Log("trim: " + code[i]);
            if (code[i].Contains("="))
            {
                Debug.Log("We got an =");
                if (code[i].ToCharArray()[code[i].Length-1] != ';')
                    return ("forgot semicolon on line " + (i + 1).ToString());
                if (code[i].Contains("=>"))
                {
                    Debug.Log("We got an =>");
                    string[] splitEq = code[i].Split('=');
                    splitEq[1] = splitEq[1].Substring(1, splitEq[1].Length-2);
                    
                    string varNameToCheck = splitEq[0];
                    Debug.Log("setting " + varNameToCheck + " to " + splitEq[1]);

                    float varVal;

                    try { varVal = float.Parse(splitEq[1]); } 
                    catch (FormatException) 
                    {
                        varVal = eval(splitEq[1]);
                        Debug.Log("found a format exception so we evaluated to " + varVal);
                    }

                    try 
                    {
                        getVal(varNameToCheck);
                        variables[varNameToCheck] = varVal;
                    }
                    catch (UndefinedVariableException) { return ("initializing undefined variable on line " + (i + 1).ToString()); }
                }
                else
                {
                    string[] splitEq = code[i].Split('=');
                    string varNameToAdd = splitEq[0];
                    string varTypeRaw = splitEq[1].Substring(0, splitEq[1].Length-1);
                    Debug.Log("before parsing, setting " + varNameToAdd + " to " + varTypeRaw);
                    float varVal;
                    try { varVal = float.Parse(varTypeRaw); }
                    catch (FormatException)
                    {
                        varVal = eval(varTypeRaw);
                        Debug.Log("found a format exception so we evaluated to " + varVal);
                    }
                    Debug.Log("after parsing, we are setting " + varNameToAdd + " to " + varVal);
                    variables.Add(varNameToAdd, varVal);
                }
            }
        }
        return formatVarsAsString();
    }
}
