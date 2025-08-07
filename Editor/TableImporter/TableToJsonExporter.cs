using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace cnoom.Editor.TableImporter
{
    public class TableToJsonExporter
    {
        public static void Export(string className, List<string> headers, List<string> types, List<List<string>> rows,
            string outputPath)
        {
            var type = CompileHelper.GetCompiledType(className);
            if (type == null) return;

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            foreach (var row in rows)
            {
                var obj = Activator.CreateInstance(type);
                for (int i = 0; i < headers.Count; i++)
                {
                    var field = type.GetField(headers[i]);
                    object value = ConvertHelper.ParseValue(types[i], row[i]);
                    field?.SetValue(obj, value);
                }

                list.Add(obj);
            }

            Directory.CreateDirectory(outputPath);
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            TextAsset textAsset = new TextAsset(json);
            AssetDatabase.CreateAsset(textAsset, Path.Combine(outputPath, className + ".asset"));
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"导出 {className} 到 {outputPath}");
        }
    }
}