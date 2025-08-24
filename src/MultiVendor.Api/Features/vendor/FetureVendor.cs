public class FetureVendor
{
   
    private readonly VendorRepository _repoVendor;

    public  FetureVendor( VendorRepository repoVendor)
    {
      
        _repoVendor = repoVendor;
    }



    public bool addvendor(string ownerid,string name , string description){

         string id = "shop"+Guid.NewGuid().ToString();
        DateTime createdAt = DateTime.Now;
       return _repoVendor.AddVendor(id,ownerid,name,description,createdAt);


    }
    public List<vendorglobaldto> getvendors(string ownerid){

        return _repoVendor.GetVendorsByOwnerId(ownerid);


    }
    public vendorglobaldto getvendorbyid(string ownerid,string id){

        return _repoVendor.GetVendorByIdAndOwner(id,ownerid);
    }

    public bool UpdateVendorById(string id, string ownerid, string? name, string? description)
{
    return _repoVendor.UpdateVendor(id, ownerid, name, description);
}
   public bool DeleteVendorById(string id, string ownerid)
{
    return _repoVendor.DeleteVendorByIdAndOwner(id, ownerid);
}
    
}
   
