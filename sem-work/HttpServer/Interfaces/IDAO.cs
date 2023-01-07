namespace HttpServer.Interfaces;

public interface IDAO<T>
{
    T? GetById(object id);
    IEnumerable<T> GetAll();
    int Insert(T entity);
    int Update(T entity);
    int Delete(T entity);
    int Delete(object id);
}