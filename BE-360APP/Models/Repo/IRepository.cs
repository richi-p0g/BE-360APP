namespace BE_360APP.Models.Repo
{
    public interface IRepository
    {

        //Task<List<Dashboard>> Dashboards(whereString ws);
        Task<Response> Logins(loginDto ws);
        Task<List<Registerfullname>> LookupFullNames(usernameDto dto);
        Task<Response> AssignToOtherRowSaves(AssignToOtherDto dto);
        Task<Response> AssignToOtherRowUpdates(AssignToOtherDto dto);
        Task<List<AssignToOtherGetAll>> AssignToOtherGetAlls(AssignToOtherGetAllDto dto);
        Task<List<AssignToMeGetAll>> AssignToMeGetAlls(AssignToMeGetAllDto dto);
        Task<List<fullnameAndid>> nameAssigns(int id, string fullname);
        Task<List<responReviewgetall>> responReviewgetalls(formFastRateDto dto);
        Task<List<fullnameAndid>> listFastRatings(string username);
        Task<Response> formFastRatesubmitForms(ListFastRatingRowSaveDto dto);
        Task<structurformFastRating> formFastRatesresponReviewnextForm(formFastRateDto dto);
        Task<structurformFastRating> formFastRates(formFastRateDto dto);
        Task<List<juniorEmployee>> personJuniors(formFastRateDto dto);
        Task<Allperson> personLayers(formFastRateDto dto);
        Task<Response> GivenFeedbackonSubmits(personLayersubmitDto dto);
        Task<structurformFastRating> formFastRatenextReviews(formFastRateDto dto);  
        Task<allmasters> allMasters();
        Task<List<projectOpty>> masterProjectopties();
        Task<Response> Registers(registerDto dto);
    }
}
