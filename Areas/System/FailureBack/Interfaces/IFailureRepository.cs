using FiyiPOS.Areas.System.FailureBack.Entities;

namespace FiyiPOS.Areas.System.FailureBack.Interfaces
{
    public partial interface IFailureRepository
    {
        Task<int> AddAsync(Failure failure);
    }
}