using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace TutorialInfo.Scripts.Editor
{
    [CustomEditor(typeof(Readme))]
    [InitializeOnLoad]
    public class ReadmeEditor : UnityEditor.Editor
    {
        static string _showedReadmeSessionStateName = "ReadmeEditor.showedReadme";
        
        static string _readmeSourceDirectory = "Assets/TutorialInfo";

        const float kSpace = 16f;

        static ReadmeEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
        }

        static void RemoveTutorial()
        {
            if (EditorUtility.DisplayDialog("Remove Readme Assets",
                
                $"All contents under {_readmeSourceDirectory} will be removed, are you sure you want to proceed?",
                "Proceed",
                "Cancel"))
            {
                if (Directory.Exists(_readmeSourceDirectory))
                {
                    FileUtil.DeleteFileOrDirectory(_readmeSourceDirectory);
                    FileUtil.DeleteFileOrDirectory(_readmeSourceDirectory + ".meta");
                }
                else
                {
                    Debug.Log($"Could not find the Readme folder at {_readmeSourceDirectory}");
                }

                var readmeAsset = SelectReadme();
                if (readmeAsset != null)
                {
                    var path = AssetDatabase.GetAssetPath(readmeAsset);
                    FileUtil.DeleteFileOrDirectory(path + ".meta");
                    FileUtil.DeleteFileOrDirectory(path);
                }

                AssetDatabase.Refresh();
            }
        }

        static void SelectReadmeAutomatically()
        {
            if (!SessionState.GetBool(_showedReadmeSessionStateName, false))
            {
                var readme = SelectReadme();
                SessionState.SetBool(_showedReadmeSessionStateName, true);

                if (readme && !readme.loadedLayout)
                {
                    LoadLayout();
                    readme.loadedLayout = true;
                }
            }
        }

        static void LoadLayout()
        {
            var assembly = typeof(EditorApplication).Assembly;
            var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
            var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
            }
        }

        static Readme SelectReadme()
        {
            var ids = AssetDatabase.FindAssets("Readme t:Readme");
            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new[] { readmeObject };

                return (Readme)readmeObject;
            }
            else
            {
                Debug.Log("Couldn't find a readme");
                return null;
            }
        }

        protected override void OnHeaderGUI()
        {
            var readme = (Readme)target;
            Init();

            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

            GUILayout.BeginHorizontal("In BigTitle");
            {
                if (readme.icon != null)
                {
                    GUILayout.Space(kSpace);
                    GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                }
                GUILayout.Space(kSpace);
                GUILayout.BeginVertical();
                {

                    GUILayout.FlexibleSpace();
                    GUILayout.Label(readme.title, TitleStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            var readme = (Readme)target;
            Init();

            foreach (var section in readme.sections)
            {
                if (!string.IsNullOrEmpty(section.heading))
                {
                    GUILayout.Label(section.heading, HeadingStyle);
                }

                if (!string.IsNullOrEmpty(section.text))
                {
                    GUILayout.Label(section.text, BodyStyle);
                }

                if (!string.IsNullOrEmpty(section.linkText))
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }

                GUILayout.Space(kSpace);
            }

            if (GUILayout.Button("Remove Readme Assets", ButtonStyle))
            {
                RemoveTutorial();
            }
        }

        bool _initialized;

        GUIStyle LinkStyle
        {
            get { return _linkStyle; }
        }

        GUIStyle _linkStyle;

        GUIStyle TitleStyle
        {
            get { return _titleStyle; }
        }

        GUIStyle _titleStyle;

        GUIStyle HeadingStyle
        {
            get { return _headingStyle; }
        }

        GUIStyle _headingStyle;

        GUIStyle BodyStyle
        {
            get { return _bodyStyle; }
        }

        GUIStyle _bodyStyle;

        GUIStyle ButtonStyle
        {
            get { return _buttonStyle; }
        }

        GUIStyle _buttonStyle;

        void Init()
        {
            if (_initialized)
                return;
            _bodyStyle = new GUIStyle(EditorStyles.label);
            _bodyStyle.wordWrap = true;
            _bodyStyle.fontSize = 14;
            _bodyStyle.richText = true;

            _titleStyle = new GUIStyle(_bodyStyle);
            _titleStyle.fontSize = 26;

            _headingStyle = new GUIStyle(_bodyStyle);
            _headingStyle.fontStyle = FontStyle.Bold;
            _headingStyle.fontSize = 18;

            _linkStyle = new GUIStyle(_bodyStyle);
            _linkStyle.wordWrap = false;

            // Match selection color which works nicely for both light and dark skins
            _linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            _linkStyle.stretchWidth = false;

            _buttonStyle = new GUIStyle(EditorStyles.miniButton);
            _buttonStyle.fontStyle = FontStyle.Bold;

            _initialized = true;
        }

        bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }
    }
}
