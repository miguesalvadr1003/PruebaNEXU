using System;
using System.Collections.Generic;

#nullable disable

namespace PruebaNEXU.DataContext
{
    public partial class Brand
    {
        public Brand()
        {
            Models = new HashSet<Model>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Model> Models { get; set; }
    }
}
