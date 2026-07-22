using FiyiPOS.Areas.FiyiPOS.Common.StoreBack.Entities;
using FiyiPOS.Areas.FiyiPOS.Common.SystemUserBack.Entities;

namespace FiyiPOS.Areas.FiyiPOS.Common.SystemUserStoreBack.Entities
{
    public class SystemUserStore
    {
        public long SystemUserId { get; set; }
        public long StoreId      { get; set; }

        public SystemUser? SystemUser { get; set; }
        public Store?      Store      { get; set; }
    }
}
