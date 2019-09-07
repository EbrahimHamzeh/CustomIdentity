using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.App.ViewModel
{
    public class DynmicRoleViewModel
    {
        public Guid Guid { get; set; }

        [Required(ErrorMessage = "عنوان الزامی میباشد")]
        public string Title { get; set; }
        
        public string JsonJSTree { get; set; }

        [MaxLength(500,ErrorMessage="توضیحات نمی‌تواند از ۵۰۰ کاراکتر بیشتر باشد.")]
        public string Description { get; set; }

        public bool Enable { get; set; }

        public string NodeSelected { get; set; }
    }

    public class DynmicRoleListViewModel{
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public string Actions { get; set; }
    }
}
