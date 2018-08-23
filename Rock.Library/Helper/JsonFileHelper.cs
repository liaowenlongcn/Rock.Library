using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rock.Library.Helper
{
    public class JsonFileHelper
    {
        private static string path = Directory.GetCurrentDirectory() + "\\Json\\";         //Json保存目录

        public static List<T> GetList<T>()
        {
            try
            {
                List<T> list = new List<T>();
                if (!File.Exists(path + typeof(T).Name + ".json")) return list;
                list = FileHelper.ReadFile(path + typeof(T).Name + ".json", Encoding.UTF8).ToObject<List<T>>();
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return null;
            }
        }

        public static bool SaveAll<T>(List<T> list)
        {
            try
            {
                return FileHelper.WriteFile(path, typeof(T).Name + ".json", Encoding.UTF8, list.ToJson());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }

        public static bool Save<T>(T model)
        {
            try
            {
                var list = GetList<T>();
                Type type = model.GetType();
                var properties = type.GetProperties();
                var fieldId = properties.FirstOrDefault(p => p.Name.ToLower() == "id");
                foreach (var item in list)
                {
                    var oldid = fieldId.GetValue(item);
                    var newid = fieldId.GetValue(model);
                    if (oldid.ToString() == newid.ToString())
                    {
                        list.Remove(item);
                        break;
                    }
                }
                list.Add(model);
                return FileHelper.WriteFile(path, typeof(T).Name + ".json", Encoding.UTF8, list.ToJson());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
        }
         
        public static int Delete<T>(Func<T, bool> predicate)
        {
            try
            {
                var result = GetList<T>();
                var list = result.Where(predicate).ToList();
                result = result.Where(p => !list.Contains(p)).ToList();

                SaveAll(result);

                return list.Count();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return -1;
            }
        } 
    }
}
