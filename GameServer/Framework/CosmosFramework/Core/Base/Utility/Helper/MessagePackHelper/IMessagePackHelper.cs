using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    public interface IMessagePackHelper
    {
        string ToJson<T>(T obj);
        byte[] ToByteArray<T>(T obj);
        byte[] ToTypelessByteArray<T>(T obj);
        T ToObject<T>(byte[] buffer);
        T ToTypelessObject<T>(byte[] buffer);
        T ToObject<T>(string json);
        object ToObject(byte[] buffer , Type objectType);
        object ToTypelessObject(byte[] buffer );
        object ToObject(string json, Type objectType);
    }
}
