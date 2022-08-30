namespace DataAccess.Interfaces
{
    public interface IIdentifiable<TKey>
    {
        TKey Id { get; set; }
    }
}