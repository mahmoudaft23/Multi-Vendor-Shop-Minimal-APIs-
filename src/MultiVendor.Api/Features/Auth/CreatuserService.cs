public class CreatuserService
{
    private readonly UserRepository _repo;

    public CreatuserService(UserRepository repo)
    {
        _repo = repo;
    }

    public bool Creatuser(string displayName,string email, string passwordHash)
    {
        string id = Guid.NewGuid().ToString();
       DateTime createdAt = DateTime.Now;


        return _repo.AddUser(id,email, passwordHash,"Customer",displayName,createdAt);
    }
}
