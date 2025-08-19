public class CreatuserService
{
    private readonly UserRepository _repo;
    private readonly VendorApprovalRepository _repoVendor;

    public CreatuserService(UserRepository repo, VendorApprovalRepository repoVendor)
    {
        _repo = repo;
        _repoVendor = repoVendor;
    }

    public bool Creatuser(string displayName, string email, string passwordHash, string chooserole)
    {
        string id = Guid.NewGuid().ToString();
        DateTime createdAt = DateTime.Now;

        if (chooserole.Equals("Customer"))
        {
            return _repo.AddUser(id, email, passwordHash, chooserole, displayName, createdAt);
        }

        if (chooserole.Equals("Vendor"))
        {
            id = "v" + id;
            return _repoVendor.AddVendor(id, displayName, email, passwordHash, createdAt);
        }

        // لو ما كان Customer ولا Vendor
        return false;
    }
}
