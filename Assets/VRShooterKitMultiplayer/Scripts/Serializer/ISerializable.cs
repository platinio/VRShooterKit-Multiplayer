namespace VRShooterKit
{
    public interface ISerializable<T>
    {
        string Serialize();
        T Deserialize();
    }
}

