#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER) && UNITY_STANDALONE)
using System.Collections.Generic;
using Cysharp.Text;
using MySqlConnector;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private bool ReadCharacterDataFloat32(MySqlDataReader reader, out CharacterDataFloat32 result)
        {
            if (reader.Read())
            {
                result = new CharacterDataFloat32();
                result.hashedKey = reader.GetInt32(0);
                result.value = reader.GetFloat(1);
                return true;
            }
            result = default;
            return false;
        }

        public void CreateCharacterDataFloat32(MySqlConnection connection, MySqlTransaction transaction, string tableName, HashSet<string> insertedIds, string characterId, CharacterDataFloat32 characterDataFloat32)
        {
            string id = ZString.Concat(characterId, "_", characterDataFloat32.hashedKey);
            if (insertedIds.Contains(id))
            {
                LogWarning(LogTag, $"Custom Float32 {id}, for character {characterId}, already inserted to table {tableName}");
                return;
            }
            insertedIds.Add(id);
            ExecuteNonQuerySync(connection, transaction, $"INSERT INTO {tableName} (id, characterId, hashedKey, value) VALUES (@id, @characterId, @hashedKey, @value)",
                new MySqlParameter("@id", id),
                new MySqlParameter("@characterId", characterId),
                new MySqlParameter("@hashedKey", characterDataFloat32.hashedKey),
                new MySqlParameter("@value", characterDataFloat32.value));
        }

        public List<CharacterDataFloat32> ReadCharacterDataFloat32s(string tableName, string characterId, List<CharacterDataFloat32> result = null)
        {
            if (result == null)
                result = new List<CharacterDataFloat32>();
            ExecuteReaderSync((reader) =>
            {
                CharacterDataFloat32 tempData;
                while (ReadCharacterDataFloat32(reader, out tempData))
                {
                    result.Add(tempData);
                }
            }, $"SELECT hashedKey, value FROM {tableName} WHERE characterId=@characterId",
                new MySqlParameter("@characterId", characterId));
            return result;
        }

        public void DeleteCharacterDataFloat32s(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId)
        {
            ExecuteNonQuerySync(connection, transaction, $"DELETE FROM {tableName} WHERE characterId=@characterId", new MySqlParameter("@characterId", characterId));
        }
    }
}
#endif