using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
public static class MaterialRecipe {
    public static Dictionary<int, List<Vector2Int>> recipe = new Dictionary<int, List<Vector2Int>> {
        { 100, new List<Vector2Int> { new Vector2Int(0, 3)}}, // Plank
        { 101, new List<Vector2Int> { new Vector2Int(2, 3)} }, // Rope
        { 102, new List<Vector2Int> { new Vector2Int(3, 3)} }, //Steel Plate
        { 103, new List<Vector2Int> { new Vector2Int(4, 1), new Vector2Int(6, 1)} }, //Wire
        { 104, new List<Vector2Int> { new Vector2Int(1, 1), new Vector2Int(3, 1)} }, //Nail
        { 200, new List<Vector2Int> { new Vector2Int(103, 2), new Vector2Int(1, 2), new Vector2Int(102, 2), new Vector2Int(104, 2)} }, //Nail
        { 201, new List<Vector2Int> { new Vector2Int(103, 1), new Vector2Int(5, 2), new Vector2Int(6, 2), new Vector2Int(102, 1)} }, //Nail
    };
    public static int findMaxIndex(int num) {
        int max = 0;
        foreach (int key in recipe.Keys) {
            if (key < num) {
                max++;
            }
        }
        return max;
    }
    public static int GetKeyByIndex(int index) {
        int currentIndex = 0;

        foreach (int key in recipe.Keys) {
            if (currentIndex == index) {
                return key;
            }
            currentIndex++;
        }

        throw new IndexOutOfRangeException("The index provided is out of range.");
    }
}