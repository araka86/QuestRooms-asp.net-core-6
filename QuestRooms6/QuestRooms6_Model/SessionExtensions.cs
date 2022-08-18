using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;

//по умолчани. код для обработки сессий может хранить только целые числа или строки,
//и эсли нужно сохранить списки целых чисел или обьектов то это не поддерживается по умолчанию .net core
// для этого нужны методы разширения

namespace QuestRooms6_Model
{
    public static class SessionExtensions
    {


        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }



        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonSerializer.Deserialize<T>(value);


        }


    }
}
