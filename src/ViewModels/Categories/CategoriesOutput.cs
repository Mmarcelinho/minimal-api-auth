using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.ViewModels.Categories
{
    public class CategoriesOutput
    {
        public CategoriesOutput(int id, string title) 
        {
            Id = id;
            Title = title;
        }
            public int Id { get; set; }

   
    public string Title { get; set; }

    }
}