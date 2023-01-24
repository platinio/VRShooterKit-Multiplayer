using System;
using System.Collections.Generic;

namespace VRShooterKit
{
    public delegate object DeserializeMethod(string data);
    public delegate string SerializeMethod(object customObject);
        
    public static class SerializerManager 
    {
        private static Dictionary<Type, SerializableHandler> registerSerializableHandleDict = new Dictionary<Type, SerializableHandler>();
        
        public static void RegisterSerializeHandler(Type t, SerializeMethod serializeMethod, DeserializeMethod deserializeMethod)
        {
            SerializableHandler handler = new SerializableHandler();
            handler.DeserializeMethod = deserializeMethod;
            handler.SerializeMethod = serializeMethod;
            registerSerializableHandleDict.Add(t, handler);
        }

        public static object Deserialize(Type t, string data)
        {
            return registerSerializableHandleDict[t].DeserializeMethod(data);
        }
        
        public static string Serialize(Type t, object data)
        {
            return registerSerializableHandleDict[t].SerializeMethod(data);
        }
    }

    public class SerializableHandler
    {
        public SerializeMethod SerializeMethod;
        public DeserializeMethod DeserializeMethod;
    }

}

