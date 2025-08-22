public class FetureHR
{
   private readonly UserRepository _repo;
    private readonly VendorApprovalRepository _repoVendor;

    public  FetureHR(UserRepository repo, VendorApprovalRepository repoVendor)
    {
        _repo = repo;
        _repoVendor = repoVendor;
    }

   

public List<VendorApprovalDto> AllVendors()
{
    
        List <VendorApprovalDto>  VendorApproval  = _repoVendor.GetAllVendors();
        return VendorApproval ;
}
public bool updatestuts(string id , string Status)

{
bool statuschek;
    if(Status.Equals("Accept")){
                 statuschek = _repoVendor.AcceptVendor(id);

    }
    else
    {
          statuschek = _repoVendor.RejectVendor(id);

    }  
    return statuschek;
     
}

   
}
