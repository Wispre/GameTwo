using UnityEngine;

public static class Helper
{
    public static bool dotProduct (this Transform transform, 
    Transform other,Vector2 direction){
        Vector2 position = other.position -transform.position;
        return Vector2.Dot(position.normalized, direction) >0.20f;
    }
}
