using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaNEXU.DataContext
{
    public partial class BrandsWithAveragePrice
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public decimal? AveragePrice { get; set; }
    }
}
