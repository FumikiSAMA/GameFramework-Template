﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace Fumiki
{
	public static partial class DataTableExtension
	{
		public static Dictionary<int,int> ParseInt32Int32Dictionary(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			Dictionary<int,int> dictionary = new Dictionary<int,int>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				string[] keyValue = splitValue[i].Split('#');
				dictionary.Add(Int32.Parse(keyValue[0].Substring(1)),Int32.Parse(keyValue[1].Substring(0, keyValue[1].Length - 1)));
			}
			return dictionary;
		}
		public static Dictionary<int,Vector3> ParseInt32Vector3Dictionary(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			Dictionary<int,Vector3> dictionary = new Dictionary<int,Vector3>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				string[] keyValue = splitValue[i].Split('#');
				dictionary.Add(Int32.Parse(keyValue[0].Substring(1)),ParseVector3(keyValue[1].Substring(0, keyValue[1].Length - 1)));
			}
			return dictionary;
		}
	}
}
