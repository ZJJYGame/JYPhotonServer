using System.Collections;
using System.Collections.Generic;
using Cosmos;
using System;
using Newtonsoft.Json;
public class NewtonjsonHelper : IJsonHelper
{
    public string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    public T ToObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
    public object ToObject(string json, Type objectType)
    {
        return JsonConvert.DeserializeObject(json, objectType);
    }
}
