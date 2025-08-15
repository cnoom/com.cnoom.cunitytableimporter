using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace cnoom.Editor.TableImporter
{
    public class TableImporterWindow : EditorWindow
    {
        private string _tablePath = "Assets/Tables";
        private string _classOutputPath = "Assets/Scripts/Generated/Tables";
        private string _jsonOutputPath = "Assets/StreamingAssets/Tables";
        private string _className = "";

        private void OnEnable()
        {
            _tablePath = PlayerPrefs.GetString(nameof(_tablePath), _tablePath);
            _classOutputPath = PlayerPrefs.GetString(nameof(_classOutputPath),_classOutputPath);
            _jsonOutputPath = PlayerPrefs.GetString(nameof(_jsonOutputPath), _jsonOutputPath);
            InitClassName();
        }

        [MenuItem("cnoom/表格导入工具")]
        static void OpenWindow() => GetWindow<TableImporterWindow>("表格导入工具");

        void OnGUI()
        {
            GUILayout.Label("表格导入工具", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            _tablePath = EditorGUILayout.TextField("表格路径", _tablePath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择表格路径"))
            {
                OnClickTableButton();
                InitClassName();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _classOutputPath = EditorGUILayout.TextField("类输出目录", _classOutputPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择类输出目录"))
            {
                OnClickClassButton();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _jsonOutputPath = EditorGUILayout.TextField("JSON输出目录", _jsonOutputPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择JSON输出目录"))
            {
                OnClickJsonButton();
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("类名(默认使用csv文件名/xlxs文件的第一个表名;也可以自己设置)");
            GUILayout.BeginHorizontal();
            _className = EditorGUILayout.TextField("类名", _className, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("生成表类"))
            {
                GenerateClass();
            }

            if (GUILayout.Button("生成JSON"))
            {
                GenerateJson();
            }
        }

        void OnClickTableButton()
        {
            string[] filters = { "Excel Files", "xlsx,csv", "All Files", "*" };
            _tablePath = EditorUtility.OpenFilePanelWithFilters("选择文件", Application.dataPath, filters);
            PlayerPrefs.SetString(nameof(_tablePath), _tablePath);
        }

        void InitClassName()
        {
            if (Path.GetExtension(_tablePath) == ".csv")
            {
                _className = Path.GetFileNameWithoutExtension(_tablePath);
            }
            else if (Path.GetExtension(_tablePath) == ".xlsx")
            {
                _className = ExcelReader.GetFirstSheetName(_tablePath);
            }
        }

        void OnClickClassButton()
        {
            _classOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, _classOutputPath);
            PlayerPrefs.SetString(nameof(_classOutputPath), _classOutputPath);
        }

        void OnClickJsonButton()
        {
            _jsonOutputPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, _jsonOutputPath);
            _jsonOutputPath = GetRelativePath(_jsonOutputPath);
            PlayerPrefs.SetString(nameof(_jsonOutputPath), _jsonOutputPath);
        }

        void GenerateClass()
        {
            TableData tableData = null;
            if (Path.GetExtension(_tablePath) == ".csv")
            {
                tableData = CsvReader.Read(_tablePath);
            }
            else if (Path.GetExtension(_tablePath) == ".xlsx")
            {
                tableData = ExcelReader.Read(_tablePath);
            }

            if (tableData == null)
            {
                Debug.LogError("读取表格失败!请检查表格格式是否正确");
                return;
            }

            TableToClassGenerator.Generate(_className, tableData.headers, tableData.types, _classOutputPath);
        }
        
        void GenerateJson()
        {
            TableData tableData = null;
            if (Path.GetExtension(_tablePath) == ".csv")
            {
                tableData = CsvReader.Read(_tablePath);
            }
            else if (Path.GetExtension(_tablePath) == ".xlsx")
            {
                tableData = ExcelReader.Read(_tablePath);
            }

            if (tableData == null)
            {
                Debug.LogError("读取表格失败!请检查表格格式是否正确");
                return;
            }
            TableToJsonExporter.Export(_className, tableData.headers, tableData.types, tableData.rows, _jsonOutputPath);
        }
        
        private string GetRelativePath(string absolutePath)
        {
            // 获取 Application.dataPath 的绝对路径
            string dataPath = Application.dataPath;

            // 确保两个路径都以斜杠结尾
            if (!dataPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                dataPath += Path.DirectorySeparatorChar;
            }
            if (!absolutePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                absolutePath += Path.DirectorySeparatorChar;
            }

            // 计算相对路径
            Uri dataUri = new Uri(dataPath);
            Uri absoluteUri = new Uri(absolutePath);
            Uri relativeUri = dataUri.MakeRelativeUri(absoluteUri);

            // 将 URI 转换为相对路径字符串
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);

            // 确保路径以 "Assets" 开头
            if (relativePath.StartsWith("Assets") || relativePath.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                return relativePath;
            }

            return "Assets" + Path.DirectorySeparatorChar + relativePath.TrimStart(Path.DirectorySeparatorChar);
        }


    }
}