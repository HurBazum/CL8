namespace CL8.Interfaces
{
    public interface IChatRelationReceiver<T> where T : class, IEntity, new()
    {
        public Task<IEnumerable<T>> GetUserChatsAsync(int id);
    }
}
