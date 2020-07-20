using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Zorgt voor de weergave van UI-knoppen.
/// </summary>
public class GUIDrawer
{
    private Color colorXAxis = Color.red;
    private Color colorYAxis = Color.green;
    private Color colorZAxis = Color.blue;

    /// <summary>
    /// Toont een knop die een functie uitvoert als hij aangeklikt wordt.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="a"></param>
    public void DisplayButton(string name, Action a)
    {
        if (GUILayout.Button(name))
        {
            a();
        }
    }

    /// <summary>
    /// Toont een gekleurde knop die een functie uitvoert als hij aangeklikt wordt.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="col"></param>
    /// <param name="a"></param>
    public void DisplayButton(string name, Color col, Action a)
    {
        Color tmpCol = GUI.backgroundColor;
        GUI.backgroundColor = col;

        if (GUILayout.Button(name))
        {
            a();
        }

        GUI.backgroundColor = tmpCol;
    }

    /// <summary>
    /// Toont een rode knop voor de x-as.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="positive"></param>
    public void DisplayXButton(Action a, bool positive)
    {
        DisplayButton(positive ? "X Axis" : "-X Axis", colorXAxis, a);
    }

    /// <summary>
    /// Toont een groene knop voor de y-as.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="positive"></param>
    public void DisplayYButton(Action a, bool positive)
    {
        DisplayButton(positive ? "Y Axis" : "-Y Axis", colorYAxis, a);
    }

    /// <summary>
    /// Toont een blauwe knop voor de z-as.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="positive"></param>
    public void DisplayZButton(Action a, bool positive)
    {
        DisplayButton(positive ? "Z Axis" : "-Z Axis", colorZAxis, a);
    }
}
