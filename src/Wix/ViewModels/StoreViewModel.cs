namespace Wix.ViewModels
{
    using global::Wix.Models;
    using System.Collections.Generic;

        public class StoreViewModel
        {
            public IEnumerable<StoreModel> Stores { get; set; }
        }
}
