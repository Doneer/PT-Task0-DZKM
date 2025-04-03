using LibrarySystem.Data.Models;

namespace LibrarySystem.Data.Interfaces
{
    public interface ILibraryStateRepository
    {
        LibraryState GetCurrentState();
        void UpdateState(LibraryState state);
    }
}