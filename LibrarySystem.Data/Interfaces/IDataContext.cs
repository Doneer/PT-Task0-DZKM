namespace LibrarySystem.Data.Interfaces
{
    public interface IDataContext
    {
        IUserRepository Users { get; }
        IBookRepository Books { get; }
        ILibraryStateRepository LibraryState { get; }
        IEventRepository Events { get; }
    }
}