using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class Utils
{
	public static Plane yZeroPlane = new Plane(Vector3.up, Vector3.zero);
	/// <summary>
	/// Multiply a float by another float, taking Time.deltaTime into account.
	/// It assumes you want to multiply the values 60 times per second.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="value"></param>
	/// <returns>The result of the multiplication</returns>
	public static float Multiply(float x, float value, float? deltaTime = null)
	{
		if (deltaTime == null)
			deltaTime = Time.deltaTime;

		if (value >= 0)
			return x * Mathf.Pow(value, deltaTime.Value * 60);
		else
			return -x * Mathf.Pow(Mathf.Abs(value), deltaTime.Value * 60);
	}

	public static T GetClosest<T>(List<T> list, Vector3 pos, T ignoreThisOne = null) where T : MonoBehaviour
	{
		T closest = null;
		var dist = 0f;

		foreach (T p in list)
		{
			if (p == ignoreThisOne) continue;

			var d = Vector3.Distance(pos, p.transform.position);
			if (d < dist || closest == null)
			{
				dist = d;
				closest = p;
			}
		}

		return closest;
	}

	public static bool LayerIsInMask(int layer, LayerMask layermask)
	{
		return layermask == (layermask | (1 << layer));
	}

	/// <summary>
	/// Uses y=mx+b to another range.
	/// 
	/// </summary>
	/// <param name="val"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="xx"></param>
	/// <param name="yy"></param>
	/// <param name="clampLower"></param>
	/// <param name="clampUpper"></param>
	/// <returns></returns>
	public static float Remap(float val, float x, float y, float xx, float yy, bool clampLower = false, bool clampUpper = false)
	{
		float slope = (yy - y) / (xx - x);
		float b = y - x * slope;
		var v = val * slope + b;

		if (clampLower)
			v = Mathf.Max(v, Mathf.Min(y, yy));

		if (clampUpper)
			v = Mathf.Min(v, Mathf.Max(y, yy));

		return v;
	}

	/// <summary>
	/// Choose a random int from an array
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public static int Choose(params int[] options)
	{
		return options[Random.Range(0, options.Length)];
	}

	/// <summary>
	/// Choose a random float from an array
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public static float Choose(params float[] options)
	{
		return options[Random.Range(0, options.Length)];
	}

	/// <summary>
	/// Choose a random image from an array
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public static Sprite Choose(params Sprite[] options)
	{
		return options[Random.Range(0, options.Length)];
	}

	/// <summary>
	/// Choose a random string from an array
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public static string Choose(params string[] options)
	{
		return options[Random.Range(0, options.Length)];
	}

	/// <summary>
	/// Choose a random object from an array
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public static object Choose(params object[] options)
	{
		return options[Random.Range(0, options.Length)];
	}

	public static Vector2 DirectionToVector(CardinalDirection d)
	{
		switch (d)
		{
			case CardinalDirection.NORTH: return Vector2.up;
			case CardinalDirection.EAST: return Vector2.right;
			case CardinalDirection.SOUTH: return Vector2.down;
			case CardinalDirection.WEST: return Vector2.left;
			default:
				throw new ArgumentOutOfRangeException(nameof(d), d, null);
		}
	}

	/// <summary>
	/// Will return a point on the edge or corner of the rect based on direction
	/// </summary>
	/// <param name="d"></param>
	/// <param name="rect"></param>
	/// <returns></returns>
	public static Vector2 GetDirectionPointOnRect(CardinalDirection d, Rect rect)
	{
		switch (d)
		{
			case CardinalDirection.NORTH: return new Vector2(rect.center.x, rect.yMax);
			case CardinalDirection.EAST: return new Vector2(rect.xMax, rect.center.y);
			case CardinalDirection.SOUTH: return new Vector2(rect.center.x, rect.yMin);
			case CardinalDirection.WEST: return new Vector2(rect.xMin, rect.center.y);
			default:
				throw new ArgumentOutOfRangeException(nameof(d), d, null);
		}
	}

	public static string AssetToResPath(string p)
	{
		var pre = "Assets/Resources/";
		var post = ".asset";
		var preIndex = p.IndexOf(pre, StringComparison.Ordinal);
		var postIndex = p.LastIndexOf(post, StringComparison.Ordinal);
		if (preIndex != -1)
			p = p.Remove(preIndex, pre.Length);
		if (postIndex != -1)
			p = p.Remove(postIndex);
		return p;
	}

	public static string AssetPath(string p)
	{
		var pre = "Assets/Resources/";
		var post = ".asset";
		if (!p.EndsWith(post))
			p += post;
		if (!p.StartsWith(pre))
			p = pre + p;
		return p;
	}

	public static string CleanScenePath(string fullPath)
	{
		fullPath = fullPath.Substring("Assets/Scenes/".Length);
		var endIndex = fullPath.IndexOf(".unity", StringComparison.Ordinal);
		return fullPath.Substring(0, endIndex);
	}

	public static string RandomString(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[Random.Range(0, s.Length)]).ToArray());
	}

	public static string[] GetAllScenesInBuild()
	{
		int sceneCount = SceneManager.sceneCountInBuildSettings;
		string[] scenes = new string[sceneCount];
		for (int i = 0; i < sceneCount; i++)
		{
			//scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
			scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
		}

		return scenes;
	}

	public enum CardinalDirection
	{
		NORTH,
		EAST,
		SOUTH,
		WEST
	}
}

/// <summary>
/// A struct containing position and rotation information.
/// I don't want to store them separately or create new Transforms 
/// to do this shit
/// </summary>
public struct PosRot
{
	public Vector3 pos;
	public Quaternion rot;

	public PosRot(Vector3 pos, Quaternion rot)
	{
		this.pos = pos;
		this.rot = rot;
	}

	public PosRot(Transform t)
	{
		pos = t.position;
		rot = t.rotation;
	}
}