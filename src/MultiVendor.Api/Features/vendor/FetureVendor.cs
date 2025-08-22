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
}
   
