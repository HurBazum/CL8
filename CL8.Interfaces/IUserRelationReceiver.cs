namespace CL8.Interfaces
{
    public interface IUserRelationReceiver<T> where T : class, IEntity, new()
    {
        public Task<T> GetOneAsync(int id);
        public Task<IEnumerable<T>> GetChatsMembersAsync(int id);
    }
}