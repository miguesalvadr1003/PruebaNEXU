using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaNEXU.DataContext
{
    public partial class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AveragePrice { get; set; }
        public string BrandName { get; set; }

        public virtual Brand BrandNameNavigation { get; set; }
    }
}
