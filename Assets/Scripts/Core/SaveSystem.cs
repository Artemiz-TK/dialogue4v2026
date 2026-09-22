using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// Sistema de salvamento do jogo.
    ///
    /// Slot 0 = Autosave
    /// Slot 1 = Save manual
    /// Slot 2 = Save manual
    /// Slot 3 = Save manual
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private static SaveSystem s_Instance;

        public static SaveSystem Singleton => s_Instance;

        private const int AutoSaveSlot = 0;
        private const int FirstManualSlot = 1;
        private const int LastManualSlot = 3;

        private const string SaveFolder = "SaveSystem";
        private const string SaveFilePrefix = "SaveSlot_";
        private const string SaveFileExtension = ".dat";

        private List<Save> m_Saves = new List<Save>();

        private string m_DataPath;

        // Caminho do arquivo antigo.
        // Serve somente para não quebrar um save que já exista do sistema anterior.
        private string m_LegacyDataPath;

        [SerializeField] private string m_KeyEncryptor;

        public string DataPath => m_DataPath;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;

            string directory = Path.Combine(
                Application.persistentDataPath,
                SaveFolder
            );

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Mantemos DataPath funcionando.
            // Agora ele representa o arquivo do Slot 0.
            m_DataPath = GetSlotPath(0);

            // Caminho utilizado pelo sistema antigo.
            m_LegacyDataPath = Path.Combine(
                Application.persistentDataPath,
                SaveFolder,
                "SaveData.json"
            );

            DontDestroyOnLoad(gameObject);

            // Mantém exatamente o comportamento original:
            // tenta carregar o slot 0 ao iniciar.
            LoadFromFile(0);
        }

        // =====================================================================
        // MÉTODO ORIGINAL
        // =====================================================================

        public void Add(Vector3 pos, int coin)
        {
            // Mantido exatamente para não quebrar quem já utiliza esse método.
            m_Saves.Add(new Save(pos, coin));
        }
        
        /// <summary>
        /// Cria um novo jogo.
        /// </summary>
        public void NewGame()
        {
            // Remove o autosave antigo da memória.
            EnsureSlotExists(AutoSaveSlot);
            m_Saves[AutoSaveSlot] = null;

            // Remove o arquivo físico do autosave.
            string autoSavePath = GetSlotPath(AutoSaveSlot);

            if (File.Exists(autoSavePath))
            {
                File.Delete(autoSavePath);
            }

            Debug.Log(
                "[SaveSystem] Novo jogo iniciado. Autosave anterior removido."
            );
        }

        // =====================================================================
        // SLOTS
        // =====================================================================

        /// <summary>
        /// Garante que exista espaço na lista interna até o slot informado.
        /// Diferentemente da versão anterior, slots ainda não utilizados ficam null,
        /// permitindo saber se realmente existe um save.
        /// </summary>
        private void EnsureSlotExists(int slot)
        {
            if (slot < 0)
                return;

            m_Saves ??= new List<Save>();

            while (m_Saves.Count <= slot)
            {
                m_Saves.Add(null);
            }
        }

        private Save GetOrCreateSave(int slot)
        {
            if (slot < 0)
                return null;

            EnsureSlotExists(slot);

            m_Saves[slot] ??= new Save(
                Vector3.zero,
                0
            );

            return m_Saves[slot];
        }

        private string GetSlotPath(int slot)
        {
            return Path.Combine(
                Application.persistentDataPath,
                SaveFolder,
                $"{SaveFilePrefix}{slot}{SaveFileExtension}"
            );
        }

        /// <summary>
        /// Verifica se existe um arquivo de save no slot.
        /// </summary>
        public bool HasSave(int slot)
        {
            if (slot is < AutoSaveSlot or > LastManualSlot)
                return false;

            return File.Exists(GetSlotPath(slot));
        }

        /// <summary>
        /// Verifica se o save já foi carregado via <see cref="slot"/>.
        /// </summary>
        /// <param name="slot">O argumento que orienta</param>
        /// <returns>Retorna o membro IsLoaded</returns>
        public bool HasLoaded(int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
                return false;

            return m_Saves[slot].IsLoaded;
        }

        public bool ModifyLoad(bool loaded, int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                return false;
            }
            
            m_Saves[slot].IsLoaded = loaded;
            return true;
        }

        /// <summary>
        /// Retorna os dados do slot já carregados em memória.
        /// </summary>
        public Save GetSave(int slot)
        {
            if (slot < 0 || slot >= m_Saves.Count)
                return null;

            return m_Saves[slot];
        }

        // =====================================================================
        // SAVE & LOAD COINS
        // =====================================================================

        public bool SavePlayerCoins(int coin, int slot = 0)
        {
            if (slot < 0)
                return false;

            var save = GetOrCreateSave(slot);

            if (save == null)
                return false;

            // Mantemos Coin como a quantidade atual.
            save.Coin = coin;

            return true;
        }

        public bool LoadPlayerCoins(out int coin, int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                coin = -1;
                return false;
            }

            coin = m_Saves[slot].Coin;

            return true;
        }

        // =====================================================================
        // SAVE & LOAD POSITION
        // =====================================================================

        public bool LoadPosition(out Vector3 pos, int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                pos = Vector3.zero;
                return false;
            }

            pos = m_Saves[slot].Position;
            return true;
        }

        public bool SavePosition(Vector3 pos, int slot = 0)
        {
            if (slot < 0)
                return false;

            Save save = GetOrCreateSave(slot);

            if (save == null)
                return false;

            save.Position = pos;

            return true;
        }

        // =====================================================================
        // CHECKPOINT
        // =====================================================================

        /// <summary>
        /// Salva o estado do checkpoint.
        ///
        /// O checkpoint é salvo no Slot 0 por padrão porque ele representa
        /// o autosave da progressão.
        /// </summary>
        public bool SaveCheckpoint(Vector3 checkpointPosition, int coins, int checkpointId, int slot = AutoSaveSlot)
        {
            if (slot < 0)
                return false;

            if (checkpointId < 0)
            {
                Debug.LogWarning(
                    $"[SaveSystem] CheckpointId inválido: {checkpointId}"
                );

                return false;
            }

            Save save =
                GetOrCreateSave(slot);

            if (save == null)
                return false;

            string sceneName =
                SceneManager.GetActiveScene().name;

            // ============================================================
            // VERIFICA O CHECKPOINT ATUAL
            // ============================================================

            if (save.CheckpointActivated &&
                save.SceneName == sceneName)
            {
                // --------------------------------------------------------
                // Se estamos tentando voltar para um checkpoint anterior,
                // não permitimos regressão.
                // --------------------------------------------------------

                if (checkpointId < save.CheckpointId)
                {
                    Debug.Log(
                        $"[SaveSystem] Checkpoint {checkpointId} ignorado. " +
                        $"Checkpoint atual: {save.CheckpointId}"
                    );

                    return true;
                }

                // --------------------------------------------------------
                // checkpointId == atual
                //
                // O jogador está ativando novamente o mesmo checkpoint.
                // Nesse caso, atualizamos o snapshot.
                // --------------------------------------------------------
            }

            // ============================================================
            // NOVO CHECKPOINT
            // ============================================================

            save.SceneName =
                sceneName;

            save.CheckpointActivated =
                true;

            save.CheckpointId =
                checkpointId;

            save.CheckpointPosition =
                checkpointPosition;

            save.CheckpointCoin =
                coins;

            // ============================================================
            // ESTADO CARREGÁVEL
            // ============================================================

            save.Position =
                checkpointPosition;

            save.Coin =
                coins;

            // ============================================================
            // SALVA
            // ============================================================

            SaveFile(slot);

            Debug.Log(
                $"[SaveSystem] CHECKPOINT {checkpointId} salvo.\n" +
                $"Cena: {sceneName}\n" +
                $"Posição: {checkpointPosition}\n" +
                $"Moedas: {coins}"
            );

            return true;
        }
        
        public bool SaveCheckpoint(
            Vector3 checkpointPosition,
            int coins,
            int slot = AutoSaveSlot)
        {
            return SaveCheckpoint(
                checkpointPosition,
                coins,
                0,
                slot
            );
        }


        /// <summary>
        /// Salva um jogo manualmente utilizando o último checkpoint válido
        /// da fase atual.
        ///
        /// Se o checkpoint ainda não foi ativado, utiliza:
        /// - posição inicial da fase;
        /// - 0 moedas;
        /// - nenhuma moeda coletada no checkpoint.
        ///
        /// O slot 0 continua sendo a fonte do autosave.
        /// </summary>
        public bool SaveManualSlot(
            int slot,
            Vector3 initialPosition,
            string sceneName)
        {
            if (slot < FirstManualSlot ||
                slot > LastManualSlot)
            {
                Debug.LogWarning(
                    $"[SaveSystem] Slot manual inválido: {slot}"
                );

                return false;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    "[SaveSystem] Cena inválida ao salvar slot manual."
                );

                return false;
            }

            // ============================================================
            // PROCURA O ÚLTIMO CHECKPOINT DA CENA ATUAL
            // ============================================================

            bool hasCheckpoint =
                AutoSaveSlot < m_Saves.Count &&
                m_Saves[AutoSaveSlot] != null &&
                m_Saves[AutoSaveSlot].CheckpointActivated &&
                m_Saves[AutoSaveSlot].SceneName == sceneName;

            // ============================================================
            // NÃO EXISTE CHECKPOINT
            // ============================================================

            if (!hasCheckpoint)
            {
                Debug.LogWarning(
                    $"[SaveSystem] Não é possível salvar o Slot {slot}. " +
                    $"O jogador ainda não passou por um checkpoint " +
                    $"na cena '{sceneName}'. O slot permanece inalterado."
                );

                return false;
            }

            // ============================================================
            // EXISTE CHECKPOINT
            // ============================================================

            Save save =
                Clone(
                    m_Saves[AutoSaveSlot]
                );

            // Garante que o estado carregável corresponde
            // exatamente ao checkpoint.
            save.Position =
                save.CheckpointPosition;

            save.Coin =
                save.CheckpointCoin;

            save.SceneName =
                sceneName;

            EnsureSlotExists(slot);

            m_Saves[slot] =
                save;

            // SaveFile também replica para o Slot 0.
            SaveFile(slot);

            Debug.Log(
                $"[SaveSystem] Slot {slot} salvo usando o checkpoint.\n" +
                $"Cena: {save.SceneName}\n" +
                $"Posição: {save.Position}\n" +
                $"Moedas: {save.Coin}\n" +
                $"Checkpoint ID: {save.CheckpointId}"
            );

            return true;
        }


        /// <summary>
        /// Retorna se o save possui checkpoint ativado.
        /// </summary>
        public bool HasCheckpoint(int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                return false;
            }

            return m_Saves[slot].CheckpointActivated;
        }

        /// <summary>
        /// Retorna a posição específica do checkpoint.
        /// </summary>
        public bool LoadCheckpointPosition(
            out Vector3 pos,
            int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                pos = Vector3.zero;
                return false;
            }

            Save save = m_Saves[slot];

            if (!save.CheckpointActivated)
            {
                pos = Vector3.zero;
                return false;
            }

            pos = save.CheckpointPosition;

            return true;
        }

        public bool LoadProgressCoins(
            out int coins,
            int slot = AutoSaveSlot)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                coins = 0;
                return false;
            }

            var save =
                m_Saves[slot];

            coins = save.CheckpointActivated ? save.CheckpointCoin : 0;

            return true;
        }

        /// <summary>
        /// Retorna a quantidade de moedas existente quando o checkpoint
        /// foi ativado.
        /// </summary>
        public bool LoadCheckpointCoins(
            out int coin,
            int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                coin = 0;
                return false;
            }

            Save save = m_Saves[slot];

            if (!save.CheckpointActivated)
            {
                coin = 0;
                return false;
            }

            coin = save.CheckpointCoin;

            return true;
        }

        // =====================================================================
        // CENA
        // =====================================================================

        /// <summary>
        /// Define manualmente qual cena deverá ser carregada quando
        /// esse save for utilizado.
        /// </summary>
        public bool SaveSceneName(
            string sceneName,
            int slot = 0)
        {
            if (slot < 0 || string.IsNullOrEmpty(sceneName))
                return false;

            Save save = GetOrCreateSave(slot);

            if (save == null)
                return false;

            save.SceneName = sceneName;

            return true;
        }

        public string LoadSceneName(int slot = 0)
        {
            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                return string.Empty;
            }

            return m_Saves[slot].SceneName;
        }

        // =====================================================================
        // MOEDAS COLETADAS
        // =====================================================================

        /// <summary>
        /// Registra uma moeda como coletada no estado atual.
        ///
        /// A lista ficará disponível para integrarmos ao CoinController
        /// sem precisar mudar o modelo do Save.
        /// </summary>
        public void RegisterCollectedCoin(
            string coinId,
            int slot = 0)
        {
            if (string.IsNullOrEmpty(coinId))
                return;

            Save save = GetOrCreateSave(slot);

            if (save == null)
                return;

            save.CollectedCoins ??= new List<string>();

            if (!save.CollectedCoins.Contains(coinId))
            {
                save.CollectedCoins.Add(coinId);
            }
        }

        public bool IsCoinCollected(
            string coinId,
            int slot = 0)
        {
            if (string.IsNullOrEmpty(coinId))
                return false;

            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                return false;
            }

            if (m_Saves[slot].CollectedCoins == null)
                return false;

            return m_Saves[slot].CollectedCoins.Contains(coinId);
        }

        /// <summary>
        /// Registra o conjunto de moedas coletadas como estado do checkpoint.
        /// </summary>
        public void SaveCheckpointCollectedCoins(
            List<string> collectedCoins,
            int slot = 0)
        {
            Save save = GetOrCreateSave(slot);

            if (save == null)
                return;

            if (collectedCoins == null)
            {
                save.CheckpointCollectedCoins =
                    new List<string>();

                return;
            }

            save.CheckpointCollectedCoins =
                new List<string>(collectedCoins);
        }

        public bool IsCoinCollectedAtCheckpoint(
            string coinId,
            int slot = 0)
        {
            if (string.IsNullOrEmpty(coinId))
                return false;

            if (slot < 0 ||
                slot >= m_Saves.Count ||
                m_Saves[slot] == null)
            {
                return false;
            }

            if (m_Saves[slot].CheckpointCollectedCoins == null)
                return false;

            return m_Saves[slot]
                .CheckpointCollectedCoins
                .Contains(coinId);
        }

        // =====================================================================
        // NOVA FASE
        // =====================================================================

        /// <summary>
        /// Prepara o autosave para começar uma nova fase.
        ///
        /// O contador de moedas da nova fase começa em 0
        /// e o checkpoint da fase anterior não continua válido.
        /// </summary>
        public void PrepareNextPhase(
            string sceneName,
            int slot = AutoSaveSlot)
        {
            if (string.IsNullOrEmpty(sceneName))
                return;

            Save save = GetOrCreateSave(slot);

            if (save == null)
                return;

            save.SceneName = sceneName;

            save.Position = Vector3.zero;
            save.Coin = 0;

            save.CheckpointActivated = false;
            save.CheckpointPosition = Vector3.zero;
            save.CheckpointCoin = 0;

            save.CollectedCoins =
                new List<string>();

            save.CheckpointCollectedCoins =
                new List<string>();

            SaveFile(slot);

            Debug.Log(
                $"[SaveSystem] Próxima fase preparada: {sceneName}"
            );
        }

        // =====================================================================
        // SAVE FILE
        // =====================================================================

        public void SaveFile(int slot = 0)
        {
            if (slot < 0)
                return;

            Save save = GetOrCreateSave(slot);

            if (save == null)
                return;

            string json =
                JsonUtility.ToJson(save, true);

            string encrypted =
                Encryptor.Encrypt(json);

            string path =
                GetSlotPath(slot);

            string directory =
                Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(
                path,
                encrypted
            );

            Debug.Log(
                $"Arquivo salvo com sucesso no slot {slot}:\n" +
                $"{path}"
            );

            // ================================================================
            // REGRA DO ENUNCIADO:
            //
            // Se salvar em um slot manual, o Slot 0 recebe a mesma informação.
            // ================================================================

            if (slot is < FirstManualSlot or > LastManualSlot) return;
            var autoSave =
                Clone(save);

            EnsureSlotExists(AutoSaveSlot);

            m_Saves[AutoSaveSlot] =
                autoSave;

            SaveFileOnly(
                AutoSaveSlot,
                autoSave
            );

            Debug.Log(
                $"[SaveSystem] Slot {slot} também foi replicado para o autosave (slot 0)."
            );
        }

        /// <summary>
        /// Grava um Save específico sem disparar novamente a replicação.
        /// </summary>
        private void SaveFileOnly(
            int slot,
            Save save)
        {
            if (save == null)
                return;

            string json =
                JsonUtility.ToJson(save, true);

            string encrypted =
                Encryptor.Encrypt(json);

            string path =
                GetSlotPath(slot);

            string directory =
                Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(
                path,
                encrypted
            );
        }

        // =====================================================================
        // LOAD FILE
        // =====================================================================

        public bool LoadFromFile(int slot = 0)
        {
            if (slot < 0)
                return false;

            string path =
                GetSlotPath(slot);

            // ================================================================
            // COMPATIBILIDADE:
            //
            // Se ainda existir o antigo SaveData.json e não existir o novo
            // Slot 0, tentamos usar o arquivo antigo.
            // ================================================================

            if (!File.Exists(path) &&
                slot == AutoSaveSlot &&
                File.Exists(m_LegacyDataPath))
            {
                path = m_LegacyDataPath;
            }

            if (!File.Exists(path))
                return false;

            try
            {
                string fileContent =
                    File.ReadAllText(path);

                string json;

                // ============================================================
                // Primeiro tentamos decriptar.
                // ============================================================

                try
                {
                    json =
                        Encryptor.Decrypted(fileContent);
                }
                catch
                {
                    // ========================================================
                    // Compatibilidade com o antigo arquivo JSON não criptografado.
                    // ========================================================

                    json = fileContent;
                }

                Save loadedSave =
                    JsonUtility.FromJson<Save>(json);

                if (loadedSave == null)
                    return false;

                EnsureSlotExists(slot);

                m_Saves[slot] =
                    loadedSave;

                // Garante as listas mesmo em saves antigos.
                m_Saves[slot].CollectedCoins ??= new List<string>();

                m_Saves[slot].CheckpointCollectedCoins ??= new List<string>();

                Debug.Log(
                    $"Save carregado do arquivo para o slot {slot}: " +
                    $"Posição {loadedSave.Position} | " +
                    $"Moedas {loadedSave.Coin} | " +
                    $"Cena {loadedSave.SceneName}"
                );

                // ============================================================
                // REGRA DO ENUNCIADO:
                //
                // Carregou um slot manual?
                // Copia imediatamente para o slot 0.
                // ============================================================

                if (slot is < FirstManualSlot or > LastManualSlot) return true;
                var autoSave =
                    Clone(loadedSave);

                EnsureSlotExists(AutoSaveSlot);

                m_Saves[AutoSaveSlot] =
                    autoSave;

                SaveFileOnly(
                    AutoSaveSlot,
                    autoSave
                );

                Debug.Log(
                    $"[SaveSystem] Slot {slot} carregado e replicado no autosave."
                );

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Erro ao ler o arquivo de save: {e.Message}"
                );
            }

            return false;
        }

        // =====================================================================
        // CARREGAR SLOT ESPECÍFICO
        // =====================================================================

        /// <summary>
        /// Carrega um slot e retorna true se conseguiu.
        /// Não muda a API antiga de LoadFromFile.
        /// </summary>
        public bool LoadSlot(int slot)
        {
            return LoadFromFile(slot);
        }

        // =====================================================================
        // CÓPIA
        // =====================================================================

        private Save Clone(Save original)
        {
            if (original == null)
                return null;

            string json =
                JsonUtility.ToJson(original);

            return JsonUtility.FromJson<Save>(json);
        }

        // =====================================================================
        // SAVE DATA
        // =====================================================================

        /// <summary>
        /// Classe responsável pelos dados do salvamento.
        /// </summary>
        [Serializable]
        public class Save
        {
            // ================================================================
            // DADOS QUE JÁ EXISTIAM
            // ================================================================

            [SerializeField]
            public Vector3 Position;

            [SerializeField]
            public int Coin;

            // ================================================================
            // NOVOS DADOS
            // ================================================================

            [SerializeField]
            public string SceneName;

            [SerializeField]
            public bool CheckpointActivated;

            [SerializeField]
            public bool IsLoaded;

            [SerializeField]
            public Vector3 CheckpointPosition;

            [SerializeField]
            public int CheckpointCoin;
            
            [SerializeField]
            public int CheckpointId;

            [SerializeField]
            public List<string> CollectedCoins =
                new List<string>();

            [SerializeField]
            public List<string> CheckpointCollectedCoins =
                new List<string>();

            public Save(
                Vector3 pos,
                int coin = 0)
            {
                Position = pos;
                Coin = coin;

                SceneName = string.Empty;

                CheckpointActivated = false;
                IsLoaded = false;
                CheckpointPosition = Vector3.zero;
                CheckpointCoin = 0;
                CheckpointId = -1;

                CollectedCoins =
                    new List<string>();

                CheckpointCollectedCoins =
                    new List<string>();
            }

            public string ToJson()
            {
                return JsonUtility.ToJson(
                    this
                );
            }

            public void FromJson(string json)
            {
                JsonUtility.FromJsonOverwrite(
                    json,
                    this
                );
            }
        }

        // =====================================================================
        // ENCRYPTOR
        // =====================================================================

        private ref struct Encryptor
        {
            // Mantidos no mesmo modelo do seu código.
            private static readonly string m_IV =
                "1a1a1a1a1a1a1a1a";

            private static readonly string m_Key =
                "1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a13";

            public static string IV => m_IV;

            public static string Key => m_Key;

            public static string Encrypt(
                string decrypted)
            {
                var textbytes =
                    Encoding.ASCII.GetBytes(
                        decrypted
                    );

                using var endec =
                    new AesCryptoServiceProvider();

                endec.BlockSize = 128;
                endec.KeySize = 256;

                endec.IV =
                    Encoding.ASCII.GetBytes(
                        IV
                    );

                endec.Key =
                    Encoding.ASCII.GetBytes(
                        Key
                    );

                endec.Padding =
                    PaddingMode.PKCS7;

                endec.Mode =
                    CipherMode.CBC;

                using var icrypt =
                    endec.CreateEncryptor(
                        endec.Key,
                        endec.IV
                    );

                var enc =
                    icrypt.TransformFinalBlock(
                        textbytes,
                        0,
                        textbytes.Length
                    );

                return Convert.ToBase64String(
                    enc
                );
            }

            public static string Decrypted(
                string encrypted)
            {
                var textbytes =
                    Convert.FromBase64String(
                        encrypted
                    );

                using var endec =
                    new AesCryptoServiceProvider();

                endec.BlockSize = 128;
                endec.KeySize = 256;

                endec.IV =
                    Encoding.ASCII.GetBytes(
                        IV
                    );

                endec.Key =
                    Encoding.ASCII.GetBytes(
                        Key
                    );

                endec.Padding =
                    PaddingMode.PKCS7;

                endec.Mode =
                    CipherMode.CBC;

                using var icrypt =
                    endec.CreateDecryptor(
                        endec.Key,
                        endec.IV
                    );

                var enc =
                    icrypt.TransformFinalBlock(
                        textbytes,
                        0,
                        textbytes.Length
                    );

                return Encoding.ASCII.GetString(
                    enc
                );
            }
        }
    }
}
