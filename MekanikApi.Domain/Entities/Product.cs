using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class ProductSpecification
    {
        public string Name { get; set; }

        public string Detail { get; set; }
    }
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public string Brand { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public string? Color { get; set; }

        public ProductCondition? Condition { get; set; }

        public string? Status { get; set; }

        public ProductSpecification? Specification { get; set; }

        public List<string> ProductImages { get; set; }


    }
}
