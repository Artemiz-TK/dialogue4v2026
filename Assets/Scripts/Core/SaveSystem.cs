using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Core.SaveSystem
{
    /// <summary>
    /// Sistema de salvamento manual do jogo.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private static SaveSystem s_Instance;

        /// <summary>
        /// Propriedade que pega a instância da class.
        /// </summary>
        /// <remarks>
        /// <para><b>Exemplo de uso:</b></para>
        /// <code>
        /// SaveSystem.Singleton!.LoadPlayerLevel(level);
        /// </code>
        /// </remarks>
        public static SaveSystem Singleton => s_Instance;

        private List<Save> m_Saves;

        private string m_DataPath;

        [SerializeField] private string m_KeyEncryptor;


        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            m_Saves = new List<Save>();
            m_Saves.Add(new Save(1, "Alguem"));
            m_DataPath = Application.persistentDataPath + "/SaveSystem/SaveData.json";
            DontDestroyOnLoad(gameObject);
        }

        #region Save & Load Name

        public bool SavePlayerName(string name, int slot = 0)
        {
            if (m_Saves.Count < slot && m_Saves[slot] == null) return false;
            m_Saves[slot].PlayerName = name;
            return true;
        }

        public bool LoadPlayerName(out string name, int slot = 0)
        {
            if (m_Saves.Count < slot && m_Saves[slot] == null)
            {
                name = "";
                return false;
            }

            name = m_Saves[slot].PlayerName;
            return true;
        }

        #endregion

        #region Save & Load Level

        public bool LoadPlayerLevel(out int level, int slot = 0)
        {
            if (m_Saves.Count < slot && m_Saves[slot] == null)
            {
                level = -1;
                return false;
            }

            level = m_Saves[slot].PlayerLevel;
            return true;
        }

        public bool SavePlayerLevel(int level, int slot = 0)
        {
            if (m_Saves.Count < slot && m_Saves[slot] == null) return false;
            m_Saves[slot].PlayerLevel = level;
            return true;
        }

        #endregion

        public void SaveFile(int slot = 0)
        {
            File.WriteAllText(m_DataPath, JsonUtility.ToJson(m_Saves[slot].ToJson(), true));
        }

        public bool LoadFromFile(int slot = 0)
        {
            if (!File.Exists(m_DataPath)) return false;
            m_Saves[slot].FromJson(File.ReadAllText(m_DataPath));
            return true;
        }


        /// <summary>
        /// Classe responsável pelos dados do salvamento.
        /// </summary>
        [Serializable]
        public class Save
        {
            private int m_PlayerLevel;

            public int PlayerLevel
            {
                get => m_PlayerLevel;
                set => m_PlayerLevel = value;
            }

            private string m_PlayerName;

            public string PlayerName
            {
                get => m_PlayerName;
                set => m_PlayerName = value;
            }

            public Save(int playerLevel, string playerName)
            {
                m_PlayerLevel = playerLevel;
                m_PlayerName = playerName;
            }

            /// <summary>
            /// Método que converte a classe para Json.
            /// </summary>
            /// <returns>
            /// Retorna o próprio json em string.
            /// </returns>
            public string ToJson()
            {
                return JsonUtility.ToJson(this);
            }

            public void FromJson(string json)
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }
        
        private ref struct Encryptor
        {
            private static readonly string m_IV = "1a1a1a1a1a1a1a1a";
            private static readonly string m_Key = "1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a13";
            
            public static string IV => m_IV;
            public static string Key => m_Key;
            

            public static string Encrypt(string decrypted)
            {
                var textbytes = Encoding.ASCII.GetBytes(decrypted);
                using var endec = new AesCryptoServiceProvider();
                endec.BlockSize = 128;
                endec.KeySize = 256;
                endec.IV = Encoding.ASCII.GetBytes(IV);
                endec.Key = Encoding.ASCII.GetBytes(Key);
                endec.Padding = PaddingMode.PKCS7;
                endec.Mode = CipherMode.CBC;
                using var icrypt = endec.CreateEncryptor(endec.Key, endec.IV);
                var enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
                return Convert.ToBase64String(enc);
            }

            public static string Decrypted(string encrypted)
            {
                var textbytes = Convert.FromBase64String(encrypted);
                using var endec = new AesCryptoServiceProvider();
                endec.BlockSize = 128;
                endec.KeySize = 256;
                endec.IV = Encoding.ASCII.GetBytes(IV);
                endec.Key = Encoding.ASCII.GetBytes(Key);
                endec.Padding = PaddingMode.PKCS7;
                endec.Mode = CipherMode.CBC;
                var icrypt = endec.CreateDecryptor(endec.Key, endec.IV);
                var enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
                icrypt.Dispose();
                return Encoding.ASCII.GetString(enc);
            }
        }
    }
}
