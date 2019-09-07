using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using DNTPersianUtils.Core;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Identity.App.Extention
{
    public static class EntityFrameworkCoreExtensions
    {
        public static void ApplyCorrectYeKe(this DbContext dbContext)
        {
            if (dbContext == null)
            {
                return;
            }

            //پیدا کردن موجودیت‌های تغییر کرده
            var changedEntities = dbContext.ChangeTracker
                                      .Entries()
                                      .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var item in changedEntities)
            {
                var entity = item.Entity;
                if (item.Entity == null)
                {
                    continue;
                }

                //یافتن خواص قابل تنظیم و رشته‌ای این موجودیت‌ها
                var propertyInfos = entity.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance
                    ).Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                var propertyReflector = new PropertyReflector();

                //اعمال یکپارچگی نهایی
                foreach (var propertyInfo in propertyInfos)
                {
                    var propName = propertyInfo.Name;
                    var value = propertyReflector.GetValue(entity, propName);
                    if (value != null)
                    {
                        var strValue = value.ToString();
                        var newVal = strValue.ApplyCorrectYeKe();
                        if (newVal == strValue)
                        {
                            continue;
                        }
                        propertyReflector.SetValue(entity, propName, newVal);
                    }
                }
            }
        }
    
        public static string GetValidationErrors(this DbContext context)
        {
            var errors = new StringBuilder();
            var entities = context.ChangeTracker.Entries()
                                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                                        .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(entity, validationContext, validationResults, validateAllProperties: true))
                {
                    foreach (var validationResult in validationResults)
                    {
                        var names = validationResult.MemberNames.Aggregate((s1, s2) => $"{s1}, {s2}");
                        errors.AppendFormat("{0}: {1}", names, validationResult.ErrorMessage);
                    }
                }
            }

            return errors.ToString();
        }
    }
}
