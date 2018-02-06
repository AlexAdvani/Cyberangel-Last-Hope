using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.IO;

public class ES2EditorTemplates
{
	public static string GetTemplate(string templateName)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		
		using (Stream stream = assembly.GetManifestResourceStream(templateName))
			using (StreamReader reader = new StreamReader(stream))
		{
			return reader.ReadToEnd();
		}
	}
}