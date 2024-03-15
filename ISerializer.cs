namespace CodeName.Serialization
{
    public interface ISerializer
    {
        public string Serialize(object value);

        public T Deserialize<T>(string data);

        public T Clone<T>(T value);
    }
}
