using SQLite;

namespace MauiAppTiraFoto
{
    public class Image
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
